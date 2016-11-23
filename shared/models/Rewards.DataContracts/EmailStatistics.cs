//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Rewards.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    [DataContract]
    public class EmailStatistics
    {
        [DataMember(EmitDefaultValue = false, Name = "offer_id")]
        [JsonProperty(PropertyName = "offer_id")]
        public string OfferId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "sent")]
        [JsonProperty(PropertyName = "sent")]
        public int SentCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "opens")]
        [JsonProperty(PropertyName = "opens")]
        public int OpenCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "clicks")]
        [JsonProperty(PropertyName = "clicks")]
        public int ClickCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "date")]
        [JsonProperty(PropertyName = "date")]
        public DateTime ComputedOn { get; set; }
    }
}