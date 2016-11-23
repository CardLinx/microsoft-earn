//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Net;
using System.Reflection;

namespace Azure.Utils
{
    public class AzureTable : IAzureTable
    {
        #region Fields

        /// <summary>
        ///     azure queue client
        /// </summary>
        private CloudTableClient tableClient;

        /// <summary>
        ///     Name of the azure table
        /// </summary>
        private string tableName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTable" /> class.
        /// </summary>
        public AzureTable()
        {
            // Ram: Introduced the default constructor here to make it easier for dependent projects to mock this class.
            // The overloaded argument accepting constructor makes it default to mock this class, since it needs the 
            // cloudstorageaccount info.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTable" /> class.
        /// </summary>
        /// <param name="cloudStorageAccount">
        ///     The cloud storage account.
        /// </param>
        /// <param name="tableName">
        ///     The table Name.
        /// </param>
        /// <param name="retryPolicy">
        ///     The retry Policy.
        /// </param>
        public AzureTable(CloudStorageAccount cloudStorageAccount, string tableName, IRetryPolicy retryPolicy = null)
        {
            CreateTable(cloudStorageAccount, tableName, retryPolicy);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the cloud table.
        /// </summary>
        /// <value>
        ///     The cloud table.
        /// </value>
        public CloudTable CloudTable { get; private set; }

        #endregion

        #region IAzureTable Methods

        /// <summary>
        ///  Initializes the new instance of azure table
        /// </summary>
        /// <param name="cloudStorageAccount">
        ///     The Cloud storage account for the azure table
        /// </param>
        /// <param name="tableName">
        ///  Name of the azure table
        /// </param>
        /// <param name="retryPolicy">
        ///  Retry policy config
        /// </param>
        public void Initialize(CloudStorageAccount cloudStorageAccount, string tableName, IRetryPolicy retryPolicy = null)
        {
            CreateTable(cloudStorageAccount, tableName, retryPolicy);
        }

        /// <summary>
        ///     Inserts an entity into the table
        /// </summary>
        /// <typeparam name="T">The ITableEntity</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>
        ///     true is saved
        /// </returns>
        public bool InsertEntity<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "The TableEntity cannot be null");
            }

            var operationContext = new OperationContext();
            try
            {
                TableOperation insertOrMergeOperation = TableOperation.Insert(entity);
                TableResult result = this.CloudTable.Execute(insertOrMergeOperation, null, operationContext);

                return result.HttpStatusCode == (int)HttpStatusCode.Created;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                {
                    if (operationContext.RequestResults != null && operationContext.RequestResults.Count > 1)
                    {
                        // In this case the conflict occured due to retry execution - just ignore the conflict
                        Log.Info(
                            "Conflict error recived. However the conflict is most likely due to retry logic and can be ignored\n "
                            +
                            "Table Name={0}; PartitionKey={1}; RowKey={2}; Retries Count={3}\n" +
                            "Error Details: {4}",
                            this.tableName,
                            entity.PartitionKey,
                            entity.RowKey,
                            operationContext.RequestResults.Count,
                            ex);
                        return true;
                    }

                    Log.Error(
                        "Can't insert duplicate row into table: {0} PartitionKey={1} RowKey={2} \nError Details {3}",
                        this.tableName,
                        entity.PartitionKey,
                        entity.RowKey,
                        ex);
                    return false;
                }

                throw;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Batch Insert
        ///     Make sure that all the entities have the same partition id
        /// </summary>
        /// <param name="entities">
        ///     The entities.
        /// </param>
        /// <typeparam name="T">
        ///     The ITableEntity
        /// </typeparam>
        /// <returns>
        ///     true is all batch entities inserted
        /// </returns>
        public bool BatchInsertEntities<T>(IList<T> entities) where T : ITableEntity
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities", "The TableEntities cannot be null");
            }

