//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// Entity that represents a request to 
    /// enroll cards in rewards programs.
    /// </summary>
    [DataContract]
    public class RewardsProgramCardEnrollment
    {
        /// <summary>
        /// The global user id.
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        [DataMember(Name = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// The card brands to be enrolled in the rewards
        /// program.
        /// </summary>
        [JsonProperty(PropertyName = "card_brands")]
        [DataMember(Name = "card_brands")]
        public string[] CardBrands { get; set; }

        /// <summary>
        /// The reward programs in which the cards 
        /// will be enrolled in.
        /// </summary>
        [JsonProperty(PropertyName = "reward_programs")]
        [DataMember(Name = "reward_programs")]
        public string[] RewardPrograms { get; set; }
    }
}