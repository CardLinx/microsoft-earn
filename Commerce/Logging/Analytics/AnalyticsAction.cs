//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    /// <summary>
    /// Represents a set of analytics action for commerce service
    /// </summary>
    public enum AnalyticsAction
    {
        /// <summary>
        /// New card has been registered
        /// </summary>
        AddCard,

        /// <summary>
        /// A new deal has been registered
        /// </summary>
        RegisterDeal,

        /// <summary>
        /// Deal has been claimed
        /// </summary>
        ClaimedDeal,

        /// <summary>
        /// User has redeemed a deal
        /// </summary>
        RedeemedDeal,

        /// <summary>
        /// User voids the redeemed deal
        /// </summary>
        ReversedDealNonTimeout,

        /// <summary>
        /// Deal is reversed becuase of timeout
        /// </summary>
        ReversedDealTimeout,

        /// <summary>
        /// New merchant registered
        /// </summary>
        RegisterMerchant
    }
}