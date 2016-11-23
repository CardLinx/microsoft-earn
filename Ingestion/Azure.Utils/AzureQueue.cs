//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Net;
using System.Threading.Tasks;

namespace Azure.Utils
{
    public class AzureQueue
    {
        private readonly CloudQueue cloudQueue;
        private readonly CloudQueueClient queueClient;

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
            try
            {
                 this.cloudQueue.CreateIfNotExistsAsync();
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

        public async Task<CloudQueueMessage> DequeueMessageAsync()
        {
            return await this.cloudQueue.GetMessageAsync().ConfigureAwait(false);
        }

        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            await this.cloudQueue.DeleteMessageAsync(messageId, popReceipt).ConfigureAwait(false);
        }
    }
}