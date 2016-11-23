//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;

    /// <summary>
    /// Job Runner for Orchestrated Jobs
    /// </summary>
    public class OrchestratedJobRunner : IJobRunner
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
        /// <param name="commerceConfig">
        /// Config
        /// </param>
        /// <param name="log">
        /// Logger
        /// </param>
        public OrchestratedJobRunner(
                    Func<ScheduledJobDetails, IScheduler, CommerceLog, IOrchestratedJob> jobFactory,
                    IScheduler scheduler,
                    CommerceConfig commerceConfig,
                    CommerceLog log)
        {
            JobFactory = jobFactory;
            CommerceConfig = commerceConfig;
            Log = log;
            Scheduler = scheduler;
        }

        /// <summary>
        /// Runs the orchestrated job described in the specified ScheduleJobDetails object.
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the orchestrated job to run.
        /// </param>
        /// <remarks>
        /// A job is limited to MaxJobRetries runs, even for successful runs, before being placed back in the queue to ensure
        /// that a job in an endless loop does not take down the entire worker role (pre-emptive multi-tasking.) But, if the job
        /// times out, another worker instance may attempt to run the job while it's still being run in another instance. Going
        /// forward, some mechanism to halt execution at time out should be added.
        /// </remarks>
        public async Task RunJobAsync(ScheduledJobDetails jobDetails)
        {
            OrchestratedExecutionResult executionResult;
            int maxRetries = CommerceConfig.MaxJobRetries;
            int retryLatency = CommerceConfig.InitialJobRetryLatency;
            int tryCount = 0;
            int tasksPerformed = 0;

            IOrchestratedJob job = JobFactory(jobDetails, Scheduler, Log);
            JobOrchestrator jobOrchestrator = null;
            if (job != null)
            {
                jobOrchestrator = new JobOrchestrator(job, jobDetails, Log);
            }

            if (jobOrchestrator != null)
            {
                do
                {
                    try
                    {
                        Task<OrchestratedExecutionResult> executionResultTask =
                            Task.Factory.StartNew(() => jobOrchestrator.Execute(out tasksPerformed));
                        executionResult = await executionResultTask;
                    }
                    catch (InvalidOperationException ex)
                    {
                        ResultCode resultCode;
                        if (Enum.TryParse<ResultCode>(ex.Message, out resultCode) == true)
                        {
                            executionResult = OrchestratedExecutionResult.NonTerminalError;
                        }
                        else
                        {
                            throw;
                        }
                    }
                    Log.Verbose("{0} orchestrated job completed {1} steps with result {2}.", jobDetails.JobType,
                                tasksPerformed, executionResult);

                    if (executionResult == OrchestratedExecutionResult.NonTerminalError && tryCount <= maxRetries)
                    {
                        Log.Verbose("Waiting {0} milliseconds before retrying job execution.", retryLatency);
                        Thread.Sleep(retryLatency);
                        retryLatency *= 2;
                    }

                    tryCount++;

                }
                while (executionResult != OrchestratedExecutionResult.TerminalError &&
                  tasksPerformed > 0 &&
                  tryCount <= maxRetries);

                // tear down the job here.
                executionResult = jobOrchestrator.Cleanup(executionResult);
            }
            else
            {
                executionResult = OrchestratedExecutionResult.TerminalError;
            }

            StringBuilder stringBuilder = new StringBuilder("{0} orchestrated job completed with result {1}.");
            if (executionResult == OrchestratedExecutionResult.NonTerminalError)
            {
                stringBuilder.Append(" Job will be sent to the back of the queue for reprocessing.");
            }
            Log.Information(stringBuilder.ToString(), jobDetails.JobType, executionResult);

            // Update Scheduler with result of running the job. 
            switch (executionResult)
            {
                case OrchestratedExecutionResult.Success:
                    await Scheduler.CompleteJobIterationAsync(jobDetails).ConfigureAwait(false);
                    break;
                case OrchestratedExecutionResult.TerminalError:
                    jobDetails.JobState = ScheduledJobState.Canceled;
                    jobDetails.Payload = null;
                    await Scheduler.UpdateJobAsync(jobDetails).ConfigureAwait(false);
                    break;
                case OrchestratedExecutionResult.NonTerminalError:
                    await Scheduler.ExponentiallyBackoffAsync(jobDetails, Log).ConfigureAwait(false);
                    break;
            }
        }

        /// <summary>
        /// Factory Method to create Job
        /// </summary>
        private Func<ScheduledJobDetails, IScheduler, CommerceLog, IOrchestratedJob> JobFactory;

        /// <summary>
        /// Handle to Config
        /// </summary>
        private CommerceConfig CommerceConfig;

        /// <summary>
        /// Handle to Logger
        /// </summary>
        private CommerceLog Log;

        /// <summary>
        /// Handle to Scheduler
        /// </summary>
        private IScheduler Scheduler;
    }
}