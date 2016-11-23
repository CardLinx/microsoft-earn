//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Represents operations on User objects within the data store.
    /// </summary>
    public interface IUserOperations
    {
        /// <summary>
        /// Adds or updates the user in the context within the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        ResultCode AddOrUpdateUser();

        /// <summary>
        /// Retrieves the user with the identifier in the context from the data store.
        /// </summary>
        /// <returns>
        /// * The user if found.
        /// * Else returns null.
        /// </returns>
        User RetrieveUser();

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}