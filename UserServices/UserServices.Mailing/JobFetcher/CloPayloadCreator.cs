//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Linq;
    using System.Net.Http.Headers;
    using Analytics.API.Contract;
    using Lomo.Logging;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.WindowsAzure;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using OffersEmail.Scheduler.DataContracts;
    using Users.Dal;
    using Users.Dal.DataModel;

    public static class CloPayloadCreator
    {
        #region Consts

        /// <summary>
        /// The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.UserServices.ConnectionString";

        /// <summary>
        /// The message queue name.
        /// </summary>
        private const string EmailJobsQueueNameSetting = "LoMo.EmailJobs.Queue";

        /// <summary>
        ///     The Users DAL connection string
        /// </summary>
        private const string UsersDalConnectionStringSetting = "LoMo.UsersDal.ConnectionString";

        #endregion

        private static UsersDal _usersDal;

        private static string _analyticsUserName;

        private static string _analyticsPassword;

        private static Uri _analyticsTokenUri;

        private static IJobsQueue<EmailCargo> _emailJobsQueue;

        private static List<string> _lstTestEmailAddress;

        private static string _testUserId;

        public static void Initialize()
        {
            Log.Verbose("Initializing CloPayloadCreator");
            
            _analyticsUserName = CloudConfigurationManager.GetSetting("LoMo.AnalyticsApi.UserName");
            _analyticsPassword = CloudConfigurationManager.GetSetting("LoMo.AnalyticsApi.Password");
            Uri analyticsBaseUri = new Uri(CloudConfigurationManager.GetSetting("LoMo.AnalyticsApi.Address"));
            _analyticsTokenUri = new Uri(analyticsBaseUri, "token");

            _lstTestEmailAddress = CloudConfigurationManager.GetSetting("LoMo.EmailJobs.TestEmailAddress").Split(',').ToList();
            _testUserId = CloudConfigurationManager.GetSetting("LoMo.EmailJobs.TestUserId");

            string storageSetting = CloudConfigurationManager.GetSetting(StorageSetting);
            string emailJobsQueueName = CloudConfigurationManager.GetSetting(EmailJobsQueueNameSetting);
            string userDalConnectionString = CloudConfigurationManager.GetSetting(UsersDalConnectionStringSetting);

            Log.Verbose("Finished reading settings for clopayloadcreator");

            _usersDal = new UsersDal(userDalConnectionString);

            //Initialize the jobs queue
            _emailJobsQueue = new JobsQueue<EmailCargo>(storageSetting, emailJobsQueueName);
            Log.Verbose("Instantiated Users dal and Email Jobs queue");
        }

        public static void Create(OffersEmailSchedule offersEmailSchedule)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var dataContract = JsonConvert.DeserializeObject<BaseContract>(offersEmailSchedule.MetaData, settings);

            DealsContract dealsContract = dataContract as DealsContract;

            if (dealsContract != null && !string.IsNullOrEmpty(offersEmailSchedule.TemplateUrl))
            {
                if (offersEmailSchedule.IsTestEmail)
                {
                    CreateTestEmailCargo(dealsContract, new Tuple<string, string>(offersEmailSchedule.TemplateUrl, offersEmailSchedule.CampaignName));
                }
                else
                {
                    CreateEmailCargo(dealsContract, new Tuple<string, string>(offersEmailSchedule.TemplateUrl, offersEmailSchedule.CampaignName));
                }
            }
        }

        private static void CreateTestEmailCargo(DealsContract dealsContract, Tuple<string, string> tuple)
        {
            Log.Verbose("Creating test email cargo");
            List<Guid> dealGuids = null;
            if (dealsContract.Deals != null && dealsContract.Deals.Any())
            {
                dealGuids = dealsContract.Deals.Select(dealId => new Guid(dealId)).ToList();
            }
            
            User user = _usersDal.GetUserByUserId(new Guid(_testUserId));
            if (!dealsContract.Location.StartsWith("us:postal:"))
            {
                dealsContract.Location = string.Format("{0}:{1}", "us:postal",dealsContract.Location);
            }
            foreach (string email in _lstTestEmailAddress)
            {
                DealsEmailCargo dealsEmailCargo = new DealsEmailCargo
                {
                    Id = user.Id,
                    UserId = user.Id,
                    EmailAddress = email,
                    Campaign = tuple.Item2,
                    Hints = new EmailJobHints { IsTest = false, IncludeDeals = true, IsTestEmail = true },
                    LocationId = dealsContract.Location,
                    UnsubscribeUrl = _usersDal.GetUnsubscribeUrlInfo(user.Id).UnsubscribeUrl,
                    DealIds = dealGuids,
                    IsCloDeal = true,
                    Subject = dealsContract.Subject
                };
                dealsEmailCargo.EmailRenderingServiceAddress = string.Format(tuple.Item1, dealsEmailCargo.Campaign);
                _emailJobsQueue.Enqueue(dealsEmailCargo);
                Log.Verbose("Enqueued Deals test email cargo : {0} ", dealsEmailCargo.ToString());
            }
        }

        private static void CreateEmailCargo(DealsContract dealsContract, Tuple<string, string> tuple)
        {
            Log.Verbose("Creating clo email cargo");
            string accessToken = GetAnalyticsAccessToken();
            Log.Verbose("got analytics access token");
            object continuationContext = null;
            bool hasMore = true;
            List<Guid> dealGuids = null;
            if (dealsContract.Deals != null && dealsContract.Deals.Any())
            {
                dealGuids = dealsContract.Deals.Select(dealId => new Guid(dealId)).ToList();
            }

            while (hasMore)
            {
                try
                {
                    EmailsSubscriptionsBatchResponse response = _usersDal.GetNextEmailSubscriptionsBatch(10000, true, continuationContext, SubscriptionType.WeeklyDeals);
                    {
                        if (response.EmailSubscriptions != null)
                        {
                            foreach (EmailSubscription emailSubscription in response.EmailSubscriptions)
                            {
                                if (emailSubscription.LocationId.Contains("us:postal:"))
                                {
                                    Tuple<bool, string> cloRegionInfo = CloHelper.IsCloRegion(emailSubscription.LocationId);
                                    if (cloRegionInfo.Item1)
                                    {
                                        User user = _usersDal.GetUserByUserId(emailSubscription.UserId);
                                        if (!string.IsNullOrEmpty(user.MsId))
                                        {
                                            Tuple<DateTime?, DateTime?> cloInfo = GetCloInfo(user.Id, accessToken);
                                            if (cloInfo.Item1 != null)
                                            {
                                                DealsEmailCargo dealsEmailCargo = new DealsEmailCargo
                                                {
                                                    Id = user.Id,
                                                    UserId = user.Id,
                                                    EmailAddress = user.Email,
                                                    Campaign = tuple.Item2,
                                                    Hints = new EmailJobHints { IsTest = false, IncludeDeals = true },
                                                    LocationId = emailSubscription.LocationId,
                                                    UnsubscribeUrl = _usersDal.GetUnsubscribeUrlInfo(user.Id).UnsubscribeUrl,
                                                    DealIds = dealGuids,
                                                    IsCloDeal = true,
                                                    Subject = dealsContract.Subject
                                                };
                                                dealsEmailCargo.EmailRenderingServiceAddress = string.Format(tuple.Item1, dealsEmailCargo.Campaign);
                                                _emailJobsQueue.Enqueue(dealsEmailCargo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    hasMore = response.HasMore;
                    continuationContext = response.ContinuationContext;
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message);
                }
            }
        }

        private static Tuple<DateTime?, DateTime?> GetCloInfo(Guid userId, string accessToken)
        {
            Tuple<DateTime?, DateTime?> tuple = new Tuple<DateTime?, DateTime?>(null, null);
            Uri analyticsBaseUri = new Uri(CloudConfigurationManager.GetSetting("LoMo.AnalyticsApi.Address"));
            const string analyticsUserInfoUri = "api/user?userId={0}";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var analyticsUri = new Uri(analyticsBaseUri, string.Format(analyticsUserInfoUri, userId));
            HttpResponseMessage response = client.GetAsync(analyticsUri).Result;
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var userInfo = JsonConvert.DeserializeObject<UserContract>(response.Content.ReadAsStringAsync().Result);
                    tuple = new Tuple<DateTime?, DateTime?>(userInfo.CloUserAddedDateTime, userInfo.CloFirstCardAddedDateTime);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
            }

            return tuple;
        }

        private static string GetAnalyticsAccessToken()
        {
            var client = new HttpClient();
            var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", _analyticsUserName),
                    new KeyValuePair<string, string>("password", _analyticsPassword)
                };
            HttpContent content = new FormUrlEncodedContent(requestParams);
            HttpResponseMessage response = client.PostAsync(_analyticsTokenUri, content).Result;
            string accessTokenResponse = response.Content.ReadAsStringAsync().Result;
            JObject jresponse = JObject.Parse(accessTokenResponse);

            return jresponse["access_token"].ToString();
        }
    }
}