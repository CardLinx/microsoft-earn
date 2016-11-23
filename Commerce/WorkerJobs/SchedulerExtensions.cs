//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;

    /// <summary>
    /// Commerce Specific IScheduler Extension Methods.
    /// </summary>
    public static class SchedulerExtensions
    {
        /// <summary>
        /// Exponentially backoff the next run time of a job after every failure (Non-Terminal) upto a defined MAX.
        /// Currently, only one time jobs are exponentially backed off, and recurring jobs are just scheduled for next occurence.
        /// </summary>
        /// <param name="scheduler">
        /// Instance of the Scheduler
        /// </param>
        /// <param name="jobDetails">
        /// Scheduled Job Details.
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public static async Task ExponentiallyBackoffAsync(this IScheduler scheduler, ScheduledJobDetails jobDetails, CommerceLog log)
        {
            // this branch should not happen once we schedule the job once.
            if (jobDetails != null && jobDetails.Recurrence != null)
            {
                // if job is scheduled to run only once
                if (jobDetails.Recurrence.Count == 1)
                {
                    // initialize retry count if it does not exist
                    if (jobDetails.Payload == null)
                    {
                        jobDetails.Payload = new Dictionary<string, string>();
                        jobDetails.Payload["RetryCount"] = "0";
                    }
                    else if (!jobDetails.Payload.ContainsKey("RetryCount"))
                    {
                        jobDetails.Payload["RetryCount"] = "0";
                    }

                    int retryCount;
                    if (!int.TryParse(jobDetails.Payload["RetryCount"], out retryCount))
                    {
                        retryCount = 0;
                    }

                    //Important: Since the job is a run once job, so recurrence for the next retry is solely
                    // dependent on the retry interval. Past recurrence is immaterial.
                    jobDetails.Recurrence = new Recurrence()
                    {
                        Frequency = RecurrenceFrequency.Second,
                        Count = 1,
                        Interval = GetWaitTimeInSeconds(retryCount)
                    };

                    log.Verbose("Job Id {0} has been retried {1} times, back off to try the next time after {2} seconds", 
                                jobDetails.JobId, 
                                retryCount, 
                                jobDetails.Recurrence.Interval);

                    // increment retry count in payload
                    jobDetails.Payload["RetryCount"] = (retryCount + 1).ToString(CultureInfo.InvariantCulture);

                    // schedule it to run later
                    await scheduler.UpdateJobAsync(jobDetails).ConfigureAwait(false);
                }
                else // recurring job
                {
                    // just mark current iteration as done. We will try again next time
                    await scheduler.CompleteJobIterationAsync(jobDetails).ConfigureAwait(false);
                }
            }
            else
            {
                log.Warning("After first run of job, job or recurrence should not be null.");
                await Task.Factory.StartNew(() => { }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Depending on the retry count, get the next scheduled backoff wait time
        /// </summary>
        /// <param name="retryCount">
        /// # of retries
        /// </param>
        /// <returns>
        /// Seconds after we should try again
        /// </returns>
        private static int GetWaitTimeInSeconds(int retryCount)
        {
            //maximum 10 min backoff- should move to a config
            int maxBackoffInSeconds = 600;

            // 2^10 is 1024, so anything above 9 exceeds max
            if (retryCount > 9)
            {
                return maxBackoffInSeconds;
            }

            int calculatedBackoffInSeconds = ((int)Math.Pow(2, retryCount));

            return Math.Min(maxBackoffInSeconds, calculatedBackoffInSeconds);
        }
    }
}