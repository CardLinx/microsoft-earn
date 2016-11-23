//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Threading.Tasks;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerJobs;
    using Lomo.Scheduler;

    /// <summary>
    /// Job Runner for Simple Jobs
    /// </summary>
    public class SimpleJobRunner : IJobRunner
    {
        /// <summary>
        /// Ctor for runner
        /// </summary>
        /// <param name="jobFactory">
        /// Job Factory to get implementation of the job
        /// </param>
        /// <param name="scheduler">
        /// Scheduler
        /// </param>
        /// <param name="log">
        /// Logger
        /// </param>
        public SimpleJobRunner(Func<ScheduledJobType, IScheduledJob> jobFactory, CommerceLog log, IScheduler scheduler)
        {
            JobFactory = jobFactory;
            Log = log;
            Scheduler = scheduler;
        }

        /// <summary>
        /// Run the job
        /// </summary>
        /// <param name="jobDetails">
        /// Job Details
        /// </param>
        /// <returns>
        /// Task Wrapper
        /// </returns>
        public async Task RunJobAsync(ScheduledJobDetails jobDetails)
        {
            await ExecuteJob(JobFactory(jobDetails.JobType), jobDetails);

            // Mark as done
            await Scheduler.CompleteJobIterationAsync(jobDetails);
        }

        /// <summary>
        /// Excute the job
        /// </summary>
        /// <param name="job">
        /// The job to execute
        /// </param>
        /// <param name="details">
        /// Job Details
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        internal async Task ExecuteJob(IScheduledJob job, ScheduledJobDetails details)
        {
            try
            {
                await job.Execute(details, Log);
            }
            catch (Exception exception)
            {
                Log.Error(" Error in execution job \r\n" +
                          " Details : {0}",
                          exception,
                          (int)ResultCode.JobExecutionError,
                          details);
            }
        }

        /// <summary>
        /// Factory method to create a job
        /// </summary>
        private Func<ScheduledJobType, IScheduledJob> JobFactory;

        /// <summary>
        /// Logger handle
        /// </summary>
        private CommerceLog Log;

        /// <summary>
        /// Handle to scheduler
        /// </summary>
        private IScheduler Scheduler;
    }
}