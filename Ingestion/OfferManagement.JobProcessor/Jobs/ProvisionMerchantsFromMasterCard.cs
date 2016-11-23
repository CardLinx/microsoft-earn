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
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.JobProcessor.Jobs
{
    public class ProvisionMerchantsFromMasterCard : IScheduledJob
    {
        private readonly IAzureBlob azureBlob;
        private Provider provider;
        
        public ProvisionMerchantsFromMasterCard(IAzureBlob azureBlob)
        {
            this.azureBlob = azureBlob;
        }

        public async Task ExecuteAsync(ScheduledJobInfo scheduledJobInfo)
        {
            await ValidatePayload(scheduledJobInfo).ConfigureAwait(false);
            IDictionary<string, string> payload = scheduledJobInfo.JobPayload;
            string merchantFileName = payload[JobConstants.BlobName];
            string blobContainer = payload[JobConstants.ContainerName];
            string providerId = payload[JobConstants.ProviderId];

            MemoryStream ms = await GetMerchantsFromBlobAsync(merchantFileName, blobContainer).ConfigureAwait(false);

            MerchantFileProcessor merchantFileProcessor = MerchantProcessorFactory.GetMerchantFileProcessor(merchantFileName);
            Tuple<string, IList<Merchant>> provisioningData = merchantFileProcessor.ImportMasterCardProvisioningFile(ms);
            if (provisioningData == null)
            {
                throw new Exception("Error in processing the mastercard provisioning file. Unable to provision merchants");
            }

            string masterCardMerchantsProvisioningDate = provisioningData.Item1;
            IList<Merchant> lstProvisionedMerchants = provisioningData.Item2;
            if (lstProvisionedMerchants.Any())
            {
                //Update the provider with the MasterCard merchant provisioning file date info. This is needed
                //at the time of exporting the file back to MasterCard.
                if (provider.ExtendedAttributes == null)
                {
                    provider.ExtendedAttributes = new Dictionary<string, string>();                    
                }
                if (!provider.ExtendedAttributes.ContainsKey(MerchantConstants.MCProvisioningDate))
                    provider.ExtendedAttributes.Add(MerchantConstants.MCProvisioningDate, masterCardMerchantsProvisioningDate);
                else
                    provider.ExtendedAttributes[MerchantConstants.MCProvisioningDate] = masterCardMerchantsProvisioningDate;
                await EarnRepository.Instance.UpdateAsync<Provider>(new List<Provider> { provider });
                Log.Info($"Updated the provider {providerId} with the MasterCard provisioning date");

                //Add the new merchants to db.
                //TODO: As of now, the code can handle only new merchant additions from the MC file. 
                //Need to add support to handle updates and deletes

                foreach(Merchant merchant in lstProvisionedMerchants)
                {
                    merchant.Id = Guid.NewGuid().ToString();
                    merchant.ProviderId = provider.Id;
                    GeoCodeMerchantLocation(merchant);
                    if (merchant.ExtendedAttributes == null)
                    {
                        merchant.ExtendedAttributes = new Dictionary<string, string>();
                    }
                    string masterCardId = MasterCardIdGenerator.GetUniqueId(provider, merchant, "P");
                    Log.Info($"Generated mastercardid {masterCardId} for merchant {merchant.Name}");
                    merchant.ExtendedAttributes.Add(MerchantConstants.MCID, masterCardId);
                }

                Log.Info($"Total Merchants to add to db {lstProvisionedMerchants.Count}");
                await EarnRepository.Instance.AddMerchantsInBatchAsync(lstProvisionedMerchants);
                Log.Info($"Successfully added {lstProvisionedMerchants.Count} merchants to db");

                scheduledJobInfo.JobCompletedTime = DateTime.UtcNow;
            }
            else
            {
                Log.Warn("Provisioning file from MasterCard has 0 merchants");
            } 
        }

        private async Task ValidatePayload(ScheduledJobInfo scheduledJobInfo)
        {
            IDictionary<string, string> payload = scheduledJobInfo.JobPayload;
            if (payload == null)
            {
                throw new ArgumentNullException($"{nameof(ProvisionMerchantsFromMasterCard)} {scheduledJobInfo.JobId} has an empty payload");
            }
            if (!payload.ContainsKey(JobConstants.ContainerName))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMerchantsFromMasterCard)} {scheduledJobInfo.JobId} is missing the blob container name");
            }
            if (!payload.ContainsKey(JobConstants.BlobName))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMerchantsFromMasterCard)} {scheduledJobInfo.JobId} is missing the blob name");
            }
            if (!payload.ContainsKey(JobConstants.ProviderId))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMerchantsFromMasterCard)} {scheduledJobInfo.JobId} is missing the provider id for the merchants");
            }

            string providerId = payload[JobConstants.ProviderId];
            if (string.IsNullOrWhiteSpace(providerId))
            {
                throw new ArgumentException($"ProviderId is empty in {nameof(ProvisionMerchantsFromMasterCard)} {scheduledJobInfo.JobId}");
            }
            
            provider = await EarnRepository.Instance.GetProviderAsync(providerId).ConfigureAwait(false);            
            if (provider == null)
            {
                throw new ArgumentException($"Provider {providerId} does not exist");                
            }

            if (!payload.ContainsKey(JobConstants.MerchantFileType))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMerchantsFromMasterCard)} {scheduledJobInfo.JobId} is missing the merchant file type info for the merchants");
            }

            MerchantFileType merchantFileType;
            if (!Enum.TryParse<MerchantFileType>(payload[JobConstants.MerchantFileType], out merchantFileType))
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMerchantsFromMasterCard)} {scheduledJobInfo.JobId} has an invalid merchant filetype info for the merchants");
            }
            if (merchantFileType != DataModel.MerchantFileType.MasterCardProvisioning)
            {
                throw new ArgumentException(
                    $"Payload for {nameof(ProvisionMerchantsFromMasterCard)} {scheduledJobInfo.JobId} has an invalid value for merchant filetype info for the merchants");
            }
        }
               
        private async Task<MemoryStream> GetMerchantsFromBlobAsync(string merchantFileName, string blobContainer)
        {            
            Log.Info("Getting merchant file {0} from blob container {0}", merchantFileName, blobContainer);
            MemoryStream memoryStream = await azureBlob.DownloadBlobToStreamAsync(blobContainer, merchantFileName).ConfigureAwait(false);
            Log.Info("Successfully downloaded merchant file {0} from blob container {0}", merchantFileName, blobContainer);
            memoryStream.Position = 0;

            return memoryStream;
        }

        private bool GeoCodeMerchantLocation(Merchant merchant)
        {
            bool geocoded = false;
            if (merchant.IsAddressAvailable() && !merchant.IsLocationGeocoded())
            {
                var geocodeResult = Geocoding.GetLocation(merchant.Location.Address, merchant.Location.State, merchant.Location.Zip, merchant.Location.City);
                if (geocodeResult != null)
                {
                    if (merchant.ExtendedAttributes == null)
                        merchant.ExtendedAttributes = new Dictionary<string, string>();

                    merchant.ExtendedAttributes.Add(MerchantConstants.BingAddress, geocodeResult.address.formattedAddress);
                    merchant.Location.Latitude = geocodeResult.point.coordinates[0];
                    merchant.Location.Longitude = geocodeResult.point.coordinates[1];
                    geocoded = true;
                }
            }

            return geocoded;
        }

    }
}
