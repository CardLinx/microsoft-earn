//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Wrapper for Azure Table Operations
    /// </summary>
    class AzureTableProvider
    {
        /// <summary>
        /// Create a new Instance for the Wrapper
        /// </summary>
        /// <param name="connectionString">
        /// Storage Account Connection String
        /// </param>
        /// <param name="tableName">
        /// Name of the table
        /// </param>
        internal AzureTableProvider(string connectionString, string tableName)
        {
            this.tableName = tableName;
            tableClient = GetCloudTableClient(connectionString, new ExponentialRetry(TimeSpan.FromSeconds(10), 5));
            tableClient.CreateTableIfNotExist(tableName);
        }

        /// <summary>
        /// Insert an entity into table
        /// </summary>
        /// <param name="entity">
        /// ScheduledJobEntity to be inserted
        /// </param>
        /// <returns>
        /// Task of TableResult 
        /// </returns>
        public async Task<TableResult> InsertAsync(ScheduledJobEntity entity)
        {
            CloudTable table = tableClient.GetTableReference(tableName);
            return await Task<TableResult>.Factory.FromAsync(
                    table.BeginExecute,
                    table.EndExecute,
                    TableOperation.Insert(entity),
                    null).ConfigureAwait(false);
        }

        /// <summary>
        /// Update an entity in table
        /// </summary>
        /// <param name="entity">
        /// ScheduledJobEntity to be updated
        /// </param>
        /// <returns>
        /// Task of TableResult 
        /// </returns>
        public async Task<TableResult> UpdateAsync(ScheduledJobEntity entity)
        {
            CloudTable table = tableClient.GetTableReference(tableName);
            return await Task<TableResult>.Factory.FromAsync(
                       table.BeginExecute,
                       table.EndExecute,
                       TableOperation.Replace(entity),
                       null).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an entity from the table
        /// </summary>
        /// <param name="partitionKey">
        /// PartitionKey of the entity
        /// </param>
        /// <param name="rowKey">
        /// RowKey for the entity
        /// </param>
        /// <returns>
        /// Task of TableResult 
        /// </returns>
        public async Task<TableResult> RetrieveAsync(string partitionKey, string rowKey)
        {
            CloudTable table = tableClient.GetTableReference(tableName);
            return await Task<TableResult>.Factory.FromAsync(
                     table.BeginExecute,
                     table.EndExecute,
                     TableOperation.Retrieve<ScheduledJobEntity>(
                               partitionKey,
                               rowKey),
                     null).ConfigureAwait(false);
        }

        /// <summary>
        /// Testing hook
        /// </summary>
        /// <param name="connectionString">
        /// Connection string
        /// </param>
        /// <param name="tableName">
        /// Table Name
        /// </param>
        internal static void DeleteTable(string connectionString, string tableName)
        {
            GetCloudTableClient(connectionString).GetTableReference(tableName).DeleteIfExists();
        }

        /// <summary>
        /// Gets CloudTableClient handle
        /// </summary>
        /// <param name="connectionString">
        /// Storage Account connection string
        /// </param>
        /// <param name="retryPolicy">
        /// Optional retry policy
        /// </param>
        /// <returns>
        /// Instance of CloudTableClient
        /// </returns>
        static CloudTableClient GetCloudTableClient(string connectionString, IRetryPolicy retryPolicy = null)
        {
            CloudTableClient client = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString).CreateCloudTableClient();
            if (retryPolicy != null)
            {
                client.RetryPolicy = retryPolicy;
            }
            return client;
        }

        /// <summary>
        /// Gets handle to CloudTable.
        /// Can be used to execute queries agains the table.
        /// </summary>
        internal CloudTable Table
        {
            get
            {
                return tableClient.GetTableReference(tableName);
            } 
        }

        /// <summary>
        /// Table Client
        /// </summary>
        private CloudTableClient tableClient;

        /// <summary>
        /// Name of the table
        /// </summary>
        private string tableName;
    }
}