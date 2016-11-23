//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Rewards.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    [DataContract]
    public class ConversionSummary
    {
        [DataMember(EmitDefaultValue = false, Name = "offer_id")]
        [JsonProperty(PropertyName = "offer_id")]
        public string OfferId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "total_conversions")]
        [JsonProperty(PropertyName = "total_conversions")]
        public int TotalConversionCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "organic_conversions")]
        [JsonProperty(PropertyName = "organic_conversions")]
        public int OrganicConversionCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "inorganic_conversions")]
        [JsonProperty(PropertyName = "inorganic_conversions")]
        public int InorganicConversionCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "test_conversions")]
        [JsonProperty(PropertyName = "test_conversions")]
        public int TestConversionCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "total_rewards")]
        [JsonProperty(PropertyName = "total_rewards")]
        public int TotalRewardsCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "test_rewards")]
        [JsonProperty(PropertyName = "test_rewards")]
        public int TestRewardsCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "rewards_sent")]
        [JsonProperty(PropertyName = "rewards_sent")]
        public int RewardsSentCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "test_rewards_sent")]
        [JsonProperty(PropertyName = "test_rewards_sent")]
        public int TestRewardsSentCount { get; set; }
    }
}