            try
            {
                var batchOperation = new TableBatchOperation();
                foreach (T entity in entities)
                {
                    batchOperation.Insert(entity);
                }

                IList<TableResult> result = this.CloudTable.ExecuteBatch(batchOperation);
                bool succuess = true;
                foreach (TableResult tableResult in result)
                {
                    succuess = succuess && tableResult.HttpStatusCode == (int)HttpStatusCode.Created;
                }

                return succuess;
            }
            catch (StorageException ex)
            {
                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        ///     Batch Insert
        ///     Make sure that all the entities have the same partition id
        /// </summary>
        /// <param name="entities">
        ///     The entities.
        /// </param>
        /// <param name="failureCount">
        ///     The count of failures.
        /// </param>
        /// <typeparam name="T">
        ///     The ITableEntity
        /// </typeparam>
        /// <returns>
        ///     true is all batch entities inserted
        /// </returns>
        public bool BatchInsertOrMergeEntities<T>(IList<T> entities, out int failureCount) where T : ITableEntity
        {
            failureCount = 0;
            if (entities == null)
            {
                throw new ArgumentNullException("entities", "The TableEntities cannot be null");
            }

            var batchOperation = new TableBatchOperation();
            foreach (T entity in entities)
            {
                batchOperation.InsertOrMerge(entity);
            }

            IList<TableResult> result = this.CloudTable.ExecuteBatch(batchOperation);
            bool succuess = true;
            foreach (TableResult tableResult in result)
            {
                if (tableResult.HttpStatusCode != (int)HttpStatusCode.Created
                    && tableResult.HttpStatusCode != (int)HttpStatusCode.NoContent)
                {
                    succuess = false;
                    failureCount++;
                }
            }

            return succuess;
        }

        /// <summary>
        ///     Prepares a query
        /// </summary>
        /// <typeparam name="T">Type that derives from TableEntity</typeparam>
        /// <returns>a DataServiceQuery query</returns>
        public DataServiceQuery<T> CreateQuery<T>() where T : ITableEntity
        {
            return this.CloudTable.ServiceClient.GetTableServiceContext().CreateQuery<T>(this.CloudTable.Name);
        }

        /// <summary>
        ///     Deletes an entity
        /// </summary>
        /// <param name="partitionKey">the partition key</param>
        /// <param name="rowKey">the row key</param>
        /// <returns>
        ///     true if deleted
        /// </returns>
        public bool DeleteEntity(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentException("The Key Row Key is invalid.", "rowKey");
            }

            ITableEntity entity;
            if (GetEntity(partitionKey, rowKey, out entity))
            {
                return this.DeleteEntity(entity);
            }

            return false;
        }

        /// <summary>
        ///     Deletes an entity without retrieving it
        /// </summary>
        /// <param name="partitionKey">the partition key</param>
        /// <param name="rowKey">the row key</param>
        /// <returns>
        ///     true if deleted
        /// </returns>
        public bool DeleteEntityForce(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentException("The Key Row Key is invalid.", "rowKey");
            }

            var entity = new TableEntity() { PartitionKey = partitionKey, RowKey = rowKey, ETag = "*" };
            this.CloudTable.Execute(TableOperation.Delete(entity));
            return true;
        }

        /// <summary>
        ///     Deletes entities
        /// </summary>
        /// <param name="partitionKey">the partition key</param>
        /// <returns>
        ///     true if deleted
        /// </returns>
        public bool DeleteEntities(string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            var entities = GetEntities(partitionKey);
            foreach (var entity in entities)
            {
                entity.ETag = "*";
                this.CloudTable.Execute(TableOperation.Delete(entity));
            }

            return true;
        }

