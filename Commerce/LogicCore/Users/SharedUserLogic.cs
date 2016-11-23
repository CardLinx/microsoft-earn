//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains methods to perform shared business logic for user objects.
    /// </summary>
    public class SharedUserLogic
    {
        /// <summary>
        /// Initializes a new instance of the SharedUserLogic class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        /// <param name="userOperations">
        /// The object to use to perform operations on users.
        /// </param>
        public SharedUserLogic(CommerceContext context,
                               IUserOperations userOperations)
        {
            Context = context;
            UserOperations = userOperations;
        }

        /// <summary>
        /// Retrieves the specified user from the data store if one exists and logs accordingly.
        /// </summary>
        /// <returns>
        /// * The specified user from the data store if successful.
        /// * Else returns null.
        /// </returns>
        public User RetrieveUser()
        {
            Context.Log.Verbose("Retrieving specified User from the data store.");
            User result = UserOperations.RetrieveUser();
            if (result != null)
            {
                Context.Log.Verbose("Specified User retrieved from data store.");
            }
            else
            {
                Context.Log.Verbose("Specified User does not exist within the data store.");
            }

            return result;
        }

        /// <summary>
        /// Adds the User object in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddUser()
        {
            Context.Log.Verbose("Attempting to add the User to the data store.");
            ResultCode result = UserOperations.AddOrUpdateUser();
            Context.Log.Verbose("ResultCode after adding the User to the data store: {0}", result);
            
            return result;
        }

        /// <summary>
        /// Gets or sets the object to use to perform operations on users.
        /// </summary>
        internal IUserOperations UserOperations { get; set; }

        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        private CommerceContext Context { get; set; }
    }
}