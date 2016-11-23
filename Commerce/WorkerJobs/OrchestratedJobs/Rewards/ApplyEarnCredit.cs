//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;
    using System;

    /// <summary>
    /// Applys an earn credit.
    /// </summary>
    public class ApplyEarnCredit
    {
        /// <summary>
        /// Initializes a new instance of the ApplyEarnCredit class.
        /// </summary>
        /// <param name="rewardPayoutRecord">
        /// The RewardPayoutRecord to process.
        /// </param>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ApplyEarnCredit(RewardPayoutRecord rewardPayoutRecord,
                               CommerceLog log)
        {
            RewardPayoutRecord = rewardPayoutRecord;
            Log = log;
        }

        /// <summary>
        /// Applies the Earn reward according to RewardPayoutRecord parameters.
        /// </summary>
        /// <returns></returns>
        public OrchestratedExecutionResult Apply()
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.NonTerminalError;

            CommerceContext context = new CommerceContext("Updating reward status", CommerceWorkerConfig.Instance);
            context[Key.RewardPayoutId] = RewardPayoutRecord.RewardPayoutId;
            context[Key.RewardPayoutStatus] = RewardPayoutStatus.NoEligibleUser;
            string actionDescription = String.Empty;
            if (RewardPayoutRecord.PayeeType == PayeeType.User)
            {
                if (RewardPayoutRecord.PayeeId != Guid.Empty)
                {
                    if (RewardPayoutRecord.Rescinded == false)
                    {
                        result = OrchestratedExecutionResult.Success;
                        context[Key.RewardPayoutStatus] = RewardPayoutStatus.Paid;
                        actionDescription = "was paid";
                    }
                    else
                    {
                        context[Key.RewardPayoutStatus] = RewardPayoutStatus.Rescinded;
                        Log.Information("A rescinded reward cannot be paid out.");
                        actionDescription = "could not be paid because the reward has been rescinded";
                    }
                }
                else
                {
                    Log.Warning("Payee ID cannot be Guid.Empty.");
                    actionDescription = "could not be paid because no eligible user could be found";
                }
            }
            else
            {
                Log.Warning("Earn Rewards cannot be applied to payees of type {0}.", RewardPayoutRecord.PayeeType);
                actionDescription = String.Format("could not be paid because payees of type {0} cannot receive Earn Rewards",
                                                  RewardPayoutRecord.PayeeType);
            }

            Log.Verbose("Updating reward payout record status to indicate the reward {0}.", actionDescription);
            IRewardOperations rewardOperations = CommerceOperationsFactory.RewardOperations(context);
            ResultCode resultCode = rewardOperations.UpdateRewardPayoutStatus();
            if (resultCode != ResultCode.Success && resultCode != ResultCode.PayoutStatusTooAdvanced)
            {
                result = OrchestratedExecutionResult.NonTerminalError;
                Log.Warning("{0} call unsuccessfully processed.\r\n\r\nResultCode: {1}\r\n\r\nExplanation: {2}",
                            (int)resultCode, context.ApiCallDescription, resultCode,
                            ResultCodeExplanation.Get(resultCode));
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the reward payout record being processed by this task.
        /// </summary>
        private RewardPayoutRecord RewardPayoutRecord { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}