//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.ObjectModel;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Represents operations on Reward objects within the data store.
    /// </summary>
    public interface IRewardOperations
    {
        /// <summary>
        /// Adds a reward payout record for a redemption event.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode AddRedemptionReward();

        /// <summary>
        /// Retrieves information about the unprocessed redemption reward specified in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode RetrieveUnprocessedRedemptionReward();

        /// <summary>
        /// Updates the status of the reward payout specified in the context to the status in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode UpdateRewardPayoutStatus();

        /// <summary>
        /// Adds a reward to the user who referred the user who redeemed a deal.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode AddReferredRedemptionReward();

        /// <summary>
        /// Retrieves all outstanding referred redemption reward records.
        /// </summary>
        /// <returns>
        /// The collection of OutstandingReferredRedemptionReward Record objects.
        /// </returns>
        Collection<OutstandingReferredRedemptionReward> RetrieveOutstandingReferredRedemptionRewardRecords();

        /// <summary>
        /// Updates the data store to reflect the pending status of referred redemption reward payouts.
        /// </summary>
        void UpdatePendingReferredRedemptionRewards();

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}