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
    public class DealPreviewCardlinkInfoModel
    {
        /// <summary>
        /// Gets or sets discount amount
        /// </summary>
        [JsonProperty(PropertyName = "discount_amount")]
        public float? DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets aiscount percent
        /// </summary>
        [JsonProperty(PropertyName = "discount_percent")]
        public float? DiscountPercent { get; set; }
    }
}