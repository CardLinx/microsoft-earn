//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.DataContracts
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The Merchant billing statement transactions
    /// </summary>
    public class MerchantBillingStatementTransactionContract
    {
        /// <summary>
        /// Gets or sets Date
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets Location
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the Card Brand
        /// </summary>
        [JsonProperty(PropertyName = "card_brand")]
        public string CardBrand { get; set; }

        /// <summary>
        /// Gets or sets the Last 4 digits of the card used in the transaction
        /// </summary>
        [JsonProperty(PropertyName = "card_last_four_digits")]
        public string CardLastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the Offer Description
        /// </summary>
        [JsonProperty(PropertyName = "offer_description")]
        public string OfferShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the original transaction amount.
        /// </summary>
        [JsonProperty(PropertyName = "original_transaction_amount")]
        public double OriginalTransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount provided to consumer.
        /// </summary>
        [JsonProperty(PropertyName = "discount_provided_to_consumer")]
        public double DiscountProvidedToConsumer { get; set; }

        /// <summary>
        /// Gets or sets the discount due from merchant.
        /// </summary>
        [JsonProperty(PropertyName = "discount_due_from_merchant")]
        public double DiscountDueFromMerchant { get; set; }

        /// <summary>
        /// Gets or sets the bing transaction fee.
        /// </summary>
        [JsonProperty(PropertyName = "bing_transaction_fee")]
        public double BingTransactionFee { get; set; }

        /// <summary>
        /// Gets or sets the bing advertising fee.
        /// </summary>
        [JsonProperty(PropertyName = "bing_advertising_fee")]
        public double BingAdvertisingFee { get; set; }

        /// <summary>
        /// Gets or sets the bing advertising fee.
        /// </summary>
        [JsonProperty(PropertyName = "bing_charged_advertising_fee")]
        public double BingChargedAdvertsingFee { get; set; }
    }
}