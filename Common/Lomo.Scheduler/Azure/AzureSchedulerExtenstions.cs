//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Extension methods for Scheduler Storage Wrappers
    /// </summary>
    internal static class AzureSchedulerExtenstions
    {
        /// <summary>
        /// Create a Queue if it does not exist
        /// </summary>
        /// <param name="client">
        /// Cloud queue Client
        /// </param>
        /// <param name="queue">
        /// Name of the queue
        /// </param>
        /// <param name="options">
        /// Optional request options
        /// </param>
        /// <param name="context">
        /// Optional context
        /// </param>
        /// <returns>
        /// Boolean status indicating status of the operation
        /// </returns>
        internal static bool CreateQueueIfNotExist(this CloudQueueClient client, string queue, QueueRequestOptions options = null, OperationContext context = null)
        {
            return client.GetQueueReference(queue).CreateIfNotExists(options, context);
        }

        /// <summary>
        /// Create a Table if it does not exist
        /// </summary>
        /// <param name="client">
        /// Cloud Table Client
        /// </param>
        /// <param name="table">
        /// Name of the table
        /// </param>
        /// <param name="options">
        /// Optional request options
        /// </param>
        /// <param name="context">
        /// Optional context
        /// </param>
        /// <returns>
        /// Boolean status indicating status of the operation
        /// </returns>
        internal static bool CreateTableIfNotExist(this CloudTableClient client, string table, TableRequestOptions options = null, OperationContext context = null)
        {
            return client.GetTableReference(table).CreateIfNotExists(options, context);
        }
    }
}