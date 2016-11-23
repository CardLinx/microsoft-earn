//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Deal Model
    /// </summary>
    public class DealModel
    {
        /// <summary>
        /// Gets or sets deal id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets deal transaction url
        /// </summary>
        [JsonProperty(PropertyName = "transaction_url")]
        public string TransactionUrl { get; set; }

        /// <summary>
        /// Gets or sets deal tile
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets business information
        /// </summary>
        [JsonProperty(PropertyName = "business_name")]
        public string BusinessName { get; set; }

        /// <summary>
        ///  Gets or sets deal attribution
        /// </summary>
        [JsonProperty(PropertyName = "attribution")]
        public string Attribution { get; set; }

        /// <summary>
        /// Gets or sets deal description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets deal website
        /// </summary>
        [JsonProperty(PropertyName = "website")]
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets deal hero image url
        /// </summary>
        [JsonProperty(PropertyName = "hero_image_url")]
        public string HeroImageUrl { get; set; }

        /// <summary>
        /// Gets or sets deal square image url
        /// </summary>
        [JsonProperty(PropertyName = "square_image_url")]
        public string SquareImageUrl { get; set; }

        /// <summary>
        /// Gets or sets deal type
        /// </summary>
        [JsonProperty(PropertyName = "deal_type")]
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets deal price
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }

        /// <summary>
        /// Gets or sets deal original price
        /// </summary>
        [JsonProperty(PropertyName = "original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// Gets or sets deal discount
        /// </summary>
        [JsonProperty(PropertyName = "discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets multi discount cardlink deal information
        /// </summary>
        [JsonProperty(PropertyName = "cardlink_dealinfos")]
        public IEnumerable<CardlinkDealInfo> CardlinkDealInfos { get; set; }
    }
}