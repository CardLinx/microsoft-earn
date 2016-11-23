//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   Analytics actions for commerce service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Actions
{
    /// <summary>
    /// Analytics actions for commerce service
    /// </summary>
    public static partial class Actions
    {
        /// <summary>
        /// User has been registered
        /// </summary>
        public const string RegisterUser = "Commerce.RegisterUser";

        /// <summary>
        /// User has redeemed a deal
        /// </summary>
        public const string RedeemedDeal = "RedeemedDeal";

        /// <summary>
        /// User's redeemed deal has settled.
        /// </summary>
        public const string Settlement = "Commerce.Settlement";

        /// <summary>
        /// New card has been registered
        /// </summary>
        public const string AddCard = "AddCard";

        /// <summary>
        /// A new deal has been registered
        /// </summary>
        public const string RegisterDeal = "RegisterDeal";

        /// <summary>
        /// Deal has been claimed
        /// </summary>
        public const string ClaimedDeal = "ClaimedDeal";

        /// <summary>
        /// User voids the redeemed deal
        /// </summary>
        public const string ReversedDealNonTimeout = "ReversedDealNonTimeout";

        /// <summary>
        /// Deal is reversed becuase of timeout
        /// </summary>
        public const string ReversedDealTimeout = "ReversedDealTimeout";

        /// <summary>
        /// New merchant registered
        /// </summary>
        public const string RegisterMerchant = "RegisterMerchant";
    }
}