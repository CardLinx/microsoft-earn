//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Models;
using Lomo.Commerce.DataContracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Earn.Offers.Earn.Services
{
    public class CommerceService
    {
        public static async Task<V2GetCardsResponse> GetRegisteredCards(string secureToken)
        {

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-FD-BingIDToken", secureToken);
                client.Timeout = TimeSpan.FromMilliseconds(5000);
                client.DefaultRequestHeaders.Add("X-Flight-ID", "Earn");
                string data = await client.GetStringAsync("https://commerce.earnbymicrosoft.com/api/commerce/v2/cards");
                V2GetCardsResponse response = JsonConvert.DeserializeObject<V2GetCardsResponse>(data);
                return response;
            }
        }

        public static async Task<GetEarnBurnTransactionHistoryResponse> GetTransactionHistory(UserModel user, string secureToken)
        {
            string key = "trans_history:" + user.UserId;

            GetEarnBurnTransactionHistoryResponse response = HttpRuntime.Cache.Get(key) as GetEarnBurnTransactionHistoryResponse;
            if (response != null)
            {
                return response;
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-FD-BingIDToken", secureToken);
                client.Timeout = TimeSpan.FromMilliseconds(5000);
                client.DefaultRequestHeaders.Add("X-Flight-ID", "Earn");
                string data = await client.GetStringAsync("https://commerce.earnbymicrosoft.com/api/commerce/redemptionhistory/getearnhistory");
                response = JsonConvert.DeserializeObject<GetEarnBurnTransactionHistoryResponse>(data);
                HttpRuntime.Cache.Insert(key, response, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
                return response;
            }
        }

        //To be used only by certain apis.
        public static async Task<bool> IsUserRegisteredWithCardLink(UserModel user, string secureToken)
        {
            string key = "clo_user:" + user.UserId;
            bool userRegistered = false;
            var obj = HttpRuntime.Cache.Get(key);
            if (obj != null)
            {
                userRegistered = (bool)obj;
                return userRegistered;
            }

            V2GetCardsResponse response = await GetRegisteredCards(secureToken);
            if (response.ResultSummary.ResultCode == "UnregisteredUser")
            {
                return false;
            }

            HttpRuntime.Cache.Insert(key, true, null, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
            return true;
        }


        //To be used only by certain apis.
        public static async Task<string> GetTotalEarnedAmount(UserModel user, string secureToken)
        {
            try
            {
                string key = "earn_amount:" + user.UserId;
                string earnedAmount = HttpRuntime.Cache.Get(key) as string;
                if (earnedAmount != null)
                {
                    return earnedAmount;
                }

                GetEarnBurnTransactionHistoryResponse history = await GetTransactionHistory(user, secureToken);
                HttpRuntime.Cache.Insert(key, history.CreditBalance, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
                return history.CreditBalance;
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public static async Task<string> LoadReferralCode(UserModel user, string secureToken)
        {
            string key = "referral_code:" + user.UserId;
            string referralCode = HttpRuntime.Cache.Get(key) as string;
            if (!string.IsNullOrWhiteSpace(referralCode))
            {
                return referralCode;
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-FD-BingIDToken", secureToken);
                client.Timeout = TimeSpan.FromMilliseconds(5000);
                client.DefaultRequestHeaders.Add("X-Flight-ID", "Earn");
                HttpResponseMessage msg = await client.PostAsync("https://commerce.earnbymicrosoft.com/api/commerce/v2/referraltypes?rewardrecipient=0", new StringContent(""));
                string data = await msg.Content.ReadAsStringAsync();
                AddReferralTypeResponse response = JsonConvert.DeserializeObject<AddReferralTypeResponse>(data);
                if (response != null && response.ResultSummary != null && (response.ResultSummary.ResultCode == "Success" || response.ResultSummary.ResultCode == "Created") && !string.IsNullOrWhiteSpace(response.ReferralCode))
                {
                    HttpRuntime.Cache.Insert(key, response.ReferralCode, null, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
                    return response.ReferralCode;
                }
                else
                {
                    throw new InvalidOperationException("Call to referral code failed.");
                }
            }
        }

        public static async Task<List<ReferralCodeReportDataContract>> LoadReferralReport(UserModel user, string secureToken)
        {
            string key = "referral_report:" + user.UserId;
            List<ReferralCodeReportDataContract> referralReport = HttpRuntime.Cache.Get(key) as List<ReferralCodeReportDataContract>;
            if (referralReport != null)
            {
                return referralReport;
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-FD-BingIDToken", secureToken);
                client.Timeout = TimeSpan.FromMilliseconds(5000);
                client.DefaultRequestHeaders.Add("X-Flight-ID", "Earn");
                string data = await client.GetStringAsync("https://commerce.earnbymicrosoft.com/api/commerce/v2/referrals");
                GetUsersReferralsResponse response = JsonConvert.DeserializeObject<GetUsersReferralsResponse>(data);
                if (response != null && response.ResultSummary != null && response.ResultSummary.ResultCode == "Success")
                {
                    List<ReferralCodeReportDataContract> result = response.ReferralCodeReports.ToList();
                    HttpRuntime.Cache.Insert(key, result, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
                    return result;
                }
                else
                {
                    throw new InvalidOperationException("Call to load referral report failed.");
                }
            }
        }
    }
}