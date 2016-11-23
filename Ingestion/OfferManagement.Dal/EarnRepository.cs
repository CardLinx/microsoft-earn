//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lomo.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using OfferManagement.DataModel;
using Microsoft.Azure.Documents.Linq;
using System.Diagnostics;

namespace OfferManagement.Dal
{
    public class EarnRepository : DocumentDbRepository, IEarnRepository
    {
        #region Constants

        private const string EarnDbUrlSettingName = "EarnDbUrl";
        private const string EarnDbKeySettingName = "EarnDbKey";
        private const string EarnDbSettingName = "EarnDbName";
        private const string EarnCollectionSettingName = "EarnCollectionName";

        #endregion

        public static EarnRepository Instance = new EarnRepository();

        public EarnRepository() : base(ConfigurationManager.AppSettings[EarnDbUrlSettingName],
            ConfigurationManager.AppSettings[EarnDbKeySettingName],
            ConfigurationManager.AppSettings[EarnDbSettingName],
            ConfigurationManager.AppSettings[EarnCollectionSettingName])
        { }

        public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
        {
            List<Provider> providers = null;            
            try
            {
                SqlParameterCollection parameters = new SqlParameterCollection();
                parameters.Add(new SqlParameter("@type", "Provider"));
                SqlQuerySpec querySpec = new SqlQuerySpec("SELECT * FROM PROVIDERS p WHERE p.type = @type", parameters);
                await ExecuteWithRetries(() => Task.Run(() => providers = Client.CreateDocumentQuery<Provider>(Collection.DocumentsLink, querySpec)
                            .AsEnumerable().ToList()
                )).ConfigureAwait(false);               
                
            }
            catch (Exception e)
            {
                Log.Error(e, $"[{nameof(EarnRepository)}.{nameof(GetAllProvidersAsync)}]");
            }

            return providers;
        }

        public async Task<Provider> GetProviderAsync(string providerId)
        {
            Provider provider = null;
            try
            {
                SqlParameterCollection parameters = new SqlParameterCollection();
                parameters.Add(new SqlParameter("@type", "Provider"));
                parameters.Add(new SqlParameter("@providerid", providerId));
                SqlQuerySpec querySpec = new SqlQuerySpec("SELECT * FROM PROVIDERS p WHERE p.type = @type AND p.id = @providerid", parameters);
                await ExecuteWithRetries(() => Task.Run(() => provider = Client.CreateDocumentQuery<Provider>(Collection.DocumentsLink, querySpec)
                            .AsEnumerable().FirstOrDefault()
                )).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, $"[{nameof(EarnRepository)}.{nameof(GetProviderAsync)}]");
            }

            return provider;
        }

        public async Task<IEnumerable<Merchant>> GetMerchantsForProviderAsync(string providerId)
        {
            List<Merchant> merchants = new List<Merchant>();
            try
            {
                SqlParameterCollection parameters = new SqlParameterCollection();
                parameters.Add(new SqlParameter("@type", "Merchant"));
                parameters.Add(new SqlParameter("@providerid", providerId));
                SqlQuerySpec querySpec = new SqlQuerySpec("SELECT * FROM PROVIDERS p WHERE p.type = @type AND p.provider_id = @providerid", parameters);
                FeedOptions feedOptions = new FeedOptions
                {
                    MaxItemCount = -1
                };
                await ExecuteWithRetries(() => Task.Run(() => merchants = Client.CreateDocumentQuery<Merchant>(Collection.DocumentsLink, querySpec, feedOptions)
                            .AsEnumerable().ToList()
                )).ConfigureAwait(false);                
            }
            catch (Exception e)
            {
                Log.Error(e, $"[{nameof(EarnRepository)}.{nameof(GetMerchantsForProviderAsync)}]");
            }

            return merchants;
        }

        public async Task<Merchant> GetMerchantByIdAsync(string merchantId)
        {
            Merchant merchant = null;
            try
            {
                SqlParameterCollection parameters = new SqlParameterCollection();
                parameters.Add(new SqlParameter("@type", "Merchant"));
                parameters.Add(new SqlParameter("@merchantid", merchantId));
                SqlQuerySpec querySpec = new SqlQuerySpec("SELECT * FROM PROVIDERS p WHERE p.type = @type AND p.id = @merchantid", parameters);               
                await ExecuteWithRetries(() => Task.Run(() => merchant = Client.CreateDocumentQuery<Merchant>(Collection.DocumentsLink, querySpec)
                            .AsEnumerable().FirstOrDefault()
                )).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, $"[{nameof(EarnRepository)}.{nameof(GetMerchantByIdAsync)}]");
            }

