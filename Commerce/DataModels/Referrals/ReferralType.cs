//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Represents a referral type.
    /// </summary>
    public class ReferralType
    {
        /// <summary>
        /// Initializes a new instance of the ReferralType class for serialization.
        /// </summary>
        public ReferralType()
        {
            Id = Guid.NewGuid();
            Code = String.Empty;
            ReferralEventRewards = new Collection<ReferralEventReward>();
        }

        /// <summary>
        /// Initializes a new instance of the ReferralType class, using the fields from the specified other ReferralType.
        /// </summary>
        /// <param name="referralType">
        /// The other ReferralType whose fields to copy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter deal cannot be null.
        /// </exception>
        public ReferralType(ReferralType referralType)
        {
            if (referralType == null)
            {
                throw new ArgumentNullException("referralType", "Parameter referralType cannot be null.");
            }

            Id = referralType.Id;
            ReferrerId = referralType.ReferrerId;
            ReferrerType = referralType.ReferrerType;
            ReferralVector = referralType.ReferralVector;
            RewardRecipient = referralType.RewardRecipient;
            Code = referralType.Code;
            ReferralEventRewards = new Collection<ReferralEventReward>();
            foreach (ReferralEventReward referralEventReward in referralType.ReferralEventRewards)
            {
                ReferralEventRewards.Add(new ReferralEventReward(referralEventReward));
            }
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
            ReferralType referralType = (ReferralType)obj;
            return Id == referralType.Id &&
                   ReferrerId == referralType.ReferrerId &&
                   ReferrerType == referralType.ReferrerType &&
                   ReferralVector == referralType.ReferralVector &&
                   RewardRecipient == referralType.RewardRecipient &&
                   Code == referralType.Code &&
                   ReferralEventRewards.Except(referralType.ReferralEventRewards).Any() == false &&
                   referralType.ReferralEventRewards.Except(ReferralEventRewards).Any() == false;
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
            int result = Id.GetHashCode() +
                         ReferrerId.GetHashCode() +
                         ReferrerType.GetHashCode() +
                         ReferralVector.GetHashCode() +
                         RewardRecipient.GetHashCode() +
                         Code.GetHashCode();
            
            foreach (ReferralEventReward referralEventReward in ReferralEventRewards)
            {
                result += referralEventReward.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the ID for this ReferralType
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity making this referral type.
        /// </summary>
        public Guid ReferrerId { get; set; }

        /// <summary>
        /// Gets or sets the type of entity making this referral type.
        /// </summary>
        public ReferrerType ReferrerType { get; set; }

        /// <summary>
        /// Gets or sets the vector for which this referral type is being made.
        /// </summary>
        public ReferralVector ReferralVector { get; set; }

        /// <summary>
        /// Gets or sets the reward recipient for this referral type.
        /// </summary>
        public RewardRecipient RewardRecipient { get; set; }

        /// <summary>
        /// Gets or sets the code that represents this referral type outside the system.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the events and rewards associated with this referral type.
        /// </summary>
        public Collection<ReferralEventReward> ReferralEventRewards { get; private set; }
    }
}