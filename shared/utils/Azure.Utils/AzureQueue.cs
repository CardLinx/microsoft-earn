//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Azure.Utils
{
    using Lomo.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Encapsulates common logic required to work with azure queues
    /// </summary>
    public class AzureQueue
    {
        #region Fields

        /// <summary>
        ///     Reference to the Azure Cloud Queue
        /// </summary>
        private readonly CloudQueue cloudQueue;

        /// <summary>
        ///     azure queue client
        /// </summary>
        private readonly CloudQueueClient queueClient;

        /// <summary>
        ///     number of pending async enqueue operations
        /// </summary>
        private int pendingWrites;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AzureQueue" /> class.
        ///     Prevents a default instance of the AzureQueueClient class from being created.
        /// </summary>
        /// <param name="storageAccount">
        ///     The storage Account.
        /// </param>
        /// <param name="queueName">
        ///     The queue Name.
        /// </param>
        /// <param name="operationTimeout">
        ///     Queue operation timeout
        ///     If not set will use the default values. http://msdn.microsoft.com/en-us/library/windowsazure/dd179396.aspx
        /// </param>
        /// <param name="retryPolicy">Retry policy to use for queue opeartions. If null, exponential retry with default values will be used</param>
        public AzureQueue(
            CloudStorageAccount storageAccount,
            string queueName,
            TimeSpan? operationTimeout = null,
            IRetryPolicy retryPolicy = null)
        {
            this.queueClient = storageAccount.CreateCloudQueueClient();
            if (operationTimeout.HasValue)
            {
                this.queueClient.DefaultRequestOptions.MaximumExecutionTime = operationTimeout.Value;
            }

            this.queueClient.DefaultRequestOptions.RetryPolicy = retryPolicy ?? new ExponentialRetry();
            this.cloudQueue = this.queueClient.GetQueueReference(queueName);
            this.QueueName = queueName;
            try
            {
                this.cloudQueue.CreateIfNotExists();
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                {
                    return;
                }

                throw;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        public string QueueName { get; private set; }

        /// <summary>
        ///     Gets or Sets the Number of pending writers
        /// </summary>
        public int PendingWrites
        {
            get
            {
                return this.pendingWrites;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clears all messages
        /// </summary>
        /// <returns>true if succeeded</returns>
        public bool ClearMessages()
        {
            try
            {
                this.cloudQueue.Clear();
                return true;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        ///     Deletes a message from an azure queue
        /// </summary>
        /// <param name="message">
        ///     the message
        /// </param>
        /// <returns>
        ///     true if succeeded
        /// </returns>
        public bool DeleteMessage(CloudQueueMessage message)
        {
            try
            {
                if (message != null)
                {
                    this.cloudQueue.DeleteMessage(message);
                }

                return true;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        ///     Deletes a message from an azure queue
        /// </summary>
        /// <param name="messageId">
        ///     the message id
        /// </param>
        /// <param name="popReceipt">the pop receipt</param>
        /// <returns>
        ///     true if succeeded
        /// </returns>
        public bool DeleteMessage(string messageId, string popReceipt)
        {
            try
            {
                this.cloudQueue.DeleteMessage(messageId, popReceipt);

                return true;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        ///     Deletes a queue
        /// </summary>
        /// <returns>Return true on success, false if not found, throw exception on error.</returns>
        public bool DeleteQueue()
        {
            try
            {
                this.cloudQueue.Delete();
                return true;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        ///     Retrieves a message from the queue
        /// </summary>
        /// <param name="message">
        ///     a message
        /// </param>
        /// <returns>
        ///     true on success
        /// </returns>
        public bool Dequeue(out CloudQueueMessage message)
        {
            message = null;

            try
            {
                message = this.cloudQueue.GetMessage();
                return message != null;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        ///     De-queues the number of messages specified by messageCount
        /// </summary>
        /// <param name="messageCount">
        ///     number of message to de-queue
        /// </param>
        /// <param name="invisibleTime">
        ///     The invisible Time.
        /// </param>
        /// <returns>
        ///     a list of messages
        /// </returns>
        public IEnumerable<CloudQueueMessage> Dequeue(int messageCount, TimeSpan? invisibleTime = null)
        {
            try
            {
                return this.cloudQueue.GetMessages(messageCount, invisibleTime);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    // queue does not exist
                    return null;
                }

                throw;
            }
        }

        /// <summary>
        ///     Enqueue a message
        /// </summary>
        /// <param name="message">
        ///     a message
        /// </param>
        /// <returns>
        ///     true on success
        /// </returns>
        public bool Enqueue(CloudQueueMessage message)
        {
            try
            {
                this.cloudQueue.AddMessage(message);
                return true;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    // attempt to recreate the queue
                    this.cloudQueue.CreateIfNotExists();
                    this.cloudQueue.AddMessage(message);
                    return true;
                }

                throw;
            }
        }

        /// <summary>
        ///     Enqueue a message async
        /// </summary>
        /// <param name="message">
        ///     a message
        /// </param>
        /// <returns>
        ///     true on success
        /// </returns>
        public Task EnqueueAsync(CloudQueueMessage message)
        {
            Task task = Task.Factory.FromAsync(this.cloudQueue.BeginAddMessage, this.cloudQueue.EndAddMessage, message, this.cloudQueue).ContinueWith(
                (contTask) =>
                {
                    Interlocked.Decrement(ref this.pendingWrites);
                    StorageException storageException = (contTask.Exception != null) ? contTask.Exception.InnerException as StorageException : null;
                    if (storageException != null && storageException.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                    {
                        // attempt to recreate the queue
                        this.cloudQueue.CreateIfNotExists();
                        this.cloudQueue.AddMessage(message);
                    }
                    else if (contTask.Exception != null)
                    {
                        throw contTask.Exception.InnerException;
                    }
                });
            Interlocked.Increment(ref this.pendingWrites);
            return task;
        }

        /// <summary>
        ///     Calls the queue service in order to receive an approximate of the number of messages in the queue
        /// </summary>
        /// <returns>the approximate message count</returns>
        public int RetrieveApproximateMessageCount()
        {
            this.cloudQueue.FetchAttributes();
            return this.cloudQueue.ApproximateMessageCount.GetValueOrDefault();
        }

        /// <summary>
        /// The UpdateMessage method must specify the visibility delay of a message
        /// </summary>
        /// <param name="message">A queue message</param>
        /// <param name="invisibleTime">The visibility timeout for the message</param>
        public void UpdateMessage(CloudQueueMessage message, TimeSpan invisibleTime)
        {
            this.cloudQueue.UpdateMessage(
                message,
                invisibleTime,
                MessageUpdateFields.Visibility | MessageUpdateFields.Content);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Callback for the queue message async call
        /// </summary>
        /// <param name="asyncResult">
        /// the async result
        /// </param>
        private void QueueAddMessageCallback(IAsyncResult asyncResult)
        {
            Interlocked.Decrement(ref this.pendingWrites);
            var queue = (CloudQueue)asyncResult.AsyncState;
            try
            {
                queue.EndAddMessage(asyncResult);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error enqueuing item to azure queue");
            }
        }

        #endregion
    }
}