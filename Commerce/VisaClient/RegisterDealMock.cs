//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.VisaClient
{
    using System;
    using System.Linq; 
    using System.Net;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Runtime.Serialization;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.DataModels;

    /// <summary>
    ///     Model the request
    /// </summary>
    public class RegisterDealRequest
    {
        public string BingOfferDealId { get; set; }
    }

    /// <summary>
    ///     Model the response
    /// </summary>
    public class RegisterDealResponse
    {
        public int Status { get; set; }
        public string BingOfferDealId { get; set; }
        public string VisaDealId { get; set; }
    }

    [DataContract]
    public class VisaMockDealMapping
    {
                
        [DataMember(EmitDefaultValue = false, Name = "key")]
        public string Key { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "value")]
        public string Value { get; set; } 
    }

    /// <summary>
    ///     mock the call
    /// </summary>
    public class RegisterDealMock
    {
        /// <summary>
        /// dictionary storing the bingofferid to visa deal id mapping
        /// </summary>
        private static Dictionary<String, String> predefinedDeals; // = new Dictionary<string, string>();
        
        /// <summary>
        /// time of last update
        /// </summary>
        private static long lastAccessTick;

        /// <summary>
        /// sync object, multiple reader writer ?
        /// </summary>
        private static Object syncobj; 

        /// <summary>
        /// Hash to check whether data are changed. 
        /// </summary>
        private static int dataHash; 


        /// <summary>
        /// Static constructor to initialize the data
        /// </summary>
        static RegisterDealMock()
        {
            syncobj = new Object();
            lastAccessTick = 0;
            dataHash = 0;
            predefinedDeals = new Dictionary<string, string>();
            PopulateDicitonarySync(); 
            //predefinedDeals.Add("9cf0106f-732e-45de-ba4f-165dd3754bd7", "462715");
            //predefinedDeals.Add("0a9fa6ca-ebb5-4bac-b046-cd477ae6c4c1", "462716");
            //predefinedDeals.Add("c32b1241-3fba-49d6-8948-4af211462b5c", "462717");
            //predefinedDeals.Add("98e50bcd-9d4b-4383-ae4e-407a8ac12042", "462718");
        }

        /// <summary>
        /// check the blob to get the latest deal mapping
        /// </summary>
        /// <returns></returns>
        private static async Task PopulateDictionary()
        {
            TimeSpan span = new TimeSpan(DateTime.Now.Ticks - lastAccessTick);
            if (span.TotalMinutes > 5) // 5 minutes
            {
                try
                {
                    string url = ConfigurationManager.AppSettings["VisaDealMappingUrl"] ?? VisaConstants.VisaDealMappingUrl;
                    string data = await (new WebClient()).DownloadStringTaskAsync(url).ConfigureAwait(false); 
                    
                    int newDataHash = data.GetHashCode();
                    if (data != null && newDataHash != dataHash)
                    {
                        List<VisaMockDealMapping> mapping = General.DeserializeJson<List<VisaMockDealMapping>>(data);
                        Dictionary<string, string> dealDict = mapping.ToDictionary(x => x.Key, x => x.Value);
                        lock (syncobj)
                        {
                            // update dictionary and hash
                            predefinedDeals = dealDict;
                            dataHash = newDataHash; 
                        }
                    }
                    // update access time
                    lastAccessTick = DateTime.Now.Ticks;
                }
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                    // don't do anything?
                }
            }
        }

        /// <summary>
        ///  Initalize the mapping synchronously for the first time/ 
        /// </summary>
        private static void PopulateDicitonarySync()
        {
            try
            {
                string url = ConfigurationManager.AppSettings["VisaDealMappingUrl"] ?? VisaConstants.VisaDealMappingUrl;
                String data = (new WebClient()).DownloadString(url);
                List<VisaMockDealMapping> mapping = General.DeserializeJson<List<VisaMockDealMapping>>(data);
                Dictionary<string, string> dealDict = mapping.ToDictionary(x => x.Key, x => x.Value);
                predefinedDeals = dealDict;
                lastAccessTick = DateTime.Now.Ticks;
                dataHash = data.GetHashCode();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
                
            }
        }

        /// <summary>
        ///     pretend to make a network call, actually trying to resolve the visaofferid issues locally
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<RegisterDealResponse> RegisterDeal(RegisterDealRequest request)
        {
            await PopulateDictionary().ConfigureAwait(false); 

            var visaDealId = ResolveVisaDealId(request.BingOfferDealId);

            RegisterDealResponse response = new RegisterDealResponse
                {
                    Status = 200,
                    BingOfferDealId = request.BingOfferDealId,
                    VisaDealId = visaDealId
                };
            if (response.VisaDealId == "-1")
                response.Status = 100; 
            return response;
        }

        /// <summary>
        ///  Get the visa deal id
        /// </summary>
        /// <param name="bingDealId"> bing offer deal id</param>
        /// <returns></returns>
        private static string ResolveVisaDealId(String bingDealId)
        {
            String res;
            lock (syncobj)
            {
                res = predefinedDeals.ContainsKey(bingDealId) ? predefinedDeals[bingDealId] : "111111";
            }
            return res; 
        }

        /// <summary>
        ///  Get the visa deal id
        /// </summary>
        /// <param name="deal"> bing offer deal</param>
        /// <returns></returns>
        public static string ResolveVisaDealId(Deal deal)
        {
            PopulateDictionary().Wait();
            string visaDealId = ResolveVisaDealId(deal.GlobalId.ToString());
            return visaDealId; 
        }
    }
}