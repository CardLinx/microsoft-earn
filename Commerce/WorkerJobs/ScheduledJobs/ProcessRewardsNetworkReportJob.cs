//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Commerce.RewardsNetworkWorker;

namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;

    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;

    public class ProcessRewardsNetworkReportJob : IScheduledJob
    {
        private const string RewardNetworkReportLastRunDate = "RewardNetworkReportLastRunDate";

        public ProcessRewardsNetworkReportJob()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
        }

        public Task Execute(ScheduledJobDetails details, CommerceLog logger)
        {
            this.log = logger;
            DateTime startDate;
            DateTime endDate = DateTime.UtcNow.Date;

            //Check if we have the last successful report run date in the payload
            if (details.Payload != null && details.Payload.ContainsKey(RewardNetworkReportLastRunDate))
            {
                string strLastRunDate = details.Payload[RewardNetworkReportLastRunDate];
                if (!DateTime.TryParse(strLastRunDate, out startDate))
                {
                    log.Error(
                        "LastRunDate specified for the RewardNetworksReportJob in the payload is invalid. Invalid value is {0}",
                        null, (int) ResultCode.JobExecutionError, strLastRunDate);
                }
                //If we have the last successful run date, add a day to it to set as the start date for the next run
                startDate = startDate.AddDays(1).Date;
            }
            else
            {
                startDate = DateTime.UtcNow.AddDays(-1).Date;
            }

            RewardNetworkReportProcessor rewardNetworkReportProcessor = new RewardNetworkReportProcessor(Context);
            rewardNetworkReportProcessor.GenerateReportForDays(startDate, endDate);
            DateTime lastSuccessfulRun = (DateTime) Context[Key.RewardNetworkReportLastRunDate];

            if (details.Payload == null)
            {
                details.Payload = new Dictionary<string, string>();
            }

            if (details.Payload.ContainsKey(RewardNetworkReportLastRunDate))
            {
                details.Payload[RewardNetworkReportLastRunDate] = lastSuccessfulRun.ToString("yyyy-MM-dd");
            }

            this.log.Verbose("Execution of job {0} complete ", details.JobId);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        private CommerceLog log;
    }
}