//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerCommon
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Queue Client Interface
    /// </summary>
    public interface IQueueClient
    {
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
        Task EnqueueAsync(CloudQueueMessage message, TimeSpan initialVisibilityDelay);

        /// <summary>
        /// Dequeue a message from the queue
        /// </summary>
        /// <param name="timeout">
        /// For how long should message be hidden from other readers or the queue?
        /// This should be big enough so we can process the message and delete from the queue.
        /// </param>
        /// <returns>
        /// Cloud Queue Message
        /// </returns>
        Task<CloudQueueMessage> DequeueAsync(TimeSpan timeout);
    }
}