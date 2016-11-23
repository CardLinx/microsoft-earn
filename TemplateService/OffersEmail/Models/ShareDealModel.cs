//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using Newtonsoft.Json;

    /// <summary>
    ///  Email share deal contract
    /// </summary>
    public class ShareDealModel
    {
        /// <summary>
        /// Gets or sets id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Transaction Url
        /// </summary>
        [JsonProperty(PropertyName = "deal_url")]
        public string DealUrl { get; set; }

        /// <summary>
        /// Gets or sets Deal Title
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Deal Brand
        /// </summary>
        [JsonProperty(PropertyName = "attribution")]
        public string Attribution { get; set; }

        /// <summary>
        /// Gets or sets Deal Store PropertyName
        /// </summary>
        [JsonProperty(PropertyName = "business_name")]
        public string BusinessName { get; set; }

        /// <summary>
        /// Gets or sets Deal Large Image Url
        /// </summary>
        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets discount
        /// </summary>
        [JsonProperty(PropertyName = "discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the minimum spend.
        /// </summary>
        [JsonProperty(PropertyName = "minimum_spend")]
        public string MinimumSpend { get; set; }
    }
}