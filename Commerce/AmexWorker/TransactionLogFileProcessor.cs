//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.AmexClient;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Worker.Actions;
    using Lomo.Scheduler;

    /// <summary>
    /// Process Amex Transaction Log file
    /// </summary>
    public class TransactionLogFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the TransactionLogFileProcessor class.
        /// </summary>
        public TransactionLogFileProcessor()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TransactionLogFileProcessor class.
        /// </summary>
        /// <param name="enableRedemptionRewards">
        /// Specifies whether redemption rewards should be enabled.
        /// </param>
        public TransactionLogFileProcessor(bool enableRedemptionRewards)
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            RewardOperations = CommerceOperationsFactory.RewardOperations(Context);
            EnableRedemptionRewards = enableRedemptionRewards;
            Context[Key.Partner] = Partner.Amex;
            TransactionIdSet = new HashSet<string>();
        }

        /// <summary>
        /// Process the transaction log file
        /// </summary>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async Task Process()
        {
            TransactionLogParser transactionLogParser = new TransactionLogParser(Context.Log);
            TransactionLogFile transactionLogFile = transactionLogParser.Parse(TransactionLogFileName, TransactionLogFileStream);

            if (transactionLogFile != null)
            {
                foreach (TransactionLogDetail detail in transactionLogFile.TransactionLogRecords)
                {
                    // block the reversed transactions
                    if(TransactionIdSet.Contains(detail.TransactionId) || detail.TransactionAmount <= 0)
                    {
                        continue;
                    }

                    TransactionIdSet.Add(detail.TransactionId);

                    // 1. process the detail record here -> Insert as redeemed deal
                    RedeemedDeal redeemedDeal = new RedeemedDeal()
                    {
                        AnalyticsEventId = Guid.NewGuid()
                    };
                    Context[Key.RedeemedDeal] = redeemedDeal;
                    MarshalRedeemDeal(detail);
                    ResultCode result = RedeemedDealOperations.AddRedeemedDeal();
                    
                    //2. If the record was processed successfully, attempt to add a redemption reward if applicable and analytics
                    if (result == ResultCode.Created)
                    {
                        RedeemedDealInfo redemptionInfo = (RedeemedDealInfo) Context[Key.RedeemedDealInfo];
                        // First add a redemption reward to the redeeming user if they're enabled.
                        if (EnableRedemptionRewards)
                        {
                            if (WorkerActions.RewardRedemption(RewardOperations, Context) == ResultCode.Success)
                            {
                                // Add job to process the reward payout.
                                ConcurrentDictionary<string, string> payload = new ConcurrentDictionary<string, string>();
                                payload[Key.RewardPayoutId.ToString()] = ((Guid)Context[Key.RewardPayoutId]).ToString();
                                payload[Key.PartnerCardId.ToString()] = (string)Context[Key.PartnerCardId];
                                payload[Key.PartnerRedeemedDealId.ToString()] = redemptionInfo.PartnerRedeemedDealId;

                                IScheduler scheduler = PartnerFactory.Scheduler(CommerceWorkerConfig.Instance.SchedulerQueueName,
                                                                                CommerceWorkerConfig.Instance.SchedulerTableName,
                                                                                CommerceWorkerConfig.Instance);
                                ScheduledJobDetails scheduledJobDetails = new ScheduledJobDetails
                                {
                                    JobId = Guid.NewGuid(),
                                    JobType = ScheduledJobType.ApplyRedemptionReward,
                                    JobDescription = redemptionInfo.GlobalUserId.ToString(),
                                    Orchestrated = true,
                                    Payload = payload
                                };

                                await scheduler.ScheduleJobAsync(scheduledJobDetails).ConfigureAwait(false);
                            }
                        }

                        // Then add a referred redemption reward to the user who referred the redeeming user.
                        Context[Key.RedeemedDealId] = ((RedeemedDeal)Context[Key.RedeemedDeal]).Id;
                        Context[Key.GlobalUserId] = ((RedeemedDealInfo)Context[Key.RedeemedDealInfo]).GlobalUserId;
                        WorkerActions.RewardReferredRedemption(RewardOperations, Context);

                        // Update analytics.
                        // For FDC this happens at AUTH time 
                        // Butfor Amex flow, we put analytics at the time of Transaction File Processing
                        SharedUserLogic sharedUserLogic = new SharedUserLogic(Context,
                                                                                      CommerceOperationsFactory.UserOperations(Context));
                        Context[Key.GlobalUserId] = redemptionInfo.GlobalUserId;
                        User user = sharedUserLogic.RetrieveUser();
                        Analytics.AddRedemptionEvent(redemptionInfo.GlobalUserId, redeemedDeal.AnalyticsEventId, user.AnalyticsEventId,
                                                     redemptionInfo.ParentDealId, redemptionInfo.Currency,
                                                     redeemedDeal.AuthorizationAmount, redemptionInfo.DiscountAmount,
                                                     redemptionInfo.GlobalId, (string)Context[Key.PartnerMerchantId], 
                                                     CommerceWorkerConfig.Instance);
                        
                    }
                }
            }
        }

        /// <summary>
        /// Marshal Transaction Log File Record into a RedeemedDeal Record
        /// </summary>
        /// <param name="detail">
        /// Transaction Log File Detail Record
        /// </param>
        private void MarshalRedeemDeal(TransactionLogDetail detail)
        {
            RedeemedDeal redeemedDeal = (RedeemedDeal)Context[Key.RedeemedDeal];
            redeemedDeal.CallbackEvent = RedemptionEvent.Settlement;
            redeemedDeal.PurchaseDateTime = detail.TransactionDate;
            redeemedDeal.AuthorizationAmount = (int)(detail.TransactionAmount * 100);
            redeemedDeal.Currency = "USD";
            redeemedDeal.PartnerRedeemedDealScopeId = detail.TransactionId;
            //redeemedDeal.PartnerRedeemedDealId = detail.TransactionId;

            Context[Key.PartnerDealId] = detail.OfferId;
            Context[Key.PartnerCardId] = detail.CardToken;
            Context[Key.PartnerMerchantId] = detail.MerchantNumber;
            Context[Key.CreditStatus] = CreditStatus.ClearingReceived;
        }

        /// <summary>
        /// Gets or sets data Stream for transaction log file contents
        /// </summary>
        public Stream TransactionLogFileStream { get; set; }

        /// <summary>
        /// Gets or sets extract file name
        /// </summary>
        public string TransactionLogFileName { get; set; }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        internal CommerceContext Context { get; set; }

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

        /// <summary>
        /// Set of unique transaction Ids
        /// </summary>
        private HashSet<string> TransactionIdSet;

    }
}