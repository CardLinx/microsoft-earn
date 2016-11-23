//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.VisaClient;
    using Lomo.Core.Extensions;
    using Lomo.Scheduler;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    ///     Contains logic necessary to process the Visa OnClear EndpointMessageRequest 
    /// </summary>
    public class VisaRedeemDealExecutor
    {
        /// <summary>
        ///     Initializes a new instance of the VisaRedeemDealExecutor class.
        /// </summary>
        /// <param name="context">
        ///     The context for the API being invoked.
        /// </param>
        public VisaRedeemDealExecutor(CommerceContext context)
        {
            Context = context;
            Context[Key.Partner] = Partner.Visa;
        }

        /// <summary>
        ///     Executes processing of the request.
        /// </summary>
        public ResultCode Execute()
        {
            ResultCode result = ResultCode.None;
            EndPointMessageRequest request = (EndPointMessageRequest)Context[Key.Request];
            Dictionary<String, String> messageElementCollectionDictionary = new Dictionary<string, string>();
            foreach (MessageElementsCollection c in request.MessageElementsCollection)
                messageElementCollectionDictionary.Add(c.Key, c.Value);

            String requestType = messageElementCollectionDictionary[VisaEPMConstants.EventEventType];
            if (string.Equals(requestType, VisaEPMConstants.OnClearEventTypeValue, StringComparison.OrdinalIgnoreCase))
            {
                Dictionary<String, String> userDefinedFieldsCollectionDictionary = new Dictionary<string, string>();
                foreach (UserDefinedFieldsCollection c in request.UserDefinedFieldsCollection)
                    userDefinedFieldsCollectionDictionary.Add(c.Key, c.Value);
                
                String cardId = request.CardId;
                String merchantId = messageElementCollectionDictionary[VisaEPMConstants.TransactionVisaMerchantId];
                String storeId = messageElementCollectionDictionary[VisaEPMConstants.TransactionVisaStoreId];

                // Marshal the redeem deal request into a RedeemedDeal object.
                RedeemedDeal redeemedDeal = new RedeemedDeal()
                    {
                        AnalyticsEventId = Guid.NewGuid()
                    };
                Context[Key.RedeemedDeal] = redeemedDeal;

                redeemedDeal.CallbackEvent = RedemptionEvent.Settlement;
                redeemedDeal.PartnerRedeemedDealScopeId = messageElementCollectionDictionary[VisaEPMConstants.TransactionVipTransactionId];
                redeemedDeal.PartnerRedeemedDealId =
                    messageElementCollectionDictionary[VisaEPMConstants.TransactionTransactionID];
                String time = messageElementCollectionDictionary[VisaEPMConstants.TransactionTimeStampYYMMDD];
                // UTC Time: 2013-12-05T07:25:06
                redeemedDeal.PurchaseDateTime = DateTime.Parse(time);
                redeemedDeal.PurchaseDateTime = DateTime.SpecifyKind(redeemedDeal.PurchaseDateTime, DateTimeKind.Utc);
                String amount = messageElementCollectionDictionary[VisaEPMConstants.TransactionClearingAmount];
                redeemedDeal.AuthorizationAmount = AmexUtilities.ParseAuthAmount(amount);
                redeemedDeal.Currency = VisaConstants.CurrencyUSD;
                
                Context[Key.PartnerCardId] = cardId;
                Context[Key.PartnerClaimedDealId] = null;  // could be the BingOfferDealId
                Context[Key.PartnerMerchantId] = string.Format("{0};{1}", merchantId, storeId); 
                Context[Key.OutletPartnerMerchantId] = null; // storedId;
                Context[Key.CreditStatus] = CreditStatus.ClearingReceived;

                string merchantCity = messageElementCollectionDictionary.NullIfNotExist(VisaEPMConstants.MerchantCityString);
                string merchantState = messageElementCollectionDictionary.NullIfNotExist(VisaEPMConstants.MerchantStateString);
                var merchantPostalCode = messageElementCollectionDictionary.NullIfNotExist(VisaEPMConstants.MerchantPostalCodeString);

                KeyTransactionData keyTransactionData = new KeyTransactionData
                {
                    MerchantCity = merchantCity,
                    MerchantState = merchantState,
                    MerchantPostalCode = merchantPostalCode
                };

                Context[Key.PartnerData] = keyTransactionData.XmlSerialize();

                LogRedeedmedDealRequestParameters(redeemedDeal);

                result = AddRedeemedDeal();
                Context[Key.ResultCode] = result;

                // If the deal was successfully redeemed, apply any applicable rewards.
                if (result == ResultCode.Created)
                {
                    RedeemedDealInfo redeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];
                    if (redeemedDealInfo != null)
                    {
                        // Update analytics.
                        SharedUserLogic sharedUserLogic = new SharedUserLogic(Context,
                                                                              CommerceOperationsFactory.UserOperations(
                                                                                  Context));
                        Context[Key.GlobalUserId] = redeemedDealInfo.GlobalUserId;
                        User user = sharedUserLogic.RetrieveUser();
                        if (user != null)
                        {
                            Analytics.AddRedemptionEvent(redeemedDealInfo.GlobalUserId, redeemedDeal.AnalyticsEventId,
                                                         user.AnalyticsEventId,
                                                         redeemedDealInfo.ParentDealId, redeemedDealInfo.Currency,
                                                         redeemedDeal.AuthorizationAmount,
                                                         redeemedDealInfo.DiscountAmount,
                                                         redeemedDealInfo.GlobalId, (string) Context[Key.PartnerMerchantId]);
                        }

                        // Add rewards for any active rewards program.
                        AddRedemptionRewards();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Taken down the parameters used to call the addReedemedDeal stored procedure
        /// </summary>
        /// <param name="redeemedDeal">RedeemedDeal just populated</param>
        private void LogRedeedmedDealRequestParameters(RedeemedDeal redeemedDeal)
        {
            String request = String.Format("ReededmedDealRequest: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}", 
                                            redeemedDeal.Id,
                                            (int)Context[Key.Partner],
                                            Context[Key.PartnerDealId],
                                            Context[Key.PartnerCardId],
                                            Context[Key.PartnerClaimedDealId],
                                            Context[Key.PartnerMerchantId],
                                            Context[Key.OutletPartnerMerchantId],
                                            (int)(redeemedDeal.CallbackEvent),
                                            redeemedDeal.PurchaseDateTime.ToString(),
                                            redeemedDeal.AuthorizationAmount,
                                            redeemedDeal.Currency,
                                            redeemedDeal.PartnerRedeemedDealId,
                                            redeemedDeal.AnalyticsEventId,
                                            Context[Key.CreditStatus]);
            Context.Log.Verbose(request); 
        }

        /// <summary>
        /// Adds the redeemed deal to the data store and logs accordingly.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode AddRedeemedDeal()
        {
            ResultCode result = ResultCode.Success;

            // Add the redemption event info to the data store.
            Context.Log.Verbose("Attempting to add the redeemed deal to the data store.");
            IRedeemedDealOperations redeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            result = redeemedDealOperations.AddRedeemedDeal();
            Context.Log.Verbose("ResultCode after adding the redeemed deal to the data store: {0}", result);

            return result;
        }

        /// <summary>
        /// Adds a redemption reward for the transaction in the context.
        /// </summary>
        internal void AddRedemptionRewards()
        {
            RedeemedDealInfo redeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];
            if (Context.Config.EnableRedemptionRewards == true && (ReimbursementTender)redeemedDealInfo.ReimbursementTenderId == ReimbursementTender.MicrosoftEarn)
            {
                IRewardOperations rewardOperations = CommerceOperationsFactory.RewardOperations(Context);
                Context[Key.RewardId] = Context.Config.FirstEarnRewardId;
                Context[Key.FirstEarnRewardAmount] = Context.Config.FirstEarnRewardAmount;
                Context[Key.FirstEarnRewardExplanation] = Context.Config.FirstEarnRewardExplanation;
                ConcurrentDictionary<string, string> payload = new ConcurrentDictionary<string, string>();
                IScheduler scheduler = PartnerFactory.Scheduler(Context.Config.SchedulerQueueName,
                                                                Context.Config.SchedulerTableName,
                                                                Context.Config);
                if (rewardOperations.AddRedemptionReward() == ResultCode.Success)
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
                    scheduler.ScheduleJobAsync(scheduledJobDetails).Wait();
                }

                // Add a job to potentially reward the person who referred this user for this user's first Earn.
                Context[Key.RedeemedDealId] = ((RedeemedDeal)Context[Key.RedeemedDeal]).Id;
                Guid globalUserId = ((RedeemedDealInfo)Context[Key.RedeemedDealInfo]).GlobalUserId;
                Context[Key.GlobalUserId] = globalUserId;
                string userId = globalUserId.ToString();
                if (rewardOperations.AddReferredRedemptionReward() == ResultCode.Success)
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
                    scheduler.ScheduleJobAsync(scheduledJobDetails).Wait();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}