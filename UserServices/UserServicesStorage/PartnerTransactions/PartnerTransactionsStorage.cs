//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   Stores and retrives partner transactions from azure storage
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.Storage.PartnerTransactions
{
    using System;
    using System.Collections.Generic;
    using Azure.Utils;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    /// <summary>
    /// Stores and retrives partner transactions from azure storage
    /// </summary>
    public class PartnerTransactionsStorage : IPartnerTransactions
    {
        #region Constants

        /// <summary>
        /// The default partner transactions azure table
        /// </summary>
        public const string DefaultTableName = "PartnerTransactions";

        #endregion

        #region Private Data Members

        /// <summary>
        /// The partner transactions table.
        /// </summary>
        private readonly AzureTable _partnerTransactionsHistoryTable;

        #endregion

        #region Constructor

        public PartnerTransactionsStorage(string storageAccountConnectionString, string tableName = null)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            const int retryCount = 4;
            TimeSpan backoffDelta = TimeSpan.FromSeconds(6);
                
            IRetryPolicy retryPolicy = new ExponentialRetry(backoffDelta, retryCount);
            this._partnerTransactionsHistoryTable = new AzureTable(cloudStorageAccount, tableName ?? DefaultTableName, retryPolicy);
        }

        #endregion

        /// <summary>
        /// Saves partner transactions to azure storage
        /// </summary>
        /// <param name="partnerTransactionsEntity">
        /// The partner transaction entity.
        /// </param>
        public void SavePartnerTransactionEntity(PartnerTransactionsEntity partnerTransactionsEntity)
        {
            this._partnerTransactionsHistoryTable.InsertEntity(partnerTransactionsEntity);
        }

        /// <summary>
        /// Returns partner transactions from azure storage
        /// </summary>
        /// <returns>
        /// list of partner transaction entities
        /// </returns>
        public IEnumerable<PartnerTransactionsEntity> GetPartnerTransactionEntities(Guid transactionId)
        {
            return this._partnerTransactionsHistoryTable.GetEntities<PartnerTransactionsEntity>(transactionId.ToString());
        }

    }
}