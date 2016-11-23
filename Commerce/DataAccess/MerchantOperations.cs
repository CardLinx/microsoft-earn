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
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Globalization;

    /// <summary>
    /// Represents operations on Merchant objects within the data store.
    /// </summary>
#if UNIT_TESTS
    [TestClass]
#endif    
    public class MerchantOperations : CommerceOperations
    {
        /// <summary>
        /// Adds or updates the merchant in the context within the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The Merchant object in the context does not include partner merchant info. Only a fully populated Merchant can be added to the data store.
        /// </exception>
        public ResultCode AddOrUpdateMerchant()
        {
            ResultCode result = ResultCode.Success;

            Merchant merchant = (Merchant)Context[Key.Merchant];
            if (merchant.IncludePartnerMerchantIDs == false)
            {
                throw new ArgumentException("The Merchant object in the context does not include partner merchant info." +
                                            "Only a fully populated Merchant can be added to the data store.");
            }

            int merchantId = -1;
            bool created = false;
            using (DataTable partnerMerchantsAuthorizationIDsTable = new DataTable("PartnerMerchantIDs"))
            {
                // Build the PartnerMerchantAuthorizationIDs table parameter.
                partnerMerchantsAuthorizationIDsTable.Locale = CultureInfo.InvariantCulture;
                partnerMerchantsAuthorizationIDsTable.Columns.Add("Partner", typeof(int));
                partnerMerchantsAuthorizationIDsTable.Columns.Add("EventID", typeof(string));
                partnerMerchantsAuthorizationIDsTable.Columns.Add("AddOrUpdate", typeof(bool));
                foreach (PartnerMerchantAuthorizationID partnerMerchantAuthorizationID in merchant.PartnerMerchantAuthorizationIDs)
                {
                    partnerMerchantsAuthorizationIDsTable.Rows.Add(partnerMerchantAuthorizationID.Partner, partnerMerchantAuthorizationID.AuthorizationID,
                                                                   partnerMerchantAuthorizationID.AddOrUpdate);
                }
                using (DataTable partnerMerchantsSettlementIDsTable = new DataTable("PartnerMerchantIDs"))
                {
                    // Build the PartnerMerchantSettlementIDs table parameter.
                    partnerMerchantsSettlementIDsTable.Locale = CultureInfo.InvariantCulture;
                    partnerMerchantsSettlementIDsTable.Columns.Add("Partner", typeof(int));
                    partnerMerchantsSettlementIDsTable.Columns.Add("EventID", typeof(string));
                    partnerMerchantsSettlementIDsTable.Columns.Add("AddOrUpdate", typeof(bool));
                    foreach (PartnerMerchantSettlementID partnerMerchantSettlementID in merchant.PartnerMerchantSettlementIDs)
                    {
                        partnerMerchantsSettlementIDsTable.Rows.Add(partnerMerchantSettlementID.Partner, partnerMerchantSettlementID.SettlementID,
                                                                    partnerMerchantSettlementID.AddOrUpdate);
                    }

                    result = SqlProcedure("AddOrUpdateMerchant",
                                          new Dictionary<string, object>
                                          {
                                              { "@globalMerchantID", merchant.GlobalID },
                                              { "@name", merchant.Name },
                                              { "@globalProviderID", merchant.GlobalProviderID },
                                              { "@partnerMerchantAuthorizationIDList", partnerMerchantsAuthorizationIDsTable },
                                              { "@partnerMerchantSettlementIDList", partnerMerchantsSettlementIDsTable }
                                          },
                        (sqlDataReader) =>
                        {
                            if (sqlDataReader.Read() == true)
                            {
                                merchantId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("MerchantId"));
                                created = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Created"));
                            }
                        });
                }
            }

            if (result == ResultCode.Success)
            {
                merchant.Id = merchantId;
                if (created == true)
                {
                    result = ResultCode.Created;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves the Merchant with the global ID in the context from the data store.
        /// </summary>
        /// <returns>
        /// * The merchant with the global ID in the context if it exists.
        /// * Else returns null.
        /// </returns>
        /// <remarks>
        /// By default, only the top-level merchant information is populated. To fully hydrate the Merchant object with its partner merchant information, set Key.IncludePartnerMerchants = true in the context.
        /// </remarks>
        public Merchant RetrieveMerchant()
        {
            Merchant result = null;

            List<PartnerMerchantAuthorizationID> partnerMerchantAuthorizationIDs = new List<PartnerMerchantAuthorizationID>();
            List<PartnerMerchantSettlementID> partnerMerchantSettlementIDs = new List<PartnerMerchantSettlementID>();
            string globalMerchantID = (string)Context[Key.GlobalMerchantID];
            bool includePartnerMerchants = (bool)Context[Key.IncludePartnerMerchants];
            SqlProcedure("GetMerchantByGlobalId",
                new Dictionary<string, object>
                {
                    { "@globalMerchantID", globalMerchantID },
                    { "@includePartnerMerchantIDs", includePartnerMerchants }
                },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        result = new Merchant
                        {
                            Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("MerchantId")),
                            GlobalID = globalMerchantID,
                            Name = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Name")),
                            GlobalProviderID = sqlDataReader.GetString(sqlDataReader.GetOrdinal("GlobalProviderID")),
                            IncludePartnerMerchantIDs = includePartnerMerchants
                        };
                    }

                    if (includePartnerMerchants == true)
                    {
                        // Authorization IDs
                        if (sqlDataReader.NextResult() == true)
                        {
                            while (sqlDataReader.Read() == true)
                            {
                                PartnerMerchantAuthorizationID partnerMerchantAuthorizationID = new PartnerMerchantAuthorizationID
                                {
                                    Partner = (CardBrand)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Partner")),
                                    AuthorizationID = sqlDataReader.GetString(sqlDataReader.GetOrdinal("AuthorizationID"))
                                };
                                partnerMerchantAuthorizationIDs.Add(partnerMerchantAuthorizationID);
                            }
                            result.PartnerMerchantAuthorizationIDs = partnerMerchantAuthorizationIDs;
                        }

                        // Settlement IDs
                        if (sqlDataReader.NextResult() == true)
                        {
                            while (sqlDataReader.Read() == true)
                            {
                                PartnerMerchantSettlementID partnerMerchantSettlementID = new PartnerMerchantSettlementID
                                {
                                    Partner = (CardBrand)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Partner")),
                                    SettlementID = sqlDataReader.GetString(sqlDataReader.GetOrdinal("SettlementID"))
                                };
                                partnerMerchantSettlementIDs.Add(partnerMerchantSettlementID);
                            }
                            result.PartnerMerchantSettlementIDs = partnerMerchantSettlementIDs;
                        }
                    }
                });

            return result;
        }

        //****************************//
        #region Tests
