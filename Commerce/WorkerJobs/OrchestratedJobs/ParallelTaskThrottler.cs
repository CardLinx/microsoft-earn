//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Lomo.Scheduler;
    using Microsoft.Azure;

    /// <summary>
    /// Use this class to Throttle the rate at which tasks <see cref="IOrchestratedTask"/> are executed.
    /// The idea is to achive a max parallelism and adjust it if necessary
    /// </summary>
    /// <remarks>
    /// This is Singleton as we want all tasks to pass through a single throttler
    /// If we in future need different throttlers for different scenarios, we should use configuration settings
    /// to create new throttlers.
    /// </remarks>
    internal sealed class ParallelTaskThrottler
    {
        
        /// <summary>
        /// Gets an Instance of the throttler
        /// </summary>
        public static ParallelTaskThrottler Instance
        {
            get
            {
                if (UnderlyingInstance == null)
                {
                    lock (LockObject)
                    {
                        if (UnderlyingInstance == null)
                        {
                            UnderlyingInstance = new ParallelTaskThrottler();
                        }
                    }
                }

                return UnderlyingInstance;
            }
        }

        /// <summary>
        /// Run all the tasks and return their results
        /// We will run max of "MaxParallelTasks" defined in config at one time
        /// </summary>
        /// <param name="tasks">
        /// Tasks to run
        /// </param>
        /// <returns>
        /// Execution results of those tasks
        /// </returns>
        public IEnumerable<OrchestratedExecutionResult> Run(Collection<IOrchestratedTask> tasks)
        {
            List<Task> executors = new List<Task>();
            List<OrchestratedExecutionResult> results = new List<OrchestratedExecutionResult>();

            foreach (IOrchestratedTask task in tasks)
            {
                Throttler.Wait();
                executors.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        results.Add(task.Execute());
                    }
                    finally
                    {
                        Throttler.Release();
                    }
                }));
            }

            Task.WaitAll(executors.ToArray());
            return results;
        }

        /// <summary>
        /// Private Ctor
        /// </summary>
        private ParallelTaskThrottler()
        {
            string maxParallelTasks = CloudConfigurationManager.GetSetting("Lomo.Commerce.MaxParallelTasks");
            int tasksToRun = 0;
            if (!int.TryParse(maxParallelTasks, out tasksToRun))
            {
                // could not parse the value. Use 10 as default
                tasksToRun = 10;
            }
            
            Throttler = new SemaphoreSlim(tasksToRun);
        }

        /// <summary>
        /// Semaphore to manage parallelism
        /// </summary>
        private SemaphoreSlim Throttler;

        /// <summary>
        /// Underlying instance of the throttler
        /// </summary>
        private static volatile ParallelTaskThrottler UnderlyingInstance;

        /// <summary>
        /// Lock for concurrent access to create instance
        /// </summary>
        private static object LockObject = new Object();
    }
}