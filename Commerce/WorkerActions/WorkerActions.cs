//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Linq;

namespace Lomo.Commerce.Worker.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Performs actions on behalf of task threads if the calling thread is authorized to perform tasks.
    /// </summary>
    public class WorkerActions
    {
        /// <summary>
        /// Gets the single instance of the WorkerActions class.
        /// </summary>
        public static WorkerActions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorkerActions();
                }

                return instance;
            }
        }
        private static WorkerActions instance = null;

        /// <summary>
        /// If the calling thread is the currently authorized thread, processes the redeemed deal record received from a partner
        /// against the redeemed deals in the data store.
        /// </summary>
        /// <param name="partner">
        /// The Partner from which the redeemed deal record originated.
        /// </param>
        /// <param name="record">
        /// The redeemed deal record to process.
        /// </param>
        /// <param name="redeemedDealOperations">
        /// The object to use to perform redeemed deal operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <param name="threadId">
        /// The ID of the thread attempting to perform this action.
        /// </param>
        /// <returns>
        /// The ResultCode describing the result of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// * Parameter redeemedDealOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public static ResultCode ProcessPartnerRedeemedDealRecord(Partner partner,
                                                           SettlementDetail record,
                                                           IRedeemedDealOperations redeemedDealOperations,
                                                           CommerceContext context)
        {
            if (redeemedDealOperations == null)
            {
                throw new ArgumentNullException("redeemedDealOperations", "Parameter redeemedDealOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            ResultCode result = ResultCode.None;

            try
            {
                context.Log.Verbose("Processing partner redeemed deal record.");
                context[Key.Partner] = partner;
                context[Key.SettlementDetail] = record;
                result = redeemedDealOperations.ProcessPartnerRedeemedDealRecord();
                if (result == ResultCode.Success)
                {
                    context.Log.Verbose("Partner redeemed deal record processed successfully.");
                }
                else
                {
                    context.Log.Warning("Partner redeemed deal record unsuccessfully processed.\r\n\r\nResultCode: {0}" +
                                        "\r\n\r\nExplanation: {1}", (int)result, result, ResultCodeExplanation.Get(result));
                }
            }
            catch(Exception ex)
            {
                context.Log.Critical("Partner redeemed deal record processing ended with an error.", ex);
            }

            return result;
        }

        /// <summary>
        /// Adds the redemption event to the reward subsystem.
        /// </summary>
        /// <param name="rewardOperations">
        /// The object to use to perform reward operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <returns>
        /// The ResultCode describing the result of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// * Parameter record cannot be null.
        /// -OR-
        /// * Parameter rewardOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public static ResultCode RewardRedemption(IRewardOperations rewardOperations,
                                                  CommerceContext context)
        {
            if (rewardOperations == null)
            {
                throw new ArgumentNullException("rewardOperations", "Parameter rewardOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            ResultCode result = ResultCode.Success;

            context.Log.Verbose("Attempting to add redemption reward.");
           
            context[Key.RewardId] = CommerceWorkerConfig.Instance.FirstEarnRewardId;
            context[Key.FirstEarnRewardAmount] = CommerceWorkerConfig.Instance.FirstEarnRewardAmount;
            context[Key.FirstEarnRewardExplanation] = CommerceWorkerConfig.Instance.FirstEarnRewardExplanation;
            context.Log.Verbose("Adding redemption reward to data store.");
            result = rewardOperations.AddRedemptionReward();
            if (result != ResultCode.Success)
            {
                context.Log.Warning("Unable to add redemption reward for this transaction.\r\n\r\nResultCode: {0}" +
                                    "\r\n\r\nExplanation: {1}", (int)result, result, ResultCodeExplanation.Get(result));
            }

            return result;
        }

        /// <summary>
        /// Adds the referred redemption event to the reward subsystem.
        /// </summary>
        /// <param name="rewardOperations">
        /// The object to use to perform reward operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <returns>
        /// The ResultCode describing the result of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// * Parameter record cannot be null.
        /// -OR-
        /// * Parameter rewardOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public static ResultCode RewardReferredRedemption(IRewardOperations rewardOperations,
                                                          CommerceContext context)
        {
            if (rewardOperations == null)
            {
                throw new ArgumentNullException("rewardOperations", "Parameter rewardOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            ResultCode result = ResultCode.Success;

            context.Log.Verbose("Attempting to reward for a referred redemption.");
            result = rewardOperations.AddReferredRedemptionReward();
            if (result != ResultCode.Success)
            {
                context.Log.Warning("Unable to add referred redemption reward for this settlement event.\r\n\r\nResultCode: {0}" +
                                    "\r\n\r\nExplanation: {1}", (int)result, result, ResultCodeExplanation.Get(result));
            }

            return result;
        }

        /// <summary>
        /// If the calling thread is the currently authorized thread, evaluates a set of redeemed deal metadata based on the
        /// number of records and total net redeemed amount against the expected values expressed by the Partner.
        /// </summary>
        /// <param name="numberOfRedeemedDealRecords">
        /// The number of redeemed deal records processed.
        /// </param>
        /// <param name="expectedNumberOfRedeemedDealRecords">
        /// The number of redeemed deal records expected by the partner.
        /// </param>
        /// <param name="totalRedemptionTransactionAmount">
        /// The total amount for all redemption transactions.
        /// </param>
        /// <param name="expectedTotalRedemptionTransactionAmount">
        /// The total amount for all redemption transactions expected by the partner.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public static void EvaluateRedeemedDealMetadata(long numberOfRedeemedDealRecords,
                                                 long expectedNumberOfRedeemedDealRecords,
                                                 decimal totalRedemptionTransactionAmount,
                                                 decimal expectedTotalRedemptionTransactionAmount,
                                                 CommerceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            if (numberOfRedeemedDealRecords != expectedNumberOfRedeemedDealRecords)
            {
                context.Log.Warning("Partner expected {0} redeemed deal records to be found, but {1} were processed.",
                                    expectedNumberOfRedeemedDealRecords, numberOfRedeemedDealRecords);
            }

            if (totalRedemptionTransactionAmount != expectedTotalRedemptionTransactionAmount)
            {
                context.Log.Warning("Partner expected {0} net redeemed amount, but {1} was the calculated net amount.",
                                    expectedTotalRedemptionTransactionAmount, totalRedemptionTransactionAmount);
            }
        }

        /// <summary>
        /// If the calling thread is the currently authorized thread, retrieves the outstanding redeemed deal records for the
        /// partner in the conext.
        /// </summary>
        /// <param name="partner">
        /// The Partner from which the redeemed deal record originated.
        /// </param>
        /// <param name="redeemedDealOperations">
        /// The object to use to perform redeemed deal operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <returns>
        /// The collection of OutstandingRedeemedDealInfo objects built from the outstanding redeemed deal records.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// * Parameter redeemedDealOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public static Collection<OutstandingRedeemedDealInfo> RetrieveOutstandingPartnerRedeemedDealRecords(Partner partner,
                                                                                  IRedeemedDealOperations redeemedDealOperations,
                                                                                  CommerceContext context)
        {
            if (redeemedDealOperations == null)
            {
                throw new ArgumentNullException("redeemedDealOperations", "Parameter redeemedDealOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            Collection<OutstandingRedeemedDealInfo> result = new Collection<OutstandingRedeemedDealInfo>();

            context.Log.Verbose("Retrieving partner redeemed deal records that are ready for settlement.");
            context[Key.Partner] = partner;
            result = redeemedDealOperations.RetrieveOutstandingPartnerRedeemedDealRecords();
            context.Log.Verbose("Retrieved {0} partner redeemed deal records.", result.Count);

            return result;
        }

        /// <summary>
        /// Updates the pending redeemed deals in the merchant record list to the credit status specified.
        /// </summary>
        /// <param name="records">
        /// The list of merchant records whose redeemed deals to update.
        /// </param>
        /// <param name="creditStatus">
        /// The credit status to which to set the redeemed deals.
        /// </param>
        /// <param name="redeemedDealOperations">
        /// The object to use to perform redeemed deal operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// * Parameter redeemedDealOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public static void UpdatePendingRedeemedDeals(Collection<OutstandingRedeemedDealInfo> records,
                                               CreditStatus creditStatus,
                                               IRedeemedDealOperations redeemedDealOperations,
                                               CommerceContext context)
        {
            if (redeemedDealOperations == null)
            {
                throw new ArgumentNullException("redeemedDealOperations", "Parameter redeemedDealOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            context.Log.Verbose("Updating specified redeemed deals to credit status {0}.", creditStatus);
            context[Key.MerchantRecords] = records;
            context[Key.CreditStatus] = creditStatus;
            redeemedDealOperations.UpdatePendingPartnerRedeemedDeals();
        }

        /// <summary>
        /// Updates the reward payout records for those in the ID list to the reward payout status specified.
        /// </summary>
        /// <param name="trackedRedemptionRewardsIds">
        /// The IDs of reward payout records whose status to update.
        /// </param>
        /// <param name="rewardPayoutStatus">
        /// The reward payout status to which to set the records.
        /// </param>
        /// <param name="rewardOperations">
        /// The object to use to perform reward operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// * Parameter rewardOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public static void UpdateOutstandingReferredRedemptionRewards(Collection<int> trackedRedemptionRewardsIds,
                                                                      RewardPayoutStatus rewardPayoutStatus,
                                                                      IRewardOperations rewardOperations,
                                                                      CommerceContext context)
        {
            if (rewardOperations == null)
            {
                throw new ArgumentNullException("rewardOperations", "Parameter rewardOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            context.Log.Verbose("Updating specified reward payout records to credit status {0}.", rewardPayoutStatus);
            context[Key.TrackedRedemptionRewardsIds] = trackedRedemptionRewardsIds;
            context[Key.RewardPayoutStatus] = rewardPayoutStatus;
            rewardOperations.UpdatePendingReferredRedemptionRewards();
        }

        /// <summary>
        /// Updates the pending MasterCard redeemed deals in the record list to the credit status specified.
        /// </summary>
        /// <param name="records">
        /// The list of records whose redeemed deals to update.
        /// </param>
        /// <param name="redeemedDealOperations">
        /// The object to use to perform redeemed deal operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// * Parameter redeemedDealOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public static void MarkSettledAsRedeemed(Collection<OutstandingRedeemedDealInfo> records,
                                                 IRedeemedDealOperations redeemedDealOperations,
                                                 CommerceContext context)
        {
            if (redeemedDealOperations == null)
            {
                throw new ArgumentNullException("redeemedDealOperations", "Parameter redeemedDealOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            context.Log.Verbose("Updating specified redeemed deals as SettledAsRedeemed.");
            context[Key.OutstandingRedeemedDeals] = records;
            redeemedDealOperations.MarkSettledAsRedeemed();
        }

        /// <summary>
        /// Updates the deals in the reference numbers to the credit status specified.
        /// </summary>
        /// <param name="referenceNumbers">
        /// The list of reference numbers of the deals to update.
        /// </param>
        /// <param name="creditStatus">
        /// The credit status to which to set the redeemed deals.
        /// </param>
        /// <param name="redeemedDealOperations">
        /// The object to use to perform redeemed deal operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <param name="threadId">
        /// The ID of the thread attempting to perform this action.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// * Parameter redeemedDealOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public void UpdateDealStatus(Collection<int> referenceNumbers,
                                               CreditStatus creditStatus,
                                               IRedeemedDealOperations redeemedDealOperations,
                                               CommerceContext context,
                                               int threadId)
        {
            if (redeemedDealOperations == null)
            {
                throw new ArgumentNullException("redeemedDealOperations",
                                                "Parameter redeemedDealOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            if (threadId == AuthorizedThreadId)
            {
                context.Log.Verbose("Updating specified redeemed deals to credit status {0}.", creditStatus);
                context[Key.ReferenceNumbers] = referenceNumbers;
                context[Key.CreditStatus] = creditStatus;
                redeemedDealOperations.UpdateRedeemedDeals();

                //// Add analytics info.
                //Collection<SettledDealInfo> settledDealInfoList = redeemedDealOperations.RetrieveSettlementAnalyticsInfo();
                //foreach (SettledDealInfo settledDealInfo in settledDealInfoList)
                //{
                //    Analytics.AddSettlementEvent(settledDealInfo.UserId, settledDealInfo.EventId, settledDealInfo.CorrelationId,
                //                                 settledDealInfo.ParentDealId, settledDealInfo.Currency,
                //                                 settledDealInfo.SettlementAmount, settledDealInfo.DiscountAmount,
                //                                 settledDealInfo.DealId, settledDealInfo.PartnerMerchantId);
                //}
            }
        }

        /// <summary>
        /// Retrieves the PartnerCardIds for any MasterCard that has not yet been filtered.
        /// </summary>
        /// <param name="cardOperations">
        /// The object to use to perform card operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// * Parameter cardOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        /// <returns>
        /// A list of PartnerCardIds for any MasterCard that has not yet been filtered.
        /// </returns>
        public static IEnumerable<string> RetrieveUnfilteredMasterCards(ICardOperations cardOperations,
                                                                        CommerceContext context)
        {
            if (cardOperations == null)
            {
                throw new ArgumentNullException("cardOperations",
                                                "Parameter cardOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            context.Log.Verbose("Retrieving PartnerCardIDs for MasterCards that have not yet been filtered.");
            return cardOperations.RetrieveUnfilteredMasterCards();
        }

        /// <summary>
        /// Adds to the data store the filtering date for the MasterCards cards in the specified list.
        /// </summary>
        /// <param name="filteredMasterCards">
        /// The list of MasterCard cards whose filtering data to add to the data store.
        /// </param>
        /// <param name="cardOperations">
        /// The object to use to perform card operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// * Parameter cardOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        public static ResultCode AddFilteredMasterCards(IEnumerable<string> filteredMasterCards,
                                                        ICardOperations cardOperations,
                                                        CommerceContext context)
        {
            if (cardOperations == null)
            {
                throw new ArgumentNullException("cardOperations",
                                                "Parameter cardOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            context.Log.Verbose("Adding to the data store the filtering date for the list of previously unfiltered MasterCard cards.");
            return cardOperations.AddFilteredMasterCards(filteredMasterCards);
        }

        /// <summary>
        /// Retrieves all outstanding referred redemption reward records.
        /// </summary>
        /// <param name="rewardOperations">
        /// The object to use to perform reward operations.
        /// </param>
        /// <param name="context">
        /// The context of the worker action being executed.
        /// </param>
        /// <returns>
        /// The collection of OutstandingReferredRedemptionReward objects built from the outstanding referred redemption rewards.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// * Parameter rewardOperations cannot be null.
        /// -OR-
        /// * Parameter context cannot be null.
        /// </exception>
        public static Collection<OutstandingReferredRedemptionReward> RetrieveOutstandingReferredRedemptionRewardRecords(IRewardOperations rewardOperations,
                                                                                                                         CommerceContext context)
        {
            if (rewardOperations == null)
            {
                throw new ArgumentNullException("rewardOperations", "Parameter rewardOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            Collection<OutstandingReferredRedemptionReward> result = new Collection<OutstandingReferredRedemptionReward>();

            context.Log.Verbose("Retrieving outstanding referred redemption reward records.");
            result = rewardOperations.RetrieveOutstandingReferredRedemptionRewardRecords();
            context.Log.Verbose("Retrieved {0} referred redemption reward records.", result.Count);

            return result;
        }

        /// <summary>
        /// Retrieve partner user Id for user having globalUserId
        /// </summary>
        /// <param name="globalUserId">Global identifier for a user</param>
        /// <param name="partner">Represents partners with which the Commerce platform interacts.</param>
        /// <param name="userOperations">The object to use to perform user operations.</param>
        /// <param name="context">The context of the worker action being executed.</param>
        /// <returns>Partner User Id.</returns>
        public static string RetrieveUserId (Guid globalUserId, Partner partner, IUserOperations userOperations,
            CommerceContext context)
        {
            var user = RetrieveUserRecord(globalUserId, userOperations, context);
            string parterUserId = null;
            if (user != null)
            {
                parterUserId = user.PartnerUserInfoList.Where(pu => pu.PartnerId == partner).Select(pu => pu.PartnerUserId).FirstOrDefault();
            }

            if (parterUserId == null)
            {
                throw new Exception($"Unable to find PartnerUserId for GlobalUserId:{globalUserId}, Partner:{partner}");
            }

            return parterUserId;
        }

        /// <summary>
        /// Retrieve user object for user having globalUserId
        /// </summary>
        /// <param name="globalUserId">Global identifier for a user</param>
        /// <param name="userOperations">The object to use to perform user operations.</param>
        /// <param name="context">The context of the worker action being executed.</param>
        /// <returns>User object.</returns>
        public static User RetrieveUserRecord(Guid globalUserId, IUserOperations userOperations, CommerceContext context)
        {
            if (userOperations == null)
            {
                throw new ArgumentNullException("userOperations", "Parameter userOperations cannot be null.");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            context.Log.Verbose("Retrieving user record for globalUserId {0}.", globalUserId);
            context[Key.GlobalUserId] = globalUserId;
            var result = userOperations.RetrieveUser();
            context.Log.Verbose("Retrieved user record {0}", result != null);

            return result;
        }

        /// <summary>
        /// The ID of the task thread currently authorized to perform actions.
        /// </summary>
        public int AuthorizedThreadId { get; set; }

        /// <summary>
        /// A value indicating that no task thread is currently authorized to perform actions.
        /// </summary>
        public const int NoThreadAuthorized = Int32.MinValue;
    }
}