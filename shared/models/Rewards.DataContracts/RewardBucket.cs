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
    public class RewardBucket
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

        [DataMember(EmitDefaultValue = false, Name = "codes")]
        [JsonProperty(PropertyName = "codes")]
        public string[] Codes { get; set; }
    }
}