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
    public class RewardType
    {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "name")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "days_till_apply_by_date")]
        [JsonProperty(PropertyName = "days_till_apply_by_date")]
        public int MinimumDaysTillApplyByDate { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "denomination")]
        [JsonProperty(PropertyName = "denomination")]
        public string Denomination { get; set; }
    }
}