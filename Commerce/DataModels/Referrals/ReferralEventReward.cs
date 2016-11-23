//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents an association between an event and a reward for a referral.
    /// </summary>
    public class ReferralEventReward
    {
        /// <summary>
        /// Initializes a new instance of the ReferralEventReward class.
        /// </summary>
        public ReferralEventReward()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the ReferralEventRewards class, using the fields from the specified other
        /// ReferralEventRewards.
        /// </summary>
        /// <param name="referralEventRewards">
        /// The other ReferralEventRewards whose fields to copy.
        /// </param>
        internal ReferralEventReward(ReferralEventReward referralEventRewards)
        {
            Id = referralEventRewards.Id;
            ReferralEvent = referralEventRewards.ReferralEvent;
            RewardId = referralEventRewards.RewardId;
            PerUserLimit = referralEventRewards.PerUserLimit;
        }

        /// <summary>
        /// Determines whether the specified object has equal values to this object in all fields.
        /// </summary>
        /// <param name="obj">
        /// The object whose values to compare.
        /// </param>
        /// <returns>
        /// True if the two objects have the same values.
        /// </returns>
        public override bool Equals(object obj)
        {
            ReferralEventReward referralEventRewards = (ReferralEventReward)obj;
            return Id == referralEventRewards.Id &&
                   ReferralEvent == referralEventRewards.ReferralEvent &&
                   RewardId == referralEventRewards.RewardId &&
                   PerUserLimit == referralEventRewards.PerUserLimit;
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns>
        /// The hash code for this object.
        /// </returns>
        /// <remarks>
        /// * CA2218:
        ///   * If two objects are equal in value based on the Equals override, they must both return the same value for calls
        ///     to GetHashCode.
        ///   * GetHashCode must be overridden whenever Equals is overridden.
        /// * It is fine if the value overflows.
        /// </remarks>
        public override int GetHashCode()
        {
            return Id.GetHashCode() +
                   ReferralEvent.GetHashCode() +
                   RewardId.GetHashCode() +
                   PerUserLimit.GetHashCode();
        }

        /// <summary>
        /// Gets or sets the ID for this ReferralEventReward.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the ReferralEvent to associate with a reward.
        /// </summary>
        public ReferralEvent ReferralEvent { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Reward to associate with a referral event.
        /// </summary>
        public Guid RewardId { get; set; }

        /// <summary>
        /// Gets or sets the number of referrals per user that will result in a payout to the reward recipient.
        /// </summary>
        public int PerUserLimit { get; set; }
    }
}