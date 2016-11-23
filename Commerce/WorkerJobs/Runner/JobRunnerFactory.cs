//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;

    /// <summary>
    /// Factory to get implementation of a job runner
    /// </summary>
    public static class JobRunnerFactory
    {
        /// <summary>
        /// Get a Job Runner <see cref="IJobRunner"/>
        /// </summary>
        /// <param name="jobDetails">
        /// Job Details
        /// </param>
        /// <param name="scheduler">
        /// Scheduler to use
        /// </param>
        /// <param name="commerceConfig">
        /// Configuration
        /// </param>
        /// <param name="log">
        /// Logger
        /// </param>
        /// <returns>
        /// Instance of a runner
        /// </returns>
        public static IJobRunner Runner(ScheduledJobDetails jobDetails, IScheduler scheduler, CommerceConfig commerceConfig, CommerceLog log)
        {
            if (jobDetails.Orchestrated)
            {
                return new OrchestratedJobRunner(OrchestratedJobFactory.Create, scheduler, commerceConfig, log);
            }

            return new SimpleJobRunner(ScheduledJobFactory.GetJobByType, log, scheduler );
        }
    }
}