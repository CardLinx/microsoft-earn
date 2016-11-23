//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The redemptions statistics contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Analytics.API.Contract
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The redemptions statistics contract.
    /// </summary>
    public class StatisticsContract
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the discount amount sum.
        /// </summary>
        [DataMember(Name = "redemptions_discount_amount_sum")]
        public int RedemptionsDiscountAmountSum { get; set; }

        /// <summary>
        /// Gets or sets the discount amount sum.
        /// </summary>
        [DataMember(Name = "redemptions_amount_sum")]
        public int RedemptionsAmountSum { get; set; }

        /// <summary>
        /// Gets or sets the redemptions count.
        /// </summary>
        [DataMember(Name = "redemptions_count")]
        public int RedemptionsCount { get; set; }

        #endregion
    }
}