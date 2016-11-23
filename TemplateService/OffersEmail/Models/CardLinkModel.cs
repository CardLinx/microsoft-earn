//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// CardLink email test data
    /// </summary>
    public class CardLinkModel
    {
        /// <summary>
        /// Gets or sets user name
        /// </summary>
        [JsonProperty(PropertyName = "user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets merchant name
        /// </summary>
        [JsonProperty(PropertyName = "merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets deal discount summary
        /// </summary>
        [JsonProperty(PropertyName = "discount_summary")]
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets credit amount
        /// </summary>
        [JsonProperty(PropertyName = "credit_amount")]
        public string CreditAmount { get; set; }

        /// <summary>
        /// Gets or sets last four digits
        /// </summary>
        [JsonProperty(PropertyName = "last_four_digits")]
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the type of card  VISA or MasterCard.
        /// </summary>
        [JsonProperty(PropertyName = "card_type")]
        public string CardType { get; set; }

        /// <summary>
        /// Gets or sets active cards count
        /// </summary>
        [JsonProperty(PropertyName = "active_cards_count")]
        public int ActiveCardsCount { get; set; }

        /// <summary>
        /// Gets or sets unauthenticated user status
        /// </summary>
        [JsonProperty(PropertyName = "unauthenticated_user_status")]
        public string UnauthenticatedUserStatus { get; set; }

        /// <summary>
        /// Gets or sets email verification url
        /// </summary>
        [JsonProperty(PropertyName = "email_verification_url")]
        public string EmailVerificationUrl { get; set; }

        /// <summary>
        /// Gets or sets account linking url
        /// </summary>
        [JsonProperty(PropertyName = "account_linking_url")]
        public string AccountLinkingUrl { get; set; }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets transaction date.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_date")]
        public string TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Partner Merchant Id.
        /// </summary>
        [JsonProperty(PropertyName = "partner_merchant_id")]
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets Partner Id.
        /// </summary>
        [JsonProperty(PropertyName = "partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the deal Id (Parent).
        /// </summary>
        [JsonProperty(PropertyName = "deal_id")]
        public string DealId { get; set; }

        /// <summary>
        /// Gets or sets the discount id (global).
        /// </summary>
        [JsonProperty(PropertyName = "discount_id")]
        public string DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the discount id (global).
        /// </summary>
        [JsonProperty(PropertyName = "percent")]
        public float Percent { get; set; }
    }
}