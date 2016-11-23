//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.SftpClient;
using Lomo.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfferManagement.BingMapClient;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using OfferManagement.MerchantFeedParser.RewardNetworks;
using OfferManagement.MerchantFileParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OfferManagement.JobProcessor.Jobs
{
    public class ProvisionRewardNetworkMerchants : IScheduledJob
    {
        private const string ProviderId = "ProviderId";
        Provider provider = null;        
        private IEnumerable<Merchant> merchantsInDb;        
        string feedFile = null;
        HttpClient httpClient = new HttpClient();

        public async Task ExecuteAsync(ScheduledJobInfo scheduledJobInfo)
        {
            IDictionary<string, string> payload = scheduledJobInfo.JobPayload;
            if (payload == null)
            {
                throw new ArgumentNullException($"VisaMerchantLookupJob {scheduledJobInfo.JobId} has an empty payload");
            }
            if (!payload.ContainsKey(ProviderId))
            {
                throw new ArgumentException(
                    $"Payload for VisaMerchantLookupJob {scheduledJobInfo.JobId} is missing the provider id");
            }

            if (string.IsNullOrWhiteSpace(payload[ProviderId]))
            {
                throw new ArgumentException($"ProviderId is empty in VisaMerchantLookupJob {scheduledJobInfo.JobId}");
            }

            provider = await EarnRepository.Instance.GetProviderAsync(payload[ProviderId]).ConfigureAwait(false);
            if (provider == null)
            {
                throw new Exception($"Provider id {payload[ProviderId]} not found");
            }
            string feedInformation = null;
            if (provider.ExtendedAttributes != null && provider.ExtendedAttributes.ContainsKey(JobConstants.FeedInformation))
            {
                feedInformation = provider.ExtendedAttributes[JobConstants.FeedInformation];
            }
            if (string.IsNullOrEmpty(feedInformation))
            {
                throw new Exception($"Reward network feed information is missing in {provider.Id}");
            }
            RewardNetworkFeedInformation rnFeedInformation = JsonConvert.DeserializeObject<RewardNetworkFeedInformation>(feedInformation);
            if (string.IsNullOrWhiteSpace(rnFeedInformation.Url))
            {
                throw new Exception($"Feed information is missing the reward network url");
            }
            if (string.IsNullOrWhiteSpace(rnFeedInformation.UserName))
            {
                throw new Exception($"Feed information is missing the reward network username");
            }
            if (string.IsNullOrWhiteSpace(rnFeedInformation.Password))
            {
                throw new Exception($"Feed information is missing the reward network password");
            }

            MemoryStream ms = await DownloadFeedFileAsync(rnFeedInformation).ConfigureAwait(false);
            IList<Merchant> lstMerchants = await ParseFeedFileAsync(ms).ConfigureAwait(false);
            if (lstMerchants != null)
            {
                IList<Merchant> lstNewMerchants = await LookForNewMerchantsAsync(provider, lstMerchants).ConfigureAwait(false);
                if (lstNewMerchants.Any())
                {
                    await EnrichMerchantInformationAsync(lstMerchants, rnFeedInformation).ConfigureAwait(false);
                    await EarnRepository.Instance.AddMerchantsInBatchAsync(lstNewMerchants).ConfigureAwait(false);
                    Log.Info("Finished adding reward network merchants to db");
                }
            }
            scheduledJobInfo.JobCompletedTime = DateTime.UtcNow;
        }

        public async Task<MemoryStream> DownloadFeedFileAsync(RewardNetworkFeedInformation rnFeedInformation)
        {          
            feedFile = string.Format(rnFeedInformation.FeedFileName, DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd"));
            Log.Info($"Downloading reward network feed file {feedFile} from {rnFeedInformation.Url}");
            MemoryStream rewardNetworkDataStream = new MemoryStream();
            DefaultSftpClient sftpClient = new DefaultSftpClient(rnFeedInformation.UserName, rnFeedInformation.Password, rnFeedInformation.Url);
            await sftpClient.DownloadFileAsync(feedFile, rewardNetworkDataStream, rnFeedInformation.FeedFolder);            
            Log.Info($"Finished downloading reward network feed file {feedFile} from {rnFeedInformation.Url}");

            return rewardNetworkDataStream;
        }

        public async Task<IList<Merchant>> ParseFeedFileAsync(MemoryStream ms)
        {         
            RewardNetworkParser rnParser = new RewardNetworkParser();
            var merchants = await rnParser.ImportMerchantsAsync(ms).ConfigureAwait(false);
            if (merchants != null)
            {
                Log.Info($"Total Merchants parsed from reward network file {feedFile} is {merchants.Count()}");              
            }

            return merchants;
        }

        public async Task<IList<Merchant>> LookForNewMerchantsAsync(Provider provider, IList<Merchant> merchants)
        {
            Log.Info($"Getting all merchants for provider {provider.Id} from db");
            merchantsInDb = await EarnRepository.Instance.GetMerchantsForProviderAsync(provider.Id).ConfigureAwait(false);
            Log.Info($"Got {merchantsInDb.Count()} merchant entries for provider {provider.Id} from db");
            List<Merchant> lstNewMerchants = new List<Merchant>();

            foreach (var merchant in merchants)
            {
                //TODO: Write code to handle merchant updates
                if (IsNewMerchant(merchant))
                {
                    merchant.ProviderId = provider.Id;
                    if (merchant.ExtendedAttributes == null)
                        merchant.ExtendedAttributes = new Dictionary<string, string>();
                    string masterCardId = MasterCardIdGenerator.GetUniqueId(provider, merchant, "R");
                    merchant.ExtendedAttributes.Add(MerchantConstants.MCID, masterCardId);
                    lstNewMerchants.Add(merchant);
                    GeoCodeMerchantLocation(merchant);
                }
            }

            Log.Info($"Total New Merchants in reward network file {feedFile} is {lstNewMerchants.Count()}");

            return lstNewMerchants;
        }

        public async Task EnrichMerchantInformationAsync(IList<Merchant> lstMerchants, RewardNetworkFeedInformation rnFeedInformation)
        {
            string accessToken = GetAccessToken(rnFeedInformation.AuthTokenUrl, rnFeedInformation.ApiKey);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                foreach (var newMerchant in lstMerchants)
                {
                    Uri rewardNetworkUri = new Uri(string.Format(rnFeedInformation.MerchantInfoUrl, newMerchant.PartnerMerchantId));
                    RewardNetworkMerchant rnMerchant = await GetRestaurantInfoAsync(rewardNetworkUri, accessToken).ConfigureAwait(false);
                    if (rnMerchant?.merchantContent?.photos.Count() > 0)
                    {
                        foreach (string imageFileName in rnMerchant.merchantContent.photos)
                        {
                            if (newMerchant.Images == null)
                                newMerchant.Images = new List<Image>();
                            newMerchant.Images.Add(new Image
                            {
                                Url = string.Format(rnFeedInformation.ImageUrl, imageFileName),
                                Status = ImageStatusType.GoodImage
                            });
                        }
                    }
                    if (rnMerchant?.merchantDetails?.cuisines.Count() > 0 && !newMerchant.ExtendedAttributes.ContainsKey(MerchantConstants.RewardNetworkCuisine))
                    {
                        newMerchant.ExtendedAttributes.Add(MerchantConstants.RewardNetworkCuisine,
                            string.Join("|", rnMerchant.merchantDetails.cuisines));
                    }
                }
            }
        }

        public bool IsNewMerchant(Merchant merchant)
        {
            bool isNewMerchant = true;

            if (merchantsInDb.Any())
            {
                Merchant merchantInDb = merchantsInDb.FirstOrDefault(m => m.PartnerMerchantId == merchant.PartnerMerchantId);
                isNewMerchant = merchantInDb == null;
            }

            return isNewMerchant;
        }

        public void GeoCodeMerchantLocation(Merchant merchant)
        {            
            if (merchant.IsAddressAvailable() && !merchant.IsLocationGeocoded())
            {
                try
                {
                    var geocodeResult = Geocoding.GetLocation(merchant.Location.Address, merchant.Location.State, merchant.Location.Zip, merchant.Location.City);
                    if (geocodeResult != null)
                    {
                        if (merchant.ExtendedAttributes == null)
                            merchant.ExtendedAttributes = new Dictionary<string, string>();

                        merchant.ExtendedAttributes.Add(MerchantConstants.BingAddress, geocodeResult.address.formattedAddress);
                        merchant.Location.Latitude = geocodeResult.point.coordinates[0];
                        merchant.Location.Longitude = geocodeResult.point.coordinates[1];
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception, $"Error in geocoding merchant {merchant.Name}");
                }
            }
        }

        private string GetAccessToken(string authTokenUrl, string apiKey)
        {
            string accessToken = null;
            Uri rewardNetworkUri = new Uri(authTokenUrl);
            var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
            HttpContent content = new FormUrlEncodedContent(requestParams);
            try
            {
                HttpResponseMessage responseMessage = httpClient.PostAsync(rewardNetworkUri, content).Result;
                string responseContent = responseMessage.Content.ReadAsStringAsync().Result;
                JObject jresponse = JObject.Parse(responseContent);
                accessToken = jresponse["access_token"].Value<string>();
            }
            catch(Exception exception)
            {
                Log.Error(exception, "Error in getting the access token from reward network");
            }

            return accessToken;
        }

        private async Task<RewardNetworkMerchant> GetRestaurantInfoAsync(Uri rewardNetworkUri, string accessToken)
        {            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await httpClient.GetAsync(rewardNetworkUri).ConfigureAwait(false);
            string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            RewardNetworkMerchant rewardNetworkMerchant = JsonConvert.DeserializeObject<RewardNetworkMerchant>(responseContent, settings);

            return rewardNetworkMerchant;
        }
    }

}