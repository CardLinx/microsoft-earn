//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents.Linq;

namespace Earn.Offers.Earn.Dal
{
    public class DocumentDBRepository
    {
        private DocumentClient client;
        private Database database;
        private DocumentCollection offersCollection;
        private StoredProcedure getRewardNetworkDealsByState;

        private DocumentDBRepository()
        {
            InitClient();
            InitDatabase();
            InitOffersCollection();
            InitStoredProcedures();
        }

        public static DocumentDBRepository Instance = new DocumentDBRepository();

        private void InitClient()
        {
            if (this.client == null)
            {
                string endpoint = ConfigurationManager.AppSettings["ddb-endpoint"];
                string authKey = ConfigurationManager.AppSettings["ddb-authkey"];
                Uri endpointUri = new Uri(endpoint);
                this.client = new DocumentClient(endpointUri, authKey);
            }
        }

        private void InitDatabase()
        {
            if (this.database == null)
            {
                string databaseId = ConfigurationManager.AppSettings["ddb-database-id"];
                this.database = this.Client.CreateDatabaseQuery().Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();
            }
        }

        private void InitOffersCollection()
        {
            if (this.offersCollection == null)
            {
                string offersCollectionId = ConfigurationManager.AppSettings["ddb-offers-collection-id"];
                this.offersCollection = Client.CreateDocumentCollectionQuery(Database.SelfLink).Where(c => c.Id == offersCollectionId).AsEnumerable().FirstOrDefault();
            }
        }

        private void InitStoredProcedures()
        {
            if (this.getRewardNetworkDealsByState == null)
            {
                string procedureName = ConfigurationManager.AppSettings["getRewardNetworkDealsByStateProcedureName"];
                this.getRewardNetworkDealsByState = this.Client.CreateStoredProcedureQuery(this.OffersCollection.StoredProceduresLink).Where(sp => sp.Id == procedureName).AsEnumerable().FirstOrDefault();
            }
        }

        public DocumentClient Client
        {
            get
            {
                if (this.client == null)
                {
                    InitClient();
                }

                return this.client;
            }
        }

        private Database Database
        {
            get
            {
                if (this.database == null)
                {
                    InitDatabase();
                }

                return this.database;
            }
        }

        public DocumentCollection OffersCollection
        {
            get
            {
                if (this.offersCollection == null)
                {
                    InitOffersCollection();
                }

                return this.offersCollection;
            }
        }

        public StoredProcedure GetRewardNetworkDealsByState
        {
            get
            {
                if (this.getRewardNetworkDealsByState == null)
                {
                    InitStoredProcedures();
                }

                return this.getRewardNetworkDealsByState;
            }
        }
    }
}