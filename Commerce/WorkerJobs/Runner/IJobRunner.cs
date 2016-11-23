//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System.Threading.Tasks;
    using Lomo.Scheduler;

    /// <summary>
    /// Interface for job runner
    /// </summary>
    public interface IJobRunner
    {
        /// <summary>
        /// Run a job given the details
        /// </summary>
        /// <param name="jobJobDetails">
        /// Scheduled Job Details
        /// </param>
        /// <returns>
        /// Task wrapper
        /// </returns>
        Task RunJobAsync(ScheduledJobDetails jobJobDetails);
    }
}