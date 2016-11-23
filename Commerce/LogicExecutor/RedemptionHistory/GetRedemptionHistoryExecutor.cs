//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to execute a get redemption history request.
    /// </summary>
    public class GetRedemptionHistoryExecutor
    {
        /// <summary>
        /// Redemption type to return for Earn transaction
        /// </summary>
        private const string EarnRedemption = "Earn Credit";

        /// <summary>
        /// Redemption type to return for Burn transaction
        /// </summary>
        private const string BurnRedemption = "Redemption";

        /// <summary>
        /// Initializes a new instance of the GetRedemptionHistoryExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        public GetRedemptionHistoryExecutor(CommerceContext context)
        {
            Context = context;
            Context[Key.MerchantIds] = new List<Guid>();
            RedemptionHistoryOperations = CommerceOperationsFactory.RedemptionHistoryOperations(Context);
        }

        /// <summary>
        /// Executes the get redemption history invocation.
        /// </summary>
        public void Execute()
        {
            SharedUserLogic sharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));

            User user = sharedUserLogic.RetrieveUser();
            Context[Key.User] = user;
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];
            if (user != null)
            {
                RewardPrograms rewardProgram = (RewardPrograms)Context[Key.RewardProgramType];

                if ((rewardProgram & RewardPrograms.CardLinkOffers) == RewardPrograms.CardLinkOffers)
                {
                    resultSummary.SetResultCode(GetRedemptionHistory());
                }
                else if ((rewardProgram & RewardPrograms.EarnBurn) == RewardPrograms.EarnBurn)
                {
                    resultSummary.SetResultCode(GetEarnBurnHistory());
                }
            }
            else
            {
                resultSummary.SetResultCode(ResultCode.UnexpectedUnregisteredUser);
            }
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for History operations.
        /// </summary>
        internal IRedemptionHistoryOperations RedemptionHistoryOperations { get; set; }

        /// <summary>
        /// Retrieves the redemption history for the specified user.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode GetRedemptionHistory()
        {
            List<RedemptionHistoryItemDataContract> historyItems = new List<RedemptionHistoryItemDataContract>();

            // Retrieve list of redemption history items.
            Context.Log.Verbose("Retrieving redemption history belonging to user {0}", ((User)Context[Key.User]).GlobalId);
            IEnumerable<RedemptionHistoryItem> items = RedemptionHistoryOperations.RetrieveRedemptionHistory();
            Context.Log.Verbose("{0} redemption history entries were retrieved from the data store.", items.Count());

            // If any redemption history items were returned, add them to the context.
            if (items.Any() == true)
            {
                // Build the list of HistoryItemDataContracts from the list of redemption history items.
                foreach (RedemptionHistoryItem redemptionHistoryItem in items)
                {
                    historyItems.Add(new RedemptionHistoryItemDataContract
                               {
                                   MerchantName = redemptionHistoryItem.MerchantName,
                                   DiscountSummary = redemptionHistoryItem.DiscountSummary,
                                   DealAmount = redemptionHistoryItem.DealAmount,
                                   DealPercent = redemptionHistoryItem.DealPercent,
                                   DealCurrency = redemptionHistoryItem.DealCurrency,
                                   MinimumPurchase = redemptionHistoryItem.MinimumPurchase,
                                   MaximumDiscount = redemptionHistoryItem.MaximumDiscount,
                                   Event = redemptionHistoryItem.Event.ToString(),
                                   EventDateTime = redemptionHistoryItem.EventDateTime,
                                   EventAmount = redemptionHistoryItem.EventAmount,
                                   EventCurrency = redemptionHistoryItem.EventCurrency,
                                   Reversed = redemptionHistoryItem.Reversed,
                                   CreditStatus = redemptionHistoryItem.CreditStatus.ToString(),
                                   DiscountAmount = redemptionHistoryItem.DiscountAmount,
                                   NameOnCard = redemptionHistoryItem.NameOnCard,
                                   LastFourDigits = redemptionHistoryItem.LastFourDigits,
                                   CardExpiration = redemptionHistoryItem.CardExpiration,
                                   CardBrand = redemptionHistoryItem.CardBrand.ToString()
                               });
                }
            }

            ((GetRedemptionHistoryResponse)Context[Key.Response]).RedemptionHistory = historyItems;

            return ResultCode.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ResultCode GetEarnBurnHistory()
        {
            List<EarnBurnTransactionItemDataContract> historyItems = new List<EarnBurnTransactionItemDataContract>();

            // Retrieve list of redemption history items.
            Context.Log.Verbose("Retrieving earn/burn redemption history belonging to user {0}", ((User)Context[Key.User]).GlobalId);
            IEnumerable<RedemptionHistoryItem> items = RedemptionHistoryOperations.RetrieveMicrosoftEarnRedemptionHistory();
            Context.Log.Verbose("{0} earn/burn redemption history entries were retrieved from the data store.", items.Count());

            double totalEarnedCredits = 0;
            double totalBurnedCredits = 0;

            // If any redemption history items were returned, add them to the context.
            if (items.Any())
            {
                // Build the list of HistoryItemDataContracts from the list of redemption history items.
                foreach (RedemptionHistoryItem redemptionHistoryItem in items)
                {
                    double discountAmount = ((double)redemptionHistoryItem.DiscountAmount / 100);

                    CreditStatus creditStatus = redemptionHistoryItem.CreditStatus;
                    historyItems.Add(new EarnBurnTransactionItemDataContract()
                    {
                        RedemptionType = this.GetRedemptionTypeForEarnBurn(redemptionHistoryItem.ReimbursementTender),
                        MerchantName = redemptionHistoryItem.MerchantName,
                        DiscountSummary = redemptionHistoryItem.DiscountSummary,
                        DealPercent = redemptionHistoryItem.DealPercent,
                        EventDateTime = redemptionHistoryItem.EventDateTime,
                        EventAmount = string.Format("{0:C}", ((double)redemptionHistoryItem.EventAmount / 100)),
                        Reversed = redemptionHistoryItem.Reversed,
                        CreditStatus = creditStatus.ToString(),
                        DiscountAmount = string.Format("{0:C}", discountAmount),
                        LastFourDigits = redemptionHistoryItem.LastFourDigits,
                        CardBrand = redemptionHistoryItem.CardBrand.ToString()
                    });

                    //Consider only the settled earn transactions to calculate total earned credits
                    //For calculating burned credits, include both cleared and pending transaction
                    if (redemptionHistoryItem.ReimbursementTender == ReimbursementTender.MicrosoftEarn && creditStatus == CreditStatus.CreditGranted)
                    {
                        totalEarnedCredits += discountAmount;
                    }
                    else if (redemptionHistoryItem.ReimbursementTender == ReimbursementTender.MicrosoftBurn)
                    {
                        totalBurnedCredits += discountAmount;
                    }

                }
            }

            double availableCredits = (totalEarnedCredits > totalBurnedCredits) ? (totalEarnedCredits - totalBurnedCredits) : 0;

            GetEarnBurnTransactionHistoryResponse response = (GetEarnBurnTransactionHistoryResponse)Context[Key.Response];
            response.RedemptionHistory = historyItems;
            response.CreditBalance = string.Format("{0:C}", availableCredits);

            return ResultCode.Success;
        }

        /// <summary>
        /// Determines which redemption type to apply based on the tender,
        /// </summary>
        /// <param name="reimbursementTenderId">
        /// The tender used to determine the redemption type.
        /// </param>
        /// <returns>
        /// The redemption type for the transaction.
        /// </returns>
        private string GetRedemptionTypeForEarnBurn(ReimbursementTender reimbursementTender)
        {
            string redemptionType = null;

            if (reimbursementTender == ReimbursementTender.MicrosoftEarn)
            {
                redemptionType = EarnRedemption;
            }
            else if (reimbursementTender == ReimbursementTender.MicrosoftBurn)
            {
                redemptionType = BurnRedemption;
            }

            return redemptionType;
        }
    }
}