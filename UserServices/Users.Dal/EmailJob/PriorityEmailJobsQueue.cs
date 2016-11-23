//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email confirmation queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal
{
    using Azure.Utils;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    /// <summary>
    ///     The email confirmation queue.
    /// </summary>
    public class PriorityEmailJobsQueue<T> : IPriorityEmailJobsQueue<T> where T : PriorityEmailCargo
    {
        #region Constants

        /// <summary>
        /// The queue name.
        /// </summary>
        private const string QueueName = "priority-email-jobs";

                /// <summary>
        /// The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.UserServices.ConnectionString";
        
        #endregion

        #region Fields

        /// <summary>
        /// The queue.
        /// </summary>
        private readonly AzureQueue queue;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityEmailJobsQueue"/> class.
        /// </summary>
        public PriorityEmailJobsQueue()
            : this(CloudConfigurationManager.GetSetting(StorageSetting))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityEmailJobsQueue"/> class.
        /// </summary>
        /// <param name="storageAccountConfiguration">
        /// The storage Account Configuration.
        /// </param>
        public PriorityEmailJobsQueue(string storageAccountConfiguration)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(storageAccountConfiguration);
            this.queue = new AzureQueue(cloudStorageAccount, QueueName);
        }

        #endregion

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
        public void Delete(string messageId, string popReceipt)
        {
            this.queue.DeleteMessage(messageId, popReceipt);
        }

        /// <summary>
        /// The enqueue.
        /// </summary>
        public void Enqueue(T priorityEmailCargo)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var json = JsonConvert.SerializeObject(priorityEmailCargo, Formatting.Indented, settings);
            var queueMessage = new CloudQueueMessage(json);
            this.queue.Enqueue(queueMessage);
        }

        /// <summary>
        /// The try dequeue.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="JsonException"> The job info couldn't be deserialized</exception>
        /// <exception cref="StorageException"> error while accessing the storage</exception>
        public bool TryDequeue(out PriorityQueueMessage message)
        {
            bool dequeued = false;
            CloudQueueMessage queueMessage;
            if (this.queue.Dequeue(out queueMessage))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                var priorityEmailCargo = (T)JsonConvert.DeserializeObject(queueMessage.AsString, null, settings);
                message = new PriorityQueueMessage
                              {
                                  EmailCargo = priorityEmailCargo, 
                                  MessageId = queueMessage.Id, 
                                  PopReceipt = queueMessage.PopReceipt
                              };
                dequeued = true;
            }
            else
            {
                message = null;
            }

            return dequeued;
        }

        #endregion
    }
}