#if UNIT_TESTS
        /// <summary>
        /// Ensures that AddOrUpdateMerchant behaves as expected.
        /// </summary>
        [TestMethod]
        public void AddOrUpdateMerchantTest()
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

            // Create a merchant with populated partner merchant records.
            List<PartnerMerchantAuthorizationID> partnerMerchantAuthorizationIDs = new List<PartnerMerchantAuthorizationID>();
            PartnerMerchantAuthorizationID visaAuthorizationID = new PartnerMerchantAuthorizationID
            {
                Partner = CardBrand.Visa,
                AuthorizationID = "VisaMerchant1AuthAndSettlementID1"
            };
            partnerMerchantAuthorizationIDs.Add(visaAuthorizationID);

            PartnerMerchantAuthorizationID masterCardAuthorizationID = new PartnerMerchantAuthorizationID
            {
                Partner = CardBrand.MasterCard,
                AuthorizationID = "MasterCardMerchant1AuthID1"
            };
            partnerMerchantAuthorizationIDs.Add(masterCardAuthorizationID);

            List<PartnerMerchantSettlementID> partnerMerchantSettlementIDs = new List<PartnerMerchantSettlementID>();
            PartnerMerchantSettlementID visaSettlementID = new PartnerMerchantSettlementID
            {
                Partner = CardBrand.Visa,
                SettlementID = "VisaMerchant1AuthAndSettlementID1"
            };
            partnerMerchantSettlementIDs.Add(visaSettlementID);

            PartnerMerchantSettlementID masterCardSettlementID = new PartnerMerchantSettlementID
            {
                Partner = CardBrand.MasterCard,
                SettlementID = "MasterCardMerchant1SettlementID1"
            };
            partnerMerchantSettlementIDs.Add(masterCardSettlementID);

            Merchant merchant = new Merchant()
            {
                GlobalID = "MerchantID1",
                Name = "Merchant1",
                GlobalProviderID = provider.GlobalID,
                IncludePartnerMerchantIDs = true,
                PartnerMerchantAuthorizationIDs = partnerMerchantAuthorizationIDs,
                PartnerMerchantSettlementIDs = partnerMerchantSettlementIDs
            };

            // Place objects in the Context.
            Context[Key.Merchant] = merchant;

            // Add the merchant.
            Assert.AreEqual(ResultCode.Created, AddOrUpdateMerchant());

            // Update the merchant (deactivate a PartnerMerchantAuthorizationID record).
            partnerMerchantAuthorizationIDs.Remove(visaAuthorizationID);
            Assert.AreEqual(ResultCode.Success, AddOrUpdateMerchant());

            // Update the merchant (deactivate a PartnerMerchantSettlementID record).
            partnerMerchantSettlementIDs.Remove(masterCardSettlementID);
            Assert.AreEqual(ResultCode.Success, AddOrUpdateMerchant());

            // Update the merchant (reactivate a PartnerMerchantAuthorizationID record).
            partnerMerchantAuthorizationIDs.Add(visaAuthorizationID);
            Assert.AreEqual(ResultCode.Success, AddOrUpdateMerchant());

            // Update the merchant (reactivate a PartnerMerchantSettlementID record).
            partnerMerchantSettlementIDs.Add(masterCardSettlementID);
            Assert.AreEqual(ResultCode.Success, AddOrUpdateMerchant());

            // Invalid global provider ID.
            merchant.GlobalProviderID = "invalid";
            Assert.AreEqual(ResultCode.InvalidMerchant, AddOrUpdateMerchant());
            merchant.GlobalProviderID = provider.GlobalID;
            
            // Without PartnerMerchants flagged not populated.
            bool correctExceptionThrown = false;
            merchant.IncludePartnerMerchantIDs = false;
            try
            {
                AddOrUpdateMerchant();
            }
            catch (ArgumentException)
            {
                correctExceptionThrown = true;
            }
            finally
            {
                Context[Key.Merchant] = null;
            }
            Assert.IsTrue(correctExceptionThrown);
        }


        // RetrieveMerchantTest
        //  Ensures that RetrieveMerchant behaves as expected.
        [TestMethod]
        public void RetrieveMerchantTest()
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

            // Create a merchant with populated partner merchant records.
            List<PartnerMerchantAuthorizationID> partnerMerchantAuthorizationIDs = new List<PartnerMerchantAuthorizationID>();
            PartnerMerchantAuthorizationID visaAuthorizationID = new PartnerMerchantAuthorizationID
            {
                Partner = CardBrand.Visa,
                AuthorizationID = "VisaMerchant1AuthAndSettlementID1"
            };
            partnerMerchantAuthorizationIDs.Add(visaAuthorizationID);

            PartnerMerchantAuthorizationID masterCardAuthorizationID = new PartnerMerchantAuthorizationID
            {
                Partner = CardBrand.MasterCard,
                AuthorizationID = "MasterCardMerchant1AuthID1"
            };
            partnerMerchantAuthorizationIDs.Add(masterCardAuthorizationID);

            List<PartnerMerchantSettlementID> partnerMerchantSettlementIDs = new List<PartnerMerchantSettlementID>();
            PartnerMerchantSettlementID visaSettlementID = new PartnerMerchantSettlementID
            {
                Partner = CardBrand.Visa,
                SettlementID = "VisaMerchant1AuthAndSettlementID1"
            };
            partnerMerchantSettlementIDs.Add(visaSettlementID);

            PartnerMerchantSettlementID masterCardSettlementID = new PartnerMerchantSettlementID
            {
                Partner = CardBrand.MasterCard,
                SettlementID = "MasterCardMerchant1SettlementID1"
            };
            partnerMerchantSettlementIDs.Add(masterCardSettlementID);

            Merchant merchant = new Merchant()
            {
                GlobalID = "MerchantID1",
                Name = "Merchant1",
                GlobalProviderID = provider.GlobalID,
                IncludePartnerMerchantIDs = true,
                PartnerMerchantAuthorizationIDs = partnerMerchantAuthorizationIDs,
                PartnerMerchantSettlementIDs = partnerMerchantSettlementIDs
            };

            // Place objects in the Context.
            Context[Key.Merchant] = merchant;

            // Add the merchant.
            Assert.AreEqual(ResultCode.Created, AddOrUpdateMerchant());

            // Retrieve the merchant object without the partner merchant info.
            Context[Key.Merchant] = null;
            Context[Key.GlobalMerchantID] = merchant.GlobalID;
            Context[Key.IncludePartnerMerchants] = false;
            Merchant retrievedMerchant = RetrieveMerchant();

            // Compare top-level merchant info.
            Assert.AreEqual(merchant.Id, retrievedMerchant.Id);
            Assert.AreEqual(merchant.GlobalID, retrievedMerchant.GlobalID);
            Assert.AreEqual(merchant.Name, retrievedMerchant.Name);

            // Ensure partner merchant info list is in the expected state.
            Assert.IsFalse(retrievedMerchant.IncludePartnerMerchantIDs);
            Assert.IsNotNull(retrievedMerchant.PartnerMerchantAuthorizationIDs);
            Assert.AreEqual(0, retrievedMerchant.PartnerMerchantAuthorizationIDs.Count());

            // Retrieve the merchant object with partner merchant info.
            Context[Key.IncludePartnerMerchants] = true;
            retrievedMerchant = RetrieveMerchant();

            // Compare top-level merchant info.
            Assert.AreEqual(merchant.Id, retrievedMerchant.Id);
            Assert.AreEqual(merchant.GlobalID, retrievedMerchant.GlobalID);
            Assert.AreEqual(merchant.Name, retrievedMerchant.Name);

            // Compare partner merchant info list.
            Assert.IsTrue(retrievedMerchant.IncludePartnerMerchantIDs);
            IEnumerable<PartnerMerchantAuthorizationID> retrievedPartnerMerchantAuthorizationIDs = retrievedMerchant.PartnerMerchantAuthorizationIDs;
            Assert.IsNotNull(retrievedPartnerMerchantAuthorizationIDs);
            Assert.AreEqual(partnerMerchantAuthorizationIDs.Count, retrievedPartnerMerchantAuthorizationIDs.Count());
            IEnumerable<PartnerMerchantSettlementID> retrievedPartnerMerchantSettlementIDs = retrievedMerchant.PartnerMerchantSettlementIDs;
            Assert.IsNotNull(retrievedPartnerMerchantSettlementIDs);
            Assert.AreEqual(partnerMerchantSettlementIDs.Count, retrievedPartnerMerchantSettlementIDs.Count());

            // Compare the first partner merchant authorization ID object.
            PartnerMerchantAuthorizationID initialPartnerMerchantAuthorizationID = partnerMerchantAuthorizationIDs[0];
            PartnerMerchantAuthorizationID retrievedPartnerMerchantAuthorizationID = retrievedPartnerMerchantAuthorizationIDs.ElementAt(0);
            Assert.AreEqual(initialPartnerMerchantAuthorizationID.Partner, retrievedPartnerMerchantAuthorizationID.Partner);
            Assert.AreEqual(initialPartnerMerchantAuthorizationID.AuthorizationID, retrievedPartnerMerchantAuthorizationID.AuthorizationID);

            // Compare the second partner merchant authorization ID object.
            initialPartnerMerchantAuthorizationID = partnerMerchantAuthorizationIDs[1];
            retrievedPartnerMerchantAuthorizationID = retrievedPartnerMerchantAuthorizationIDs.ElementAt(1);
            Assert.AreEqual(initialPartnerMerchantAuthorizationID.Partner, retrievedPartnerMerchantAuthorizationID.Partner);
            Assert.AreEqual(initialPartnerMerchantAuthorizationID.AuthorizationID, retrievedPartnerMerchantAuthorizationID.AuthorizationID);

            // Compare the first partner merchant settlement ID object.
            PartnerMerchantSettlementID initialPartnerMerchantSettlementID = partnerMerchantSettlementIDs[0];
            PartnerMerchantSettlementID retrievedPartnerMerchantSettlementID = retrievedPartnerMerchantSettlementIDs.ElementAt(0);
            Assert.AreEqual(initialPartnerMerchantSettlementID.Partner, retrievedPartnerMerchantSettlementID.Partner);
            Assert.AreEqual(initialPartnerMerchantSettlementID.SettlementID, retrievedPartnerMerchantSettlementID.SettlementID);

            // Compare the second partner merchant authorization ID object.
            initialPartnerMerchantSettlementID = partnerMerchantSettlementIDs[1];
            retrievedPartnerMerchantSettlementID = retrievedPartnerMerchantSettlementIDs.ElementAt(1);
            Assert.AreEqual(initialPartnerMerchantSettlementID.Partner, retrievedPartnerMerchantSettlementID.Partner);
            Assert.AreEqual(initialPartnerMerchantSettlementID.SettlementID, retrievedPartnerMerchantSettlementID.SettlementID);

            // Finally test an invalid global merchant ID.
            Context[Key.GlobalMerchantID] = "invalid";
            Assert.IsNull(RetrieveMerchant());
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