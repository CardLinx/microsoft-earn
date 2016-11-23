//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace DealsEmailDispatcher
{
    using System.IO;
    using System.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Text;
    using DataCollection.IdentityModel;
    using LoMo.UserServices.DealsMailing;
    using Lomo.Logging;
    using Users.Dal;
    using Users.Dal.DataModel;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Analytics.API.Contract;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using Microsoft.WindowsAzure;
    using Microsoft.Azure;

    class Program
    {
        private static string CampaignRenderingServiceURL = string.Empty;

        /// <summary>
        /// The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.EmailJobs.ConnectionString";

        /// <summary>
        /// The message queue name.
        /// </summary>
        private const string EmailJobsQueueName = "LoMo.EmailJobs.Queue";

        static UsersDal usersDal = new UsersDal();

        private static JobsQueue<EmailCargo> emailJobsQueue;

        static Dictionary<string, Guid> categoriesDictionary = new Dictionary<string, Guid>
        {
            
            {"activities & attractions", new Guid("8FF1C092-63A1-4892-8ED9-F59E9A4C2307	")},
            {"adult", new Guid("FF0A585B-CE09-4A76-AD04-1A061AED3DA8")},
            {"automotive", new Guid("1FFB0322-9465-4376-AB24-0FB3631D73D1")},
            {"beauty & spa", new Guid("E9C13725-8391-4711-ACCE-2A56B75BB3A9")},
            {"entertainment", new Guid("A729ACEE-239D-437B-9D16-F1EFA4ABD28F")},
            {"food & drink", new Guid("6F224E0F-437A-4AAB-8836-172B8546D7A0")},
            {"groceries", new Guid("890342A8-26FD-4D0C-8908-50B039B1F80E")},
            {"health & fitness", new Guid("1AA37916-75AF-4F25-BE4E-B2E6D3E50A99")},
            {"home improvement", new Guid("7EA4DDFA-D102-4780-955B-FC1F129B6E80")},
            {"internal black list", new Guid("A6C70D0D-A7D6-4B11-A4B8-BAE271C024B9")},
            {"other deals", new Guid("75A220A4-396E-4AE0-83C1-613E6D52D369")},
            { "professional services", new Guid("5AA28C4E-B60D-47A0-A401-1AF29DABE323") },
            { "shopping", new Guid("C1FCEB7B-21E7-4E73-A75F-FB47AA85D606") },
            { "travel & hotels", new Guid("F8A00EE6-3D41-4E1B-869E-5637CA25D9C1") },
        };

        static void Main(string[] args)
        {
            bool exit = false;
            Record("Email Jobs Dispatcher Started");
            string accountName = CloudConfigurationManager.GetSetting(StorageSetting);
            string queueName = CloudConfigurationManager.GetSetting(EmailJobsQueueName);
            emailJobsQueue = new JobsQueue<EmailCargo>(accountName, queueName);

            if (args[0] == "create_users" && args.Length >= 3)
            {
                string emailSource = args.Length == 4 ? args[3] : null;
                CreateUsers(args[1], args[2], emailSource, usersDal);
            }
            else if (args[0] == "WeeklyDeals" && args.Length == 5)
            {
                bool includeUsers = false;
                if (args[1] == "include_users")
                {
                    includeUsers = true;
                }
                else if (args[1] != "exclude_users")
                {
                    Record("bad arguments - dispatch");
                    exit = true;
                }
                string dealType = args[3].Trim().ToUpper();
                if (!exit && (dealType == "CLO" || dealType == "PP"))
                {
                    bool isClo = dealType == "CLO";
                    string EmailRenderingServiceURL = ConfigurationManager.AppSettings["LoMo.WeeklyEmailRenderingServiceUrl"];
                    EmailRenderingServiceURL = string.Format(EmailRenderingServiceURL, args[4]);
                    List<Guid> userIds = File.ReadAllLines(args[2]).Select(elem => new Guid(elem)).ToList();
                    DispatchWeeklyDeals(usersDal, emailJobsQueue, includeUsers, userIds, EmailRenderingServiceURL, args[4], isClo);
                }
            }
            else if (args[0] == "Promotional" && args.Length == 3)
            {
                string campaignName = args[1];
                CampaignRenderingServiceURL = ConfigurationManager.AppSettings["LoMo.CampaignEmailRenderingServiceUrl"];
                CampaignRenderingServiceURL = string.Format(CampaignRenderingServiceURL, campaignName);

                List<Guid> userIds = File.ReadAllLines(args[2]).Select(elem => new Guid(elem)).ToList();
                DispatchPromotionalMails(usersDal, emailJobsQueue, userIds, campaignName);
            }
            else if (args[0] == "Notification" && args.Length == 5)
            {
                string campaignName = args[2];
                List<Guid> userIds = File.ReadAllLines(args[1]).Select(elem => new Guid(elem)).ToList();
                CampaignRenderingServiceURL = args[3];
                string subject = args[4];
                DispatchNotificationMail(usersDal, emailJobsQueue, campaignName, subject, userIds);
            }
            else if (args[0] == "Campaign")
            {
                DispatchCampaignMails();
            }
            else if (args[0] == "suppress" && args.Length == 2)
            {
                SuppressUsers(args[1], usersDal);
            }
            else if (args[0] == "CleanUp")
            {
                DeactivateSpamMailAddress(usersDal);
            }
            else if (args[0] == "Lookup" && args.Length == 3)
            {
                GetUsersByEmail(usersDal, args[1], args[2]);
            }
            else if (args[0] == "SignupRemainder" && args.Length == 4)
            {
                int emailCount;
                if (!int.TryParse(args[2], out emailCount))
                {
                    emailCount = 100;
                }
                DispatchRemainderEmails(usersDal, emailJobsQueue, args[1], emailCount, args[3]);
            }
            else if (args[0] == "TrendingDeals" && args.Length == 5)
            {
                bool includeUsers = false;
                if (args[1] == "include_users")
                {
                    includeUsers = true;
                }
                else if (args[1] != "exclude_users")
                {
                    Record("bad arguments - dispatch");
                    exit = true;
                }
                if (!exit)
                {
                    string emailRenderingServiceUrl = ConfigurationManager.AppSettings["LoMo.TrendingDealEmailRenderingServiceUrl"];
                    emailRenderingServiceUrl = string.Format(emailRenderingServiceUrl, args[3]);
                    List<Guid> userIds = File.ReadAllLines(args[2]).Select(elem => new Guid(elem)).ToList();
                    List<Guid> dealIds = File.ReadAllLines(args[4]).Select(elem => new Guid(elem)).ToList();
                    DispatchTrendingDeals(usersDal, emailJobsQueue, includeUsers, userIds, emailRenderingServiceUrl, args[3], dealIds);
                }
            }
            else if (args[0] == "Statistics" && args.Length == 2)
            {
                CalculateEmailStatistics(usersDal, args[1]);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Usage:");
                sb.AppendLine("EmailJobsDispatcher create_users <users_info_file_path>");
                sb.AppendLine("Or");
                sb.AppendLine("EmailJobsDispatcher dispatch");
                throw new Exception(sb.ToString());
            }

            Log.Info("Email Jobs Dispatcher Completed");

            Console.WriteLine("Email Jobs Dispatcher Completed");
            Console.ReadKey();
        }

        private static void CalculateEmailStatistics(UsersDal usersDal, string location)
        {
            object continuationContext = null;
            bool hasMore = true;
            string accessToken = GetAnalyticsAccessToken();
            int loggedIn = 0;
            int notLoggedIn = 0;
            int signedUp = 0;
            int addedCard = 0;

            while (hasMore)
            {
                EmailsSubscriptionsBatchResponse response = usersDal.GetNextEmailSubscriptionsBatch(10000, true, continuationContext, SubscriptionType.WeeklyDeals);
                if (response.EmailSubscriptions != null)
                {
                    foreach (EmailSubscription emailSubscription in response.EmailSubscriptions)
                    {
                        if (emailSubscription.LocationId.Contains("us:postal:"))
                        {
                            Tuple<bool, string> tuple = CloHelper.IsCloRegion(emailSubscription.LocationId);
                            if (tuple.Item1 && String.Compare(location, tuple.Item2, StringComparison.CurrentCultureIgnoreCase) == 0)
                            {
                                User user = usersDal.GetUserByUserId(emailSubscription.UserId);
                                if (!String.IsNullOrEmpty(user.MsId))
                                {
                                    loggedIn++;
                                    Tuple<DateTime?, DateTime?> tuple1 = GetCloInfo(user.Id, accessToken);
                                    if (tuple1.Item1 != null)
                                    {
                                        signedUp++;
                                    }
                                    if (tuple1.Item2 != null)
                                    {
                                        addedCard++;
                                    }
                                }
                                else
                                {
                                    notLoggedIn++;
                                }

                            }
                        }
                    }
                }

                hasMore = response.HasMore;
                continuationContext = response.ContinuationContext;
            }

            Debug.WriteLine("Total users {0}, Logged in {1}, Not Logged in {2}, Signed up {3}, Added card {4}", (loggedIn + notLoggedIn), loggedIn, notLoggedIn, signedUp, addedCard);

        }

        private static Tuple<DateTime?, DateTime?> GetCloInfo(Guid userId, string accessToken)
        {
            Tuple<DateTime?, DateTime?> tuple = new Tuple<DateTime?, DateTime?>(null, null);
            Uri analyticsBaseUri = new Uri(ConfigurationManager.AppSettings["LoMo.AnalyticsUri"]);
            string analyticsUserInfoUri = "api/user?userId={0}";
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
                Console.WriteLine(exception.Message);
            }

            return tuple;
        }

        private static void DispatchRemainderEmails(UsersDal usersDal, JobsQueue<EmailCargo> emailJobsQueue, string inputFile, int emailCount, string campaign)
        {
            Uri analyticsBaseUri = new Uri(ConfigurationManager.AppSettings["LoMo.AnalyticsUri"]);
            string analyticsUserInfoUri = "api/user?userId={0}";
            string accessToken = GetAnalyticsAccessToken();

            if (!string.IsNullOrEmpty(accessToken))
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                string[] lines = File.ReadAllLines(inputFile);
                Console.WriteLine("Total Count of users in the input file : {0}", lines.Count());
                for (int i = 0; i < lines.Count() && emailCount > 0; i++)
                {
                    User user = usersDal.GetUserByUserId(new Guid(lines[i]));
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        var analyticsUri = new Uri(analyticsBaseUri, string.Format(analyticsUserInfoUri, user.Id));
                        HttpResponseMessage response = client.GetAsync(analyticsUri).Result;
                        try
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var userInfo = JsonConvert.DeserializeObject<UserContract>(response.Content.ReadAsStringAsync().Result);
                                if (userInfo.CloUserAddedDateTime != null && userInfo.CloFirstCardAddedDateTime == null)
                                {
                                    DispatchRemainderEmail(emailJobsQueue, user, campaign);
                                    emailCount--;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }
                    }
                }
            }
        }

        private static string GetAnalyticsAccessToken()
        {
            var client = new HttpClient();
            string userName = ConfigurationManager.AppSettings["LoMo.AnalyticsUserName"];
            string password = ConfigurationManager.AppSettings["LoMo.AnalyticsPassword"];
            Uri analyticsBaseUri = new Uri(ConfigurationManager.AppSettings["LoMo.AnalyticsUri"]);
            var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", userName),
                    new KeyValuePair<string, string>("password", password)
                };
            HttpContent content = new FormUrlEncodedContent(requestParams);
            var tokenUri = new Uri(analyticsBaseUri, "token");

            // Make the call sync in this sample
            HttpResponseMessage response = client.PostAsync(tokenUri, content).Result;
            string str = response.Content.ReadAsStringAsync().Result;
            JObject jresponse = JObject.Parse(str);
            return jresponse["access_token"].ToString();
        }

        private static void DispatchRemainderEmail(JobsQueue<EmailCargo> emailJobsQueue, User user, string campaign)
        {
            PromotionalEmailCargo remainderEmailCargo = new PromotionalEmailCargo
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = user.Email,
                    Campaign = campaign,
                    UserId = user.Id,
                    EmailRenderingServiceAddress = ConfigurationManager.AppSettings["LoMo.RemainderEmailRenderingServiceUrl"],
                    PromotionalEmailType = PromotionalEmailType.CompleteSignup.ToString(),
                    Subject = ConfigurationManager.AppSettings["LoMo.RemainderEmail.Subject"]
                };

            emailJobsQueue.Enqueue(remainderEmailCargo);
            Record(string.Format("Email Job Enqueued. JobInfo = {0}", remainderEmailCargo));
        }

        private static void GetUsersByEmail(UsersDal usersDal, string inputFile, string outputFilePath)
        {
            string[] lines = File.ReadAllLines(inputFile);
            Console.WriteLine("Total Count of Email address for lookup : {0}", lines.Count());
            List<Guid> lstUserIds = new List<Guid>();
            int count = 0;
            int found = 0;
            int notFound = 0;

            using (TextWriter tx = new StreamWriter(outputFilePath))
            {
                foreach (string line in lines)
                {
                    count++;
                    string[] lineParts = line.Split(',');
                    string emailAddress = lineParts[0].Trim();
                    User user = usersDal.GetUserByExternalId(emailAddress, UserExternalIdType.Email);
                    if (user != null)
                    {
                        found++;
                        tx.WriteLine("{0} - {1}", emailAddress, user.Id);
                        lstUserIds.Add(user.Id);
                    }
                    else
                    {
                        tx.WriteLine("{0},Not Found", emailAddress);
                        notFound++;
                    }

                    if (count % 10 == 0)
                    {
                        Console.WriteLine("Finished Lookingup {0} users...", count);
                    }
                }
            }

            Console.WriteLine("Lookup Finished....Found {0} users, Not Found {1} users", found, notFound);
        }

        private static void DeactivateSpamMailAddress(UsersDal usersDal)
        {
            int count = 0;
            int deactivatedCount = 0;
            List<string> sendGridInvalidUsers = new List<string>();
            sendGridInvalidUsers.AddRange(GetInvalidEmails("https://sendgrid.com/api/user.bounces.json?api_user=offersteam&api_key=123deals&user=offersteamtest&task=get&date=1"));
            sendGridInvalidUsers.AddRange(GetInvalidEmails("https://sendgrid.com/api/user.spamreports.json?api_user=offersteam&api_key=123deals&user=offersteamtest&task=get&date=1"));
            sendGridInvalidUsers.AddRange(GetInvalidEmails("https://sendgrid.com/api/user.invalidemails.json?api_user=offersteam&api_key=123deals&user=offersteamtest&task=get&date=1"));
            sendGridInvalidUsers.AddRange(GetInvalidEmails("https://sendgrid.com/api/blocks.get.json?api_user=offersteam&api_key=123deals&user=offersteamtest&task=get&date=1"));


            sendGridInvalidUsers.AddRange(GetInvalidEmails("https://sendgrid.com/api/bounces.get.json?api_user=offersteam&api_key=123deals&date=1"));
            sendGridInvalidUsers.AddRange(GetInvalidEmails("https://sendgrid.com/api/spamreports.get.json?api_user=offersteam&api_key=123deals&date=1"));
            sendGridInvalidUsers.AddRange(GetInvalidEmails("https://sendgrid.com/api/invalidemails.get.json?api_user=offersteam&api_key=123deals&date=1"));
            sendGridInvalidUsers.AddRange(GetInvalidEmails("https://sendgrid.com/api/blocks.get.json?api_user=offersteam&api_key=123deals&date=1"));

            Record(string.Format("Total invalid address from sendgrid: {0}", sendGridInvalidUsers.Count));

            foreach (var user in sendGridInvalidUsers)
            {
                count++;
                User userInfo = usersDal.GetUserByExternalId(user, UserExternalIdType.Email);
                if (userInfo != null && userInfo.MsId == null)
                {
                    var userSubscription = usersDal.GetEmailSubscriptionsByUserId(userInfo.Id, true);
                    if (userSubscription.Any())
                    {
                        foreach (var subscription in userSubscription)
                        {
                            if (subscription.IsActive)
                            {
                                usersDal.CreateOrUpdateEmailSubscription(new EmailSubscription
                                    {
                                        IsActive = false,
                                        LocationId = subscription.LocationId,
                                        UserId = subscription.UserId,
                                        SubscriptionType = subscription.SubscriptionType
                                    });
                                deactivatedCount++;
                            }
                        }
                    }
                }

                if (count % 10 == 0)
                {
                    Record(string.Format("Processed {0} records", count));
                }
            }

            Record(string.Format("Total number of subscriptions deactivated : {0}", deactivatedCount));
        }

        private static void Record(string logMessage)
        {
            Console.WriteLine(logMessage);
            Log.Info(logMessage);
        }

        private static List<string> GetInvalidEmails(string sendGridUri)
        {
            List<string> invalidEmails = new List<string>();

            WebClient webClient = new WebClient();
            string response = webClient.DownloadString(sendGridUri);
            if (!string.IsNullOrEmpty(response))
            {
                foreach (var invalidEmail in JsonConvert.DeserializeObject<List<SendGridResponse>>(response))
                {
                    invalidEmails.Add(invalidEmail.email);
                }
            }

            return invalidEmails;
        }

        private static void SuppressUsers(string filePath, UsersDal usersDal)
        {
            int count = 0;
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                usersDal.UpdateUserSuppressInfo(line, true);
                count++;

                if (count % 100 == 0)
                {
                    Console.WriteLine("Processed={0}", count);
                }
            }
            Console.WriteLine("Completed Processing. Processed={0}", count);
            Console.ReadLine();
        }

        private static void CreateUsers(string filePath, string outputFilePath, string emailSource, IUsersDal usersDal)
        {
            string[] lines = File.ReadAllLines(filePath);
            List<UserInfo> usersInfo = new List<UserInfo>();
            foreach (string line in lines)
            {
                string[] lineParts = line.Split(',');
                UserInfo userInfo = new UserInfo { UserEmail = lineParts[0] };
                if (lineParts.Length >= 2)
                {
                    userInfo.UserLocation = string.Format("us:postal:{0}", lineParts[1].Trim());
                }
                List<Guid> categories = new List<Guid>();
                for (int i = 2; i < lineParts.Length; i++)
                {
                    string cat = lineParts[i];
                    if (!string.IsNullOrEmpty(cat))
                    {
                        Guid catGuid = GetCategoryGuid(cat.Trim().ToLowerInvariant());
                        categories.Add(catGuid);
                    }
                }
                userInfo.UserPreferences = categories;
                userInfo.Source = emailSource;
                usersInfo.Add(userInfo);
            }
            using (TextWriter tx = new StreamWriter(outputFilePath))
            {
                foreach (UserInfo userInfo in usersInfo)
                {
                    CreateUser(userInfo, usersDal, userId => tx.WriteLine("{0}\t{1}\t{2}", userInfo.UserEmail, userInfo.UserLocation, userId));
                }
            }

        }
        private static Guid GetCategoryGuid(string cat)
        {
            if (categoriesDictionary.ContainsKey(cat))
            {
                return categoriesDictionary[cat];
            }
            throw new Exception(string.Format("Unknown Category: {0}", cat));
        }

        private static void CreateUser(UserInfo userInfo, IUsersDal usersDal, Action<Guid> writer, SubscriptionType subscriptionType = SubscriptionType.Promotional)
        {
            try
            {
                // In manual creation context the user email isn't confirmed
                bool isEmailConfirmed = false;
                User user = usersDal.CreateOrGetUserByEmail(userInfo.UserEmail, isEmailConfirmed, userInfo.Source);
                if (!string.IsNullOrEmpty(userInfo.UserLocation))
                {
                    usersDal.CreateOrUpdateEmailSubscription(new EmailSubscription { IsActive = true, LocationId = userInfo.UserLocation, UserId = user.Id, SubscriptionType = subscriptionType });
                }

                if (userInfo.UserPreferences.Any())
                {
                    usersDal.UpdateUserInfo(user.Id, new Users.Dal.DataModel.UserInfo { Preferences = new UserPreferences { Categories = userInfo.UserPreferences } });
                }
                writer(user.Id);
                Log.Info("User Created. Email={0}, Id={1}, Location={2}, User Email Source {3}, Preferences Count={4}", userInfo.UserEmail, user.Id, userInfo.UserLocation, userInfo.Source, userInfo.UserPreferences.Count);
            }
            catch (Exception exception)
            {
                if (exception.InnerException.Message == "Cant update a suppressed user")
                {
                    Trace.WriteLine(string.Format("Cannot create user {0} - Suppressed user", userInfo.UserEmail));
                }
            }
        }

        private static void DispatchNotificationMail(
            UsersDal usersDal,
            JobsQueue<EmailCargo> emailJobsQueue,
            string campaignName,
            string emailSubject,
            List<Guid> userIds)
        {
            int totalDispatched = 0;
            foreach (Guid userId in userIds)
            {
                User user = usersDal.GetUserByUserId(userId);
                EmailSubscription subscription = new EmailSubscription
                                                     {
                                                         IsActive = true,
                                                         //Dummy location
                                                         LocationId = "us:postal:00000",
                                                         SubscriptionType = SubscriptionType.Promotional,
                                                         UserId = user.Id,
                                                     };

                if (user.IsSuppressed == true)
                {
                    Record(
                        string.Format("User With Id={0} is suppressed", user.Id));
                }
                else
                {
                    totalDispatched++;
                    DispatchEmailJob(
                        subscription,
                        usersDal,
                        emailJobsQueue,
                        campaignName,
                        CampaignRenderingServiceURL,
                        false,
                        null,
                        false,
                        false,
                        emailSubject);
                    Record(string.Format("Total dispatched {0}", totalDispatched));
                }
            }
        }

        private static void DispatchCampaignMails()
        {
            object continuationContext = null;
            bool hasMore = true;
            string analyticsToken = GetAnalyticsAccessToken();
            CampaignRenderingServiceURL = ConfigurationManager.AppSettings["LoMo.StarbucksEmailRenderingServiceUrl"];

            IDictionary<string, string> campaignData = new Dictionary<string, string>();
            campaignData.Add("Seattle+unauth", "1");
            campaignData.Add("Seattle+auth", "2");
            campaignData.Add("Phoenix+unauth", "3");
            campaignData.Add("Phoenix+auth", "4");
            campaignData.Add("Boston+unauth", "5");
            campaignData.Add("Boston+auth", "6");
            string campaignId = null;
            CampaignEmailCargo campaignEmailCargo = new CampaignEmailCargo();

            while (hasMore)
            {
                EmailsSubscriptionsBatchResponse response = usersDal.GetNextEmailSubscriptionsBatch(1, true, continuationContext, SubscriptionType.WeeklyDeals);
                if (response.EmailSubscriptions != null)
                {
                    foreach (EmailSubscription emailSubscription in response.EmailSubscriptions)
                    {
                        if (emailSubscription.LocationId.Contains("us:postal:"))
                        {
                            Tuple<bool, string> cloRegionInfo = CloHelper.IsCloRegion(emailSubscription.LocationId);
                            if (cloRegionInfo.Item1)
                            {
                                User user = usersDal.GetUserByUserId(emailSubscription.UserId);
                                bool sendEmail;
                                int mode = 0;
                                if (!string.IsNullOrEmpty(user.MsId))
                                {
                                    Tuple<DateTime?, DateTime?> cloInfo = GetCloInfo(user.Id, analyticsToken);
                                    if (cloInfo.Item1 == null)
                                    {
                                        sendEmail = true;
                                        campaignId = campaignData[string.Format("{0}+{1}", cloRegionInfo.Item2, "auth")];
                                        mode = 5;
                                        campaignEmailCargo.Content = "http://www.bing-exp.com/offers/card-linked-signup/?bor=BO_EMAIL&bof=be_sgc&boab=5";
                                    }
                                    else
                                    {
                                        sendEmail = false;
                                    }
                                }
                                else
                                {
                                    sendEmail = true;
                                    campaignId = campaignData[string.Format("{0}+{1}", cloRegionInfo.Item2, "unauth")];
                                    mode = 6;
                                    campaignEmailCargo.Content = "http://www.bing-exp.com/offers/card-linked-signup/?bor=BO_EMAIL&bof=be_sgc&boab=6";
                                }
                                if (sendEmail && mode != 0)
                                {
                                    campaignEmailCargo.Campaign = string.Format("{0},be_sgc", campaignId);
                                    campaignEmailCargo.Content = string.Format("http://www.bing-exp.com/offers/card-linked-signup/?bor=BO_EMAIL&bof={0}&boab={1}", campaignEmailCargo.Campaign, mode);
                                    campaignEmailCargo.EmailAddress = "ramgan@microsoft.com";
                                    campaignEmailCargo.EmailRenderingServiceAddress = string.Format(CampaignRenderingServiceURL, campaignEmailCargo.Campaign);
                                    campaignEmailCargo.LocationId = emailSubscription.LocationId;
                                    campaignEmailCargo.Id = campaignEmailCargo.UserId = user.Id;
                                    campaignEmailCargo.Subject = ConfigurationManager.AppSettings["LoMo.StartBucksCampaign.Subject"];
                                    campaignEmailCargo.UnsubscribeUrl = usersDal.GetUnsubscribeUrlInfo(user.Id).UnsubscribeUrl;
                                    emailJobsQueue.Enqueue(campaignEmailCargo);
                                }
                            }
                        }
                    }
                }

                hasMore = response.HasMore;
                continuationContext = response.ContinuationContext;
            }
        }

        private static void DispatchPromotionalMails(UsersDal usersDal, JobsQueue<EmailCargo> emailJobsQueue, List<Guid> userIds, string campaignName)
        {
            int count = 0;
            foreach (Guid userId in userIds)
            {
                Record(string.Format("Start dispatching. User Id={0}", userId));
                var subscriptions = usersDal.GetEmailSubscriptionsByUserId(userId, true, SubscriptionType.WeeklyDeals.ToString());
                foreach (EmailSubscription emailSubscription in subscriptions)
                {
                    if (emailSubscription.LocationId.Contains("us:postal:"))
                    {
                        DispatchEmailJob(emailSubscription, usersDal, emailJobsQueue, campaignName, CampaignRenderingServiceURL, false, null, false);
                        count++;
                    }
                }
            }

            Record(string.Format("Total mails sent : {0}", count));
        }

        private static void DispatchTrendingDeals(UsersDal usersDal, JobsQueue<EmailCargo> emailJobsQueue, bool includeList, List<Guid> userIds, string emailRenderingUrl, string campaignName, IEnumerable<Guid> dealIds)
        {
            string subject = ConfigurationManager.AppSettings["LoMo.TrendingDeals.Subject"];
            if (includeList)
            {
                foreach (Guid userId in userIds)
                {
                    Record(string.Format("Start dispatching. User Id={0}", userId));
                    var subscriptions = usersDal.GetEmailSubscriptionsByUserId(userId, true, SubscriptionType.WeeklyDeals.ToString());
                    foreach (EmailSubscription emailSubscription in subscriptions)
                    {
                        if (emailSubscription.LocationId.Contains("us:postal:"))
                        {
                            if (CloHelper.IsCloRegion(emailSubscription.LocationId).Item1)
                            {
                                DispatchEmailJob(emailSubscription, usersDal, emailJobsQueue, campaignName, emailRenderingUrl, true, null, dealIds: dealIds, subject: subject);
                            }
                        }
                    }
                }
            }
            else
            {
                object continuationContext = null;
                bool hasMore = true;
                while (hasMore)
                {
                    EmailsSubscriptionsBatchResponse response = usersDal.GetNextEmailSubscriptionsBatch(10000, true, continuationContext, SubscriptionType.WeeklyDeals);
                    if (response.EmailSubscriptions != null)
                    {
                        foreach (EmailSubscription emailSubscription in response.EmailSubscriptions)
                        {
                            if (userIds != null && userIds.Contains(emailSubscription.UserId))
                            {
                                Record(string.Format("User With Id {0} is excluded from this run.", emailSubscription.UserId));
                            }
                            else
                            {
                                if (emailSubscription.LocationId.Contains("us:postal:") && CloHelper.IsCloRegion(emailSubscription.LocationId).Item1)
                                {
                                    DispatchEmailJob(emailSubscription, usersDal, emailJobsQueue, campaignName, emailRenderingUrl, true, null, dealIds: dealIds, subject: subject);
                                }
                            }
                        }
                    }
                    hasMore = response.HasMore;
                    continuationContext = response.ContinuationContext;
                }
            }
        }

        private static void DispatchWeeklyDeals(UsersDal usersDal, JobsQueue<EmailCargo> emailJobsQueue, bool includeList, List<Guid> userIds, string emailRenderingUrl, string campaignName, bool isClo)
        {
            int cloUser = 0;
            int nonCloUser = 0;

            if (includeList)
            {
                foreach (Guid userId in userIds)
                {
                    Record(string.Format("Start dispatching. User Id={0}", userId));
                    var subscriptions = usersDal.GetEmailSubscriptionsByUserId(userId, true, SubscriptionType.WeeklyDeals.ToString());
                    foreach (EmailSubscription emailSubscription in subscriptions)
                    {
                        if (emailSubscription.LocationId.Contains("us:postal:"))
                        {
                            if (CloHelper.IsCloRegion(emailSubscription.LocationId).Item1)
                            {
                                DispatchEmailJob(emailSubscription, usersDal, emailJobsQueue, campaignName, emailRenderingUrl, true, null);
                                cloUser++;
                            }
                            else
                            {
                                DispatchEmailJob(emailSubscription, usersDal, emailJobsQueue, campaignName, emailRenderingUrl, false, null);
                                nonCloUser++;
                            }
                        }
                    }
                }

                Record(string.Format("Total {0} mails sent : {1}", isClo ? "CLO" : "Non CLO", isClo ? cloUser : nonCloUser));
            }
            else
            {

                // List of excluded users
                object continuationContext = null;
                bool hasMore = true;
                while (hasMore)
                {
                    EmailsSubscriptionsBatchResponse response = usersDal.GetNextEmailSubscriptionsBatch(10000, true, continuationContext, SubscriptionType.WeeklyDeals);
                    if (response.EmailSubscriptions != null)
                    {
                        foreach (EmailSubscription emailSubscription in response.EmailSubscriptions)
                        {
                            if (userIds != null && userIds.Contains(emailSubscription.UserId))
                            {
                                Record(string.Format("User With Id {0} is excluded from this run.", emailSubscription.UserId));
                            }
                            else
                            {
                                if (emailSubscription.LocationId.Contains("us:postal:"))
                                {
                                    if (isClo)
                                    {
                                        if (CloHelper.IsCloRegion(emailSubscription.LocationId).Item1)
                                        {
                                            DispatchEmailJob(emailSubscription, usersDal, emailJobsQueue,
                                                             campaignName, emailRenderingUrl,
                                                             true, null);
                                            cloUser++;
                                        }
                                    }
                                    else
                                    {
                                        if (!CloHelper.IsCloRegion(emailSubscription.LocationId).Item1)
                                        {
                                            DispatchEmailJob(emailSubscription, usersDal, emailJobsQueue,
                                                             campaignName,
                                                             emailRenderingUrl, false, null);
                                            nonCloUser++;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    hasMore = response.HasMore;
                    continuationContext = response.ContinuationContext;
                }

                Record(string.Format("Total {0} mails sent : {1}", isClo ? "CLO" : "Non CLO", isClo ? cloUser : nonCloUser));
            }

        }

        private static void DispatchEmailJob(EmailSubscription emailSubscription, UsersDal usersDal, JobsQueue<EmailCargo> emailJobsQueue, string campaign, string emailRenderingUrl,
            bool isCloDeal, string[] categories, bool includeDeals = true, bool useTestAccount = false, string subject = null, IEnumerable<Guid> dealIds = null)
        {
            try
            {
                User user = usersDal.GetUserByUserId(emailSubscription.UserId);
                string unsubscribeUrl = usersDal.GetUnsubscribeUrlInfo(user.Id).UnsubscribeUrl;
                if (string.IsNullOrEmpty(user.Email))
                {
                    Record(string.Format("can't dispatch email job for user without emil address. User Id={0}", user.Id));

                    return;
                }
                if (string.IsNullOrEmpty(unsubscribeUrl))
                {
                    Record(string.Format("can't dispatch email job for user without unsubscribe url. User Id={0}", user.Id));

                    return;
                }

                DealsEmailCargo dealsEmailCargo = new DealsEmailCargo
                    {
                        Id = Guid.NewGuid(),
                        EmailAddress = user.Email,
                        Campaign = campaign,
                        Hints = new EmailJobHints { IsTest = useTestAccount, IncludeDeals = includeDeals },
                        LocationId = emailSubscription.LocationId,
                        UnsubscribeUrl = unsubscribeUrl,
                        UserId = user.Id,
                        DealIds = dealIds,
                        IsCloDeal = isCloDeal,
                        EmailRenderingServiceAddress = emailRenderingUrl
                    };

                if (categories != null && categories.Any())
                {
                    List<Guid> lstGuid = new List<Guid>();
                    foreach (string category in categories)
                    {
                        lstGuid.Add(GetCategoryGuid(category));
                    }
                    dealsEmailCargo.Categories = lstGuid;
                }
                else
                {
                    dealsEmailCargo.Categories = (user.Info != null && user.Info.Preferences != null) ? user.Info.Preferences.Categories : null;
                }

                if (dealsEmailCargo.Hints.IncludeDeals && !string.IsNullOrEmpty(user.MsId) && !user.MsId.StartsWith("fb"))
                {
                    dealsEmailCargo.Anid = AnidIdentityProvider.Instance.DeriveIdentity(user.MsId.ToUpper());
                }
                if (!string.IsNullOrEmpty(subject))
                {
                    dealsEmailCargo.Subject = subject;
                }
                emailJobsQueue.Enqueue(dealsEmailCargo);
                Record(string.Format("Email Job Enqueued. Id={0}; LocationId={1};UnsubscribeUrl={2};UserId={3};CategoriesCount={4}",
                    dealsEmailCargo.Id,
                    dealsEmailCargo.LocationId,
                    dealsEmailCargo.UnsubscribeUrl,
                    dealsEmailCargo.UserId,
                    dealsEmailCargo.Categories == null ? 0 : dealsEmailCargo.Categories.Count()));
            }
            catch (Exception e)
            {
                Log.Error("Error while dispathing email job. User Id={0}; Location Id={1}; Error={2}", emailSubscription.UserId, emailSubscription.LocationId, e);
            }

        }

    }
}