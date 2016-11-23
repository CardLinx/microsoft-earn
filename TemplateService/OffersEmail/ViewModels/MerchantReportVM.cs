//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
// 
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace OffersEmail.ViewModels
{
    using Newtonsoft.Json;

    /// <summary>
    /// Merchant report email View Model
    /// </summary>
    public class MerchantReportVM
    {
        /// <summary>
        ///     Gets or sets the report start date.
        /// </summary>
        [JsonProperty(PropertyName = "from_date")]
        public string FromDate { get; set; }

        /// <summary>
        ///     Gets or sets the report end date.
        /// </summary>
        [JsonProperty(PropertyName = "to_date")]
        public string ToDate { get; set; }

        /// <summary>
        ///    Gets or sets the schedule for receiving merchant email
        /// </summary>
        [JsonProperty(PropertyName = "schedule_type")]
        public string ScheduleType { get; set; }

        /// <summary>
        ///     Gets or sets the URL for the merchant portal.
        /// </summary>
        [JsonProperty(PropertyName = "merchantportal_url")]
        public string MerchantPortalUrl { get; set; }

        /// <summary>
        /// Gets or sets all the transactions for this merchant store in the specified time.
        /// </summary>
        [JsonProperty(PropertyName = "merchant_store_transactions")]
        public MerchantStoreTransactionContractVM[] MerchantStoreTransactions { get; set; }

        /// <summary>
        /// Merchant store transaction View Model
        /// </summary>
        public class MerchantStoreTransactionContractVM
        {
            /// <summary>
            ///  Gets or sets the merchant name.
            /// </summary>
            [JsonProperty(PropertyName = "merchant_name")]
            public string MerchantName { get; set; }

            /// <summary>
            /// Gets or sets the merchant store location.
            /// </summary>
            [JsonProperty(PropertyName = "store_location")]
            public MerchantLocationContractVM StoreLocation { get; set; }

            /// <summary>
            /// Gets or sets the transactions in this store.
            /// </summary>
            [JsonProperty(PropertyName = "transactions")]
            public MerchantTransactionContractVM[] Transactions { get; set; }

            /// <summary>
            /// Contract for Merchant Store Location
            /// </summary>
            public class MerchantLocationContractVM
            {
                /// <summary>
                /// Gets or sets address for the merchant store.
                /// </summary>
                [JsonProperty(PropertyName = "address")]
                public string Address { get; set; }

                /// <summary>
                /// Gets or sets the city of the merchant store.
                /// </summary>
                [JsonProperty(PropertyName = "city")]
                public string City { get; set; }

                /// <summary>
                /// Gets or sets state of the merchant store.
                /// </summary>
                [JsonProperty(PropertyName = "state")]
                public string State { get; set; }

                /// <summary>
                /// Gets or sets postal code for the merchant store.
                /// </summary>
                [JsonProperty(PropertyName = "postal")]
                public string Postal { get; set; }
            }

            /// <summary>
            /// Contract for Merchant Transaction
            /// </summary>
            public class MerchantTransactionContractVM
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
    }
}