//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class Merchant : ModelBase
    {
        public Merchant()
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

        [DataMember(EmitDefaultValue = false, Name = "provider_id")]
        [JsonProperty("provider_id")]
        public string ProviderId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "name")]    
        [JsonProperty("name")]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "url")]
        [JsonProperty("url")]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "location")]
        [JsonProperty("location")]
        public Location Location { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "phone_number")]
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "images")]
        [JsonProperty("images")]
        public IList<Image> Images { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "payments")]
        [JsonProperty("payments")]
        public IList<Payment> Payments { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "extended_attributes")]
        [JsonProperty("extended_attributes")]
        public IDictionary<string, string> ExtendedAttributes { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "is_active")]
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        /// <summary>
        ///     Gets or sets merchant rank.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "rank")]
        [JsonProperty("rank")]
        public int Rank { get; set; }


        [DataMember(EmitDefaultValue = false, Name = "partner_merchant_id")]
        [JsonProperty("partner_merchant_id")]
        public string PartnerMerchantId { get; set; }

        public bool IsAddressAvailable()
        {
            return (!string.IsNullOrWhiteSpace(Location?.Zip)) || 
                (!string.IsNullOrWhiteSpace(Location?.Address) && !string.IsNullOrWhiteSpace(Location?.City) && !string.IsNullOrWhiteSpace(Location?.State));
        }

        public bool IsLocationGeocoded()
        {
            return Location?.Latitude != 0 && Location?.Longitude != 0;
        }

        public override string ToString()
        {
            return $"{Name};{Location?.ToString()}";
        }
        
    }
}