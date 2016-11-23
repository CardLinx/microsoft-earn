//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The EmailJobsQueue interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using Microsoft.WindowsAzure.Storage;
    using Newtonsoft.Json;

    /// <summary>
    /// The JobsQueue interface.
    /// </summary>
    public interface IJobsQueue<T> where T : BaseCargo
    {
        /// <summary>
        /// Try to dequeue a job from the jobs queue. Returns false if the queue is empty otherwise - returns true.
        /// </summary>
        /// <param name="emailCargo">
        /// The dequeue job when the method return true. Otherwise - unspecified.
        /// </param>
        /// <returns>
        /// Returns false if the queue is empty otherwise - returns true
        /// </returns>
        /// <exception cref="JsonException"> The job info couldn't be deserialized</exception>
        /// <exception cref="StorageException"> error while accessing the storage</exception>
        bool TryDequeue(out T emailCargo);

        /// <summary>
        /// enqueue a new job
        /// </summary>
        /// <param name="emailCargo">
        /// The job to enqueue.
        /// </param>
        /// <exception cref="JsonException"> The job info couldn't be deserialized</exception>
        /// <exception cref="StorageException"> error while accessing the storage</exception>
        void Enqueue(T emailCargo);
    }
}