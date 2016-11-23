//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
#if UNIT_TESTS
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Lomo.Commerce.DataAccess.Test;
#endif
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;
    using System.Collections.Generic;
    using System;
    using Lomo.Commerce.Context;

    /// <summary>
    /// Represents operations on Provider objects within the data store.
    /// </summary>
#if UNIT_TESTS
    [TestClass]
#endif    
    public class ProviderOperations : CommerceOperations
    {
        /// <summary>
        /// Adds or updates the provider in the context within the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        public ResultCode AddOrUpdateProvider()
        {
            ResultCode result = ResultCode.Success;

            Provider provider = (Provider)Context[Key.Provider];
            int providerId = -1;
            bool created = false;
            result = SqlProcedure("AddOrUpdateProvider",
                                    new Dictionary<string, object>
                                    {
                                        { "@globalProviderID", provider.GlobalID },
                                        { "@name", provider.Name }
                                    },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        providerId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ProviderId"));
                        created = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Created"));
                    }
                });

            if (result == ResultCode.Success)
            {
                provider.Id = providerId;
                if (created == true)
                {
                    result = ResultCode.Created;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves the provider with the global ID in the context from the data store.
        /// </summary>
        /// <returns>
        /// * The provider with the global ID in the context if it exists.
        /// * Else returns null.
        /// </returns>
        public Provider RetrieveProvider()
        {
            Provider result = null;

            string globalProviderID = (string)Context[Key.GlobalProviderID];
            SqlProcedure("GetProviderByGlobalID",
                new Dictionary<string, object>
                {
                    { "@globalProviderID", globalProviderID }
                },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        result = new Provider
                        {
                            Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ProviderId")),
                            GlobalID = globalProviderID,
                            Name = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Name"))
                        };
                    }
                });

            return result;
        }

        //****************************//
        #region Tests
#if UNIT_TESTS
        /// <summary>
        /// Ensures that AddOrUpdateProvider behaves as expected.
        /// </summary>
        [TestMethod]
        public void AddOrUpdateProviderTest()
        {
            // Create a provider object.
            Provider provider = new Provider
            {
                GlobalID = "ProviderID1",
                Name = "Provider1"
            };

            // Place objects in the Context.
            Context = new CommerceContext(String.Empty);
            Context[Key.Provider] = provider;

            // Add the provider.
            Assert.AreEqual(ResultCode.Created, AddOrUpdateProvider());

            // Update the provider.
            provider.Name = "Provider1a";
            Assert.AreEqual(ResultCode.Success, AddOrUpdateProvider());
        }

        // RetrieveProviderTest
        //  Ensures that RetrieveProvider behaves as expected.
        [TestMethod]
        public void RetrieveProviderTest()
        {
            // Create a provider.
            Provider provider = new Provider()
            {
                GlobalID = "ProviderID1",
                Name = "Provider1"
            };

            // Place objects in the Context.
            Context = new CommerceContext(String.Empty);
            Context[Key.Provider] = provider;

            // Add the provider.
            Assert.AreEqual(ResultCode.Created, AddOrUpdateProvider());

            // Retrieve the provider object.
            Context[Key.Provider] = null;
            Context[Key.GlobalProviderID] = provider.GlobalID;
            Provider retrievedProvider = RetrieveProvider();

            // Compare top-level provider info.
            Assert.AreEqual(provider.Id, retrievedProvider.Id);
            Assert.AreEqual(provider.GlobalID, retrievedProvider.GlobalID);
            Assert.AreEqual(provider.Name, retrievedProvider.Name);
        }

        /// <summary>
        /// Cleans up the datastore after every test.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            DatabaseTestManagement.CleanupDataStore();
        }
#endif        
        #endregion
    }
}