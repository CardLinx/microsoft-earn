//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Utils.Interface;
using Lomo.Logging;
using OfferManagement.BingMapClient;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using OfferManagement.MerchantFileParser;

namespace OfferManagement.JobProcessor.Jobs
{
    public class ProvisionAmexMids : IScheduledJob
    {
        private readonly IAzureBlob azureBlob;
        private Provider provider;
        private string merchantFileName;
        private string blobContainer;
        private IEnumerable<Merchant> merchantsInDb;
        private string author;

        public ProvisionAmexMids(IAzureBlob azureBlob)
        {
            this.azureBlob = azureBlob;
        }

        public async Task ExecuteAsync(ScheduledJobInfo scheduledJobInfo)
        {
            // validate the job info
            IDictionary<string, string> payload = scheduledJobInfo.JobPayload;
            if (payload == null)
            {
                throw new ArgumentNullException($"ProvisionAmexMidsJob {scheduledJobInfo.JobId} has an empty payload");
            }
            if (!payload.ContainsKey(JobConstants.ContainerName))
            {
                throw new ArgumentException($"ProvisionAmexMidsJob {scheduledJobInfo.JobId} is missing the blob container name");
            }
            blobContainer = payload[JobConstants.ContainerName];
            if (string.IsNullOrWhiteSpace(blobContainer))
            {
                throw new ArgumentException($"ProvisionAmexMidsJob {scheduledJobInfo.JobId} has invalid blob container name");
            }
            if (!payload.ContainsKey(JobConstants.BlobName))
            {
                throw new ArgumentException($"ProvisionAmexMidsJob {scheduledJobInfo.JobId} is missing the blob name");
            }
            merchantFileName = payload[JobConstants.BlobName];
            if (string.IsNullOrWhiteSpace(merchantFileName))
            {
                throw new ArgumentException($"ProvisionAmexMidsJob {scheduledJobInfo.JobId} has invalid blob name");
            }
            if (!payload.ContainsKey(JobConstants.ProviderId))
            {
                throw new ArgumentException($"ProvisionAmexMidsJob {scheduledJobInfo.JobId} is missing the provider id");
            }
            string providerId = payload[JobConstants.ProviderId];
            if (string.IsNullOrWhiteSpace(providerId))
            {
                throw new ArgumentException($"ProvisionAmexMidsJob {scheduledJobInfo.JobId} has invalid provider id");
            }
            provider = await EarnRepository.Instance.GetProviderAsync(providerId).ConfigureAwait(false);
            if (provider == null)
            {
                throw new ArgumentException($"ProvisionAmexMidsJob {scheduledJobInfo.JobId}: Provider {providerId} does not exist");
            }
            if (payload.ContainsKey(JobConstants.Author))
            {
                author = payload[JobConstants.Author];
            }

            // process the job
            MemoryStream memoryStream = await DownloadMerchantFileFromBlobAsync().ConfigureAwait(false);
            Tuple<IList<Merchant>, IList<Merchant>> merchants = await ProcessMerchantFileAsync(memoryStream).ConfigureAwait(false);

            // Need to do Visa lookup for the newly added merchants
            //if (merchants.Item1.Any())
            //{
            //    Log.Info($"Total Merchants to add : {merchants.Item1.Count}");
            //    // need to validate before creating new merchants
            //    await EarnRepository.Instance.AddMerchantsInBatchAsync(merchants.Item1).ConfigureAwait(false);
            //    Log.Info("Finished adding merchants");
            //}

            if (merchants.Item2.Any())
            {
                Log.Info($"Total Merchants to update : {merchants.Item2.Count}");
                await EarnRepository.Instance.UpdateAsync(merchants.Item2).ConfigureAwait(false);
                Log.Info("Finished updating merchants");
            }

            scheduledJobInfo.JobCompletedTime = DateTime.UtcNow;
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
            IList<Merchant> importedMerchants = null;
            MerchantFileProcessor merchantFileProcessor = MerchantProcessorFactory.GetMerchantFileProcessor(merchantFileName);
            importedMerchants = merchantFileProcessor.ImportAmexMidFile(memoryStream);

            if (importedMerchants == null || !importedMerchants.Any())
            {
                throw new Exception($"ProvisionAmexMidsJob: An error occurred while processing Amex import file - No Data");
            }

            Log.Info($"ProvisionAmexMidsJob: Getting all merchants for provider {provider.Id} from db");
            merchantsInDb = await EarnRepository.Instance.GetMerchantsForProviderAsync(provider.Id).ConfigureAwait(false);
            Log.Info($"ProvisionAmexMidsJob: Got {merchantsInDb.Count()} merchant entries for provider {provider.Id} from db");
            // list of merchants to add and merchants to update
            Tuple<IList<Merchant>, IList<Merchant>> sortedMerchants = AddOrUpdateMerchant(importedMerchants);

            return sortedMerchants;
        }

