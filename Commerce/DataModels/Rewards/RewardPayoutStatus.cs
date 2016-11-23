//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    /// <summary>
    /// Represents the status a reward payout may be assigned.
    /// </summary>
    public enum RewardPayoutStatus
    {
        /// <summary>
        /// Indicates the reward payout has not yet been processed.
        /// </summary>
		Unprocessed = 0,

        /// <summary>
        /// Indicates the reward payout is pending further action.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Indicates the reward has been paid out.
        /// </summary>
		Paid = 2,

        /// <summary>
        /// Indicates the reward will not be paid out because the user has reached the limit for the reward.
        /// </summary>
        LimitReached = 3,

        /// <summary>
        /// Indicates that the reward will not be paid out because no eligible user is available to receive it.
        /// </summary>
        NoEligibleUser = 4,

        /// <summary>
        /// Indicates that the reward will not be paid out because it was rescinded.
        /// </summary>
        Rescinded = 5
    }
}