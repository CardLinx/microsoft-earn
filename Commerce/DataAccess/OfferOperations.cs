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
    using System;
    using System.Collections.Generic;
    using Lomo.Commerce.Context;

    /// <summary>
    /// Represents operations on Offer objects within the data store.
    /// </summary>
#if UNIT_TESTS
    [TestClass]
#endif    
    public class OfferOperations : CommerceOperations
    {
        /// <summary>
        /// Adds or updates the offer in the context within the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        public ResultCode AddOrUpdateOffer()
        {
            ResultCode result = ResultCode.Success;

            Offer offer = (Offer)Context[Key.Offer];
            int offerId = -1;
            bool created = false;
            result = SqlProcedure("AddOrUpdateOffer",
                                    new Dictionary<string, object>
                                    {
                                        { "@globalOfferID", offer.GlobalID },
                                        { "@globalProviderID", offer.GlobalProviderID },
                                        { "@offerType", offer.OfferType },
                                        { "@percentBack", offer.PercentBack },
                                        { "@active",  offer.Active }
                                    },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        offerId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("OfferId"));
                        created = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Created"));
                    }
                });

            if (result == ResultCode.Success)
            {
                offer.Id = offerId;
                if (created == true)
                {
                    result = ResultCode.Created;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves the offer with the global ID in the context from the data store.
        /// </summary>
        /// <returns>
        /// * The offer with the global ID in the context if it exists.
        /// * Else returns null.
        /// </returns>
        public Offer RetrieveOffer()
        {
            Offer result = null;

            Guid globalOfferID = (Guid)Context[Key.GlobalOfferID];
            SqlProcedure("GetOfferByGlobalId",
                new Dictionary<string, object>
                {
                    { "@globalOfferID", globalOfferID }
                },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        result = new Offer
                        {
                            Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("OfferId")),
                            GlobalID = globalOfferID,
                            GlobalProviderID = sqlDataReader.GetString(sqlDataReader.GetOrdinal("GlobalProviderID")),
                            OfferType = (OfferType)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("OfferType")),
                            PercentBack = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("PercentBack")),
                            Active = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Active"))
                        };
                    }
                });

            return result;
        }

        //****************************//
        #region Tests
#if UNIT_TESTS
        /// <summary>
        /// Ensures that AddOrUpdateOffer behaves as expected.
        /// </summary>
        [TestMethod]
        public void AddOrUpdateOfferTest()
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
            ProviderOperations providerOperations = new ProviderOperations();
            providerOperations.Context = Context;
            Assert.AreEqual(ResultCode.Created, providerOperations.AddOrUpdateProvider());

            // Create an offer.
            Offer offer = new Offer
            {
                GlobalID = Guid.NewGuid(),
                GlobalProviderID = "ProviderID1",
                OfferType = OfferType.Earn,
                PercentBack = 7,
                Active = true
            };

            // Place objects in the Context.
            Context[Key.Offer] = offer;

            // Add the offer.
            Assert.AreEqual(ResultCode.Created, AddOrUpdateOffer());

            // Update the offer.
            offer.PercentBack = 25;
            Assert.AreEqual(ResultCode.Success, AddOrUpdateOffer());

            // Invalid global provider ID.
            offer.GlobalProviderID = "invalid";
            Assert.AreEqual(ResultCode.InvalidDeal, AddOrUpdateOffer());
        }

        /// <summary>
        /// Ensures that RetrieveOffer behaves as expected.
        /// </summary>
        [TestMethod]
        public void RetrieveOfferTest()
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
            ProviderOperations providerOperations = new ProviderOperations();
            providerOperations.Context = Context;
            Assert.AreEqual(ResultCode.Created, providerOperations.AddOrUpdateProvider());

            // Create an offer.
            Offer offer = new Offer
            {
                GlobalID = Guid.NewGuid(),
                GlobalProviderID = "ProviderID1",
                OfferType = OfferType.Earn,
                PercentBack = 7,
                Active = true
            };

            // Place objects in the Context.
            Context[Key.Offer] = offer;

            // Add the offer.
            Assert.AreEqual(ResultCode.Created, AddOrUpdateOffer());

            // Retrieve the offer.
            Context[Key.Offer] = null;
            Context[Key.GlobalOfferID] = offer.GlobalID;
            Offer retrievedOffer = RetrieveOffer();

            // Compare the offers.
            Assert.AreEqual(offer.Id, retrievedOffer.Id);
            Assert.AreEqual(offer.GlobalID, retrievedOffer.GlobalID);
            Assert.AreEqual(offer.GlobalProviderID, retrievedOffer.GlobalProviderID);
            Assert.AreEqual(offer.OfferType, retrievedOffer.OfferType);
            Assert.AreEqual(offer.PercentBack, retrievedOffer.PercentBack);
            Assert.AreEqual(offer.Active, retrievedOffer.Active);

            // Invalid global offer ID
            Context[Key.GlobalOfferID] = Guid.NewGuid();
            Assert.IsNull(RetrieveOffer());
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