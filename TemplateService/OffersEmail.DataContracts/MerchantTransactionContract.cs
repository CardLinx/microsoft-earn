//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// //   Contract for Merchant Transaction
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Contract for Merchant Transaction
    /// </summary>
    public class MerchantTransactionContract
    {
        /// <summary>
        /// Gets or sets Deal Title
        /// </summary>
        [JsonProperty(PropertyName = "deal_title")]
        public string DealTitle { get; set; }

        /// <summary>
        /// Gets or sets the time of redemption
        /// </summary>
        [JsonProperty(PropertyName = "redemption_time")]
        public string RedemptionTime { get; set; }

        /// <summary>
        /// Gets or sets the date of settlement
        /// </summary>
        [JsonProperty(PropertyName = "settlement_date")]
        public string SettlementDate { get; set; }

        /// <summary>
        /// Gets or sets the settlement amount
        /// </summary>
        [JsonProperty(PropertyName = "settlement_amount")]
        public string SettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount amount
        /// </summary>
        [JsonProperty(PropertyName = "discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the Last 4 digits of the card used in the transaction
        /// </summary>
        [JsonProperty(PropertyName = "card_last_four_digits")]
        public string CardLastFourDigits { get; set; }
    }
}