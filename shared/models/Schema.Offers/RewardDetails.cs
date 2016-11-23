//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>
    /// Specifies the reward details for an offer.
    /// </summary>
    [DataContract]
    public class RewardDetails
    {
        /// <summary>
        /// Specifies the list of buckets from which the rewards to be allocated for this offer.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "reward_bucket_ids")]
        [ProtoMember(1)]
        public List<int> RewardBucketIds { get; set; }

        /// <summary>
        /// Specifies the number of codes per reward for this offer.
        /// It is usually one, but for some rewards like Go-go codes,
        /// we give out 4 codes per reward.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "code_count")]
        [ProtoMember(2)]
        public int CodeCount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "pid")]
        [ProtoMember(3)]
        public string Pid { get; set; }
    }
}