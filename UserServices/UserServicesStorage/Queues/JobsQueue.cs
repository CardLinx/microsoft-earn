//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email jobs queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using Azure.Utils;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    /// <summary>
    /// The jobs queue.
    /// </summary>
    public class JobsQueue<T> : IJobsQueue<T> where T : BaseCargo
    {
        #region DataMembers

        /// <summary>
        /// The jobs queue.
        /// </summary>
        private readonly AzureQueue _jobsQueue;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the jobsqueue class.
        /// </summary>
        /// <param name="accountName">
        /// The account name.
        /// </param>
        /// <param name="queueName">
        /// The queue name.
        /// </param>
        public JobsQueue(string accountName, string queueName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(accountName);
            this._jobsQueue = new AzureQueue(cloudStorageAccount, queueName);
        }

        #endregion

        #region IEmailJobsQueue Implementation

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
        public bool TryDequeue(out T emailCargo)
        {
            bool dequeued = false;
            CloudQueueMessage queueMessage;
            if (this._jobsQueue.Dequeue(out queueMessage))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                emailCargo = (T)JsonConvert.DeserializeObject(queueMessage.AsString, null, settings);
                this._jobsQueue.DeleteMessage(queueMessage);
                dequeued = true;
            }
            else
            {
                emailCargo = default(T);
            }
            
            return dequeued;
        }

        /// <summary>
        /// enqueue a new job
        /// </summary>
        /// <param name="emailCargo">
        /// The job to enqueue.
        /// </param>
        /// <exception cref="JsonException"> The job info couldn't be deserialized</exception>
        /// <exception cref="StorageException"> error while accessing the storage</exception>
        public void Enqueue(T emailCargo)
        {
            var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};
            var json = JsonConvert.SerializeObject(emailCargo, Formatting.Indented, settings);
            CloudQueueMessage queueMessage = new CloudQueueMessage(json);
            this._jobsQueue.Enqueue(queueMessage);
        }

        #endregion
      
    }
}