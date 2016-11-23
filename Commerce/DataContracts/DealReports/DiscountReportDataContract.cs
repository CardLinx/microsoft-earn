//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents information about redemptions of a discount.
    /// </summary>
    [DataContract]
    public class DiscountReportDataContract
    {
        /// <summary>
        /// Gets or sets the ID of the discount described within this report.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_id")]
        public Guid DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the list of redemptions for this discount.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_redemptions")]
        public IEnumerable<DiscountRedemptionDataContract> DiscountRedemptions { get; set; }

        /// <summary>
        /// Gets or sets the currency specified within the discount.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the total number of redemptions for this discount.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "total_redemptions")]
        public int TotalRedemptions { get; set; }

        /// <summary>
        /// Gets or sets the total settled amount of all transactions within the DiscountRedemptions list.
        /// </summary>
        /// <remarks>
        /// * This is the gross amount, i.e. before the discounts are applied.
        /// * Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "total_settlement_amount")]
        public int TotalSettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the total amount of all discounts applied within the DiscountRedemptions list.
        /// </summary>
        /// <remarks>
        /// Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "total_discount")]
        public int TotalDiscount { get; set; }
    }
}