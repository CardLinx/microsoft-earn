//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The user history storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.Storage.UserHistory
{
    using System;
    using System.Collections.Generic;

    using Azure.Utils;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    /// <summary>
    /// The user history storage.
    /// </summary>
    public class UserHistoryStorage : IUserHistoryStorage
    {
        #region Constants

        /// <summary>
        /// The default email history table name.
        /// </summary>
        public const string DefaultEmailHistoryTableName = "UserEmailHistory";

        #endregion

        #region Private Data Members

        /// <summary>
        /// The user email history table.
        /// </summary>
        private readonly AzureTable userEmailHistoryTable;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserHistoryStorage"/> class.
        /// </summary>
        /// <param name="storageAccountConnectionString">
        /// The storage account name.
        /// </param>
        /// <param name="tableName">
        /// The table Name.
        /// </param>
        public UserHistoryStorage(string storageAccountConnectionString, string tableName = null)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            const int RetryCount = 4;
            TimeSpan backoffDelta = TimeSpan.FromSeconds(6);
                
            IRetryPolicy retryPolicy = new ExponentialRetry(backoffDelta, RetryCount);
            this.userEmailHistoryTable = new AzureTable(cloudStorageAccount, tableName ?? DefaultEmailHistoryTableName, retryPolicy);
        }

        #endregion

        /// <summary>
        /// The save user email entity.
        /// </summary>
        /// <param name="emailEntity">
        /// The email entity.
        /// </param>
        public void SaveUserEmailEntity(UserEmailEntity emailEntity)
        {
            this.userEmailHistoryTable.InsertEntity(emailEntity);
        }

        /// <summary>
        /// The get user email entities.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The list of historical user email data
        /// </returns>
        public IEnumerable<UserEmailEntity> GetUserEmailEntities(Guid userId, int count)
        {
            return this.userEmailHistoryTable.GetEntities<UserEmailEntity>(userId.ToString(), count);
        }
    }
}