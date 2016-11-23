//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using LoMo.UserServices.DataContract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Earn.Offers.Earn.Services
{
    public class UserService
    {
        public static async Task<User> GetUserInfo(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-FD-BingIDToken", token);
                string data = await client.GetStringAsync("https://preferences.earnbymicrosoft.com/api/userInfo");
                return JsonConvert.DeserializeObject<User>(data);
            }
        }
    }
}