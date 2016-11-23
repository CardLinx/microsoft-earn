//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;

    /// <summary>
    /// Orchestrated task to claim a deal for a user's card.
    /// </summary>
    public class ApplyRewardTask : IOrchestratedTask
    {
        /// <summary>
        /// Initializes a new instance of the ApplyReferralRewardTask class.
        /// </summary>
        /// <param name="rewardPayoutRecord">
        /// The RewardPayoutRecord to process.
        /// </param>
        /// <param name="userId">
        /// The ID of the user who was referred into the system.
        /// </param>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ApplyRewardTask(RewardPayoutRecord rewardPayoutRecord,
                               string userId,
                               CommerceLog log)
        {
            RewardPayoutRecord = rewardPayoutRecord;
            UserId = userId;
            Log = log;
        }

        /// <summary>
        /// Initializes the ApplyReferralRewardTask instance.
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the job being run.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler managing the jobs.
        /// </param>
        public void Initialize(ScheduledJobDetails jobDetails,
                               IScheduler scheduler)
        {
            JobDetails = jobDetails;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>
        /// The result of the execution of the task.
        /// </returns>
        public OrchestratedExecutionResult Execute()
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.NonTerminalError;

            switch (RewardPayoutRecord.RewardType)
            {
                case RewardType.Undefined:
                case RewardType.StatementCredit:
                    Log.Verbose("Updating reward payout record status to indicate the reward is pending.");
                    CommerceContext context = new CommerceContext("Updating reward status", CommerceWorkerConfig.Instance);
                    context[Key.RewardPayoutId] = RewardPayoutRecord.RewardPayoutId;
                    context[Key.RewardPayoutStatus] = RewardPayoutStatus.Pending;
                    IRewardOperations rewardOperations = CommerceOperationsFactory.RewardOperations(context);
                    ResultCode resultCode = rewardOperations.UpdateRewardPayoutStatus();
                    if (resultCode != ResultCode.Success)
                    {
                        result = OrchestratedExecutionResult.NonTerminalError;
                        Log.Warning("{0} call unsuccessfully processed.\r\n\r\nResultCode: {1}\r\n\r\nExplanation: {2}",
                                    (int)resultCode, context.ApiCallDescription, resultCode,
                                    ResultCodeExplanation.Get(resultCode));
                    }
                    else
                    {
                        result = OrchestratedExecutionResult.Success;
                    }
                    break;
                case RewardType.EarnCredit:
                    ApplyEarnCredit applyEarnCredit = new ApplyEarnCredit(RewardPayoutRecord, Log);
                    result = applyEarnCredit.Apply();
                    break;
                default:
                    Log.Error("Invalid reward type {0} in reward payout record.", null, RewardPayoutRecord.RewardType);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the details of the job being run.
        /// </summary>
        private ScheduledJobDetails JobDetails { get; set; }

        /// <summary>
        /// Gets or sets the reward payout record being processed by this task.
        /// </summary>
        private RewardPayoutRecord RewardPayoutRecord { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who was referred into the system.
        /// </summary>
        private string UserId { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}