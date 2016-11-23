//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Worker.Actions;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Scheduler;

    /// <summary>
    /// Processes First Data Extract files.
    /// </summary>
    public class FirstDataExtractProcessor : ISettlementFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the FirstDataExtractProcessor class.
        /// </summary>
        public FirstDataExtractProcessor()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FirstDataExtractProcessor class.
        /// </summary>
        /// <param name="enableRedemptionRewards">
        /// Specifies whether redemption rewards should be enabled.
        /// </param>
        public FirstDataExtractProcessor(bool enableRedemptionRewards)
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            RewardOperations = CommerceOperationsFactory.RewardOperations(Context);
            EnableRedemptionRewards = enableRedemptionRewards;
        }

        /// <summary>
        /// Processes Extract files.
        /// </summary>
        public async Task Process()
        {
            await ProcessExtract().ConfigureAwait(false);
        }

        /// <summary>
        /// The ID of the task thread in which this object is operating.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Processes an extract file.
        /// </summary>
        internal async Task ProcessExtract()
        {
            // Deserialize extract file into an Extract object.
            ExtractParser extractParser = new ExtractParser(Context.Log);
            Extract extract = extractParser.Parse(ExtractFileName, ExtractFileStream);

            if (extract != null)
            {
                // Process each settlement detail record in turn, keeping track of the total amount represented for settlement
                // detail records in the extract.
                decimal totalSettlementTransactionAmount = 0;
                foreach (SettlementDetail settlementDetail in extract.SettlementDetailRecords)
                {
                    if (settlementDetail != null)
                    {
                        ResultCode recordResult = WorkerActions.ProcessPartnerRedeemedDealRecord(Partner.FirstData,
                                                                                                          settlementDetail,
                                                                                                          RedeemedDealOperations,
                                                                                                          Context);
                        if (recordResult == ResultCode.Success || recordResult == ResultCode.CreditStatusTooAdvanced)
                        {
//TODO: How are reversals handled here? May need a real-world reversal example to be sure.
                            totalSettlementTransactionAmount += settlementDetail.TotalTransactionAmount;
                            // If the record was processed successfully, add any applicable rewards tied to the redemption event.
                            if (recordResult == ResultCode.Success)
                            {
                                await ProcessRewardPayoutAsync(settlementDetail);
                            }
                        }
                    }
                }

                // Evaluate the integrity of the Extract file.
                if (extract.Footer != null)
                {
                    WorkerActions.EvaluateRedeemedDealMetadata(extract.SettlementDetailRecords.Count,
                                                                        extract.Footer.NumberOfSettlementRecords,
                                                                        totalSettlementTransactionAmount,
                                                                        extract.Footer.TotalSettlementRecordAmount,
                                                                        Context);
                }
            }
        }

        /// <summary>
        /// Process the rewards if applicable
        /// </summary>
        /// <param name="settlementDetail">
        /// Settlement Details
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        internal async Task ProcessRewardPayoutAsync(SettlementDetail settlementDetail)
        {
            if (settlementDetail.TransactionType == TransactionType.SettlementRedemption)
            {
                // First add a redemption reward to the redeeming user if they're enabled.
                if (EnableRedemptionRewards == true && WorkerActions.RewardRedemption(RewardOperations, Context) == ResultCode.Success)
                {
                    // Add job to process the reward payout. Note that this job will be scheduled 30 minutes in the
                    // future to guard against applying a reward for a transaction that was reversed in a later
                    // record.
                    ConcurrentDictionary<string, string> payload = new ConcurrentDictionary<string, string>();
                    payload[Key.RewardPayoutId.ToString()] = ((Guid) Context[Key.RewardPayoutId]).ToString();
                    payload[Key.PartnerCardId.ToString()] = (string) Context[Key.PartnerCardId];
                    payload[Key.PartnerRedeemedDealId.ToString()] = settlementDetail.TransactionId;

                    IScheduler scheduler = PartnerFactory.Scheduler(CommerceWorkerConfig.Instance.SchedulerQueueName,
                        CommerceWorkerConfig.Instance.SchedulerTableName,
                        CommerceWorkerConfig.Instance);
                    ScheduledJobDetails scheduledJobDetails = new ScheduledJobDetails
                    {
                        JobId = Guid.NewGuid(),
                        JobType = ScheduledJobType.ApplyRedemptionReward,
                        JobDescription = settlementDetail.ConsumerId,
                        Orchestrated = true,
                        StartTime = DateTime.UtcNow.AddMinutes(30),
                        Payload = payload
                    };

                    await scheduler.ScheduleJobAsync(scheduledJobDetails).ConfigureAwait(false);
                }

                // Then add a referred redemption reward to the user who referred the redeeming user.
                WorkerActions.RewardReferredRedemption(RewardOperations, Context);
            }
            else
            {
                Context.Log.Verbose("No Bing Reward can be given for a reversed transaction.");
            }
        }

        /// <summary>
        /// Gets or sets data Stream for extract file contents
        /// </summary>
        public Stream ExtractFileStream { get;  set; }

        /// <summary>
        /// Gets or sets extract file name
        /// </summary>
        public string ExtractFileName { get; set; }

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
    }
}