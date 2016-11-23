//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Utils.Interface;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using OfferManagement.MerchantFileParser;
using System.Linq;
using System.Net;
using Azure.Utils;
using Lomo.Logging;
using Newtonsoft.Json;
using OfferManagement.BingMapClient;

namespace OfferManagement.JobProcessor.Jobs
{
    public class ProvisionMasterCardMids : IScheduledJob
    {
        private readonly IAzureBlob azureBlob;
        private readonly AzureScheduler azureScheduler;
        private readonly SchedulerQueueInfo schedulerQueueInfo;
        private ScheduledJobInfo jobInfo;
        private MerchantFileType merchantFileType;
        private IEnumerable<Merchant> merchantsInDb;
        private Provider provider;        
        private string merchantFileName;
        private string blobContainer;

        public ProvisionMasterCardMids(IAzureBlob azureBlob, AzureScheduler azureScheduler = null, SchedulerQueueInfo schedulerQueueInfo = null)
        {
            this.azureBlob = azureBlob;
            this.azureScheduler = azureScheduler;
            this.schedulerQueueInfo = schedulerQueueInfo;
        }

        public async Task ExecuteAsync(ScheduledJobInfo scheduledJobInfo)
        {
            this.jobInfo = scheduledJobInfo;
            await ValidatePayload().ConfigureAwait(false);
            MemoryStream memoryStream = await DownloadMerchantFileFromBlobAsync().ConfigureAwait(false);
            Tuple<IList<Merchant>, IList<Merchant>> lstMerchants = await ProcessMerchantFileAsync(memoryStream).ConfigureAwait(false);

            if (lstMerchants.Item1.Any())
            {
                Log.Info($"Total Merchants to add : {lstMerchants.Item1.Count}");
                await EarnRepository.Instance.AddMerchantsInBatchAsync(lstMerchants.Item1).ConfigureAwait(false);
                Log.Info("Finished adding merchants");
            }

            if (lstMerchants.Item2.Any())
            {
                Log.Info($"Total Merchants to update : {lstMerchants.Item2.Count}");
                await EarnRepository.Instance.UpdateAsync(lstMerchants.Item2).ConfigureAwait(false);
                Log.Info("Finished updating merchants");
            }          

            await ScheduleVisaLookupJob().ConfigureAwait(false);
            scheduledJobInfo.JobCompletedTime = DateTime.UtcNow;
        }

        private async Task ValidatePayload()
        {
            IDictionary<string, string> payload = this.jobInfo.JobPayload;
            if (payload == null)
            {
                throw new ArgumentNullException($"{nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId} has an empty payload");
            }
            if (!payload.ContainsKey(JobConstants.ContainerName))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId} is missing the blob container name");
            }
            blobContainer = payload[JobConstants.ContainerName];
            if (string.IsNullOrWhiteSpace(blobContainer))
            {
                throw new ArgumentException($"Blob Container is empty in {nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId}");
            }

