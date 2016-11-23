//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
#if UNIT_TESTS
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess.Test;
#endif
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Contains data store operations for User objects.
    /// </summary>
#if UNIT_TESTS
    [TestClass]
#endif    
    public class UserOperations : CommerceOperations, IUserOperations
    {
        /// <summary>
        /// Adds or updates the user in the context within the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        public ResultCode AddOrUpdateUser()
        {
            ResultCode result = ResultCode.Success;

            // Add the user to the data layer.
            User user = (User)Context[Key.User];
            int userId = -1;
            bool created = false;
            using (DataTable partnerUserIDsTable = new DataTable("PartnerUserIDRecords"))
            {
                // Build the PartnerUserIDs table parameter.
                partnerUserIDsTable.Locale = CultureInfo.InvariantCulture;
                partnerUserIDsTable.Columns.Add("Partner", typeof(int));
                partnerUserIDsTable.Columns.Add("PartnerUserID", typeof(string));
                foreach (PartnerUserInfo partnerUserID in user.PartnerUserInfoList)
                {
                    // Only commit a partner user ID to the data store if it was assigned by the partner. Otherwise, they are generated programatically.
                    if (partnerUserID.AssignedByPartner == true)
                    {
                        // NOTE: PartnerId is off by 1 from CardBrand. We're in the midst of fixing this.
                        partnerUserIDsTable.Rows.Add((int)partnerUserID.PartnerId + 1, partnerUserID.PartnerUserId);
                    }
                }
                result = SqlProcedure("AddOrUpdateUser",
                                        new Dictionary<string, object>
                                        {
                                            { "@globalUserID", user.GlobalId },
                                            { "@partnerUserIDRecords", partnerUserIDsTable }
                                        },
                    (sqlDataReader) =>
                    {
                        if (sqlDataReader.Read() == true)
                        {
                            userId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("UserId"));
                            created = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Created"));
                        }
                    });
            }

            if (result == ResultCode.Success)
            {
                user.Id = userId;
                if (created == true)
                {
                    result = ResultCode.Created;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves the user with the identifier in the context from the data store.
        /// </summary>
        /// <returns>
        /// * The user if found.
        /// * Else returns null.
        /// </returns>
        public User RetrieveUser()
        {
            User result = null;

            Guid globalUserId = (Guid)Context[Key.GlobalUserId];
            SqlProcedure("GetUserByGlobalId",
                         new Dictionary<string, object>
                         {
                             { "@globalId", globalUserId },
                             { "@includePartnerUserIDs", true } // We'll incorporate this fully when working on the Logic layer and / or Data Models.
                         },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        // Get the ID returned from the database.
                        result = new User
                        {
                            Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Id")),
                            GlobalId = globalUserId
                        }; 
                    }

                    if (sqlDataReader.NextResult() == true)
                    {
                        while (sqlDataReader.Read() == true)
                        {
                            PartnerUserInfo partnerUserInfo = new PartnerUserInfo
                            {
                                // NOTE: PartnerId is off by 1 from CardBrand. We're in the midst of fixing this.
                                PartnerId = (Partner)(sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Partner")) - 1),
                                PartnerUserId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerUserID")),
                                AssignedByPartner = true
                            };
                            result.PartnerUserInfoList.Add(partnerUserInfo);
                        }
                    }
                });

            if (result != null)
            {
                Context.Log.Verbose("GetUserByGlobalId retrieved the specified User.");
            }
            else
            {
                Context.Log.Verbose("GetUserByGlobalId could not find the specified User.");
            }

            return result;
        }

        //****************************//
        #region Tests
#if UNIT_TESTS
        /// <summary>
        /// Ensures that AddOrUpdateUser behaves as expected.
        /// </summary>
        [TestMethod]
        public void AddOrUpdateUserTest()
        {
            // Create a user object.
            Guid globalUserID = Guid.NewGuid();
            User user = new User
            {
                GlobalId = globalUserID
            };

            PartnerUserInfo visaPartnerUserInfo = new PartnerUserInfo
            {
                AssignedByPartner = true,
                PartnerId = Partner.Visa,
                PartnerUserId = "VisaPartnerUserID"
            };
            user.PartnerUserInfoList.Add(visaPartnerUserInfo);

            PartnerUserInfo masterCardPartnerUserInfo = new PartnerUserInfo
            {
                AssignedByPartner = true,
                PartnerId = Partner.MasterCard,
                PartnerUserId = "MasterCardPartnerUserID"
            };
            user.PartnerUserInfoList.Add(masterCardPartnerUserInfo);

            // Place objects in the Context.
            Context = new CommerceContext(String.Empty);
            Context[Key.User] = user;

            // Add the user.
            Assert.AreEqual(ResultCode.Created, AddOrUpdateUser());

            // Update the user.
            user.PartnerUserInfoList.RemoveAt(1);
            Assert.AreEqual(ResultCode.Success, AddOrUpdateUser());
        }

        // RetrieveUserTest
        //  Ensures that RetrieveUser behaves as expected.
        [TestMethod]
        public void RetrieveUserTest()
        {
            // Create a user object.
            Guid globalUserID = Guid.NewGuid();
            User user = new User
            {
                GlobalId = globalUserID
            };

            PartnerUserInfo visaPartnerUserInfo = new PartnerUserInfo
            {
                AssignedByPartner = true,
                PartnerId = Partner.Visa,
                PartnerUserId = "VisaPartnerUserID"
            };
            user.PartnerUserInfoList.Add(visaPartnerUserInfo);

            PartnerUserInfo masterCardPartnerUserInfo = new PartnerUserInfo
            {
                AssignedByPartner = true,
                PartnerId = Partner.MasterCard,
                PartnerUserId = "MasterCardPartnerUserID"
            };
            user.PartnerUserInfoList.Add(masterCardPartnerUserInfo);

            // Place objects in the Context.
            Context = new CommerceContext(String.Empty);
            Context[Key.User] = user;

            // Add the user.
            Assert.AreEqual(ResultCode.Created, AddOrUpdateUser());

            // Retrieve the user object.
            Context[Key.User] = null;
            Context[Key.GlobalUserId] = user.GlobalId;
            User retrievedUser = RetrieveUser();

            // Compare top-level user info.
            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual(user.Id, retrievedUser.Id);
            Assert.AreEqual(user.GlobalId, retrievedUser.GlobalId);

            // Compare partner user objects.
            Assert.IsNotNull(retrievedUser.PartnerUserInfoList);
            Assert.AreEqual(2, retrievedUser.PartnerUserInfoList.Count);
            PartnerUserInfo retrievedVisaPartnerUserInfo = retrievedUser.PartnerUserInfoList.First();
            Assert.AreEqual(visaPartnerUserInfo.AssignedByPartner, retrievedVisaPartnerUserInfo.AssignedByPartner);
            Assert.AreEqual(visaPartnerUserInfo.PartnerId, retrievedVisaPartnerUserInfo.PartnerId);
            Assert.AreEqual(visaPartnerUserInfo.PartnerUserId, retrievedVisaPartnerUserInfo.PartnerUserId);
            PartnerUserInfo retrievedMasterCardPartnerUserInfo = retrievedUser.PartnerUserInfoList.ElementAt(1);
            Assert.AreEqual(masterCardPartnerUserInfo.AssignedByPartner, retrievedMasterCardPartnerUserInfo.AssignedByPartner);
            Assert.AreEqual(masterCardPartnerUserInfo.PartnerId, retrievedMasterCardPartnerUserInfo.PartnerId);
            Assert.AreEqual(masterCardPartnerUserInfo.PartnerUserId, retrievedMasterCardPartnerUserInfo.PartnerUserId);

            // Try to get non-existent user.
            Context[Key.User] = null;
            Context[Key.GlobalUserId] = Guid.NewGuid();
            Assert.IsNull(RetrieveUser());
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