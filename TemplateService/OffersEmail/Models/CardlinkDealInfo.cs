//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Cardlink Deal Info
    /// </summary>
    public class CardlinkDealInfo
    {
        /// <summary>
        /// Gets or sets discount
        /// </summary>
        [JsonProperty(PropertyName = "discount")]
        ////public string Discount { get; set; }
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets discount amount
        /// </summary>
        [JsonProperty(PropertyName = "discount_amount")]
        ////public string DiscountAmount { get; set; }
        public string DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets minimum purchase
        /// </summary>
        [JsonProperty(PropertyName = "minimum_spend")]
        ////public string MinimumSpend { get; set; }
        public string MinimumSpend { get; set; }
    }
}