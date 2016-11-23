//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Worker.Actions;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Scheduler;

    /// <summary>
    /// Processes MasterCard clearing files.
    /// </summary>
    public class MasterCardClearingProcessor : ISettlementFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the MasterCardClearingProcessor class.
        /// </summary>
        public MasterCardClearingProcessor()
            : this(CommerceWorkerConfig.Instance.EnableRedemptionRewards)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MasterCardClearingProcessor class.
        /// </summary>
        /// <param name="enableRedemptionRewards">
        /// Specifies whether redemption rewards are enabled.
        /// </param>
        public MasterCardClearingProcessor(bool enableRedemptionRewards)
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            RewardOperations = CommerceOperationsFactory.RewardOperations(Context);
            EnableRedemptionRewards = enableRedemptionRewards;
        }

        /// <summary>
        /// Processes the clearing file.
        /// </summary>
        public async virtual Task Process()
        {
            // Deserialize clearing file into a Clearing object.
            ClearingParser clearingParser = new ClearingParser(Context.Log);
            Clearing clearing = clearingParser.Parse(FileName, Stream);

            if (clearing != null)
            {
                foreach (ClearingData clearingData in clearing.DataRecords)
                {
                    if (clearingData != null)
                    {
                        // Add a redeemed deal record to the data store for the transaction.
                        RedeemedDeal redeemedDeal = new RedeemedDeal();
                        ResultCode result = AddRedeemedDealRecord(clearingData, redeemedDeal);

                        // If the redeemed deal record was added successfully, update rewards.
                        if (result == ResultCode.Created)
                        {
                            await AddRedemptionRewards(redeemedDeal.Id).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The ID of the task thread in which this object is operating.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the stream containing clearing file contents.
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// Gets or sets the name of the clearing file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Adds a RedeemedDeal record to the data store for the specified ClearingData record.
        /// </summary>
        /// <param name="clearingData">
        /// The ClearingData record for which to add a redeemed deal record.
        /// </param>
        /// <param name="redeemedDeal">
        /// The RedeemedDeal object to use when committing the redeemed deal record.
        /// </param>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode AddRedeemedDealRecord(ClearingData clearingData,
                                                 RedeemedDeal redeemedDeal)
        {
            ResultCode result = ResultCode.NoApplicableDealFound;

            redeemedDeal.AnalyticsEventId = Guid.NewGuid();
            Context[Key.RedeemedDeal] = redeemedDeal;
            new MasterCard(Context).MarshalRedeemDeal(clearingData);
            Context[Key.Partner] = Partner.MasterCard;
            result = RedeemedDealOperations.AddRedeemedDeal();

            return result;
        }

        /// <summary>
        /// Adds a redemption reward for the transaction in the context.
        /// </summary>
        /// <param name="redeemedDealId">
        /// The ID of the redeemed deal for which redemption rewards are granted.
        /// </param>
        /// <returns>
        /// A Task to perform the operation.
        /// </returns>
        private async Task AddRedemptionRewards(Guid redeemedDealId)
        {
            RedeemedDealInfo redeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];
            if (EnableRedemptionRewards == true && (ReimbursementTender)redeemedDealInfo.ReimbursementTenderId == ReimbursementTender.MicrosoftEarn)
            {
                ConcurrentDictionary<string, string> payload = new ConcurrentDictionary<string, string>();
                IScheduler scheduler = PartnerFactory.Scheduler(CommerceWorkerConfig.Instance.SchedulerQueueName,
                                                                CommerceWorkerConfig.Instance.SchedulerTableName,
                                                                CommerceWorkerConfig.Instance);
                if (WorkerActions.RewardRedemption(RewardOperations, Context) == ResultCode.Success)
                {
                    // Add a job to potentially reward user for their first Earn.
                    payload[Key.RewardPayoutId.ToString()] = ((Guid)Context[Key.RewardPayoutId]).ToString();
                    payload[Key.PartnerCardId.ToString()] = (string)Context[Key.PartnerCardId];
                    payload[Key.PartnerRedeemedDealId.ToString()] = redeemedDealInfo.PartnerRedeemedDealId;
                    payload[Key.RewardId.ToString()] = Context.Config.FirstEarnRewardId.ToString();
                    ScheduledJobDetails scheduledJobDetails = new ScheduledJobDetails
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ApplyRedemptionReward,
                        JobDescription = redeemedDealInfo.GlobalUserId.ToString(),
                        Orchestrated = true,
                        Payload = payload
                    };
                    await scheduler.ScheduleJobAsync(scheduledJobDetails).ConfigureAwait(false);
                }

                // Add a job to potentially reward the person who referred this user for this user's first Earn.
                Context[Key.RedeemedDealId] = ((RedeemedDeal)Context[Key.RedeemedDeal]).Id;
                Guid globalUserId = ((RedeemedDealInfo)Context[Key.RedeemedDealInfo]).GlobalUserId;
                Context[Key.GlobalUserId] = globalUserId;
                string userId = globalUserId.ToString();
                if (WorkerActions.RewardReferredRedemption(RewardOperations, Context) == ResultCode.Success)
                {
                    payload[Key.GlobalUserId.ToString()] = userId;
                    payload[Key.ReferralEvent.ToString()] = ReferralEvent.Signup.ToString();
                    ScheduledJobDetails scheduledJobDetails = new ScheduledJobDetails
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ApplyReferralReward,
                        JobDescription = userId,
                        Orchestrated = true,
                        StartTime = DateTime.UtcNow,
                        Payload = payload
                    };
                    await scheduler.ScheduleJobAsync(scheduledJobDetails).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Updates analytics with information about the transaction in the context.
        /// </summary>
        private void UpdateAnalytics()
        {
            RedeemedDealInfo redeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];
            SharedUserLogic sharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));
            Context[Key.GlobalUserId] = redeemedDealInfo.GlobalUserId;
            User user = sharedUserLogic.RetrieveUser();
            RedeemedDeal redeemedDeal = (RedeemedDeal)Context[Key.RedeemedDeal];
            Analytics.AddRedemptionEvent(redeemedDealInfo.GlobalUserId, redeemedDeal.AnalyticsEventId, user.AnalyticsEventId,
                                         redeemedDealInfo.ParentDealId, redeemedDealInfo.Currency,
                                         redeemedDeal.AuthorizationAmount, redeemedDealInfo.DiscountAmount,
                                         redeemedDealInfo.GlobalId, (string)Context[Key.PartnerMerchantId],
                                         CommerceWorkerConfig.Instance);
        }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRedeemedDealOperations RedeemedDealOperations { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for Reward operations.
        /// </summary>
        private IRewardOperations RewardOperations { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether redemption rewards are enabled.
        /// </summary>
        private bool EnableRedemptionRewards { get; set; }
    }
}