            return merchant;
        }
        
        public async Task<IEnumerable<Merchant>> GetMerchantByNameAsync(string merchantName)
        {
            IEnumerable<Merchant> merchants = null;
            try
            {
                SqlParameterCollection parameters = new SqlParameterCollection();
                parameters.Add(new SqlParameter("@type", "Merchant"));
                parameters.Add(new SqlParameter("@name", merchantName));
                SqlQuerySpec querySpec = new SqlQuerySpec("SELECT * FROM PROVIDERS p WHERE p.type = @type AND p.name = @name", parameters);
                await ExecuteWithRetries(() => Task.Run(() => merchants = Client.CreateDocumentQuery<Merchant>(Collection.DocumentsLink, querySpec)
                            .AsEnumerable()
                )).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, $"[{nameof(EarnRepository)}.{nameof(GetMerchantByNameAsync)}]");
            }

            return merchants;
        }

        public async Task<DataModel.Offer> GetOfferAsync(string offerId)
        {
            DataModel.Offer offer = null;
            try
            {
                SqlParameterCollection parameters = new SqlParameterCollection();
                parameters.Add(new SqlParameter("@type", "Offer"));
                parameters.Add(new SqlParameter("@merchantid", offerId));
                SqlQuerySpec querySpec = new SqlQuerySpec("SELECT * FROM PROVIDERS p WHERE p.type = @type AND p.id = @merchantid", parameters);
                await ExecuteWithRetries(() => Task.Run(() => offer = Client.CreateDocumentQuery<DataModel.Offer>(Collection.DocumentsLink, querySpec)
                            .AsEnumerable().FirstOrDefault()
                )).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, $"[{nameof(EarnRepository)}.{nameof(GetOfferAsync)}]");
            }

            return offer;
        }

        public async Task<IEnumerable<DataModel.Offer>> GetOffersForProviderAsync(string providerId)
        {
            List<DataModel.Offer> offers = new List<DataModel.Offer>();
            try
            {
                SqlParameterCollection parameters = new SqlParameterCollection();
                parameters.Add(new SqlParameter("@type", "Offer"));
                parameters.Add(new SqlParameter("@providerid", providerId));
                SqlQuerySpec querySpec = new SqlQuerySpec("SELECT * FROM PROVIDERS p WHERE p.type = @type AND p.provider_id = @providerid", parameters);
                await ExecuteWithRetries(() => Task.Run(() => offers = Client.CreateDocumentQuery<DataModel.Offer>(Collection.DocumentsLink, querySpec)
                            .AsEnumerable().ToList()
                )).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, $"[{nameof(EarnRepository)}.{nameof(GetOffersForProviderAsync)}]");
            }

            return offers;
        }


        public async Task<int> AddMerchantsInBatchAsync(IList<Merchant> merchants)
        {
            if (merchants == null)
            {
                throw new ArgumentNullException(nameof(merchants));
            }

            if (!merchants.Any())
            {
                throw new ArgumentException("Merchants list is empty", nameof(merchants));
            }

            StoredProcedure storedProcedure = await ExecuteWithRetries(() => CreateOrGetStoredProcAsync(Collection, "AddMerchants", "AddMerchants.js")).ConfigureAwait(false);
            int currentCount = 0;
            int maxMerchantsPerBatch = Math.Min(500, merchants.Count);
            Log.Info($"Total merchants to add in a batch to the db is {maxMerchantsPerBatch}");

            Stopwatch sw = Stopwatch.StartNew();
            while (currentCount < merchants.Count)
            {
                string argsJson = CreatePayloadForAddMerchant(merchants, currentCount, maxMerchantsPerBatch);
                var args = new dynamic[] { JsonConvert.DeserializeObject<dynamic>(argsJson) };
                StoredProcedureResponse<int> scriptResult = await ExecuteWithRetries(() => Client.ExecuteStoredProcedureAsync<int>(storedProcedure.SelfLink, args));

                int currentlyInserted = scriptResult.Response;
                currentCount += currentlyInserted;

                Log.Info($"Added {currentlyInserted} new merchants to db");
            }
            long timeTaken = sw.ElapsedMilliseconds;
            Log.Info($"Total time taken to add {merchants.Count()} merchants is {timeTaken}");

            return currentCount;
        }

        private static string CreatePayloadForAddMerchant(IList<Merchant> merchants, int currentIndex, int maxCountPerBatch)
        {
            int idx = 0;
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var jsonDocumentArray = new StringBuilder();
            jsonDocumentArray.Append("[");
            jsonDocumentArray.Append(JsonConvert.SerializeObject(merchants[currentIndex++], Formatting.None, serializerSettings));
            idx++;

            while (currentIndex < merchants.Count && idx < maxCountPerBatch)
            {
                jsonDocumentArray.Append(", " + JsonConvert.SerializeObject(merchants[currentIndex++], Formatting.None, serializerSettings));
                idx++;
            }

            jsonDocumentArray.Append("]");

            return jsonDocumentArray.ToString();
        }
    }
}