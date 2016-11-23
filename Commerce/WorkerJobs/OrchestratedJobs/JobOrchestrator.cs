//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
//TODO: Make sure we are honoring the entirety of the ScheduledJobDetails contract.
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;

    /// <summary>
    /// Orchestrates the execution of orchestrated jobs.
    /// </summary>
    public class JobOrchestrator
    {
        /// <summary>
        /// Initializes a new instance of the JobOrchestrator class.
        /// </summary>
        /// <param name="job">
        /// The job being executed.
        /// </param>
        /// <param name="jobDetails">
        /// Details about the job being executed.
        /// </param>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public JobOrchestrator(IOrchestratedJob job,
                                 ScheduledJobDetails jobDetails,
                                 CommerceLog log)
        {
            Job = job;
            JobDetails = jobDetails;
            Log = log;
        }

        /// <summary>
        /// Executes the specified orchestrated job.
        /// </summary>
        /// <param name="tasksExecuted">
        /// Receives the number of tasks executed.
        /// </param>
        /// <returns>
        /// The result of the execution of the job.
        /// </returns>
        public OrchestratedExecutionResult Execute(out int tasksExecuted)
        {
            OrchestratedExecutionResult result;
            tasksExecuted = 0;

            // Run startup tasks and propage their result.
            OrchestratedExecutionResult startUpResult = Job.StartUp();
            result = startUpResult;
            OrchestratedExecutionResult jobResult = result;

            // If the startup tasks did not encounter an error, run the job.
            if (result == OrchestratedExecutionResult.Success)
            {
                if (Job.Asynchronous == true)
                {
                    jobResult = ExecuteAsynchronous(out tasksExecuted);
                }
                else
                {
                    jobResult = ExecuteSynchronous(out tasksExecuted);
                }

                result = jobResult;
            }

            return result;
        }

        /// <summary>
        /// Clean up the job state
        /// </summary>
        /// <param name="executionResult">
        /// Execution Result of the job
        /// </param>
        /// <returns></returns>
        public OrchestratedExecutionResult Cleanup(OrchestratedExecutionResult executionResult)
        {
            OrchestratedExecutionResult result = executionResult;
            OrchestratedExecutionResult tearDownResult = Job.TearDown(executionResult);

            if (tearDownResult == OrchestratedExecutionResult.TerminalError)
            {
                result = OrchestratedExecutionResult.TerminalError;
            }
            else if (tearDownResult == OrchestratedExecutionResult.NonTerminalError &&
                     result != OrchestratedExecutionResult.TerminalError)
            {
                result = OrchestratedExecutionResult.NonTerminalError;
            }

            return result;
        }

        /// <summary>
        /// Executes job tasks and child jobs asynchronously.
        /// </summary>
        /// <param name="tasksExecuted">
        /// Receives the number of tasks executed by this job and all child jobs.
        /// </param>
        /// <returns>
        /// The result of the execution of this job and all child jobs.
        /// </returns>
        private OrchestratedExecutionResult ExecuteAsynchronous(out int tasksExecuted)
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.Success;
            tasksExecuted = 0;

            Log.Verbose("ScheduledJob ID {0}: Executing job type {1} tasks and child jobs asynchronously.", JobDetails.JobId,
                        Job.GetType().Name);

            // Determine how many item will execute while running this job.
            int itemCount = 0;
            if (Job.Tasks != null)
            {
                itemCount = Job.Tasks.Count;
            }
            if (Job.ChildJobs != null)
            {
                itemCount += Job.ChildJobs.Count;
            }

            // If there are any items to execute, do so asynchronously.
            if (itemCount > 0)
            {
                // Spin up threads to execute the tasks and child jobs.
                List<Task> executors = new List<Task>(itemCount);
                ConcurrentDictionary<Guid, Tuple<int, OrchestratedExecutionResult>> itemTaskResults =
                                   new ConcurrentDictionary<Guid, Tuple<int, OrchestratedExecutionResult>>(itemCount, itemCount);

                if (Job.Tasks != null)
                {
                    IEnumerable<OrchestratedExecutionResult> results = ParallelTaskThrottler.Instance.Run(Job.Tasks);
                    foreach (OrchestratedExecutionResult orchestratedExecutionResult in results)
                    {
                        itemTaskResults[Guid.NewGuid()] = new Tuple<int, OrchestratedExecutionResult>(1, orchestratedExecutionResult);
                    }
                }

                if (Job.ChildJobs != null)
                {
                    foreach (IOrchestratedJob job in Job.ChildJobs)
                    {
                        JobOrchestrator jobOrchestrator = new JobOrchestrator(job, JobDetails, Log);
                        executors.Add(Task.Factory.StartNew(() =>
                                      {
                                          int childTasksExecuted;
                                          OrchestratedExecutionResult childResult =
                                                                                 jobOrchestrator.Execute(out childTasksExecuted);
                                          itemTaskResults[Guid.NewGuid()] =
                                                    new Tuple<int, OrchestratedExecutionResult>(childTasksExecuted, childResult);
                                      }));
                    }
                }

                // Wait until all threads have completed their work.
                Task.WaitAll(executors.ToArray());

                // Tally up the completed tasks and get overall result.
                foreach (Guid key in itemTaskResults.Keys)
                {
                    Tuple<int, OrchestratedExecutionResult> itemTaskResult = itemTaskResults[key];
                    tasksExecuted += itemTaskResult.Item1;

                    if (itemTaskResult.Item2 == OrchestratedExecutionResult.TerminalError)
                    {
                        result = OrchestratedExecutionResult.TerminalError;
                    }
                    else if (itemTaskResult.Item2 == OrchestratedExecutionResult.NonTerminalError &&
                             result != OrchestratedExecutionResult.TerminalError)
                    {
                        result = OrchestratedExecutionResult.NonTerminalError;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes job tasks and child jobs synchronously.
        /// </summary>
        /// <param name="tasksExecuted">
        /// Receives the number of tasks executed by this job and all child jobs.
        /// </param>
        /// <returns>
        /// The result of the execution of this job and all child jobs.
        /// </returns>
        private OrchestratedExecutionResult ExecuteSynchronous(out int tasksExecuted)
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.Success;

            if (Job.TasksFirst == true)
            {
                Log.Verbose("ScheduledJob ID {0}: Executing synchronous job type {1}, tasks first.", JobDetails.JobId,
                            Job.GetType().Name);
                result = ExecuteSynchronous(ExecuteTasksSynchronous, ExecuteChildJobsSynchronous, out tasksExecuted);
            }
            else
            {
                Log.Verbose("ScheduledJob ID {0}: Executing synchronous job type {1}, child jobs first.", JobDetails.JobId,
                            Job.GetType().Name);
                result = ExecuteSynchronous(ExecuteChildJobsSynchronous, ExecuteTasksSynchronous, out tasksExecuted);
            }

            return result;
        }

        /// <summary>
        /// Executes job tasks and child jobs synchronously.
        /// </summary>
        /// <param name="firstStep">
        /// First synchronous step to execute, based on value of Job.TasksFirst.
        /// </param>
        /// <param name="secondStep">
        /// Second synchronous step to execute, based on value of Job.TasksFirst.
        /// </param>
        /// <param name="tasksExecuted">
        /// Receives the number of tasks executed by this job and all child jobs.
        /// </param>
        /// <returns>
        /// The result of the execution of this job and all child jobs.
        /// </returns>
        private static OrchestratedExecutionResult ExecuteSynchronous(SynchronousExecutionStep firstStep,
                                                                      SynchronousExecutionStep secondStep,
                                                                      out int tasksExecuted)
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.Success;
            tasksExecuted = 0;

            int stepTasksExecuted;
            OrchestratedExecutionResult stepResult = firstStep(out stepTasksExecuted);
            tasksExecuted += stepTasksExecuted;
            if (stepResult == OrchestratedExecutionResult.TerminalError)
            {
                result = OrchestratedExecutionResult.TerminalError;
            }
            else if (stepResult == OrchestratedExecutionResult.NonTerminalError)
            {
                result = OrchestratedExecutionResult.NonTerminalError;
            }

            if (result != OrchestratedExecutionResult.TerminalError)
            {
                stepResult = secondStep(out stepTasksExecuted);
                tasksExecuted += stepTasksExecuted;
                if (stepResult == OrchestratedExecutionResult.TerminalError)
                {
                    result = OrchestratedExecutionResult.TerminalError;
                }
                else if (stepResult == OrchestratedExecutionResult.NonTerminalError)
                {
                    result = OrchestratedExecutionResult.NonTerminalError;
                }
            }

            return result;
        }

        /// <summary>
        /// Executes job tasks synchronously.
        /// </summary>
        /// <param name="tasksExecuted">
        /// Receives the number of tasks executed by this job's tasks.
        /// </param>
        /// <returns>
        /// The result of the execution of this job's tasks.
        /// </returns>
        private OrchestratedExecutionResult ExecuteTasksSynchronous(out int tasksExecuted)
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.Success;
            tasksExecuted = 0;

            if (Job.Tasks != null)
            {
                Log.Verbose("ScheduledJob ID {0}: Executing synchronous job type {1} tasks.", JobDetails.JobId,
                            Job.GetType().Name);

                // Run each task in turn.
                foreach (IOrchestratedTask task in Job.Tasks)
                {
                    OrchestratedExecutionResult taskResult = task.Execute();
                    tasksExecuted++;
                    if (taskResult == OrchestratedExecutionResult.TerminalError)
                    {
                        result = OrchestratedExecutionResult.TerminalError;
                        break;
                    }
                    else if (taskResult == OrchestratedExecutionResult.NonTerminalError)
                    {
                        result = OrchestratedExecutionResult.NonTerminalError;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes job's child jobs synchronously.
        /// </summary>
        /// <param name="tasksExecuted">
        /// Receives the number of tasks executed by this job's child jobs.
        /// </param>
        /// <returns>
        /// The result of the execution of this job's child jobs.
        /// </returns>
        private OrchestratedExecutionResult ExecuteChildJobsSynchronous(out int tasksExecuted)
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.Success;
            tasksExecuted = 0;

            if (Job.ChildJobs != null)
            {
                Log.Verbose("ScheduledJob ID {0}: Executing synchronous job type {1} child jobs.", JobDetails.JobId,
                            Job.GetType().Name);

                foreach (IOrchestratedJob job in Job.ChildJobs)
                {
                    JobOrchestrator jobOrchestrator = new JobOrchestrator(job, JobDetails, Log);
                    OrchestratedExecutionResult jobResult = jobOrchestrator.Execute(out tasksExecuted);
                    if (jobResult == OrchestratedExecutionResult.TerminalError)
                    {
                        result = jobResult;
                        break;
                    }
                    else if (jobResult == OrchestratedExecutionResult.NonTerminalError)
                    {
                        result = jobResult;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the Job being executed.
        /// </summary>
        private IOrchestratedJob Job { get; set; }

        /// <summary>
        /// Gets or sets details about the job being executed. 
        /// </summary>
        private ScheduledJobDetails JobDetails { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// Delegate for methods that perform a synchronous execution step.
        /// </summary>
        /// <param name="tasksExecuted">
        /// Receives the number of tasks that were executed as part of the step.
        /// </param>
        /// <returns>
        /// The result of step execution.
        /// </returns>
        private delegate OrchestratedExecutionResult SynchronousExecutionStep(out int tasksExecuted);
    }
}