//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Rewards.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [DataContract]
    public class RewardDefinition
    {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "name")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "count")]
        [JsonProperty(PropertyName = "count")]
        public int CodeCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "type_id")]
        [JsonProperty(PropertyName = "type_id")]
        public int RewardTypeId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "value")]
        [JsonProperty(PropertyName = "value")]
        public decimal CodeValue { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "pid")]
        [JsonProperty(PropertyName = "pid")]
        public string Pid { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "email_fields")]
        [JsonProperty(PropertyName = "email_fields")]
        public string RewardEmailFields { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "created_by")]
        [JsonProperty(PropertyName = "created_by")]
        public string CreatedBy { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "bucket_ids")]
        [JsonProperty(PropertyName = "bucket_ids")]
        public int[] RewardBucketIds { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "offer_ids")]
        [JsonProperty(PropertyName = "offer_ids")]
        public string[] OfferIds { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "conv_types")]
        [JsonProperty(PropertyName = "conv_types")]
        public string[] ConversionTypes { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "max_one_reward")]
        [JsonProperty(PropertyName = "max_one_reward")]
        public bool MaxOneRewardPerUser { get; set; }
    }
}