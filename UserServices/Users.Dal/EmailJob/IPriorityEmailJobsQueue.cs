//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The ConfirmJobsQueue interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using Microsoft.WindowsAzure.Storage;

    using Newtonsoft.Json;

    /// <summary>
    /// The ConfirmJobsQueue interface.
    /// </summary>
    public interface IPriorityEmailJobsQueue<in T> where T : PriorityEmailCargo
    {
        #region Public Methods and Operators

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="popReceipt">
        /// The pop receipt.
        /// </param>
        void Delete(string messageId, string popReceipt);

        /// <summary>
        /// The enqueue.
        /// </summary>
        void Enqueue(T priorityEmailCargo);

        /// <summary>
        /// The try dequeue.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="JsonException">
        /// The job info couldn't be deserialized
        /// </exception>
        /// <exception cref="StorageException">
        /// error while accessing the storage
        /// </exception>
        bool TryDequeue(out PriorityQueueMessage message);

        #endregion
    }
}