//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.Logging;
using Lomo.Commerce.VisaWorker;
using Lomo.Scheduler;
using System.Threading.Tasks;

namespace Lomo.Commerce.WorkerJobs
{
    /// <summary>
    /// Contains logic to process a Visa rebate transactions.
    /// </summary>
    public class ProcessVisaRebateJob: IScheduledJob
    {

        public async Task Execute(ScheduledJobDetails details, CommerceLog log)
        {
            Log = log;

            Log.Verbose("Starting execution of job.\r\nDetails {0}", details);

            // Process rebate job for Visa.
            var visaProcessor = VisaSettlementProcessorFactory.VisaRebateProcessor();
            await visaProcessor.Process().ConfigureAwait(false);

            Log.Verbose("Exeuction of job {0} complete ", details.JobId);
        }

        /// <summary>
        /// Gets or sets the log within which to log status of job processing.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}