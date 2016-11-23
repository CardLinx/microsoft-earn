//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//  The Partner transactions jobs queue
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using Azure.Utils;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The Partner transactions jobs queue
    /// </summary>
    public class PartnerTransactionsQueue
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
        public PartnerTransactionsQueue(string accountName, string queueName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(accountName);
            this._jobsQueue = new AzureQueue(cloudStorageAccount, queueName);
        }

        #endregion

        /// <summary>
        /// Try to dequeue a job from the jobs queue. Returns false if the queue is empty otherwise - returns true.
        /// </summary>
        /// <returns>
        /// Returns false if the queue is empty otherwise - returns true
        /// </returns>
        /// <exception cref="JsonException"> The job info couldn't be deserialized</exception>
        /// <exception cref="StorageException"> error while accessing the storage</exception>
        public bool TryDequeue(out PartnerTransactionReportingCargo partnerTransactionReportingCargo)
        {
            bool dequeued = false;
            CloudQueueMessage queueMessage;
            if (this._jobsQueue.Dequeue(out queueMessage))
            {
                JToken token = JObject.Parse(queueMessage.AsString);
                partnerTransactionReportingCargo = new PartnerTransactionReportingCargo();
                partnerTransactionReportingCargo.DealId = (string)token.SelectToken("deal_id");
                partnerTransactionReportingCargo.TransactionAmount = (string)token.SelectToken("settlement_amount");
                partnerTransactionReportingCargo.TransactionDate = (string) token.SelectToken("transaction_date");
                partnerTransactionReportingCargo.TransactionReference = (string)token.SelectToken("deal_id");
                partnerTransactionReportingCargo.Quantity = "1";
                this._jobsQueue.DeleteMessage(queueMessage);
                dequeued = true;
            }
            else
            {
                partnerTransactionReportingCargo = null;
            }
            
            return dequeued;
        }
    }
}