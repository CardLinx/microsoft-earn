//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Worker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Commerce.RewardsNetworkWorker;
    using Lomo.Commerce.AmexWorker;
    using Lomo.Commerce.FirstDataWorker;
    using Lomo.Commerce.MasterCardWorker;
    using Lomo.Scheduler;
    using Microsoft.Azure;

    /// <summary>
    /// The Class responsible to initialize all partner jobs if needed
    /// </summary>
    public static class PartnerJobInitializer
    {
        /// <summary>
        /// Initializer partner specific jobs as needed
        /// </summary>
        /// <param name="scheduler">
        /// Instance of job scheduler
        /// </param>
        public static void InitializeJobs(IScheduler scheduler)
        {
            InitializeAmexJobs(scheduler);
            InitializeFirstDataJobs(scheduler);
            InitializeMasterCardJobs(scheduler);
            InitializeVisaJobs(scheduler);
            InitializeRewardsNetworkJobs(scheduler);
        }

        /// <summary>
        /// Initialize FDC jobs
        /// </summary>
        /// <param name="scheduler">
        /// Instance of job scheduler
        /// </param>
        //Commented because Visa transactions will no longer be processed by FDC and will be processed by Visa directly
        private static void InitializeFirstDataJobs(IScheduler scheduler)
        {
            if (RunFdcFileProcessingJobs)
            {
                /*
                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessExtract)))
                {
                    // if no previous active job, schedule one to run every 4 hours
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessExtract,
                        StartTime = DateTime.UtcNow,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Hour,
                            Interval = 4
                        }
                    }).Wait(); // blocking call intentionally. We need to schedule these before worker comes up
                }

                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessPts)))
                {
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessPts,
                        StartTime = DateTime.UtcNow,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Day,
                            Interval = 1
                        }
                    }).Wait();
                }
                */

                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessAcknowledgment)))
                {
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessAcknowledgment,
                        StartTime = DateTime.UtcNow,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Hour,
                            Interval = 4
                        }
                    }).Wait(); // blocking call intentionally. We need to schedule these before worker comes up
                }
            }
        }

        /// <summary>
        /// Initialize Amex Jobs
        /// </summary>
        /// <param name="scheduler">
        /// Instance of job scheduler
        /// </param>
        private static void InitializeAmexJobs(IScheduler scheduler)
        {
            if (RunAmexFileProcessingJobs)
            {

                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessAmexTransactionLog)))
                {
                    // if no previous active job, schedule one to run every hour
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessAmexTransactionLog,
                        StartTime = DateTime.UtcNow,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Hour,
                            Interval = 1
                        }
                    }).Wait();
                }

                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessAmexStatementCredit)))
                {
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessAmexStatementCredit,
                        StartTime = DateTime.UtcNow,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Day,
                            Interval = 1
                        }
                    }).Wait();
                }

                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.AmexOfferRegistrationFileSync)))
                {
                    Dictionary<string, string> payload = new Dictionary<string, string>();
                    payload["responsePending"] = "false";
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.AmexOfferRegistrationFileSync,
                        StartTime = DateTime.UtcNow,
                        Payload = payload,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Hour,
                            Interval = 1
                        }
                    }).Wait();
                }
            }
        }

        /// <summary>
        /// Initialize MasterCard jobs
        /// </summary>
        /// <param name="scheduler">
        /// Instance of job scheduler
        /// </param>
        private static void InitializeMasterCardJobs(IScheduler scheduler)
        {
            if (RunMasterCardFileProcessingJobs)
            {
                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessMasterCardFiltering)))
                {
                    // if no previous active job, schedule one to run once every day
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessMasterCardFiltering,
                        StartTime = DateTime.UtcNow,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Day,
                            Interval = 1
                        }
                    }).Wait();
                }

                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessMasterCardClearing)))
                {
                    // if no previous active job, schedule one to run every 4 hours
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessMasterCardClearing,
                        StartTime = DateTime.UtcNow.AddHours(1).AddMinutes(10),
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Hour,
                            Interval = 4
                        }
                    }).Wait();
                }

                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessMasterCardRebate)))
                {
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessMasterCardRebate,
                        StartTime = DateTime.UtcNow.AddHours(2).AddMinutes(20),
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Day,
                            Interval = 1
                        }
                    }).Wait();
                }

                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessMasterCardRebateConfirmation)))
                {
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessMasterCardRebateConfirmation,
                        StartTime = DateTime.UtcNow.AddHours(3).AddMinutes(40),
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Hour,
                            Interval = 4
                        }
                    }).Wait();
                }
            }
        }
        

        /// <summary>
        /// Initialize Visa jobs
        /// </summary>
        /// <param name="scheduler">
        /// Instance of job scheduler
        /// </param>
        private static void InitializeVisaJobs(IScheduler scheduler)
        {
            if (RunVisaFileProcessingJobs)
            {
                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessVisaRebate)))
                {
#if IntDebug || IntRelease
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessVisaRebate,
                        StartTime = null,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Minute,
                            Interval = 10
                        }
                    }).Wait();
