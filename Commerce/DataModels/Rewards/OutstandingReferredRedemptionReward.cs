//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Xml.XPath;

    /// <summary>
    /// Represents information about an outstanding referred redemption reward.
    /// </summary>
    public class OutstandingReferredRedemptionReward
    {
        /// <summary>
        /// Gets or sets the reward payout ID for this referred redemption reward.
        /// </summary>
        public Guid RewardPayoutId { get; set; }

        /// <summary>
        /// Gets or sets the redeemed deal ID for this referred redemption reward.
        /// </summary>
        public Guid RedeemedDealId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID of the card to which to issue the reward this referred redemption reward.
        /// </summary>
        public string PartnerCardId { get; set; }

        /// <summary>
        /// Gets or sets the reward for this referred redemption reward.
        /// </summary>
        public string Reward { get; set; }

        /// <summary>
        /// Gets or sets the date at which the payout for this referred redemption reward was scheduled.
        /// </summary>
        public DateTime PayoutScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction reference number for the redemption that triggered this referred redemption reward.
        /// </summary>
        public int TrackedRedemptionRewardsId { get; set; }
    }
}