//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a deal and its redemption information.
    /// </summary>
    [DataContract]
    public class DealRedemptions
    {
        /// <summary>
        /// Gets or sets the ID of the deal whose redemption information is reported in this object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_id")]
        public Guid DealId { get; set; }

        /// <summary>
        /// Gets or sets the number of redemptions reported for this deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "number_of_redemptions")]
        public int NumberOfRedemptions { get; set; }

        /// <summary>
        /// Gets or sets the total settled amount of all transactions within for this deal.
        /// </summary>
        /// <remarks>
        /// * This is the gross amount, i.e. before the discounts are applied.
        /// * Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "total_settlement_amount")]
        public int TotalSettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the total amount of all discounts applied for this deal.
        /// </summary>
        /// <remarks>
        /// Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "total_discount_amount")]
        public int TotalDiscountAmount { get; set; }
    }
}