            if (!payload.ContainsKey(JobConstants.BlobName))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId} is missing the blob name");
            }
            merchantFileName = payload[JobConstants.BlobName];
            if (string.IsNullOrWhiteSpace(merchantFileName))
            {
                throw new ArgumentException($"Merchant file name is empty in {nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId}");
            }

            if (!payload.ContainsKey(JobConstants.ProviderId))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId} is missing the provider id for the merchants");
            }

            string providerId = payload[JobConstants.ProviderId];
            if (string.IsNullOrWhiteSpace(providerId))
            {
                throw new ArgumentException($"ProviderId is empty in {nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId}");
            }

            provider = await EarnRepository.Instance.GetProviderAsync(providerId).ConfigureAwait(false);
            if (provider == null)
            {
                throw new ArgumentException($"Provider {providerId} does not exist");
            }

            if (!payload.ContainsKey(JobConstants.MerchantFileType))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId} is missing the merchant file type info for the merchants");
            }

            if (!Enum.TryParse<MerchantFileType>(payload[JobConstants.MerchantFileType], out merchantFileType))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMasterCardMids)} {this.jobInfo.JobId} has an invalid merchant filetype info for the merchants");
            }
        }

        private async Task<MemoryStream> DownloadMerchantFileFromBlobAsync()
        {
            Log.Info("Getting merchant file {0} from blob container {0}", merchantFileName, blobContainer);
            MemoryStream memoryStream = await azureBlob.DownloadBlobToStreamAsync(blobContainer, merchantFileName).ConfigureAwait(false);
            Log.Info("Successfully downloaded merchant file {0} from blob container {0}", merchantFileName, blobContainer);
            memoryStream.Position = 0;

            return memoryStream;
        }

        private async Task<Tuple<IList<Merchant>, IList<Merchant>>> ProcessMerchantFileAsync(MemoryStream memoryStream)
        {
            IList<Merchant> lstImportedMerchants = null;
            Log.Info($"Importing data from {merchantFileType.ToString()}");
            MerchantFileProcessor merchantFileProcessor = MerchantProcessorFactory.GetMerchantFileProcessor(merchantFileName);
            if (merchantFileType == MerchantFileType.MasterCardAuth)
            {
                lstImportedMerchants = await Task.Run(() => merchantFileProcessor.ImportMasterCardAuthFile(memoryStream)).ConfigureAwait(false);
            }
            else if (merchantFileType == MerchantFileType.MasterCardClearing)
            {
                lstImportedMerchants = await Task.Run(() => merchantFileProcessor.ImportMasterCardClearingFile(memoryStream)).ConfigureAwait(false);
            }
            if (lstImportedMerchants == null || !lstImportedMerchants.Any())
            {
                throw new Exception($"Error in processing the {merchantFileType.ToString()} file.");
            }
            Log.Info($"Total unique records imported from {merchantFileType.ToString()} is {lstImportedMerchants.Count}");

            Log.Info($"Getting all merchants for provider {provider.Id} from db");
            merchantsInDb = await EarnRepository.Instance.GetMerchantsForProviderAsync(provider.Id).ConfigureAwait(false);
            Log.Info($"Got {merchantsInDb.Count()} merchant entries for provider {provider.Id} from db");

            Tuple<IList<Merchant>, IList<Merchant>> lstMerchants = await Task.Run(() => AddOrUpdateMerchant(lstImportedMerchants)).ConfigureAwait(false);

            return lstMerchants;
        }

        private Tuple<IList<Merchant>,IList<Merchant>> AddOrUpdateMerchant(IList<Merchant> merchantsFromFile)
        {
            IList<Merchant> merchantsToAdd = new List<Merchant>();
            IList<Merchant> merchantsToUpdate = new List<Merchant>();
            Merchant merchantInDb = null;

            if (merchantFileType == MerchantFileType.MasterCardClearing)
            {
                foreach (var merchantFromFile in merchantsFromFile)
                {
                    if (DoesRecordHaveValidIds(merchantFromFile))
                    {
                        merchantInDb = GetMerchantFromDb(merchantFromFile);
                        if (merchantInDb != null)
                        {
                            UpdateClearingData(merchantInDb, merchantFromFile, merchantsToUpdate);
                        }
                        else
                        {
                            Log.Info($"Cannot find {merchantFileType.ToString()} Merchant : {merchantFromFile.ToString()} in db. New merchant to be created");
                            Merchant merchant = CreateNewMerchant(merchantFromFile);
                            merchantsToAdd.Add(merchant);
                        }
                    }
                }
            }
            else if (merchantFileType == MerchantFileType.MasterCardAuth)
            {
                foreach (var merchantFromFile in merchantsFromFile)
                {
                    if (DoesRecordHaveValidIds(merchantFromFile))
                    {
                        merchantInDb = GetMerchantFromDb(merchantFromFile);
                        if (merchantInDb != null)
                        {
                            UpdateAuthData(merchantInDb, merchantFromFile, merchantsToUpdate);
                        }
                        else
                        {
                            Log.Info($"Cannot find {merchantFileType.ToString()} Merchant : {merchantFromFile.ToString()} in db. New merchant to be created");
                            Merchant merchant = CreateNewMerchant(merchantFromFile);
                            merchantsToAdd.Add(merchant);
                        }
                    }
                }
            }

            return new Tuple<IList<Merchant>, IList<Merchant>>(merchantsToAdd, merchantsToUpdate);
        }

        /// <summary>
        /// Checks if the parsed record from MasterCard file has either the MasterCardUniqueId or the MasterCardSiteId. We need one of these Id's to correlate auth/clearing recordsd
        /// with the merchant record that we already have in the db. This is a defensive check. The upstream code (import processor) will already eliminate any
        //  records that don't have the MCID or the MCSiteId attached
        /// </summary>
        /// <param name="merchantFromFile">Parsed merchant record from MC File</param>
        /// <returns></returns>
        private bool DoesRecordHaveValidIds(Merchant merchantFromFile)
        {
            bool doesRecordHaveValidIds = true;
            string mcId = merchantFromFile.ExtendedAttributes != null && merchantFromFile.ExtendedAttributes.ContainsKey(MerchantConstants.MCID)
              ? merchantFromFile.ExtendedAttributes[MerchantConstants.MCID] : string.Empty;
            string mcSiteId = merchantFromFile.ExtendedAttributes != null && merchantFromFile.ExtendedAttributes.ContainsKey(MerchantConstants.MCSiteId)
                ? merchantFromFile.ExtendedAttributes[MerchantConstants.MCSiteId] : string.Empty;

            doesRecordHaveValidIds = !string.IsNullOrEmpty(mcId) || !string.IsNullOrEmpty(mcSiteId);
            if (!doesRecordHaveValidIds)
            {
                Log.Error($"Parsed merchant {merchantFileName.ToString()} does not have MCID or MCSiteId. This is invalid");
            }

            return doesRecordHaveValidIds;
        }        

        private Merchant GetMerchantFromDb(Merchant merchantFromFile)
        {
            // Locate the merchant in the db with the same MCID as the incoming merchant. 
            // MCID is the unique identifier we assign to each merchant when they are first imported from MasterCard.     
            Merchant locatedMerchant = null;
            string mcId = merchantFromFile.ExtendedAttributes != null && merchantFromFile.ExtendedAttributes.ContainsKey(MerchantConstants.MCID)
              ? merchantFromFile.ExtendedAttributes[MerchantConstants.MCID] : string.Empty;
            string mcSiteId = merchantFromFile.ExtendedAttributes != null && merchantFromFile.ExtendedAttributes.ContainsKey(MerchantConstants.MCSiteId)
                ? merchantFromFile.ExtendedAttributes[MerchantConstants.MCSiteId] : string.Empty;

            //TODO: Searching through the list is slow. Create 2 dictionaries to store merchant by
            //MCID and SiteID and lookup the dictionary.
            if (!string.IsNullOrEmpty(mcId))
            {
                Log.Info($"Checking for matching merchant in db with MCID {mcId}");
                locatedMerchant = merchantsInDb.FirstOrDefault(m => (m.ExtendedAttributes != null && m.ExtendedAttributes.ContainsKey(MerchantConstants.MCID)
                    && m.ExtendedAttributes[MerchantConstants.MCID] == mcId));
            }

            if (locatedMerchant == null && !string.IsNullOrEmpty(mcSiteId))
            {
                Log.Info($"Checking for matching merchant in db with MCSiteId {mcSiteId}");
                locatedMerchant = merchantsInDb.FirstOrDefault(m => (m.ExtendedAttributes != null && m.ExtendedAttributes.ContainsKey(MerchantConstants.MCSiteId)
                    && m.ExtendedAttributes[MerchantConstants.MCSiteId] == mcSiteId));
            }           

            return locatedMerchant;
        }

        private Merchant CreateNewMerchant(Merchant merchantFromFile)
        {
            Merchant merchant = new Merchant
            {
                Id = Guid.NewGuid().ToString(),
                ProviderId = provider.Id,
                Name = merchantFromFile.Name,
                PhoneNumber = merchantFromFile.PhoneNumber,
                Location = merchantFromFile.Location,
                ExtendedAttributes = merchantFromFile.ExtendedAttributes,
                Payments = merchantFromFile.Payments
            };
            string masterCardId = MasterCardIdGenerator.GetUniqueId(provider, merchant, "P");            
            merchant.ExtendedAttributes.Add(MerchantConstants.MCID, masterCardId);
            GeoCodeMerchantLocation(merchant);
            Log.Info($"Created new merchant : {merchant.ToString()} with MasterCardId {masterCardId}");

            return merchant;
        }

        private void UpdateAuthData(Merchant merchantInDb, Merchant merchantFromFile,
            IList<Merchant> merchantsToUpdate)
        {
            bool merchantUpdated = false;

            if (GeoCodeMerchantLocation(merchantInDb))
            {
                merchantUpdated = true;
            }

            var incomingPaymentMids = merchantFromFile.Payments;
            List<String> incomingAuthMids = new List<string>();
            if (incomingPaymentMids.Any())
            {
                //Get the Auth Mids from the incoming data
                incomingAuthMids.AddRange(from incomingPaymentMid in incomingPaymentMids
                                          where incomingPaymentMid.PaymentMids != null &&
                                          incomingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.MCAcquiringICA)
                                          && incomingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.MCAcquiringMid)
                                          select ($"{incomingPaymentMid.PaymentMids[MerchantConstants.MCAcquiringICA]};{incomingPaymentMid.PaymentMids[MerchantConstants.MCAcquiringMid]}"));
            }

            
            //Get the MC MID's that we already have for this merchant in the db. This could be auth, clearing or both
            var existingPaymentMids =
                merchantInDb.Payments?.Where(payment => payment.Processor == DataModel.PaymentProcessor.MasterCard);
            List<String> existingAuthMids = new List<string>();

            if (existingPaymentMids != null && existingPaymentMids.Any())
            {
                //Get the existing Auth Mids
                existingAuthMids.AddRange(from existingPaymentMid in existingPaymentMids
                                          where existingPaymentMid.PaymentMids != null &&
                                          existingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.MCAcquiringICA)
                                          && existingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.MCAcquiringMid)
                                          select ($"{existingPaymentMid.PaymentMids[MerchantConstants.MCAcquiringICA]};{existingPaymentMid.PaymentMids[MerchantConstants.MCAcquiringMid]}"));
            }

            //If we don't have any MC Mid's for this merchant in the db or if we don't have any Auth Mids for this merchant in the db
            // and if there are incoming Auth Mid's , then add the incoming Auth Mid to the merchant to be updated to the db
            if (existingPaymentMids == null || !existingPaymentMids.Any() || !existingAuthMids.Any())
            {
                if (incomingAuthMids.Any())
                {
                    UpdateMerchantWithAuthMids(merchantInDb, incomingAuthMids);
                    merchantUpdated = true;                    
                }
            }
            else
            {
                //If we have auth mids for this merchant in the db, then check if there's anything new between the incoming auth mid and the 
                //existing auth mid. If there's a new entry, then add the incoming auth mid to the merchant to be updated to the db
                var newAuthMids = incomingAuthMids.Except(existingAuthMids);
                if (newAuthMids.Any())
                {
                    UpdateMerchantWithAuthMids(merchantInDb, newAuthMids);
                    merchantUpdated = true;                    
                }
            }

            if (merchantUpdated)
            {
                merchantsToUpdate.Add(merchantInDb);
            }
        }

        private void UpdateMerchantWithAuthMids(Merchant merchantInDb, IEnumerable<string> newAuthMids)
        {
            if (merchantInDb.Payments == null)
                merchantInDb.Payments = new List<Payment>();

            foreach (var authMid in newAuthMids)
            {
                string acquirerIca = authMid.Split(';')[0];
                string acquirerMid = authMid.Split(';')[1];
                merchantInDb.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    Processor = PaymentProcessor.MasterCard,
                    LastUpdate = DateTime.UtcNow,
                    PaymentMids = new Dictionary<string, string>
                            {
                                {MerchantConstants.MCAcquiringICA, acquirerIca},
                                {MerchantConstants.MCAcquiringMid, acquirerMid}
                            },
                    IsActive = true                
                });
            }
        }

        private void UpdateClearingData(Merchant merchantInDb, Merchant merchantFromFile,
            IList<Merchant> merchantsToUpdate)
        {
            // For nationals, check if the merchant data such as Name, Address, city, state, zip, phonenumber is same as the clearing file.
            // If it differs take the data from clearing file as the ground truth. Do this only for
            // merchants that are provisioned from the MasterCard Files.
            bool merchantUpdated = false;
            if (provider.MerchantSource == DataModel.Enums.MerchantSource.MasterCard)
            {
                merchantUpdated = CopyMerchantInfoFromClearingIfNeeded(merchantInDb, merchantFromFile);
            }
            if (GeoCodeMerchantLocation(merchantInDb))
            {
                merchantUpdated = true;
            }

            var incomingPaymentMids = merchantFromFile.Payments;
            List<String> incomingLocationIds = new List<string>();
            if (incomingPaymentMids.Any())
            {
                //Get the LocationID's from the incoming data
                incomingLocationIds.AddRange(from incomingPaymentMid in incomingPaymentMids
                                             where incomingPaymentMid.PaymentMids != null
                                             && incomingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.MCLocationId)
                                             select incomingPaymentMid.PaymentMids[MerchantConstants.MCLocationId]);
            }
            
            //Get the MC MID's that we already have for this merchant in the db. This could be auth, clearing or both
            var existingPaymentMids =
                merchantInDb.Payments?.Where(payment => payment.Processor == PaymentProcessor.MasterCard);
            List<String> existingLocationIds = new List<string>();

            if (existingPaymentMids != null && existingPaymentMids.Any())
            {
                //Get the existing LocationID's
                existingLocationIds.AddRange(from existingPaymentMid in existingPaymentMids
                                             where existingPaymentMid.PaymentMids != null
                                             && existingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.MCLocationId)
                                             select existingPaymentMid.PaymentMids[MerchantConstants.MCLocationId]);
            }

            //If we don't have any MC Mid's for this merchant in the db or if we don't have any locationId for this merchant in the db
            // and if there are incoming locationId's , then add the incoming location id to the merchant to be updated to the db
            if (existingPaymentMids == null || !existingPaymentMids.Any() || !existingLocationIds.Any())
            {
                if (incomingLocationIds.Any())
                {
                    UpdateMerchantWithClearingMids(merchantInDb, incomingLocationIds);
                    merchantUpdated = true;
                }
            }
            else
            {
                //If we have location id's for this merchant in the db, then check if there's anything new between the incoming location id and the 
                //existing location id. If there's a new entry, then add the incoming location id to the merchant to be updated to the db

                var newLocationIds = incomingLocationIds.Except(existingLocationIds);
                if (newLocationIds.Any())
                {
                    UpdateMerchantWithClearingMids(merchantInDb, newLocationIds);
                    merchantUpdated = true;
                }
            }

            if (merchantUpdated)
                merchantsToUpdate.Add(merchantInDb);
        }

        private void UpdateMerchantWithClearingMids(Merchant merchantInDb, IEnumerable<string> incomingLocationIds)
        {
            if (merchantInDb.Payments == null)
                merchantInDb.Payments = new List<Payment>();

            foreach (var locationId in incomingLocationIds)
            {
                merchantInDb.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    Processor = PaymentProcessor.MasterCard,
                    LastUpdate = DateTime.UtcNow,
                    PaymentMids = new Dictionary<string, string>
                            {
                                {MerchantConstants.MCLocationId, locationId}
                            },
                    IsActive = true                    
                });
            }
        }

        private bool CopyMerchantInfoFromClearingIfNeeded(Merchant merchantInDb, Merchant merchantFromFile)
        {
            bool merchantUpdated = false;
            if (String.Compare(merchantInDb.Name.Trim(), merchantFromFile.Name.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
            {
                merchantInDb.Name = merchantFromFile.Name.Trim();
                merchantUpdated = true;
            }
            if (String.Compare(merchantInDb.Location.Address.Trim(), merchantFromFile.Location.Address.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
            {
                merchantInDb.Location.Address = merchantFromFile.Location.Address.Trim();
                merchantUpdated = true;
            }
            if (String.Compare(merchantInDb.Location.City.Trim(), merchantFromFile.Location.City.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
            {
                merchantInDb.Location.City = merchantFromFile.Location.City.Trim();
                merchantUpdated = true;
            }
            if (String.Compare(merchantInDb.Location.State.Trim(), merchantFromFile.Location.State.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
            {
                merchantInDb.Location.State = merchantFromFile.Location.State.Trim();
                merchantUpdated = true;
            }
            if (String.Compare(merchantInDb.Location.Zip.Trim(), merchantFromFile.Location.Zip.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
            {
                merchantInDb.Location.Zip = merchantFromFile.Location.Zip.Trim();
                merchantUpdated = true;
            }
            if (String.Compare(merchantInDb.PhoneNumber, merchantFromFile.PhoneNumber, StringComparison.OrdinalIgnoreCase) != 0)
            {
                merchantInDb.PhoneNumber = merchantFromFile.PhoneNumber;
                merchantUpdated = true;
            }           

            return merchantUpdated;
        }

        public bool GeoCodeMerchantLocation(Merchant merchantInDb)
        {
            bool geocoded = false;
            if (merchantInDb.IsAddressAvailable() && !merchantInDb.IsLocationGeocoded())
            {
                var geocodeResult = Geocoding.GetLocation(merchantInDb.Location.Address, merchantInDb.Location.State, merchantInDb.Location.Zip, merchantInDb.Location.City);
                if (geocodeResult != null)
                {
                    if (merchantInDb.ExtendedAttributes == null)
                        merchantInDb.ExtendedAttributes = new Dictionary<string, string>();

                    merchantInDb.ExtendedAttributes.Add(MerchantConstants.BingAddress, geocodeResult.address.formattedAddress);
                    merchantInDb.Location.Latitude = geocodeResult.point.coordinates[0];
                    merchantInDb.Location.Longitude = geocodeResult.point.coordinates[1];
                    geocoded = true;
                }
            }

            return geocoded;
        }

        private async Task<bool> ScheduleVisaLookupJob()
        {
            bool runVisaLookup;
            if (!this.jobInfo.JobPayload.ContainsKey(JobConstants.RunVisaLookup))
            {
                Log.Info(
                    $"{nameof(ProvisionMasterCardMids)} payload does not have visalookup flag. Visa MID lookup job will not be scheduled");
                return false;
            }
            if (!bool.TryParse(this.jobInfo.JobPayload[JobConstants.RunVisaLookup], out runVisaLookup))
            {
                Log.Info($"{nameof(ProvisionMasterCardMids)} payload has an invalid value for visalookup flag. Value is {this.jobInfo.JobPayload[JobConstants.RunVisaLookup]}. Visa MID lookup job will not be scheduled");
                return false;
            }

            if (!runVisaLookup)
            {
                Log.Info("Visa MID lookup job will not be scheduled");
                return false;
            }

            Log.Info("Visa MID lookup job will be scheduled after mastercard data is imported");

            if (azureScheduler != null && schedulerQueueInfo != null)
            {
                Log.Info("Scheduling Visa merchant lookup job for provider : {0}", provider.Id);
                IDictionary<string, string> jobPayload = new Dictionary<string, string>
                {
                    {JobConstants.ProviderId, provider.Id}
                };
                HttpStatusCode statusCode = await ScheduleNextJob(JobType.ProvisionVisaMid, jobPayload).ConfigureAwait(false);
                if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created)
                {
                    Log.Info("Successfully scheduled visa lookup job");
                    return true;
                }
            }
            else
            {
                Log.Warn("Unable to schedule visalookup job. Azure scheduler not initialized");
            }

            return false;
        }

        private async Task<HttpStatusCode> ScheduleNextJob(JobType jobType, IDictionary<string, string> jobPayload)
        {
            ScheduledJobInfo scheduledJobInfo = new ScheduledJobInfo
            {
                JobId = Guid.NewGuid().ToString(),
                JobScheduledTime = DateTime.UtcNow,
                JobType = jobType,
                JobPayload = jobPayload
            };
            HttpStatusCode scheduleJobTask = await azureScheduler.ScheduleQueueTypeJobAsync(schedulerQueueInfo.AccountName, schedulerQueueInfo.QueueName, schedulerQueueInfo.SasToken,
                 JsonConvert.SerializeObject(scheduledJobInfo),
                 scheduledJobInfo.JobId);

            return scheduleJobTask;
        }
    }
}