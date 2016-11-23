//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Worker.Actions;

    /// <summary>
    /// Processes statement credit rewards into a First Data PTS file.
    /// </summary>
    public class RewardsPtsProcessor
    {
        /// <summary>
        /// Initializes a new instance of the RewardsPtsProcessor class.
        /// </summary>
        public RewardsPtsProcessor()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RewardOperations = CommerceOperationsFactory.RewardOperations(Context);
        }

        /// <summary>
        /// Processes the PTS file.
        /// </summary>
        public virtual async Task Process()
        {
            Collection<OutstandingReferredRedemptionReward> result = WorkerActions.RetrieveOutstandingReferredRedemptionRewardRecords(RewardOperations, Context);
            if (OnRewardsPtsBuild != null)
            {
                // Marshal OutstandingReferredRedemptionReward records into the OutstandingRedeemedDealInfo records expected by the PTS builder.
                Collection<OutstandingRedeemedDealInfo> outstandingRedeemedDealInfoList = new Collection<OutstandingRedeemedDealInfo>();
                foreach (OutstandingReferredRedemptionReward outstandingReward in result)
                {
                    outstandingRedeemedDealInfoList.Add(new OutstandingRedeemedDealInfo
                    {
                        RedeemedDealId = outstandingReward.RedeemedDealId,
                        PartnerMerchantId = "221042079998",
                        MerchantName = "REFER FRIEND",
                        OfferId = "qqqqqqqqqq",
                        AcquirerReferenceNumber = "24601",
                        Token = outstandingReward.PartnerCardId,
                        DiscountAmount = Int32.Parse(outstandingReward.Reward),
                        TransactionDate = outstandingReward.PayoutScheduledDate,
                        ReferenceNumber = outstandingReward.TrackedRedemptionRewardsId,
                        SettlementAmount = 0,
                        DiscountId = String.Empty,
                        DealId = String.Empty,
                        PartnerData = null
                    });
                }

                // FDC requires EST.
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime estNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, easternZone);

                // this will ftp the data and store it in blob
                // Always construct file as sequence number of 3, because PTS job runs once a day and this will be after Visa dump
                string ptsFileContents = PtsBuilder.Build(outstandingRedeemedDealInfoList, estNow, 3, true);
                await OnRewardsPtsBuild(ptsFileContents).ConfigureAwait(false);
                // now update db
                UpdateOutstandingReferredRedemptionRewards(result, RewardPayoutStatus.Pending);
            }
        }

        /// <summary>
        /// Updates the outstanding referred redemption rewards in the record list to the reward payout status specified.
        /// </summary>
        /// <param name="records">
        /// The list of records whose status to update.
        /// </param>
        /// <param name="rewardPayoutStatus">
        /// The reward payout status to which to set the records.
        /// </param>
        private void UpdateOutstandingReferredRedemptionRewards(Collection<OutstandingReferredRedemptionReward> records,
                                                                RewardPayoutStatus rewardPayoutStatus)
        {
            Collection<int> trackedRedemptionRewardsIds = new Collection<int>();
            foreach(OutstandingReferredRedemptionReward record in records)
            {
                trackedRedemptionRewardsIds.Add(record.TrackedRedemptionRewardsId);
            }
            WorkerActions.UpdateOutstandingReferredRedemptionRewards(trackedRedemptionRewardsIds, rewardPayoutStatus, RewardOperations, Context);
        }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Delegate to do post processing after pts contents are built
        /// </summary>
        public Func<string, Task> OnRewardsPtsBuild { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRewardOperations RewardOperations { get; set; }
    }
}