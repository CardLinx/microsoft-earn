//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The analytics client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using Azure.Utils;
    using Lomo.Logging;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;

    using PerformanceCounters;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The analytics client.
    /// </summary>
    public class AnalyticsClient : IAnalyticsClient
    {
        #region Constants

        /// <summary>
        ///     Name of the Azure Queue
        /// </summary>
        private const string QueueNamePrefix = "analytics";

        /// <summary>
        ///     Once BatchSize items are in the local queue a batch will be generated
        /// </summary>
        private const int BatchSize = 20;

        /// <summary>
        ///     Maximum number of async operation allowed waiting for the azure queue
        /// </summary>
        private const int MaxPendingWriters = 5;

        /// <summary>
        ///     Max number of items that can be queued in memory before a failure
        /// </summary>
        private const int MaxQueuedItems = 10000;


        #endregion

        #region Private Fields


        private Stopwatch maxQueueErrorEventStopwatch = new Stopwatch();


        /// <summary>
        /// Azure queue wait timeout
        /// </summary>
        private static readonly TimeSpan AzureQueueWaitTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Azure queue wait timeout
        /// </summary>
        private readonly TimeSpan azureQueueWaitTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Listen to azure queue size
        /// </summary>
        private readonly Timer azureQueueListener = new Timer();

        /// <summary>
        /// Azure queue listen interval
        /// </summary>
        private readonly TimeSpan azureQueueListenInterval = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Async Operation Failure Rate Logger
        /// measurement window = 1 hour, min elapsed time = 5 minutes, min error count  = 5, min failure rate = 0.5%
        /// </summary>
        private static readonly OperationRateLogger EnqueueOperationOperationRateLogger =
            new OperationRateLogger(TimeSpan.FromHours(1), 10, 0.005);

        /// <summary>
        ///  Set by the consuming service - if analytics needs to be on/off for the service
        /// </summary>
        private bool? isEnabled;

        /// <summary>
        ///     An in-memory buffer to hold messages to be sent to the Azure Queue
        /// </summary>
        private readonly ConcurrentQueue<AnalyticsItem> localQueue = new ConcurrentQueue<AnalyticsItem>();

        /// <summary>
        /// The status lock.
        /// </summary>
        private readonly ReaderWriterLockSlim statusLock = new ReaderWriterLockSlim();

        /// <summary>
        ///  Autoreset event used to trigger immediate queue processing upon adding an item
        /// </summary>
        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        /// <summary>
        /// The retrieve approximate queue size calls failures count.
        /// </summary>
        private int retrieveApproximateFailuresCount;

        /// <summary>
        ///     The queue.
        /// </summary>
        private AzureQueue queue;
        

        /// <summary>
        ///     The task that handles uploading data to the azure queue
        /// </summary>
        private Task uploadTask;

        #endregion

        /// <summary>
        ///  Set by the consuming service - Set to True, if the analytics is enabled by the service
        /// </summary>
        private bool QueueUploaderEnabled
        {
            get
            {
                try
                {
                    statusLock.EnterReadLock();
                    return isEnabled.GetValueOrDefault(false);
                }
                finally
                {
                    statusLock.ExitReadLock();
                }
                
            }

            set
            {
                try
                {
                    statusLock.EnterWriteLock();
                    isEnabled = value;
                }
                finally
                {
                    statusLock.ExitWriteLock();
                }               
                
            }
        }

        /// <summary>
        /// Initializes the local azure queue
        /// </summary>
        /// <param name="queueNameSuffix">
        /// The queue Suffix.
        /// </param>
        private void InitializeLocalQueue(string queueNameSuffix = null)
        {
            string localConnectionString = string.Empty;
            try
            {
                string queueName = queueNameSuffix == null ? QueueNamePrefix : QueueNamePrefix + "-" + queueNameSuffix;

                localConnectionString = CloudConfigurationManager.GetSetting("Analytics.ConnectionString");
                CloudStorageAccount queueStorageAccount = CloudStorageAccount.Parse(localConnectionString);

                // Use a define azure waitout
                queue = new AzureQueue(queueStorageAccount, queueName, this.azureQueueWaitTimeout);
                this.azureQueueListener.AutoReset = false;
                this.azureQueueListener.Interval = this.azureQueueListenInterval.TotalMilliseconds;
                this.azureQueueListener.Elapsed += this.AzureQueueListenerCallback;
                this.azureQueueListener.Start();
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to create the local queue. Verify account settings: {0} ", localConnectionString);
            }
        }


        /// <summary>
        /// Gets the queue count.
        /// </summary>
        public int QueueCount
        {
            get
            {
                return this.localQueue.Count;
            }
        }

        /// <summary>
        /// Initializes the analytics client service
        /// </summary>
        /// <param name="queueNameSuffix">
        /// The queue Name Suffix.
        /// </param>
        /// The local Connection String.
        public void Initialize(string queueNameSuffix = null)
        {
            if (this.uploadTask == null)
            {
                try
                {
                    // init local azure queue
                    this.InitializeLocalQueue(queueNameSuffix);

                    this.QueueUploaderEnabled = true;

                    // start worker thread
                    this.uploadTask = Task.Factory.StartNew(this.UploadItems, TaskCreationOptions.LongRunning);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to start the analytics queue uploader thread. Verify the account settings.");
                }
            }
        }

        /// <summary>
        /// Adds a new item to the analytics client
        /// </summary>
        /// <param name="analyticsItem">
        /// </param>
        public void Add(AnalyticsItem analyticsItem)
        {
            if (QueueUploaderEnabled)
            {
                if (this.localQueue.Count < MaxQueuedItems)
                {
                    this.localQueue.Enqueue(analyticsItem);
                    this.resetEvent.Set();
                }
                else
                {
                    if (!maxQueueErrorEventStopwatch.IsRunning || this.maxQueueErrorEventStopwatch.Elapsed > TimeSpan.FromMinutes(1))
                    {
                        // items won't be counted
                        // log critical error 
                        Log.Error("Local analytics queue (memory) contains {0} items.", this.localQueue.Count);
                        maxQueueErrorEventStopwatch.Restart();
                    }
                }
            }
        }

        /// <summary>
        ///     This action runs in the background and uploads the items in the local queue
        ///     to an azure queue
        /// </summary>
        private void UploadItems()
        {
            var batch = new AnalyticsItemList();

            while (true)
            {
                try
                {
                    AnalyticsItem item;
                    if (this.localQueue.TryDequeue(out item))
                    {
                        batch.Add(item);
                        if (batch.Count >= BatchSize)
                        {
                            SaveBatch(batch);
                        }
                    }
                    else
                    {
                        SaveBatch(batch);

                        // not a lot of items in the queue. sleep for one second  
                        Thread.Sleep(1000);
                    }

                    // update counter to indicate currentq ueue length
                    Counter.SetValue(CounterNames.AnalyticsLocalQueueLength, this.localQueue.Count);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // never exit
                    Log.Error(ex, "Error saving analytics batch");
                }
            }
        }

        /// <summary>
        /// Saves a batch to the local azure queue
        /// </summary>
        /// <param name="batch">
        /// a lits of items to be logged
        /// </param>
        private void SaveBatch(AnalyticsItemList batch)
        {
            try
            {

                if (batch == null)
                {
                    Log.Error("AnalyticsClient.SaveBatch() called with null batch parameter.");
                    return;
                }

                if (batch.Count == 0)
                {
                    return;
                }

                Counter.SetValue(CounterNames.AnalyticsAzureBatchSize, batch.Count);

                // Enqueue to azure Queue
                string message = batch.ToJsonString();                
                Task enqueueTask = queue.EnqueueAsync(new CloudQueueMessage(message));
                Counter.Increment(CounterNames.AnalyticsLocalQueuePerSec);
                enqueueTask.ContinueWith(
                    contTask =>
                    {
                        if (contTask.Exception != null && contTask.Exception.InnerException is StorageException)
                        {
                            EnqueueOperationOperationRateLogger.Failure(EventCode.AnalyticsClientStorageError, contTask.Exception.InnerException, "Error enqueuing item to azure queue");
                        }
                        else if (contTask.Exception != null)
                        {
                            Log.Error(EventCode.AnalyticsClientUnexpectedError, contTask.Exception, "Error enqueuing item to azure queue");
                        }
                        else
                        {
                            EnqueueOperationOperationRateLogger.Success();
                        }
                    }, 
                    TaskContinuationOptions.ExecuteSynchronously);
                Counter.SetValue(CounterNames.AnalyticsAzureQueuePendingWrites, queue.PendingWrites);

                // if too many pending writers. Start throtling
                int wait = 0;
                bool warningSent = false;
                bool errorSent = false;
                const int SleepTime = 100;
                while (queue.PendingWrites > MaxPendingWriters)
                {
                    wait++;
                    int totalSleepMilliseconds = wait * SleepTime;

                    // Send warning after 10 seconds of waiting
                    if (totalSleepMilliseconds > 10000 && !warningSent)
                    {
                        Log.Warn(
                            "The maximum # of pending async writes exceeds the maximum. Value={0}, waiting for {1} milliseconds", 
                            queue.PendingWrites, 
                            totalSleepMilliseconds);
                        warningSent = true;
                    }

                    // Send Error after the timeout of azure send operation passed
                    if (totalSleepMilliseconds >= AzureQueueWaitTimeout.TotalMilliseconds * 2 && !errorSent)
                    {
                        Log.Error(
                            "The maximum # of pending async writes exceeds the maximum. Value={0}, waiting for {1} milliseconds", 
                            queue.PendingWrites, 
                            totalSleepMilliseconds);
                        errorSent = true;
                    }

                    Thread.Sleep(SleepTime);
                }

                // clear batch
                batch.Clear();
            }
            catch (Exception e)
            {
                Log.Error("Error in Analytics.Client.SaveBatch() " + e);
            }
        }

        /// <summary>
        /// timer base callback to check the number of elements in azure queue 
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void AzureQueueListenerCallback(object sender, ElapsedEventArgs e)
        {
            try
            {
                int approximateAzureQueueLength = queue.RetrieveApproximateMessageCount();
                Counter.SetValue(CounterNames.AnalyticsApproximateAzureQueueLength, approximateAzureQueueLength);
                retrieveApproximateFailuresCount = 0;
            }
            catch (Exception exp)
            {
                retrieveApproximateFailuresCount++;
                if (retrieveApproximateFailuresCount > 6)
                {
                    Log.Error(exp, "Error while trying to retrieve azure analytics queue size");
                }
                else
                {
                    Log.Warn("Error while trying to retrieve azure analytics queue size. Error={0}", exp);
                }
            }
            finally
            {
                this.azureQueueListener.Start();
            }
        }
    }
}