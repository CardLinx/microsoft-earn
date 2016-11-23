//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;

    /// <summary>
    /// Represents operations on Authorization objects within the data store.
    /// </summary>
    public interface IAuthorizationOperations
    {
        /// <summary>
        /// Adds the Authorization in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode AddAuthorization();

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}