#else
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessVisaRebate,
                        //StartTime = DateTime.UtcNow.AddHours(2).AddMinutes(20),
                        StartTime = DateTime.UtcNow.AddMinutes(10),
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Hour,
                            Interval = 2
                        }
                    }).Wait();
#endif
                }
            }
        }

        /// <summary>
        /// Initializes reward network jobs.
        /// </summary>
        private static void InitializeRewardsNetworkJobs(IScheduler scheduler)
        {
            if (RunRewardsNetworkJobs)
            {
                if (IsNullOrEmpty(scheduler.GetAllActiveJobsByType(ScheduledJobType.ProcessRewardsNetworkReport)))
                {
                    // if no previous active job, schedule one to run every day
                    scheduler.ScheduleJobAsync(new ScheduledJobDetails()
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ProcessRewardsNetworkReport,
                        StartTime = DateTime.UtcNow,
                        Recurrence = new Recurrence()
                        {
                            Frequency = RecurrenceFrequency.Day,
                            Interval = 1
                        }
                    }).Wait(); // blocking call intentionally. We need to schedule these before worker comes up
                }
            }
        }

        /// <summary>
        /// Check if IEnurable collection is null or empty
        /// </summary>
        /// <typeparam name="T">
        /// Type of enumeration
        /// </typeparam>
        /// <param name="enumerable">
        /// Actual Enumeration to check for
        /// </param>
        /// <returns>
        /// True/False indication whether enumeration is null or empty
        /// </returns>
        internal static bool IsNullOrEmpty<T>(IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// Indicates whether we should start FDC FTP Scheduled jobs
        /// </summary>
        internal static bool RunFdcFileProcessingJobs = Convert.ToBoolean(
                                    CloudConfigurationManager.GetSetting(FirstDataFtpConstants.RunFirstDataFileProcessingJobsPropertyName));

        /// <summary>
        /// Indicates whether we should start Amex FTP Scheduled jobs
        /// </summary>
        internal static bool RunAmexFileProcessingJobs = Convert.ToBoolean(
                                    CloudConfigurationManager.GetSetting(AmexSftpConstants.RunAmexFileProcessingJobsPropertyName));

        /// <summary>
        /// Indicates whether we should start MasterCard FTP Scheduled jobs
        /// </summary>
        internal static bool RunMasterCardFileProcessingJobs = Convert.ToBoolean(
                                    CloudConfigurationManager.GetSetting(MasterCardFtpConstants.RunMasterCardFileProcessingJobsPropertyName));
        /// <summary>
        /// Indicates whether we should start Visa Scheduled jobs
        /// </summary>
        internal static bool RunVisaFileProcessingJobs = Convert.ToBoolean(CloudConfigurationManager.GetSetting("Lomo.Commerce.RunVisaFileProcessingJobs"));

        /// <summary>
        /// Indicates whether we should start FDC FTP Scheduled jobs
        /// </summary>
        internal static bool RunRewardsNetworkJobs = Convert.ToBoolean(
                                    CloudConfigurationManager.GetSetting(RewardsNetworkConstants.RunRewardsNetworkJobsPropertyName));
    }
}