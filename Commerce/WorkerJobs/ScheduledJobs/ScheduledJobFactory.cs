//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using Lomo.Scheduler;
    using System;

    /// <summary>
    /// Factory to get instance of a scheduled job
    /// </summary>
    public static class ScheduledJobFactory
    {
        /// <summary>
        /// Get a job instance by its type
        /// </summary>
        /// <param name="jobType">
        /// Type of the job being asked for
        /// </param>
        /// <returns>
        /// A new instance of the job by type
        /// </returns>
        public static IScheduledJob GetJobByType(ScheduledJobType jobType)
        {
            switch (jobType)
            {
                case ScheduledJobType.ProcessExtract:
                    return new ProcessExtractJob();
                case ScheduledJobType.ProcessPts:
                    return new ProcessPtsJob();
                case ScheduledJobType.ProcessAcknowledgment:
                    return new ProcessAcknowledgmentJob();
                case ScheduledJobType.PingJob:
                    return new PingJob();
                case ScheduledJobType.AmexOfferRegistrationFileSync:
                    return new AmexOfferRegistrationFileSyncJob();
                case ScheduledJobType.DiscountActivationJob:
                    return new DiscountActivationJob();
                case ScheduledJobType.ProcessAmexTransactionLog:
                    return new ProcessAmexTransactionLogJob();
                case ScheduledJobType.ProcessAmexStatementCredit:
                    return new ProcessAmexStatementCreditJob();
                case ScheduledJobType.ProcessMasterCardFiltering:
                    return new ProcessMasterCardFilteringJob();
                case ScheduledJobType.ProcessMasterCardClearing:
                    return new ProcessMasterCardClearingJob();
                case ScheduledJobType.ProcessMasterCardRebate:
                    return new ProcessMasterCardRebateJob();
                case ScheduledJobType.ProcessMasterCardRebateConfirmation:
                    return new ProcessMasterCardRebateConfirmationJob();
                case ScheduledJobType.ProcessRewardsNetworkReport:
                    return new ProcessRewardsNetworkReportJob();
                case ScheduledJobType.ProcessVisaRebate:
                    return new ProcessVisaRebateJob();
                default:
                    throw new ArgumentOutOfRangeException("jobType");
            }
        }
    }
}