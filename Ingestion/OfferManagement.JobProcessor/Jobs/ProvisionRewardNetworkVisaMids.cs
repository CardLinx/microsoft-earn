//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Azure.Utils.Interface;
using Lomo.Logging;
using OfferManagement.BingMapClient;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using OfferManagement.MerchantFileParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OfferManagement.JobProcessor.Jobs
{
    public class ProvisionRewardNetworkVisaMids : IScheduledJob
    {
        private const string ProviderId = "ProviderId";
        private IAzureBlob azureBlob;
        private ScheduledJobInfo jobInfo;
        private IEnumerable<Merchant> merchantsInDb;
        private Provider provider;
        private string merchantFileName;
        private string blobContainer;

        public ProvisionRewardNetworkVisaMids(IAzureBlob azureBlob)
        {
            this.azureBlob = azureBlob;
        }

        public async Task ExecuteAsync(ScheduledJobInfo scheduledJobInfo)
        {
            this.jobInfo = scheduledJobInfo;
            await ValidatePayload().ConfigureAwait(false);
            MemoryStream memoryStream = await DownloadMerchantFileFromBlobAsync().ConfigureAwait(false);
            IList<Merchant> lstMerchants = await ProcessMerchantFileAsync(memoryStream).ConfigureAwait(false);

            if (lstMerchants.Any())
            {
                Log.Info($"Total Merchants to update : {lstMerchants.Count}");
                await EarnRepository.Instance.UpdateAsync(lstMerchants).ConfigureAwait(false);
                Log.Info("Finished updating merchants");
            }

            scheduledJobInfo.JobCompletedTime = DateTime.UtcNow;
        }

        private async Task ValidatePayload()
        {
            IDictionary<string, string> payload = this.jobInfo.JobPayload;
            if (payload == null)
            {
                throw new ArgumentNullException($"{nameof(ProvisionRewardNetworkVisaMids)} {this.jobInfo.JobId} has an empty payload");
            }
            if (!payload.ContainsKey(JobConstants.ContainerName))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionRewardNetworkVisaMids)} {this.jobInfo.JobId} is missing the blob container name");
            }
            blobContainer = payload[JobConstants.ContainerName];
            if (string.IsNullOrWhiteSpace(blobContainer))
            {
                throw new ArgumentException($"Blob Container is empty in {nameof(ProvisionRewardNetworkVisaMids)} {this.jobInfo.JobId}");
            }

            if (!payload.ContainsKey(JobConstants.BlobName))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionRewardNetworkVisaMids)} {this.jobInfo.JobId} is missing the blob name");
            }
            merchantFileName = payload[JobConstants.BlobName];
            if (string.IsNullOrWhiteSpace(merchantFileName))
            {
                throw new ArgumentException($"Merchant file name is empty in {nameof(ProvisionRewardNetworkVisaMids)} {this.jobInfo.JobId}");
            }

            if (!payload.ContainsKey(JobConstants.ProviderId))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionRewardNetworkVisaMids)} {this.jobInfo.JobId} is missing the provider id for the merchants");
            }

            string providerId = payload[JobConstants.ProviderId];
            if (string.IsNullOrWhiteSpace(providerId))
            {
                throw new ArgumentException($"ProviderId is empty in {nameof(ProvisionRewardNetworkVisaMids)} {this.jobInfo.JobId}");
            }

            provider = await EarnRepository.Instance.GetProviderAsync(providerId).ConfigureAwait(false);
            if (provider == null)
            {
                throw new ArgumentException($"Provider {providerId} does not exist");
            }
        }

        private async Task<MemoryStream> DownloadMerchantFileFromBlobAsync()
        {
            Log.Info("Getting merchant file {0} from blob container {0}", merchantFileName, blobContainer);
            MemoryStream memoryStream = await this.azureBlob.DownloadBlobToStreamAsync(blobContainer, merchantFileName).ConfigureAwait(false);
            Log.Info("Successfully downloaded merchant file {0} from blob container {0}", merchantFileName, blobContainer);
            memoryStream.Position = 0;

            return memoryStream;
        }

        private async Task<IList<Merchant>> ProcessMerchantFileAsync(MemoryStream memoryStream)
        {
            IList<Merchant> lstImportedMerchants = null;
            MerchantFileProcessor merchantFileProcessor = MerchantProcessorFactory.GetMerchantFileProcessor(merchantFileName);
            lstImportedMerchants = await Task.Run(() => merchantFileProcessor.ImportVisaMidFile(memoryStream)).ConfigureAwait(false);
            if (lstImportedMerchants == null || !lstImportedMerchants.Any())
            {
                throw new Exception($"Error in processing the visa mid file.");
            }
            Log.Info($"Total unique records imported from the visa mid file is {lstImportedMerchants.Count}");

            Log.Info($"Getting all merchants for provider {provider.Id} from db");
            merchantsInDb = await EarnRepository.Instance.GetMerchantsForProviderAsync(provider.Id).ConfigureAwait(false);
            Log.Info($"Got {merchantsInDb.Count()} merchant entries for provider {provider.Id} from db");

            IList<Merchant> lstMerchants = await Task.Run(() => UpdateVisaPayment(lstImportedMerchants)).ConfigureAwait(false);

            return lstMerchants;
        }

        private IList<Merchant> UpdateVisaPayment(IList<Merchant> merchantsFromFile)
        {
            IList<Merchant> merchantsToUpdate = new List<Merchant>();
            Merchant merchantInDb = null;

            foreach (var merchantFromFile in merchantsFromFile)
            {
                merchantInDb = GetMerchantFromDb(merchantFromFile);
                if (merchantInDb != null)
                {
                    bool merchantUpdated = false;
                    if (GeoCodeMerchantLocation(merchantInDb))
                    {
                        merchantUpdated = true;
                    }
                    merchantUpdated = UpdateVisaStoreNamesIfNeeded(merchantInDb, merchantFromFile);
                    merchantUpdated = UpdateVisaPaymentDetails(merchantInDb, merchantFromFile, merchantsToUpdate);

                    if (merchantUpdated)
                    {
                        merchantsToUpdate.Add(merchantInDb);
                    }
                }
            }

            return merchantsToUpdate;
        }

        private Merchant GetMerchantFromDb(Merchant merchantFromFile)
        {
            // Locate the merchant in the db with the same PartnerMerchantId as the incoming merchant.      
            // PartnerMerchantId for RN is of 9 characters in length. Make sure to pad them with zeros        
            Merchant locatedMerchant = null;
            string partnerMerchantIdFromFile = merchantFromFile.PartnerMerchantId.PadLeft(9, '0');
            Log.Info($"Checking for matching merchant in db with PartnerMerchantId {partnerMerchantIdFromFile}");
            locatedMerchant = merchantsInDb.FirstOrDefault(m => m.PartnerMerchantId == partnerMerchantIdFromFile);

            if (locatedMerchant == null)
            {
                Log.Warn($"Merchant with PartnerMerchantId {partnerMerchantIdFromFile} is not found in the db");
            }

            return locatedMerchant;
        }

        private bool UpdateVisaStoreNamesIfNeeded(Merchant merchantInDb, Merchant merchantFromFile)
        {
            bool merchantUpdated = false;
            if (merchantInDb.ExtendedAttributes == null)
            {
                merchantInDb.ExtendedAttributes = merchantFromFile.ExtendedAttributes;
                merchantUpdated = true;
            }
            else
            {
                string visaMidNameInDb = merchantInDb.ExtendedAttributes.ContainsKey(MerchantConstants.VisaMidName) ? merchantInDb.ExtendedAttributes[MerchantConstants.VisaMidName] : null;
                string visaMidNameInFile = merchantFromFile.ExtendedAttributes.ContainsKey(MerchantConstants.VisaMidName) ? merchantFromFile.ExtendedAttributes[MerchantConstants.VisaMidName] : null;
                if (string.IsNullOrWhiteSpace(visaMidNameInDb))
                {
                    merchantInDb.ExtendedAttributes.Add(MerchantConstants.VisaMidName, visaMidNameInFile);
                    merchantUpdated = true;
                }
                else if (string.Compare(visaMidNameInDb.Trim(), visaMidNameInFile.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                {
                    merchantInDb.ExtendedAttributes[MerchantConstants.VisaMidName] = visaMidNameInFile;
                    merchantUpdated = true;
                }

                string visaSidNameInDb = merchantInDb.ExtendedAttributes.ContainsKey(MerchantConstants.VisaSidName) ? merchantInDb.ExtendedAttributes[MerchantConstants.VisaSidName] : null;
                string visaSidNameInFile = merchantFromFile.ExtendedAttributes.ContainsKey(MerchantConstants.VisaSidName) ? merchantFromFile.ExtendedAttributes[MerchantConstants.VisaSidName] : null;
                if (string.IsNullOrWhiteSpace(visaSidNameInDb))
                {
                    merchantInDb.ExtendedAttributes.Add(MerchantConstants.VisaSidName, visaSidNameInFile);
                    merchantUpdated = true;
                }
                else if (string.Compare(visaSidNameInDb.Trim(), visaSidNameInFile.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                {
                    merchantInDb.ExtendedAttributes[MerchantConstants.VisaSidName] = visaSidNameInFile;
                    merchantUpdated = true;
                }
            }

            return merchantUpdated;
        }

        private bool UpdateVisaPaymentDetails(Merchant merchantInDb, Merchant merchantFromFile, IList<Merchant> merchantsToUpdate)
        {
            bool merchantUpdated = false;
            merchantInDb.IsActive = true;
            if (GeoCodeMerchantLocation(merchantInDb))
            {
                merchantUpdated = true;
            }

            var incomingPaymentMids = merchantFromFile.Payments;
            List<String> incomingMids = new List<string>();
            if (incomingPaymentMids.Any())
            {
                //Get the Mids from the incoming data
                incomingMids.AddRange(from incomingPaymentMid in incomingPaymentMids
                                      where incomingPaymentMid.PaymentMids != null &&
                                      incomingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.VisaMid)
                                      && incomingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.VisaSid)
                                      select ($"{incomingPaymentMid.PaymentMids[MerchantConstants.VisaMid]};{incomingPaymentMid.PaymentMids[MerchantConstants.VisaSid]}"));
            }


            //Get the Visa MID's that we already have for this merchant in the db.
            var existingPaymentMids = merchantInDb.Payments?.Where(payment => payment.Processor == PaymentProcessor.Visa);

            List<String> existingVisaMids = new List<string>();
            if (existingPaymentMids != null && existingPaymentMids.Any())
            {
                //Get the existing Visa Mids
                existingVisaMids.AddRange(from existingPaymentMid in existingPaymentMids
                                          where existingPaymentMid.PaymentMids != null &&
                                          existingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.VisaSid)
                                          && existingPaymentMid.PaymentMids.ContainsKey(MerchantConstants.VisaSid)
                                          select ($"{existingPaymentMid.PaymentMids[MerchantConstants.VisaMid]};{existingPaymentMid.PaymentMids[MerchantConstants.VisaSid]}"));
            }

            //If we don't have any Visa Mid's for this merchant in the db and if there are incoming Mid's
            //then add the incoming  Mid to the merchant to be updated to the db
            if (existingPaymentMids == null || !existingPaymentMids.Any())
            {
                if (incomingMids.Any())
                {
                    UpdateMerchantWithMids(merchantInDb, incomingMids);
                    merchantUpdated = true;
                }
            }
            else
            {
                //If we have visa mids for this merchant in the db, then check if there's anything new between the incoming visa mid and the 
                //existing visa mids. If there's a new entry, then add the incoming visa mid to the merchant to be updated to the db
                var newAuthMids = incomingMids.Except(existingVisaMids);
                if (newAuthMids.Any())
                {
                    UpdateMerchantWithMids(merchantInDb, newAuthMids);
                    merchantUpdated = true;
                }
            }

            return merchantUpdated;
        }

        private void UpdateMerchantWithMids(Merchant merchantInDb, IEnumerable<string> newMids)
        {
            if (merchantInDb.Payments == null)
                merchantInDb.Payments = new List<Payment>();

            foreach (var mid in newMids)
            {
                string visaMid = mid.Split(';')[0];
                string visaSid = mid.Split(';')[1];
                merchantInDb.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    Processor = PaymentProcessor.Visa,
                    LastUpdate = DateTime.UtcNow,
                    PaymentMids = new Dictionary<string, string>
                            {
                                {MerchantConstants.VisaMid, visaMid},
                                {MerchantConstants.VisaSid, visaSid}
                            }
                });
            }
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

    }
}