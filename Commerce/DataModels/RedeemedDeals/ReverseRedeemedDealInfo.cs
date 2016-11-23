//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents information about a reversed redeemed deal.
    /// </summary>
    public class ReverseRedeemedDealInfo
    {
        /// <summary>
        /// Gets or sets the amount of the discount offered within the deal.
        /// </summary>
        /// <remarks>
        /// * This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// * Setting both Amount and Percent is permitted, but different partners may resolve the conflict differently.
        /// </remarks>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the percent of the discount offered within the deal.
        /// </summary>
        /// <remarks>
        /// Setting both Amount and Percent is permitted, but different partners may resolve the conflict differently.
        /// </remarks>
        public decimal Percent { get; set; }

        /// <summary>
        /// Gets or sets the amount of the authorized sale being reversed.
        /// </summary>
        /// <remarks>
        /// * This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        public int AuthorizationAmount { get; set; }

        /// <summary>
        /// Gets or sets the partner redeemed deal ID of the matched deal redemption.
        /// </summary>
        public string PartnerRedeemedDealId { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount that had been applied to the transaction.
        /// </summary>
        public int DiscountAmount { get; set; }
    }
}