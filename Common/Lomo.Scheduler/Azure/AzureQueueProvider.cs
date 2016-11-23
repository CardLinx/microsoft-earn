//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    /// <summary>
    /// Wrapper for Azure Queue Operations
    /// </summary>
    class AzureQueueProvider
    {
        /// <summary>
        /// Creates an instance of AzureQueueProvider
        /// </summary>
        /// <param name="connectionString">
        /// Storage connection string
        /// </param>
        /// <param name="queueName">
        /// Name of the queue
        /// </param>
        internal AzureQueueProvider(string connectionString, string queueName)
        {
            CloudQueueClient queueClient = GetCloudQueueClient(connectionString, new ExponentialRetry(TimeSpan.FromSeconds(10), 5));
            queueClient.CreateQueueIfNotExist(queueName);
            queue = queueClient.GetQueueReference(queueName);
        }

        /// <summary>
        /// Enqueue a message into queue
        /// </summary>
        /// <param name="message">
        /// Cloud Queue Message
        /// </param>
        /// <param name="initialVisibilityDelay">
        /// Visibility delay as to when should message appear in the queue
        /// </param>
        /// <returns>
        /// Task wrapper for async operation 
        /// </returns>
        public async Task EnqueueAsync(CloudQueueMessage message, TimeSpan initialVisibilityDelay)
        {
            await Task.Factory.FromAsync(
                   queue.BeginAddMessage(
                           message,
                           TimeSpan.FromDays(7),
                           initialVisibilityDelay,
                           null, null, null, null),
                   queue.EndAddMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Dequeue a message from the queue
        /// </summary>
        /// <param name="timeout">
        /// For how long should message be hidden from other readers or the queue.
        /// This should be big enough so we can process the message and delete from the queue.
        /// </param>
        /// <returns>
        /// Cloud Queue Message
        /// </returns>
        public async Task<CloudQueueMessage> DequeueAsync(TimeSpan timeout)
        {
            return await Task<CloudQueueMessage>.Factory.FromAsync(
                queue.BeginGetMessage(timeout, null, null, null, null),
                queue.EndGetMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a message from the queue
        /// </summary>
        /// <param name="cloudQueueMessage">
        /// Cloud Queue Message
        /// </param>
        /// <returns>
        /// Task wrapper for async operation 
        /// </returns>
        public async Task DeleteAsync(CloudQueueMessage cloudQueueMessage)
        {
            await Task.Factory.FromAsync(
                queue.BeginDeleteMessage(
                    cloudQueueMessage,
                    null, null, null, null
                    ),
                queue.EndDeleteMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a message in the queue in-place
        /// </summary>
        /// <param name="cloudQueueMessage">
        /// Cloud Queue Message
        /// </param>
        /// <returns>
        /// Task wrapper for async operation 
        /// </returns>
        public async Task UpdateAsync(CloudQueueMessage cloudQueueMessage)
        {
            await Task.Factory.FromAsync(
                queue.BeginUpdateMessage(
                    cloudQueueMessage,
                    TimeSpan.FromMinutes(1),
                    MessageUpdateFields.Content | MessageUpdateFields.Visibility,
                    null,
                    null),
                queue.EndUpdateMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Increase visibility timeout
        /// </summary>
        /// <param name="cloudQueueMessage">
        /// Cloud Queue Message
        /// </param>
        /// <param name="timeout">
        /// New timeout value
        /// </param>
        /// <returns>
        /// Task wrapper for async operation 
        /// </returns>
        public async Task IncreaseTimeout(CloudQueueMessage cloudQueueMessage, TimeSpan timeout)
        {
            await Task.Factory.FromAsync(
                queue.BeginUpdateMessage(
                    cloudQueueMessage,
                    timeout,
                    MessageUpdateFields.Visibility,
                    null,
                    null),
                queue.EndUpdateMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete the Queue - Testing Hook
        /// </summary>
        /// <param name="connectionString">
        /// Connection String
        /// </param>
        /// <param name="queueName">
        /// Name of the queue
        /// </param>
        internal static void DeleteQueue(string connectionString, string queueName)
        {
            GetCloudQueueClient(connectionString).GetQueueReference(queueName).DeleteIfExists();
        }

        /// <summary>
        /// Get a client for Azure Queue based on connection string and retry policy
        /// </summary>
        /// <param name="connectionString">
        /// Connection string for the storage account
        /// </param>
        /// <param name="retryPolicy">
        /// Optional retry policy
        /// </param>
        /// <returns>
        /// Instance of CloudQueueClient <see cref="CloudQueueClient"/>
        /// </returns>
        static CloudQueueClient GetCloudQueueClient(string connectionString, IRetryPolicy retryPolicy = null)
        {
            CloudQueueClient client = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString).CreateCloudQueueClient();
            if (retryPolicy != null)
            {
                client.RetryPolicy = retryPolicy;
            }
            return client;
        }

        /// <summary>
        /// Handle to the queue
        /// </summary>
        internal CloudQueue queue;
    }
}