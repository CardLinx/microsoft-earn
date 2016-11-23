//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using OfferManagement.DataModel.Enums;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class Provider : ModelBase
    {
        public Provider()
        {
            Type = this.GetType().Name;
            IsNational = true;
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

        [DataMember(EmitDefaultValue = false, Name = "name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "provider_type")]
        [JsonProperty("provider_type")]
        public ProviderType ProviderType { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "feed_type")]
        [JsonProperty("feed_type")]
        public FeedType FeedType { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "total_merchants")]
        [JsonProperty("total_merchants")]
        public int TotalMerchants { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "active_merchants")]
        [JsonProperty("active_merchants")]
        public int ActiveMerchants { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "offer_id")]
        [JsonProperty("offer_id")]
        public string OfferId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "is_active")]
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [DataMember(EmitDefaultValue = true, Name = "is_national")]
        [JsonProperty("is_national")]
        public bool IsNational { get; set; }


        [DataMember(EmitDefaultValue = false, Name = "merchant_source")]
        [JsonProperty("merchant_source")]
        public MerchantSource MerchantSource { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "extended_attributes")]
        [JsonProperty("extended_attributes")]
        public IDictionary<string, string> ExtendedAttributes { get; set; }
    }
}