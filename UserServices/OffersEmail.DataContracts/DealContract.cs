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
    /// Identifies the type of the deal
    /// </summary>
    public enum DealType
    {
        /// <summary>
        /// Prepaid deal
        /// </summary>
        Prepaid,

        /// <summary>
        /// CardLink offer
        /// </summary>
        CardLinked
    }

    /// <summary>
    ///  Deal contract
    /// </summary>
    public class DealContract
    {
        /// <summary>
        /// Gets or sets id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Transaction Url
        /// </summary>
        [JsonProperty(PropertyName = "transaction_url")]
        public string TransactionUrl { get; set; }

        /// <summary>
        /// Gets or sets the type of deal
        /// </summary>
        [JsonProperty(PropertyName = "deal_type")]
        public DealType DealType { get; set; }

        /// <summary>
        /// Gets or sets Deal Price
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }

        /// <summary>
        /// Gets or sets Deal Original Price
        /// </summary>
        [JsonProperty(PropertyName = "original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// Gets or sets Deal Discount Percentage
        /// </summary>
        [JsonProperty(PropertyName = "discount")]
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the total redemptions for this deal...applies only to CLO
        /// </summary>
        [JsonProperty(PropertyName = "redemptions_count")]
        public int RedemptionsCount { get; set; }

        /// <summary>
        /// Gets or sets the CardLink Offer Info
        /// </summary>
        [JsonProperty(PropertyName = "cardlink_dealinfos")]
        public CardLinkInfo[] CardLinkInfos { get; set; }

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
        /// Gets or sets Deal Description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Deal Url
        /// </summary>
        [JsonProperty(PropertyName = "website")]
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets Deal Large Image Url
        /// </summary>
        [JsonProperty(PropertyName = "hero_image_url")]
        public string LargeImageUrl { get; set; }

        /// <summary>
        /// Gets or sets Deal Medium Image Url
        /// </summary>
        [JsonProperty(PropertyName = "square_image_url")]
        public string MediumImageUrl { get; set; }
    }
}