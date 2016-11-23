//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System.Collections.Concurrent;

    /// <summary>
    /// The interface for a task whose execution will be orchestrated by a processor.
    /// </summary>
    public interface IOrchestratedTask
    {
        /// <summary>
        /// Initializes the IOrchestratedTask instance.
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the job being run.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler managing the jobs.
        /// </param>
        void Initialize(ScheduledJobDetails jobDetails,
                        IScheduler scheduler);

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>
        /// The result of the execution of the task.
        /// </returns>
        OrchestratedExecutionResult Execute();
    }
}