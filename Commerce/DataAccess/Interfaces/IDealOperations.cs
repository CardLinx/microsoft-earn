//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Represents operations on Deal objects within the data store.
    /// </summary>
    public interface IDealOperations
    {
        /// <summary>
        /// Registers the Deal in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode RegisterDeal();

        /// <summary>
        /// Retrieves the Deal with the ID in the context.
        /// </summary>
        /// <returns>
        /// * The Deal with the specified ID if it exists.
        /// * Else returns null.
        /// </returns>
        Deal RetrieveDeal();

        /// <summary>
        /// Updates the Deal in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode UpdateDeal();

        /// <summary>
        /// Retrieve all currently active deals in the system
        /// </summary>
        /// <returns>
        /// Enumeration of all such deal ids
        /// </returns>
        IEnumerable<Guid> RetrieveActiveDiscountIds();

        /// <summary>
        /// Retrive the discount id, given partner id and partner deal id
        /// </summary>
        /// <returns>
        /// DiscountId
        /// </returns>
        Guid? RetrieveDiscountIdFromPartnerDealId();

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}