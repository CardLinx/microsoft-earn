//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.Dal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Threading.Tasks;
    using Lomo.Logging;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Client.TransientFaultHandling.Strategies;
    using Microsoft.Azure.Documents.Linq;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using System.Diagnostics;
    public class DocumentDbRepository : IRepository
    {
        private readonly RetryPolicy retryPolicy = new RetryPolicy(new DocumentDbTransientErrorDetectionStrategy(new WebErrorsKnowledgeForIdempotentOperations()), 3);

        public readonly DocumentClient Client;
        public readonly DocumentCollection Collection;

        public DocumentDbRepository(string endpoint, string authorizationKey, string databaseId, string collectionId)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentNullException("endpoint");
            }

            if (string.IsNullOrWhiteSpace(authorizationKey))
            {
                throw new ArgumentNullException("authorizationKey");
            }

            if (string.IsNullOrWhiteSpace(databaseId))
            {
                throw new ArgumentNullException("databaseId");
            }

            if (string.IsNullOrWhiteSpace(collectionId))
            {
                throw new ArgumentNullException("collectionId");
            }

            Client = new DocumentClient(new Uri(endpoint),
                authorizationKey,
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });
            Database database = ReadOrCreateDatabase(databaseId);
            Collection = ReadOrCreateCollection(database.SelfLink, collectionId);

            Client.OpenAsync().Wait();
        }

        public async Task<bool> CreateAsync<T>(IEnumerable<T> items)
        {
            if (items != null && items.Any())
            {
                try
                {
                    foreach (T item in items)
                    {
                        await ExecuteWithRetries(() => Client.CreateDocumentAsync(Collection.SelfLink, item)).ConfigureAwait(false);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Log.Error(e, "DocumentDbRepository: CreateAsync");
                }
            }

            return false;
        }    

        public async Task<bool> UpdateAsync<T>(IEnumerable<T> items)
        {
            if (items != null && items.Any())
            {
                try
                {                    
                    int documentsUpdated = 0;
                    long totalTimeTaken = 0;
                    Type type = typeof(T);
                    Log.Info($"Total documents of type {type.Name} to update : {items.Count()}");
                    PropertyInfo propertyInfo = type.GetProperty("Id", typeof(string));
                    foreach (T item in items)
                    {
                        string id = propertyInfo.GetValue(item).ToString();
                        Document document = await GetDocument(id).ConfigureAwait(false);
                        if (document != null)
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            await ExecuteWithRetries(() => Client.ReplaceDocumentAsync(document.SelfLink, item)).ConfigureAwait(false);
                            long timeTaken = sw.ElapsedMilliseconds;
                            totalTimeTaken += timeTaken;                          
                            documentsUpdated++;
                        }
                    }
                    
                    long avgTimeForUpdates = documentsUpdated > 0 ? (totalTimeTaken / documentsUpdated) : 0;
                    Log.Info($"Total documents of type {type.Name} updated : {documentsUpdated} in {avgTimeForUpdates} ms");

                    return true;

                }
                catch (Exception e)
                {
                    Log.Error(e, "DocumentDbRepository: UpdateAsync");
                }
            }

            return false;
        }

        public async Task<bool> DeleteAsync(IEnumerable<string> ids)
        {
            try
            {
                foreach (string id in ids)
                {
                    Document document = await GetDocument(id).ConfigureAwait(false);
                    if (document != null)
                    {
                        await ExecuteWithRetries(() => Client.DeleteDocumentAsync(document.SelfLink)).ConfigureAwait(false);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "DocumentDbRepository: DeleteAsync");
            }

            return false;
        }

        public async Task<Document> GetDocument(string id)
        {
            Document document = null;
            try
            {
                SqlParameterCollection parameters = new SqlParameterCollection();
                parameters.Add(new SqlParameter("@id", id));                
                SqlQuerySpec querySpec = new SqlQuerySpec("SELECT * FROM PROVIDERS p WHERE p.id = @id", parameters);
                await ExecuteWithRetries(() => Task.Run(() => document = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink, querySpec)
                            .AsEnumerable().FirstOrDefault()
                )).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, $"[{nameof(DocumentDbRepository)}.{nameof(GetDocument)}]");
            }

            return document;
        }

        public async Task<T> ExecuteWithRetries<T>(Func<Task<T>> function)
        {
            TimeSpan sleepTime;

            while (true)
            {
                try
                {
                    return await retryPolicy.ExecuteAsync(() => function());
                }
                catch (DocumentClientException de)
                {
                    //If docdb server throws "Request Rate" exception, wait for "x-ms-retry-after" interval before trying the query again
                    //Request rates are limited based on the collection's performance tier
                    if (de.StatusCode != null && (int)de.StatusCode != 429)
                    {
                        throw;
                    }

                    sleepTime = de.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        throw;
                    }

                    DocumentClientException de = (DocumentClientException)ae.InnerException;
                    if (de.StatusCode != null && (int)de.StatusCode != 429)
                    {
                        throw;
                    }

                    sleepTime = de.RetryAfter;
                }

                await Task.Delay(sleepTime);
            }
        }

        public async Task<StoredProcedure> CreateOrGetStoredProcAsync(DocumentCollection collection, string storedProcName, string storedProcFile)
        {
            StoredProcedure storedProcedure = Client.CreateStoredProcedureQuery(collection.StoredProceduresLink).Where(sp => sp.Id == storedProcName).AsEnumerable().FirstOrDefault();
            if (storedProcedure == null)
            {
                string spResourceName = string.Format("{0}.{1}.{2}", Assembly.GetExecutingAssembly().GetName().Name, "Stored_Procedures", storedProcFile);
                Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(spResourceName);
                if (resourceStream == null)
                {
                    throw new MissingManifestResourceException(string.Format("Stored procedure {0} is not found", storedProcFile));
                }

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    storedProcedure = new StoredProcedure
                    {
                        Id = storedProcName,
                        Body = reader.ReadToEnd()
                    };

                    storedProcedure = await Client.CreateStoredProcedureAsync(collection.SelfLink, storedProcedure).ConfigureAwait(false);
                }
            }

            return storedProcedure;
        }

        private Database ReadOrCreateDatabase(string databaseId)
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == databaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            return db ?? Client.CreateDatabaseAsync(new Database { Id = databaseId }).Result;
        }

        private DocumentCollection ReadOrCreateCollection(string databaseLink, string collectionId)
        {
            var documentCollection = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == collectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (documentCollection == null)
            {
                var collectionSpec = new DocumentCollection { Id = collectionId };
                var requestOptions = new RequestOptions { OfferType = "S1" };

                documentCollection = Client.CreateDocumentCollectionAsync(databaseLink, collectionSpec, requestOptions).Result;
            }

            return documentCollection;
        }
    }
}