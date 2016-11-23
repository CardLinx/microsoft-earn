//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Threading.Tasks;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Notifications;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to process the redemption of a FirstData deal.
    /// </summary>
    public class FirstDataRedeemDealExecutor
    {
        /// <summary>
        /// Initializes a new instance of the FirstDataRedeemDealExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context for the API being invoked.
        /// </param>
        public FirstDataRedeemDealExecutor(CommerceContext context)
        {
            Context = context;
            Context[Key.Partner] = Partner.FirstData;
        }

        /// <summary>
        /// Executes processing of the deal redemption request.
        /// </summary>
        public ResultCode Execute()
        {
            ResultCode result;

            // Marshal the First Data redeem deal request into a RedeemedDeal object.
            RedeemedDeal redeemedDeal = new RedeemedDeal()
            {
                AnalyticsEventId = Guid.NewGuid()
            };
            Context[Key.RedeemedDeal] = redeemedDeal;
            FirstData firstData = new FirstData(Context);
            firstData.MarshalRedeemDeal();

            // If the purchase date and time was valid, Add the RedeemedDeal to the data store.
            if (redeemedDeal.PurchaseDateTime != DateTime.MinValue)
            {
                string disallowedReason = Context[Key.DisallowedReason] as string;
                if (String.IsNullOrWhiteSpace(disallowedReason) == true)
                {
                    result = AddRedeemedDeal();
                }
                else
                {
                    Context.Log.Warning("Transaction is disallowed because tender type was: {0}.", disallowedReason);
                    Context[Key.RedeemedDealInfo] = new RedeemedDealInfo
                    {
                        PartnerDealId = (string)Context[Key.PartnerDealId],
                        PartnerClaimedDealId = (string)Context[Key.PartnerClaimedDealId]
                    };
                    result = ResultCode.TransactionDisallowed;
                }
            }
            else
            {
                result = ResultCode.InvalidPurchaseDateTime;
            }

            // Build the response to send back to First Data based on the result of adding the RedeemedDeal.
            Context[Key.ResultCode] = result;
            firstData.BuildRedeemedDealResponse();

            // If the deal was successfully redeemed, send user notification and update analytics.
            if (result == ResultCode.Created)
            {
                RedeemedDealInfo redeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];

                // Send notification.
                NotifyAuthorization notifyAuthorization = new NotifyAuthorization(Context);
                Task.Run(new Action(notifyAuthorization.SendNotification));

                // Update analytics.
                SharedUserLogic sharedUserLogic = new SharedUserLogic(Context,
                                                                              CommerceOperationsFactory.UserOperations(Context));
                Context[Key.GlobalUserId] = redeemedDealInfo.GlobalUserId;
                User user = sharedUserLogic.RetrieveUser();
                Analytics.AddRedemptionEvent(redeemedDealInfo.GlobalUserId, redeemedDeal.AnalyticsEventId, user.AnalyticsEventId,
                                             redeemedDealInfo.ParentDealId, redeemedDealInfo.Currency,
                                             redeemedDeal.AuthorizationAmount, redeemedDealInfo.DiscountAmount,
                                             redeemedDealInfo.GlobalId, (string)Context[Key.PartnerMerchantId]);
            }

            return result;
        }

        /// <summary>
        /// Adds the redeemed deal to the data store and logs accordingly.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddRedeemedDeal()
        {
            ResultCode result;

            // Add the redemption event info to the data store.
            Context.Log.Verbose("Attempting to add the redeemed deal to the data store.");
            IRedeemedDealOperations redeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            result = redeemedDealOperations.AddRedeemedDeal();
            Context.Log.Verbose("ResultCode after adding the redeemed deal to the data store: {0}", result);

            return result;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}