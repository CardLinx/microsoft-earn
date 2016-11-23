//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// //   Contract for Merchant Reports
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Contract for Merchant Reports
    /// </summary>
    public class MerchantReportContract
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
        public MerchantStoreTransactionContract[] MerchantStoreTransactions { get; set; }
    }
}