//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.ViewModels
{
    /// <summary>
    /// Subscription confirmation email view model.
    /// </summary>
    public class CardLinkVM
    {
        /// <summary>
        /// Gets or sets the confirmation URL.
        /// </summary>
        /// <value>
        /// The confirmation URL.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the merchant.
        /// </summary>
        /// <value>
        /// The name of the merchant.
        /// </value>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the discount summary.
        /// </summary>
        /// <value>
        /// The discount summary.
        /// </value>
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the credit amount.
        /// </summary>
        /// <value>
        /// The credit amount.
        /// </value>
        public string CreditAmount { get; set; }

        /// <summary>
        /// Gets or sets the last four digits.
        /// </summary>
        /// <value>
        /// The last four digits.
        /// </value>
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the active cards count.
        /// </summary>
        public int ActiveCardsCount { get; set; }

        /// <summary>
        /// Gets or sets the email confirmation url.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }

        /// <summary>
        /// Gets or sets the account linking url.
        /// </summary>
        public string AccountLinkingUrl { get; set; }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets transaction date.
        /// </summary>
        public string TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Partner Merchant Id.
        /// </summary>
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets Partner Id.
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the deal Id (Parent).
        /// </summary>
        public string DealId { get; set; }

        /// <summary>
        /// Gets or sets the discount id (global).
        /// </summary>
        public string DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the feedback url which allows customer to provider feedback to merchant and merchant to response back
        /// </summary>
        public string FeedbackUrl { get; set; }

        /// <summary>
        /// Gets or sets the Percent.
        /// </summary>
        public float Percent { get; set; }
    }
}