//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Fetches and executes the Email Jobs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lomo.Logging;
    using Microsoft.WindowsAzure;
    using Users.Dal;
    using System.Threading;
    using Users.Dal.DataModel;
    using Microsoft.Azure;

    /// <summary>
    /// Fetches and executes the Email Jobs
    /// </summary>
    public static class JobFetcher
    {
        #region Const

        /// <summary>
        /// Total number of Email jobs to retrieve in a single call
        /// </summary>
        private const int MerchantEmailJobsToFetch = 10000;

        /// <summary>
        ///     The Users DAL connection string
        /// </summary>
        private const string UsersDalConnectionStringSetting = "LoMo.UsersDal.ConnectionString";

        /// <summary>
        ///     The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.UserServices.ConnectionString";

        /// <summary>
        ///     The message queue name.
        /// </summary>
        private const string EmailJobsQueueNameSetting = "LoMo.EmailJobs.Queue";

        /// <summary>
        /// Setting for reading the value for subject of transaction report email
        /// </summary>
        private const string MerchantTransactionReportSubject = "LoMo.MerchantMailing.TransactionReport.Subject";

        /// <summary>
        /// Setting for reading campaign value for transaction email
        /// </summary>
        private const string MerchantTransactionReportCampaign = "LoMo.MerchantMailing.TransactionReport.Campaign";

        /// <summary>
        /// Setting for reading Job Fetch Interval value
        /// </summary>
        private const string MerchantEmailJobFetchIntervalInMinutes = "LoMo.EmailJobs.JobFetchIntervalInMinutes";

        #endregion

        #region Members

        /// <summary>
        /// Timer for retrieving merchant email jobs periodically
        /// </summary>
        private static Timer _merchantJobtimer;

        /// <summary>
        /// The users dal
        /// </summary>
        private static UsersDal _usersDal;

        /// <summary>
        /// Subject for transaction report email
        /// </summary>
        private static string _transactionReportSubject;

        /// <summary>
        /// Campaign name for transaction email
        /// </summary>
        private static string _transactionReportCampaign;

        /// <summary>
        /// Email jobs queue
        /// </summary>
        private static JobsQueue<EmailCargo> _emailJobsQueue;

        /// <summary>
        /// interval between every merchannt email job
        /// </summary>
        private static long _merchantEmailJobsFetchInterval;

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the job fetcher
        /// </summary>
        public static void Start()
        {
            Log.Verbose("Starting EmailJob Fetcher");

            int fetchIntervalVal;
            if (int.TryParse(CloudConfigurationManager.GetSetting(MerchantEmailJobFetchIntervalInMinutes), out fetchIntervalVal))
            {
                _merchantEmailJobsFetchInterval = fetchIntervalVal * 60 * 1000;
            }
            else
            {
                _merchantEmailJobsFetchInterval = 60 * 60 * 1000;
            }
         
            _transactionReportSubject = CloudConfigurationManager.GetSetting(MerchantTransactionReportSubject);
            _transactionReportCampaign = CloudConfigurationManager.GetSetting(MerchantTransactionReportCampaign);
            string storageAccount = CloudConfigurationManager.GetSetting(StorageSetting);
            string emailJobsQueueName = CloudConfigurationManager.GetSetting(EmailJobsQueueNameSetting);
            string userDalConnectionString = CloudConfigurationManager.GetSetting(UsersDalConnectionStringSetting);

            Log.Verbose("Finished reading settings");

            _usersDal = new UsersDal(userDalConnectionString);
            _emailJobsQueue = new JobsQueue<EmailCargo>(storageAccount, emailJobsQueueName);

            Log.Verbose("Instantiated Users dal and Email Jobs queue");

            _merchantJobtimer = new Timer(callback => GetJob(), null, 0, _merchantEmailJobsFetchInterval);

            Log.Verbose("Created merchant email jobs timer with the interval set to {0} minutes", _merchantEmailJobsFetchInterval.ToString());
        }

        /// <summary>
        /// Stops the job fetcher
        /// </summary>
        public static void Stop()
        {
            _merchantJobtimer.Dispose();
            Log.Verbose("Disposed of job timers");
        }

        /// <summary>
        /// Marks a job as complete
        /// </summary>
        /// <param name="jobId"></param>
        public static void CompleteJob(Guid jobId)
        {
            _usersDal.UpdateEmailJob(jobId);
            Log.Verbose("Finished Updating merchant email Job Id {0}", jobId);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fetches the job and queues it up
        /// </summary>
        private static void GetJob()
        {
            Log.Verbose("Fetching merchant email jobs");
            object continuationContext = null;
            bool hasMore = true;
            while (hasMore)
            {
                Log.Verbose("Asking for {0} email jobs", MerchantEmailJobsToFetch.ToString());
                EmailJobBatchResponse response = _usersDal.GetEmailJobs(MerchantEmailJobsToFetch, continuationContext);
                Log.Verbose("Total jobs fetched {0}", response.EmailJobs.Count());

                foreach (var job in response.EmailJobs)
                {
                    if (job.SubscriptionType == SubscriptionType.TransactionReport)
                    {
                        User user = _usersDal.GetUserByUserId(job.UserId);
                        var userSubscription = _usersDal.GetMerchantSubscriptionsByUserId(job.UserId).FirstOrDefault(subscription => subscription.SubscriptionType == SubscriptionType.TransactionReport);
                        if (user != null && userSubscription != null)
                        {
                            Tuple<DateTime, DateTime> merchantReportDate = GetMerchantReportDate(userSubscription.EmailReportInterval);
                            MerchantReportEmailCargo emailCargo = new MerchantReportEmailCargo()
                                {
                                    Id = job.JobId,
                                    UserId = job.UserId,
                                    FromDate = merchantReportDate.Item1,
                                    ToDate = merchantReportDate.Item2,
                                    ScheduleType = userSubscription.EmailReportInterval.ToString(),
                                    EmailAddress = user.Email,
                                    Subject = _transactionReportSubject,
                                    Campaign = _transactionReportCampaign
                                };
                            _emailJobsQueue.Enqueue(emailCargo);
                        }
                    }
                }

                hasMore = response.HasMore;
                continuationContext = response.ContinuationContext;
            }
        }

        /// <summary>
        /// Returns the start date for the report based on the type of email schedule
        /// </summary>
        /// <param name="scheduleType">Schedule type for the report</param>
        /// <returns>Start and End date of the report</returns>
        private static Tuple<DateTime, DateTime> GetMerchantReportDate(ScheduleType scheduleType)
        {
            DateTime fromDateTime = new DateTime();
            DateTime toDateTime = new DateTime();
            switch (scheduleType)
            {
                case ScheduleType.Daily:
                    fromDateTime = DateTime.UtcNow.AddDays(-1);
                    toDateTime = DateTime.UtcNow.AddDays(-1);
                    break;
                case ScheduleType.Monthly:
                    DateTime previousMonth = DateTime.UtcNow.AddMonths(-1);
                    fromDateTime = new DateTime(previousMonth.Year, previousMonth.Month, 01);
                    toDateTime = new DateTime(previousMonth.Year, previousMonth.Month, DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month));
                    break;
                case ScheduleType.Weekly:
                    fromDateTime = DateTime.UtcNow.AddDays(-7);
                    toDateTime = DateTime.UtcNow.AddDays(-1);
                    break;
            }

            fromDateTime = new DateTime(fromDateTime.Year, fromDateTime.Month, fromDateTime.Day, 0, 0, 1);
            toDateTime = new DateTime(toDateTime.Year, toDateTime.Month, toDateTime.Day, 23, 59, 59);

            return new Tuple<DateTime, DateTime>(fromDateTime, toDateTime);
        }

        #endregion

    }
}