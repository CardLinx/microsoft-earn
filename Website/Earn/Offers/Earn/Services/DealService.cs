//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.DataContract;
using Earn.Offers.Earn.Dal;
using Earn.Offers.Earn.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Earn.Offers.Earn.Services
{
    public class DealService
    {
        //public static string DealServerUri = string.Format("http://{0}/api/lomodeals/GetDealsByRegion?client=BOFRS_BROWSE&region=Seattle%2C%20WA&refinements=categories:;resultsperbiz:10;sort:rank;flights:DealsServer.CloPlus;&count=35", ConfigurationManager.AppSettings["DealServerEndpoint"]);

        public static async Task<List<Deal>> GetDeals(string state)
        {
            List<Deal> deals = GetFromCache(state);
            if (deals != null)
            {
                return deals;
            }

            deals = await DocumentDbDealsProvider.Instance.GetRewardNetworkDeals(state);
            InsertCache(state, deals);
            return deals;
        }

        public static List<Deal> GetTopDeals(string state)
        {
            string key = string.Format("topdeals-state:{0}", state);
            List<Deal> topDeals = HttpRuntime.Cache.Get(key) as List<Deal>;
            if (topDeals != null && topDeals.Count > 0)
            {
                return topDeals;
            }

            List<Deal> deals = GetTopDealsDictionary()[state];
            topDeals = new List<Deal>();
            if (deals != null && deals.Count > 0)
            {
                topDeals.AddRange(deals);
                int expirationMinutes = 600;
                HttpRuntime.Cache.Insert(key, topDeals, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(expirationMinutes));
            }

            return topDeals;
        }

        public static Dictionary<string, List<Deal>> GetTopDealsDictionary()
        {
            string cacheKey = "topdealsdictionary";
            Dictionary<string, List<Deal>> dictionary = HttpRuntime.Cache.Get(cacheKey) as Dictionary<string, List<Deal>>;
            if (dictionary != null)
            {
                return dictionary;
            }

            try
            {
                VirtualFile file = HostingEnvironment.VirtualPathProvider.GetFile("~/topplaces.json");
                using (Stream stream = file.Open())
                {
                    string data = new StreamReader(stream).ReadToEnd();
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<Deal>>>(data);
                    if (dictionary != null)
                    {
                        HttpRuntime.Cache.Insert(cacheKey, dictionary, null, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(10));
                    }

                    return dictionary;
                }
            }
            catch
            {
                return null;
            }
        }

        private static List<Deal> GetFromCache(string state)
        {
            string key = string.Format("state:{0}", state);
            List<Deal> deals = HttpRuntime.Cache.Get(key) as List<Deal>;
            if (deals != null)
            {
                return deals;
            }

            return null;
        }

        private static void InsertCache(string state, List<Deal> deals)
        {
            if (deals != null && deals.Count > 0)
            {
                int expirationMinutes = 600;
                string key = string.Format("state:{0}", state);
                HttpRuntime.Cache.Insert(key, deals, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(expirationMinutes));
            }
        }
    }
}