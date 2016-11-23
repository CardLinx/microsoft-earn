//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// //   Contract for Merchant Billing Statement (pdf file)
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Contract for Merchant Billing Statement (pdf file)
    /// </summary>
    public class MerchantBillingStatementContract
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [JsonProperty(PropertyName = "start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [JsonProperty(PropertyName = "end_date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        [JsonProperty(PropertyName = "currency_symbol")]
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the name of the business.
        /// </summary>
        [JsonProperty(PropertyName = "business_name")]
        public string BusinessName { get; set; }

        /// <summary>
        /// Gets or sets the locations.
        /// </summary>
        [JsonProperty(PropertyName = "locations")]
        public List<string> Locations { get; set; }

        /// <summary>
        /// Gets or sets the active offers.
        /// </summary>
        [JsonProperty(PropertyName = "active_offers")]
        public List<string> ActiveOffers { get; set; }

        /// <summary>
        /// Gets or sets the transactions.
        /// </summary>
        [JsonProperty(PropertyName = "transactions")]
        public List<MerchantBillingStatementTransactionContract> Transactions { get; set; }
    }
}