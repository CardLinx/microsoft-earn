//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The conversion payload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Payloads
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The conversion payload.
    /// </summary>
    public class ConversionPayload : PayloadBase
    {
        /// <summary>
        /// Gets or sets the transaction type.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_type")]
        public string TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the net revenue.
        /// </summary>
        [JsonProperty(PropertyName = "net_revenue")]
        public double NetRevenue { get; set; }

        /// <summary>
        /// Gets or sets the microsoft revenue.
        /// </summary>
         [JsonProperty(PropertyName = "msft_revenue")]
        public double MsftRevenue { get; set; }

        /// <summary>
        /// Gets or sets the transaction date.
        /// </summary>
         [JsonProperty(PropertyName = "transaction_date")]
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the market.
        /// </summary>
        [JsonProperty(PropertyName = "market")]
        public string Market { get; set; }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        [JsonProperty(PropertyName = "info")]
        public string Info { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        [JsonProperty(PropertyName = "provider_name")]
        public string ProviderName { get; set; }

    }
}