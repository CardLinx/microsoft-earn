//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class Offer : ModelBase
    {
        public Offer()
        {
            Type = this.GetType().Name;
        }

        [DataMember(EmitDefaultValue = false, Name = "type")]
        [JsonProperty("type")]
        public sealed override string Type { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        [JsonProperty("id")]
        public override string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "author")]
        [JsonProperty("author")]
        public override string Author { get; set; }

        /// <summary>
        ///     Gets or sets Business.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "business")]
        [JsonProperty("business")]
        public Business Business { get; set; }

        /// <summary>
        ///     Gets or sets offer rank.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "rank")]
        [JsonProperty("rank")]
        public int Rank { get; set; }

        /// <summary>
        ///     Gets or sets Description.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets ImageUrl.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "image_url")]
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        ///     Gets or sets Images.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "images")]
        public List<Image> Images { get; set; }

        /// <summary>
        ///     Gets or sets the keywords.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "keywords")]
        [JsonProperty("keywords")]
        public List<string> Keywords { get; set; }

        /// <summary>
        ///     Gets or sets Title.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets TransactionUrl.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "transaction_url")]
        [JsonProperty("transaction_url")]
        public string TransactionUrl { get; set; }

        /// <summary>
        ///     Gets or sets provider name.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "provider_name")]
        [JsonProperty("provider_name")]
        public string ProviderName { get; set; }

        /// <summary>
        ///     Gets or sets provider id.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "provider_id")]
        [JsonProperty("provider_id")]
        public string ProviderId { get; set; }

        /// <summary>
        ///     Gets or sets the offer discount
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount")]
        [JsonProperty("discount")]
        public float Discount { get; set; }

        /// <summary>
        ///     Gets or sets provider id.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "start_date")]
        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Gets or sets provider id.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "end_date")]
        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Creates shallow copy
        /// </summary>
        /// <returns>The business.</returns>
        public Offer ShallowCopy()
        {
            return (Offer)this.MemberwiseClone();
        }
    }
}