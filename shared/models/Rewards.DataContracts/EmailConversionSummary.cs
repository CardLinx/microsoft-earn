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
    public class EmailConversionSummary
    {
        /// <summary>
        /// The offer id. For hold-out group we use the campaign id as the offer id (hack!).
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "offer_id")]
        [JsonProperty(PropertyName = "offer_id")]
        public string OfferId { get; set; }

        /// <summary>
        /// The number of users to whom the email was sent.
        /// For hold-out group, this number is the size of the hold-out group.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "sent")]
        [JsonProperty(PropertyName = "sent")]
        public int SentCount { get; set; }

        /// <summary>
        /// The number of users who opened the email
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "opens")]
        [JsonProperty(PropertyName = "opens")]
        public int OpenCount { get; set; }

        /// <summary>
        /// The number of users who clicked the link in the email.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "clicks")]
        [JsonProperty(PropertyName = "clicks")]
        public int ClickCount { get; set; }

        /// <summary>
        /// Date when the Sent/Open/Click were computed.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "date")]
        [JsonProperty(PropertyName = "date")]
        public DateTime ComputedOn { get; set; }

        /// <summary>
        /// (Excludes test conversions), organic and inorganic conversions only.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "total_conversions")]
        [JsonProperty(PropertyName = "total_conversions")]
        public int TotalConversionCount { get; set; }
        
        /// <summary>
        /// Count of users who converted organically (i.e. not attributed to the campaign)
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "organic_conversions")]
        [JsonProperty(PropertyName = "organic_conversions")]
        public int OrganicConversionCount { get; set; }

        /// <summary>
        /// Count of users who conversion is attributed to the campaign
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "inorganic_conversions")]
        [JsonProperty(PropertyName = "inorganic_conversions")]
        public int InorganicConversionCount { get; set; }

        /// <summary>
        /// Count of test users who converted
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "test_conversions")]
        [JsonProperty(PropertyName = "test_conversions")]
        public int TestConversionCount { get; set; }

        /// <summary>
        /// Total real rewards assigned (i.e. excludes test users)
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "total_rewards")]
        [JsonProperty(PropertyName = "total_rewards")]
        public int TotalRewardsCount { get; set; }

        /// <summary>
        /// Total test rewards assigned
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "test_rewards")]
        [JsonProperty(PropertyName = "test_rewards")]
        public int TestRewardsCount { get; set; }

        /// <summary>
        /// Total real rewards actually sent out (i.e. excludes test users)
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "rewards_sent")]
        [JsonProperty(PropertyName = "rewards_sent")]
        public int RewardsSentCount { get; set; }

        /// <summary>
        /// Total test rewards actually sent out
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "test_rewards_sent")]
        [JsonProperty(PropertyName = "test_rewards_sent")]
        public int TestRewardsSentCount { get; set; }
    }
}