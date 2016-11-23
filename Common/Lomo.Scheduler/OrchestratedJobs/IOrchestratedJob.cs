//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The interface for a job whose execution will be orchestrated by a processor.
    /// </summary>
    public interface IOrchestratedJob
    {
        /// <summary>
        /// Initializes the IOrchestratedJob instance
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
        /// Gets a value that indicates whether the tasks and child jobs within the orchestrated job will be run asynchronously
        /// </summary>
        bool Asynchronous { get; }

        /// <summary>
        /// Gets a value indicating whether tasks are executed before child jobs
        /// </summary>
        /// <remarks>
        /// This property is ignored when Asynchronous is True.
        /// </remarks>
        bool TasksFirst { get; }

        /// <summary>
        /// Gets the collection of tasks belonging to this job
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Non-terminal error was encountered when gathering Tasks. Exception message will contain a structured error code.
        /// </exception>
        Collection<IOrchestratedTask> Tasks { get; }

        /// <summary>
        /// Gets the collection of child jobs belonging to this job
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Non-terminal error was encountered when gathering ChildJobs. Exception message will contain a structured error code.
        /// </exception>
        Collection<IOrchestratedJob> ChildJobs { get; }

        /// <summary>
        /// Executes job startup tasks.
        /// </summary>
        /// <returns>
        /// The result of the execution of the startup tasks.
        /// </returns>
        OrchestratedExecutionResult StartUp();

        /// <summary>
        /// Executes job tear down tasks.
        /// </summary>
        /// <param name="exectuionResult">
        /// The result of execution of job.
        /// </param>
        /// <returns>
        /// The result of the execution of the tear down tasks.
        /// </returns>
        OrchestratedExecutionResult TearDown(OrchestratedExecutionResult exectuionResult);
    }
}