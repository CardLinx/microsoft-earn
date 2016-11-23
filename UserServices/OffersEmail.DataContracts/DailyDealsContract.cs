//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    ///     Daily Deals Contract
    /// </summary>
    public class DailyDealsContract
    {
        /// <summary>
        /// Gets or sets the deals
        /// </summary>
        [JsonProperty(PropertyName = "deals")]
        public DealContract[] Deals { get; set; }

        /// <summary>
        /// Gets or sets the unsubscribe url
        /// </summary>
        [JsonProperty(PropertyName = "unsubscribeUrl")]
        public string UnsubscribeUrl { get; set; }

        /// <summary>
        /// Gets or sets the location where deals are selected
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }
    }
}