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
    /// Represents information about redemptions of a deal.
    /// </summary>
    [DataContract]
    public class DealReportDataContract
    {
        /// <summary>
        /// Gets or sets the ID of the deal described within this report.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_id")]
        public Guid DealId { get; set; }

        /// <summary>
        /// Gets or sets the list of discount reports for this deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_reports")]
        public IEnumerable<DiscountReportDataContract> DiscountReports { get; set; }

        /// <summary>
        /// Gets or sets the total number of redemptions for this deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "total_redemptions")]
        public int TotalRedemptions { get; set; }

        /// <summary>
        /// Gets or sets the total settled amount of all transactions for all discounts within the deal.
        /// </summary>
        /// <remarks>
        /// * This is the gross amount, i.e. before the discounts are applied.
        /// * Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "total_settlement_amount")]
        public int TotalSettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the total amount of all applied discounts for all discounts within the deal.
        /// </summary>
        /// <remarks>
        /// Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "total_discount")]
        public int TotalDiscount { get; set; }
    }
}