//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Rewards.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [DataContract]
    public class RewardBucketMetadata
    {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "name")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "type_id")]
        [JsonProperty(PropertyName = "type_id")]
        public int RewardTypeId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "value")]
        [JsonProperty(PropertyName = "value")]
        public decimal CodeValue { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "apply_by")]
        [JsonProperty(PropertyName = "apply_by")]
        public DateTime? ApplyByDateUtc { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "use_by")]
        [JsonProperty(PropertyName = "use_by")]
        public DateTime? UseByDateUtc { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "created_by")]
        [JsonProperty(PropertyName = "created_by")]
        public string CreatedBy { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "created_date")]
        [JsonProperty(PropertyName = "created_date")]
        public DateTime CreatedDateUtc { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "last_modified_date")]
        [JsonProperty(PropertyName = "last_modified_date")]
        public DateTime LastModifiedDateUtc { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "is_disabled")]
        [JsonProperty(PropertyName = "is_disabled")]
        public bool IsDisabled { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "codes_total")]
        [JsonProperty(PropertyName = "codes_total")]
        public int TotalCodeCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "codes_used")]
        [JsonProperty(PropertyName = "codes_used")]
        public int UsedCodeCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "codes_unused")]
        [JsonProperty(PropertyName = "codes_unused")]
        public int UnusedCodeCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "codes_returned")]
        [JsonProperty(PropertyName = "codes_returned")]
        public int ReturnedCodeCount { get; set; }
    }
}