        private Tuple<IList<Merchant>, IList<Merchant>> AddOrUpdateMerchant(IList<Merchant> merchantsFromFile)
        {
            IList<Merchant> merchantsToAdd = new List<Merchant>();
            IList<Merchant> merchantsToUpdate = new List<Merchant>();
            if (merchantsInDb == null)
            {
                merchantsInDb = new List<Merchant>();
            }

            Merchant merchantInDb;
            foreach (var merchantFromFile in merchantsFromFile)
            {
                merchantInDb = merchantsInDb.FirstOrDefault(m => m.Id == merchantFromFile.Id);
                if (merchantInDb != null)
                {
                    if (merchantInDb.Payments == null)
                    {
                        merchantInDb.Payments = new List<Payment>();
                    }

                    Log.Info($"ProvisionAmexMidsJob: updating merchant {merchantFromFile.Id}");
                    List<string> existingMids = new List<string>();

                    // To concatenate the existing SENumbers
                    var existingAmexMids = merchantInDb.Payments.Where(p => p.Processor == PaymentProcessor.Amex);
                    if (existingAmexMids.Any())
                    {
                        //Get the existing Mids
                        existingMids.AddRange(from existingMid in existingAmexMids
                                              where existingMid.PaymentMids != null &&
                                              existingMid.PaymentMids.ContainsKey(MerchantConstants.AmexSENumber)
                                              select existingMid.PaymentMids[MerchantConstants.AmexSENumber]);
                    }

                    var incomingMids = merchantFromFile.Payments;
                    List<string> incomingAmexMids = new List<string>();
                    if (incomingMids.Any())
                    {
                        //Get the incoming Mids
                        incomingAmexMids.AddRange(from incomingMid in incomingMids
                                                  where incomingMid.PaymentMids != null &&
                                                  incomingMid.PaymentMids.ContainsKey(MerchantConstants.AmexSENumber)
                                                  select incomingMid.PaymentMids[MerchantConstants.AmexSENumber]);
                    }

                    var newMids = incomingAmexMids.Except(existingMids);
                    if (newMids.Any())
                    {
                        foreach (var newMid in newMids)
                        {
                            merchantInDb.Payments.Add(CreatePayment(newMid));
                        }

                        merchantsToUpdate.Add(merchantInDb);
                    }
                }
                else
                {
                    Log.Info($"ProvisionAmexMidsJob: New merchant to be created {merchantFromFile.Id}");
                    if (merchantFromFile.Payments.Any())
                    {
                        Merchant merchant = new Merchant
                        {
                            Id = Guid.NewGuid().ToString(),
                            ProviderId = provider.Id,
                            Name = merchantFromFile.Name,
                            PhoneNumber = merchantFromFile.PhoneNumber,
                            Location = merchantFromFile.Location,
                            Author = author,
                            Payments = new List<Payment>()
                        };

                        foreach (var payment in merchantFromFile.Payments)
                        {
                            if (payment.PaymentMids.ContainsKey(MerchantConstants.AmexSENumber))
                            {
                                merchant.Payments.Add(CreatePayment(payment.PaymentMids[MerchantConstants.AmexSENumber]));
                            }
                        }

                        GeoCodeMerchantLocation(merchant);
                        merchantsToAdd.Add(merchant);
                    }
                }
            }

            return new Tuple<IList<Merchant>, IList<Merchant>>(merchantsToAdd, merchantsToUpdate);
        }

        private Payment CreatePayment(string seNumber)
        {
            return new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Processor = PaymentProcessor.Amex,
                LastUpdate = DateTime.UtcNow,
                PaymentMids = new Dictionary<string, string>
                                {
                                    {MerchantConstants.AmexSENumber, seNumber}
                                },
                IsActive = true
            };
        }

        private bool GeoCodeMerchantLocation(Merchant merchantInDb)
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