//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.DataContract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Earn.Offers.Earn.Dal
{
    public class HolMonDealsProvider : IDealsProvider
    {
        string dealServerUri;

        private HolMonDealsProvider()
        {
            this.dealServerUri = string.Format("http://{0}/api/lomodeals/GetRewardNetworkDeals?state=", ConfigurationManager.AppSettings["DealServerEndpoint"]);
        }

        public static HolMonDealsProvider Instance = new HolMonDealsProvider();

        public async Task<List<Deal>> GetRewardNetworkDeals(string state)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(5000);
                client.DefaultRequestHeaders.Add("X-Flight-ID", "Earn");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string data = await client.GetStringAsync(this.dealServerUri + state);
                return JsonConvert.DeserializeObject<List<Deal>>(data);
            }
        }
    }
}