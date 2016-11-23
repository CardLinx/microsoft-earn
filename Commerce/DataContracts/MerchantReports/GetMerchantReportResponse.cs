//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a Get Merchant Report API invocation.
    /// </summary>
    [DataContract]
    public class GetMerchantReportResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the number of redemptions reported for this merchant.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "number_of_redemptions")]
        public int NumberOfRedemptions { get; set; }

        /// <summary>
        /// Gets or sets the total settled amount of all transactions within for this merchant.
        /// </summary>
        /// <remarks>
        /// * This is the gross amount, i.e. before the discounts are applied.
        /// * Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "total_settlement_amount")]
        public int TotalSettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the total amount of all discounts applied for this merchant.
        /// </summary>
        /// <remarks>
        /// Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "total_discount_amount")]
        public int TotalDiscountAmount { get; set; }
        
        /// <summary>
        /// Gets or sets the DealRedemptions object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_redemptions")]
        public IEnumerable<DealRedemptions> DealRedemptions { get; set; }

        /// <summary>
        /// Gets or sets the PartnerMerchantDealRedemptions object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "partner_merchant_deal_redemptions")]
        public IEnumerable<PartnerMerchantDealRedemptions> PartnerMerchantDealRedemptions { get; set; }
    }
}