//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Utils
{
    /// <summary>
    /// Interface classes that manage Azure tables.
    /// </summary>
    public interface IAzureTable
    {
        /// <summary>
        /// Initializes an instance of the azure table
        /// </summary>
        /// <param name="cloudStorageAccount">
        ///  Cloud storage account for the azure table
        /// </param>
        /// <param name="tableName">
        ///  Name of the azure table
        /// </param>
        /// <param name="retryPolicy">
        ///  Retry policy config 
        /// </param>
        void Initialize(CloudStorageAccount cloudStorageAccount = null, string tableName = null, IRetryPolicy retryPolicy = null);

        /// <summary>
        /// Inserts an entity into the table
        /// </summary>
        /// <typeparam name="T">
        /// The ITableEntity
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// True if saved
        /// </returns>
        bool InsertEntity<T>(T entity) where T : ITableEntity;
    }
}