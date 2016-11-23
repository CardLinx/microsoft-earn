//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Defines the overall activity of all users in Bing Offers for a given period
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the overall activity of all users in Bing Offers for a given period
    /// </summary>
    public class AggregatedActivityContract
    {
        /// <summary>
        /// Gets or sets the total amount spent in Bing offers in a given period
        /// </summary>
        [JsonProperty(PropertyName = "total_spend")]
        public string TotalSpend { get; set; }

        /// <summary>
        /// Gets or sets the total amount saved in Bing offers in a given period
        /// </summary>
        [JsonProperty(PropertyName = "total_saved")]
        public string TotalSaved { get; set; }

        /// <summary>
        /// Gets or sets the number of redemptions in Bing offers in a given period
        /// </summary>
        [JsonProperty(PropertyName = "total_redemptions")]
        public int TotalRedemptions { get; set; }
    }
}