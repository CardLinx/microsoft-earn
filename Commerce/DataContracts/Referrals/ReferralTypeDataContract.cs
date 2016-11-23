//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a referral type.
    /// </summary>
    [DataContract]
    public class ReferralTypeDataContract
    {
        /// <summary>
        /// Gets or sets the vector for which this referral type is being made.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_vector")]
        public string ReferralVector { get; set; }

        /// <summary>
        /// Gets or sets the reward recipient for this referral type.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "reward_recipient")]
        public string RewardRecipient { get; set; }

        /// <summary>
        /// Gets or sets the events and rewards associated with this referral type.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_event_rewards")]
        public IEnumerable<ReferralEventRewardDataContract> ReferralEventRewards { get; set; }
    }
}