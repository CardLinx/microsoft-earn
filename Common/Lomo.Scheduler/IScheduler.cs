//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Scheduler to schedule jobs and to process them.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Schedules a job of a given type 
        /// </summary>
        /// <param name="jobDetails">
        /// Details of the job to schedule <see cref="ScheduledJobDetails"/>
        /// </param>
        /// <returns>
        /// Async Task handler
        /// </returns>
        Task ScheduleJobAsync(ScheduledJobDetails jobDetails);

        /// <summary>
        /// Get a job to process if available.
        /// Use this method to listen for scheduled jobs.
        /// </summary>
        /// <returns>
        /// Details of the job to run, or NULL if none present
        /// </returns>
        Task<ScheduledJobDetails> GetJobToProcessAsync();

        /// <summary>
        /// Mark the job iteration as complete.
        /// 
        /// Important:
        /// You need to call this after you complete processing a run of your job.
        /// If you do not call this, scheduler will never know the job was processed
        /// and the run count will not be incremented.
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the job which we want to mark an iteration as complete
        /// </param>
        /// <returns>
        /// Task wrapper for async operation
        /// </returns>
        Task CompleteJobIterationAsync(ScheduledJobDetails jobDetails);

        /// <summary>
        /// Update the job with given details
        /// </summary>
        /// <param name="jobDetails">
        /// Details of the job to be updated
        /// </param>
        /// <returns>
        /// Task wrapper for async operation
        /// </returns>
        Task UpdateJobAsync(ScheduledJobDetails jobDetails);

        /// <summary>
        /// Get all active (Running or Paused) jobs
        /// </summary>
        /// <param name="type">
        /// Type of jobs we want to query
        /// </param>
        /// <returns>
        /// Enumeration of all entities that match the filter
        /// </returns>
        IEnumerable<ScheduledJobEntity> GetAllActiveJobsByType(ScheduledJobType type);

        /// <summary>
        /// Get all jobs of matching on type and description in specified states
        /// </summary>
        /// <param name="type">
        /// Type of jobs we want to query
        /// </param>
        /// <param name="description">
        /// Description belonging to jobs we want to query
        /// </param>
        /// <param name="states">
        /// States job must be in to be returned by query
        /// </param>
        /// <returns>
        /// Enumeration of matching jobs
        /// </returns>
        IEnumerable<ScheduledJobDetails> GetJobsByTypeAndDescription(ScheduledJobType type,
                                                                     string description,
                                                                     ScheduledJobState states);

        /// <summary>
        /// Get Job Details by Job Id.
        /// </summary>
        /// <param name="jobId">
        /// JobId of the job
        /// </param>
        /// <returns>
        /// Job Details if job found else null
        /// </returns>
        /// <remarks>
        /// Use this method to get the details of the job scheduled.
        /// Job returned by this method has no bearing on the scheduled time.
        /// To listen to scheduled jobs, use GetJobToProcessAsync
        /// </remarks>
        ScheduledJobDetails GetJobById(Guid jobId);

        /// <summary>
        /// Update the job payload to the new payload specified
        /// </summary>
        /// <param name="jobDetails">
        /// Details of the job to be updated
        /// </param>
        /// <returns>
        /// Task wrapper for async operation
        /// </returns>
        /// /// <remarks>
        /// This update is done in place, hence queue order is not changed.
        /// Please be aware that this call only updates the payload.
        /// </remarks>
        Task UpdateJobPayload(ScheduledJobDetails jobDetails);

        /// <summary>
        /// Use this to increase visibility timeout of a job which might take longer to process
        /// </summary>
        /// <param name="jobDetail">
        /// Job Details
        /// </param>
        /// <param name="newTimeout">
        /// What should be the new timeout
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        Task IncreaseVisibilityTimeout(ScheduledJobDetails jobDetail, TimeSpan newTimeout);
    }
}