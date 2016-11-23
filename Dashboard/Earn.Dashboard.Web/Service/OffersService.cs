//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Configuration;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Earn.Dashboard.Web.Service
{
    public static class OffersService
    {
        private static DocumentClient client;
        private static Database database;
        private static DocumentCollection offersCollection;
        private static StoredProcedure getRewardNetworkDealsByState;

        static OffersService()
        {
            InitClient();
            InitDatabase();
            InitOffersCollection();
            InitStoredProcedures();
        }
        
        private static void InitClient()
        {
            if (client == null)
            {
                string endpoint = ConfigurationManager.AppSettings["ddb-endpoint"];
                string authKey = ConfigurationManager.AppSettings["ddb-authkey"];
                Uri endpointUri = new Uri(endpoint);
                client = new DocumentClient(endpointUri, authKey);
            }
        }

        private static void InitDatabase()
        {
            if (database == null)
            {
                string databaseId = ConfigurationManager.AppSettings["ddb-database-id"];
                database = Client.CreateDatabaseQuery().Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();
            }
        }

        private static void InitOffersCollection()
        {
            if (offersCollection == null)
            {
                string offersCollectionId = ConfigurationManager.AppSettings["ddb-offers-collection-id"];
                offersCollection = Client.CreateDocumentCollectionQuery(Database.SelfLink).Where(c => c.Id == offersCollectionId).AsEnumerable().FirstOrDefault();
            }
        }

        private static void InitStoredProcedures()
        {
            if (getRewardNetworkDealsByState == null)
            {
                string procedureName = ConfigurationManager.AppSettings["getRewardNetworkDealsByStateProcedureName"];
                getRewardNetworkDealsByState = Client.CreateStoredProcedureQuery(OffersCollection.StoredProceduresLink).Where(sp => sp.Id == procedureName).AsEnumerable().FirstOrDefault();
            }
        }
        
        private static Database Database
        {
            get
            {
                if (database == null)
                {
                    InitDatabase();
                }

                return database;
            }
        }

        public static DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    InitClient();
                }

                return client;
            }
        }

        public static DocumentCollection OffersCollection
        {
            get
            {
                if (offersCollection == null)
                {
                    InitOffersCollection();
                }

                return offersCollection;
            }
        }

        public static StoredProcedure GetRewardNetworkDealsByState
        {
            get
            {
                if (getRewardNetworkDealsByState == null)
                {
                    InitStoredProcedures();
                }

                return getRewardNetworkDealsByState;
            }
        }
    }
}