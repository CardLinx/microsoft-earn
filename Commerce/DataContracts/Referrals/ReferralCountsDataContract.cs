//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a referral counts object.
    /// </summary>
    [DataContract]
    public class ReferralCountsDataContract
    {
        /// <summary>
        /// Gets or sets the ID of the referral event whose count is shown in this object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_event_id")]
        public int ReferralEventId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the reward payout status.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "reward_payout_status_id")]
        public int RewardPayoutStatusId { get; set; }

        /// <summary>
        /// Gets or sets the count of the referral events described in this object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "count")]
        public int Count { get; set; }
    }
}