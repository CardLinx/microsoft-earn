//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   Represents a Card Link Offer discount information
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a card link offer discount information
    /// </summary>
    public class CardLinkInfo
    {
        /// <summary>
        /// Gets or sets Deal Discount Percentage
        /// </summary>
        [JsonProperty(PropertyName = "discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the Discount amount on the deal
        /// </summary>
        [JsonProperty(PropertyName = "discount_amount")]
        public string DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the Minimum amount to spend before getting a discount
        /// </summary>
        [JsonProperty(PropertyName = "minimum_spend")]
        public string MinimumSpend { get; set; }
    }
}