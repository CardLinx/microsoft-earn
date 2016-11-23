//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Threading.Tasks;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.WorkerCommon;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    /// <summary>
    /// First Data Transaction Publisher
    /// </summary>
    public class FirstDataTransactionPublisher : ITransactionPublisher
    {
        /// <summary>
        /// Publish Fdc Transaction Downstream
        /// </summary>
        /// <param name="transactionDetail">
        /// Transaction Details
        /// </param>
        public async Task PublishTransactionAsync(TransactionDetail transactionDetail)
        {
            await Queue.EnqueueAsync(new CloudQueueMessage(JsonConvert.SerializeObject(transactionDetail)), TimeSpan.Zero);
        }

        /// <summary>
        /// Queue to be used to send messages
        /// </summary>
        public IQueueClient Queue { get; set; }
    }
}