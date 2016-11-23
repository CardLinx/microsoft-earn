//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Represents operations on ClaimedDeal objects within the data store.
    /// </summary>
    public interface IClaimedDealOperations
    {
        /// <summary>
        /// Adds record of the claimed deal in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode AddClaimedDeal();

        /// <summary>
        /// Retrieves the list of deals currently claimed by the user in the context.
        /// </summary>
        /// <returns>
        /// The list of deals currently claimed by the user in the context.
        /// </returns>
        IEnumerable<Guid> RetrieveClaimedDeals();

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}