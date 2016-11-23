//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a referral event reward.
    /// </summary>
    [DataContract]
    public class ReferralEventRewardDataContract
    {
        /// <summary>
        /// Gets or sets the ReferralEvent to associate with a reward.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_event")]
        public string ReferralEvent { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Reward to associate with a referral event.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "reward_id")]
        public Guid RewardId { get; set; }

        /// <summary>
        /// Gets or sets the limit to the number of events for each user that will result in a reward.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "per_user_limit")]
        public int PerUserLimit { get; set; }
    }
}