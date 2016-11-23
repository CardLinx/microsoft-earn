//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace BackgroundWorker
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Lomo.Logging;

    public class Worker
    {
        readonly int sleepDelay;
        readonly IWorkItem work;
        readonly BackgroundWorker worker;

        /// <summary>
        /// Constructor to create a background worker.  You need to create only one instance of this per application.
        /// </summary>
        /// <param name="workItem">The work item to execute.</param>
        /// <param name="sleepDelayMilliSeconds">Time in milliseconds that the worker will sleep when before resuming the work.</param>
        public Worker(IWorkItem workItem, int sleepDelayMilliSeconds = 1000)
        {
            this.sleepDelay = sleepDelayMilliSeconds;
            this.work = workItem;
            this.worker = new BackgroundWorker();
            this.worker.DoWork += new DoWorkEventHandler(BackgroundWork);
            this.worker.WorkerReportsProgress = false;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.RunWorkerCompleted += BackgroundWorkEnded;
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorkEnded(object sender, RunWorkerCompletedEventArgs e)
        {
            Log.Info("Finished background worker activity");
        }

        /// <summary>
        /// The actual background task handler.
        /// </summary>
        /// <param name="sender">State object.</param>
        /// <param name="e">Event argument.</param>
        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    ExecuteWorkItem(this.work);
                }
                catch (Exception ex)
                {
                    Log.Error("Background worker failed. Exception Message: {0} , Exception Stack : {1}", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(this.sleepDelay);
            }
        }

        /// <summary>
        /// The background task to execute.
        /// </summary>
        /// <param name="work">The unit of work to execute as task.</param>
        public void ExecuteWorkItem(IWorkItem work)
        {
            work.ExecuteWorkItem();
        }

        public void StartWorker()
        {
            this.worker.RunWorkerAsync();
        }
    }
}