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
    /// Represents a deal.
    /// </summary>
    [DataContract]
    public class DealDataContract
    {
        /// <summary>
        /// Gets or sets the canonical ID for this deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant offering the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "merchant_id")]
        public Guid MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the date at which the deal offer begins.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date at which the deal offer ends.
        /// </summary>
        /// <remarks>
        /// * Default value is DateTime.MaxValue. Because this must be a pure data contract, OnDeserializing cannot be
        /// implemented to provide a default value when none is specified. Because of this, DateTime.MinValue will be replaced by
        /// DateTime.MaxValue deeper within the system.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "end_date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the currency used within the deal, e.g. USD.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the means through which deal reimbursement will be tendered, e.g. Currency or CSV.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "reimbursement_tender")]
        public string ReimbursementTender { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount offered within the deal.
        /// </summary>
        /// <remarks>
        /// * This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// * Setting both Amount and Percent is permitted, but different partners may resolve the conflict differently.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the percent of the discount offered within the deal.
        /// </summary>
        /// <remarks>
        /// Setting both Amount and Percent is permitted, but different partners may resolve the conflict differently.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "percent")]
        public decimal Percent { get; set; }

        /// <summary>
        /// Gets or sets the minimum purchase amount to cause the redemption to occur.
        /// </summary>
        /// <remarks>
        /// This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "minimum_purchase")]
        public int MinimumPurchase { get; set; }

        /// <summary>
        /// Gets or sets the total number of times a deal may be redeemed before becoming inactive.
        /// </summary>
        /// <remarks>
        /// Setting this value to 0 indicates no limit to the number of times the deal may be redeemed.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the limit to the number of times a user may redeem the deal.
        /// </summary>
        /// <remarks>
        /// Setting this value to 0 indicates no limit to the number of times a user may redeem a deal.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "user_limit")]
        public int UserLimit { get; set; }

        /// <summary>
        /// Gets or sets the summary description of the deal.
        /// </summary>
        /// <remarks>
        /// This string may appear on the user's receipt, credit card statement, or both.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "discount_summary")]
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the maximum discount that will be granted for this deal.
        /// </summary>
        /// <remarks>
        /// * Setting this value to 0 indicates that no limit to the discount exists.
        /// * This field only has effect when Percent > 0. Otherwise, MaximumDiscount is set to Amount during ingestion.
        /// * This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "maximum_discount")]
        public int MaximumDiscount { get; set; }
    }
}