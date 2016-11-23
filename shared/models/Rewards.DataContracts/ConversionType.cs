//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Rewards.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    [DataContract]
    public class ConversionType
    {
        [DataMember(EmitDefaultValue = false, Name = "type")]
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "description")]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "organic")]
        [JsonProperty(PropertyName = "organic")]
        public bool IsOrganic { get; set; }
    }
}