        /// <summary>
        ///     Deletes the entity.
        /// </summary>
        /// <typeparam name="T">The ITableEntity Type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>true if deleted.</returns>
        public bool DeleteEntity<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "The TableEntity cannot be null");
            }

            try
            {
                TableOperation deleteOperation = TableOperation.Delete(entity);
                this.CloudTable.Execute(deleteOperation);
                return true;
            }
            catch (StorageException ex)
            {
                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        ///     Deletes a table
        /// </summary>
        /// <returns>Return true on success, false if not found, throw exception on error.</returns>
        public bool DeleteTable()
        {
            return this.CloudTable.DeleteIfExists();
        }

        /// <summary>
        ///     Gets the entities with provided partitionKey.
        /// </summary>
        /// <typeparam name="T"> The type of the table entity </typeparam>
        /// <param name="partitionKey">
        ///     The partition key.
        /// </param>
        /// <returns>
        ///     The ITableEntity collection.
        /// </returns>
        public IEnumerable<T> GetEntities<T>(string partitionKey) where T : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            TableQuery<T> query = new TableQuery<T>().Where(filter);
            return this.CloudTable.ExecuteQuery(query);
        }

        /// <summary>
        ///     Gets the entities with provided partitionKey and count.
        /// </summary>
        /// <typeparam name="T"> The type of the table entity  </typeparam>
        /// <param name="partitionKey">
        ///     The partition key.
        /// </param>
        /// <param name="count">
        ///     The count.
        /// </param>
        /// <returns>
        ///     The ITableEntity collection.
        /// </returns>
        public IEnumerable<T> GetEntities<T>(string partitionKey, int count) where T : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than 0", "count");
            }

            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            TableQuery<T> query = new TableQuery<T>().Where(filter).Take(count);

            return this.CloudTable.ExecuteQuery(query);
        }

        /// <summary>
        ///     Gets the entities with provided partitionKey.
        /// </summary>
        /// <param name="partitionKey">
        ///     The partition key.
        /// </param>
        /// <returns>
        ///     The DynamicTableEntity collection.
        /// </returns>
        public IEnumerable<DynamicTableEntity> GetEntities(string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(filter);
            return this.CloudTable.ExecuteQuery(query);
        }

        /// <summary>
        ///     Gets the entities with provided partitionKey and count.
        /// </summary>
        /// <param name="partitionKey">
        ///     The partition key.
        /// </param>
        /// <param name="count">
        ///     The count.
        /// </param>
        /// <returns>
        ///     The DynamicTableEntity collection.
        /// </returns>
        public IEnumerable<DynamicTableEntity> GetEntities(string partitionKey, int count)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than 0", "count");
            }

            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(filter).Take(count);

            return this.CloudTable.ExecuteQuery(query);
        }

        /// <summary>
        ///     Retrieve an entity.
        /// </summary>
        /// <typeparam name="T">
        ///     the type
        /// </typeparam>
        /// <param name="partitionKey">
        ///     the partition key
        /// </param>
        /// <param name="rowKey">
        ///     the row key
        /// </param>
        /// <param name="entity">
        ///     the entity
        /// </param>
        /// <returns>
        ///     Return true on success, false if not found, throw exception on error.
        /// </returns>
        public bool GetEntity<T>(string partitionKey, string rowKey, out T entity) where T : ITableEntity
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentException("The Key Row Key is invalid.", "rowKey");
            }

            entity = default(T);
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                TableResult retrievedResult = this.CloudTable.Execute(retrieveOperation);
                entity = (T)retrievedResult.Result;
                return retrievedResult.HttpStatusCode == (int)HttpStatusCode.OK;
            }
            catch (DataServiceRequestException)
            {
                return false;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }

                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
            catch (DataServiceQueryException)
            {
                return false;
            }
            catch (DataServiceClientException ex)
            {
                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        ///     Retrieve an entity.
        /// </summary>
        /// <param name="partitionKey">
        ///     the partition key
        /// </param>
        /// <param name="rowKey">
        ///     the row key
        /// </param>
        /// <param name="entity">
        ///     the entity
        /// </param>
        /// <returns>
        ///     Return true on success, false if not found, throw exception on error.
        /// </returns>
        public bool GetEntity(string partitionKey, string rowKey, out DynamicTableEntity entity)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "partitionKey");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentException("The Key Row Key is invalid.", "rowKey");
            }

            entity = default(DynamicTableEntity);
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);
                TableResult retrievedResult = this.CloudTable.Execute(retrieveOperation);
                entity = (DynamicTableEntity)retrievedResult.Result;
                return retrievedResult.HttpStatusCode == (int)HttpStatusCode.OK;
            }
            catch (DataServiceRequestException)
            {
                return false;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }

                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
            catch (DataServiceQueryException)
            {
                return false;
            }
            catch (DataServiceClientException ex)
            {
                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        ///     Gets the entities with filter expression
        ///     Information about filter expression construction: http://blogs.msdn.com/b/windowsazurestorage/archive/2012/11/06/windows-azure-storage-client-library-2-0-tables-deep-dive.aspx
        /// </summary>
        /// <typeparam name="T"> The type of the table entity  </typeparam>
        /// <param name="filterExpression">
        ///     The filter expression
        /// </param>
        /// <param name="takeCount"> take count</param>
        /// <returns>
        ///     The ITableEntity collection.
        /// </returns>
        public IEnumerable<T> GetFilteredEntities<T>(string filterExpression, int? takeCount = null) where T : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(filterExpression))
            {
                throw new ArgumentException("The filter expression can't be null or emtpy string.", "filterExpression");
            }

            TableQuery<T> query = new TableQuery<T>().Where(filterExpression);
            if (takeCount.HasValue)
            {
                query = query.Take(takeCount);
            }

            return this.CloudTable.ExecuteQuery(query);
        }

        /// <summary>
        ///     Gets the entities with filter expression
        ///     Information about filter expression construction: http://blogs.msdn.com/b/windowsazurestorage/archive/2012/11/06/windows-azure-storage-client-library-2-0-tables-deep-dive.aspx
        /// </summary>
        /// <param name="filterExpression">
        ///     The filter expression
        /// </param>
        /// <returns>
        ///     The DynamicTableEntity collection.
        /// </returns>
        public IEnumerable<DynamicTableEntity> GetFilteredEntities(string filterExpression)
        {
            if (string.IsNullOrWhiteSpace(filterExpression))
            {
                throw new ArgumentException("The filter expression can't be null or emtpy string.", "filterExpression");
            }

            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(filterExpression);
            return this.CloudTable.ExecuteQuery(query);
        }

        /// <summary>
        ///     Merge update an entity (preserve previous properties not overwritten).
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="entity">the object</param>
        /// <returns>
        ///     Return true on success, false if not found, throw exception on error.
        /// </returns>
        public bool InsertOrMergeEntity<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "The TableEntity cannot be null");
            }

            if (string.IsNullOrWhiteSpace(entity.PartitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "entity");
            }

            if (string.IsNullOrWhiteSpace(entity.RowKey))
            {
                throw new ArgumentException("The Key Row Key is invalid.", "entity");
            }

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            TableResult result = this.CloudTable.Execute(insertOrMergeOperation);

            return result.HttpStatusCode == (int)HttpStatusCode.NoContent;
        }

        /// <summary>
        ///     Inserts the or replace.
        /// </summary>
        /// <typeparam name="T">The ITableEntity type</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>
        ///     Return true on success, false if not found, throw exception on error.
        /// </returns>
        public bool InsertOrReplaceEntity<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "The TableEntity cannot be null");
            }

            if (string.IsNullOrWhiteSpace(entity.PartitionKey) || string.IsNullOrWhiteSpace(entity.RowKey))
            {
                throw new ArgumentException("The Key fields are invalid.");
            }

            try
            {
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);
                TableResult result = this.CloudTable.Execute(insertOrReplaceOperation);

                return result.HttpStatusCode == (int)HttpStatusCode.NoContent;
            }
            catch (StorageException ex)
            {
                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        ///     Merges the specified entity.
        /// </summary>
        /// <typeparam name="T">The ITableEntity type</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>
        ///     Return true on success, false if not found, throw exception on error
        /// </returns>
        public bool MergeEntity<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "The TableEntity cannot be null");
            }

            if (string.IsNullOrWhiteSpace(entity.PartitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "entity");
            }

            if (string.IsNullOrWhiteSpace(entity.RowKey))
            {
                throw new ArgumentException("The Key Row Key is invalid.", "entity");
            }

            try
            {
                if (string.IsNullOrEmpty(entity.ETag))
                {
                    entity.ETag = "*";
                }

                TableOperation mergeOperation = TableOperation.Merge(entity);
                TableResult result = this.CloudTable.Execute(mergeOperation);

                return result.HttpStatusCode == (int)HttpStatusCode.NoContent;
            }
            catch (StorageException ex)
            {
                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        ///     Query entities. Use LINQ clauses to filter data.
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="entity">the object</param>
        /// <returns>
        ///     Return true on success, false if not found, throw exception on error.
        /// </returns>
        public bool ReplaceEntity<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "The TableEntity cannot be null");
            }

            if (string.IsNullOrWhiteSpace(entity.PartitionKey))
            {
                throw new ArgumentException("The Partition Key is invalid.", "entity");
            }

            if (string.IsNullOrWhiteSpace(entity.RowKey))
            {
                throw new ArgumentException("The Key Row Key is invalid.", "entity");
            }

            try
            {
                if (string.IsNullOrEmpty(entity.ETag))
                {
                    entity.ETag = "*";
                }

                TableOperation replaceOperation = TableOperation.Replace(entity);
                TableResult result = this.CloudTable.Execute(replaceOperation);

                return result.HttpStatusCode == (int)HttpStatusCode.NoContent;
            }
            catch (StorageException ex)
            {
                Log.Error(
                    ex,
                    "{0} table - {1}",
                    this.tableName,
                    MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the azure table
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
        private void CreateTable(CloudStorageAccount cloudStorageAccount, string tableName, IRetryPolicy retryPolicy = null)
        {
            AzureHelper.ValidateTableName(tableName);
            this.tableName = tableName;
            this.tableClient = cloudStorageAccount.CreateCloudTableClient();
            this.tableClient.DefaultRequestOptions.RetryPolicy = retryPolicy ?? new ExponentialRetry();
            this.CloudTable = this.tableClient.GetTableReference(tableName);

            this.CloudTable.CreateIfNotExists();
        }

        #endregion
    }
}