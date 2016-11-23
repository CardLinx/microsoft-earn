//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace OffersDbSetUp
{
    class Program
    {
        static DocumentClient Client;

        static void Main(string[] args)
        {
            string endpoint = ConfigurationManager.AppSettings["EarnDbUrl"];
            string authorizationKey = ConfigurationManager.AppSettings["EarnDbKey"];
            Client = new DocumentClient(new Uri(endpoint),
               authorizationKey,
               new ConnectionPolicy
               {
                   ConnectionMode = ConnectionMode.Direct,
                   ConnectionProtocol = Protocol.Tcp
               });
            string databaseId = ConfigurationManager.AppSettings["EarnDbName"];
            string collectionId = ConfigurationManager.AppSettings["EarnCollectionName"];
            try
            {
                Database database = ReadOrCreateDatabase(databaseId);
                DocumentCollection collection = ReadOrCreateCollection(database.SelfLink, collectionId);
                CreateIndex(database.Id, collection);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in setting up db : {e.ToString()}");
            }
            Console.WriteLine("done...");
            Console.ReadKey();
        }

        private static Database ReadOrCreateDatabase(string databaseId)
        {
            Console.WriteLine("Creating database...");
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == databaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            return db ?? Client.CreateDatabaseAsync(new Database { Id = databaseId }).Result;
        }

        private static DocumentCollection ReadOrCreateCollection(string databaseLink, string collectionId)
        {
            Console.WriteLine("Creating collection...");
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

        private static void CreateIndex(string databaseId, DocumentCollection collection)
        {
            collection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            collection.IndexingPolicy.IncludedPaths.Clear();
            collection.IndexingPolicy.ExcludedPaths.Clear();
            collection.IndexingPolicy.IncludedPaths.Add(
                new IncludedPath
                {
                    Path = "/type/?",
                    Indexes = new Collection<Index> {
            new HashIndex(DataType.String) { Precision = -1 } }
                }
                 );
            collection.IndexingPolicy.IncludedPaths.Add(
                 new IncludedPath
                 {
                     Path = "/provider_id/?",
                     Indexes = new Collection<Index> {
            new HashIndex(DataType.String) { Precision = -1 } }
                 });

            // Mandatory Default 
            collection.IndexingPolicy.IncludedPaths.Add(
                new IncludedPath
                {
                    Path = "/*"
                });

            collection.IndexingPolicy.ExcludedPaths.Add(
              new ExcludedPath
              {
                  Path = "/extended_attributes/*"
              }
              );
            collection.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath
            {
                Path = "/payments/*"
            });

            var response = Client.ReplaceDocumentCollectionAsync(collection).Result;

            long smallWaitTimeMilliseconds = 1000;
            long progress = 0;

            while (progress < 100)
            {
                ResourceResponse<DocumentCollection> collectionReadResponse = Client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collection.Id)).Result;

                progress = collectionReadResponse.IndexTransformationProgress;

                Task.Delay(TimeSpan.FromMilliseconds(smallWaitTimeMilliseconds));
            }
        }

    }
}