//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Globalization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents operations on Reward objects within the data store.
    /// </summary>
    public class RewardOperations : CommerceOperations, IRewardOperations
    {
        /// <summary>
        /// Adds a reward payout record for a redemption event.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddRedemptionReward()
        {
            return SqlProcedure("AddRedemptionReward",
                                new Dictionary<string, object>
                                {
                                    { "@rewardId", (Guid)Context[Key.RewardId] },
                                    { "@amount", (int)Context[Key.FirstEarnRewardAmount] },
                                    { "@explanation", (string)Context[Key.FirstEarnRewardExplanation] }
                                },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        Context[Key.RewardPayoutId] = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("RewardPayoutId"));
                    }
                });
        }

        /// <summary>
        /// Retrieves information about the unprocessed redemption reward specified in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode RetrieveUnprocessedRedemptionReward()
        {
            Guid rewardPayoutId = (Guid)Context[Key.RewardPayoutId];

            // Marshal sproc parameters.
            Dictionary<string, object> parameterList = new Dictionary<string, object>();
            parameterList.Add("@rewardPayoutId", rewardPayoutId);
            parameterList.Add("@partnerCardId", (string)Context[Key.PartnerCardId]);
            parameterList.Add("@partnerRedeemedDealId", (string)Context[Key.PartnerRedeemedDealId]);
            parameterList.Add("@rewardId", (Guid)Context[Key.RewardId]);

            // Call the sproc and return the result code.
            return SqlProcedure("GetUnprocessedRedemptionReward", parameterList,
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        // Get the reward payout record.
                        RewardPayoutRecord rewardPayoutRecord = new RewardPayoutRecord
                        {
                            RewardPayoutId = rewardPayoutId,
                            RewardType = (RewardType)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("RewardTypeId")),
                            Properties = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Properties")),
                            PayeeId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("UserId")),
                            PayeeType = PayeeType.User,
                            Rescinded = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Rescinded"))
                        };
                        Context[Key.RewardPayoutRecord] = rewardPayoutRecord;
                    }
                });
        }

        /// <summary>
        /// Updates the status of the reward payout specified in the context to the status in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode UpdateRewardPayoutStatus()
        {
            return SqlProcedure("UpdateRewardPayoutStatus",
                                new Dictionary<string, object>
                                {
                                    { "@rewardPayoutId", (Guid)Context[Key.RewardPayoutId] },
                                    { "@payoutStatusId", (int)Context[Key.RewardPayoutStatus] }
                                });
        }

        /// <summary>
        /// Adds a reward to the user who referred the user who redeemed a deal.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddReferredRedemptionReward()
        {
            // Add the referred redemption reward to the data layer.
            return SqlProcedure("AddReferredRedemptionReward", new Dictionary<string, object>
                                                 {
                                                     { "@redeemedDealId", Context[Key.RedeemedDealId] },
                                                     { "@referredUserId", Context[Key.GlobalUserId] }
                                                 });
        }

        /// <summary>
        /// Retrieves all outstanding referred redemption reward records.
        /// </summary>
        /// <returns>
        /// The collection of OutstandingReferredRedemptionReward Record objects.
        /// </returns>
        public Collection<OutstandingReferredRedemptionReward> RetrieveOutstandingReferredRedemptionRewardRecords()
        {
            Collection<OutstandingReferredRedemptionReward> result = new Collection<OutstandingReferredRedemptionReward>();

            SqlProcedure("GetOutstandingReferredRedemptionRewardRecords", null,

                (sqlDataReader) =>
                {
                    OutstandingReferredRedemptionReward outstandingReferredRedemptionReward = null;
                    while (sqlDataReader.Read() == true)
                    {
                        outstandingReferredRedemptionReward = new OutstandingReferredRedemptionReward()
                        {
                            TrackedRedemptionRewardsId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TrackedRedemptionRewardsId")),
                            RewardPayoutId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("RewardPayoutId")),
                            RedeemedDealId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("RedeemedDealId")),
                            PartnerCardId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerCardId")),
                            Reward = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Properties")),
                            PayoutScheduledDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("PayoutScheduledDateUtc")),
                        };

                        result.Add(outstandingReferredRedemptionReward);
                    }
                });

            return result;
        }

        /// <summary>
        /// Updates the data store to reflect the pending status of referred redemption reward payouts.
        /// </summary>
        public void UpdatePendingReferredRedemptionRewards()
        {
            using (DataTable trackedRedemptionRewardsIdsTable = new DataTable("ListOfInts"))
            {
                // Build the RewardPayoutIDs table parameter.
                Collection<int> trackedRedemptionRewardsIds = (Collection<int>)Context[Key.TrackedRedemptionRewardsIds];
                trackedRedemptionRewardsIdsTable.Locale = CultureInfo.InvariantCulture;
                trackedRedemptionRewardsIdsTable.Columns.Add("Id", typeof(int));
                foreach (int trackedRedemptionRewardsId in trackedRedemptionRewardsIds)
                {
                    trackedRedemptionRewardsIdsTable.Rows.Add(trackedRedemptionRewardsId);
                }

                // Update referred redemption rewards.
                SqlProcedure("UpdatePendingReferredRedemptionRewards",
                             new Dictionary<string, object>
                             {
                                 { "@rewardPayoutStatusId", (int)Context[Key.RewardPayoutStatus] },
                                 { "@rewardPayoutIds", trackedRedemptionRewardsIdsTable }
                             });
            }
        }
    }
}