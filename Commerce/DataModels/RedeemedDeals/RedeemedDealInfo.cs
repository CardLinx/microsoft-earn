//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents information about a redeemed deal.
    /// </summary>
    public class RedeemedDealInfo
    {
        /// <summary>
        /// Gets or sets the canonical ID for this deal.
        /// </summary>
        public Guid GlobalId { get; set; }

        /// <summary>
        /// Gets or sets the currency used within the deal, e.g. USD.
        /// </summary>
        public string Currency { get; set; }

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
        /// Gets or sets the minimum purchase amount to cause the redemption to occur.
        /// </summary>
        /// <remarks>
        /// This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        public int MinimumPurchase { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who redeemed this deal.
        /// </summary>
        public Guid GlobalUserId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the user.
        /// </summary>
        public string PartnerUserId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the selected deal.
        /// </summary>
        public string PartnerDealId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the selected claimed deal.
        /// </summary>
        public string PartnerClaimedDealId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the redeemed deal.
        /// </summary>
        public string PartnerRedeemedDealId { get; set; }

        /// <summary>
        /// Gets or sets the summary description of the deal.
        /// </summary>
        /// <remarks>
        /// This string may appear on the user's receipt, credit card statement, or both.
        /// </remarks>
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the name of the merchant offering the deal.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the card number.
        /// </summary>
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount applied to the transaction.
        /// </summary>
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the text representation of the discount from this deal.
        /// </summary>
        public string DiscountText { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent deal to which this deal belongs.
        /// </summary>
        public Guid ParentDealId { get; set; }

        /// <summary>
        /// Gets or sets the Transaction Id.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the transaction date.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the Partner Merchant Id.
        /// </summary>
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets the Partner Id.
        /// </summary>
        public int PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the Reimbursement Tender.
        /// </summary>
        public int ReimbursementTenderId { get; set; }
    }
}