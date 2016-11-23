//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System.Collections.Generic;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Represents operations on user deal redemption history.
    /// </summary>
    public interface IRedemptionHistoryOperations
    {
        /// <summary>
        /// Retrieves the list of redemption history items for the user in the context.
        /// </summary>
        /// <returns>
        /// The list of redemption history items for the user in the context.
        /// </returns>
        IEnumerable<RedemptionHistoryItem> RetrieveRedemptionHistory();

        /// <summary>
        /// Returns the list of earn/burn transactions for the user in context
        /// </summary>
        /// <returns>List of earn and burn redemption items for the user in context</returns>
        IEnumerable<RedemptionHistoryItem> RetrieveMicrosoftEarnRedemptionHistory();

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}