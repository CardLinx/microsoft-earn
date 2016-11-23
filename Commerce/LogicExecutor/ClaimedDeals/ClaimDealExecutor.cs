//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.DataContracts;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains logic necessary to execute a claim deal request.
    /// </summary>
    public class ClaimDealExecutor
    {
        /// <summary>
        /// Initializes a new instance of the ClaimDealExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        /// <param name="executionCallback">
        /// The callback method to invoke to return from execution.
        /// </param>
        public ClaimDealExecutor(CommerceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            Context = context;
        }

        /// <summary>
        /// Executes the Claim Deal API invocation.
        /// </summary>
        public async Task Execute()
        {
            try
            {
                SharedUserLogic sharedUserLogic = new SharedUserLogic(Context,
                                                                              CommerceOperationsFactory.UserOperations(Context));
                SharedCardLogic sharedCardLogic = new SharedCardLogic(Context,
                                                                              CommerceOperationsFactory.CardOperations(Context));
                SharedDealLogic sharedDealLogic = new SharedDealLogic(Context,
                                                                              CommerceOperationsFactory.DealOperations(Context));
                ClaimDealConcluder claimDealConcluder = new ClaimDealConcluder(Context);

                ResultCode extractionResult = ExtractClaimDealPayload();
                if (extractionResult == ResultCode.Success)
                {
                    Deal deal  = sharedDealLogic.RetrieveDeal();
                    Context[Key.Deal] = deal;
                    if (deal != null)
                    {
                        Context[Key.DealDiscountSummary] = deal.DiscountSummary;
                        User user =  sharedUserLogic.RetrieveUser();
                        Context[Key.User] = user;
                        if (user != null)
                        {
                            Card card = sharedCardLogic.RetrieveCard();
                            Context[Key.Card] = card;
                            if (card != null)
                            {
                                // If the deal and card have common ground, claim the deal for the card.
                                if (MustClaim(deal, card) == true)
                                {
                                    Context[Key.MerchantName] = deal.MerchantName;
                                    Context.Log.Verbose("Trying to claim the deal for the user with the appropriate partner.");
                                    ClaimDealInvoker claimDealInvoker = new ClaimDealInvoker(Context);
                                    await claimDealInvoker.Invoke();
                                }
                                else
                                {
                                    Context.Log.Verbose("It is not necessary to claim this deal for this card.");
                                    ((ResultSummary)Context[Key.ResultSummary]).SetResultCode(ResultCode.Success);
                                    RestResponder.BuildAsynchronousResponse(Context);
                                }
                            }
                            else
                            {
                                claimDealConcluder.Conclude(ResultCode.UnregisteredCard);
                            }
                        }
                        else
                        {
                            claimDealConcluder.Conclude(ResultCode.UnexpectedUnregisteredUser);
                        }
                    }
                    else
                    {
                        claimDealConcluder.Conclude(ResultCode.UnactionableDealId);
                    }
                }
                else
                {
                    claimDealConcluder.Conclude(extractionResult);
                }
            }
            catch (Exception ex)
            {
                ((ResultSummary)Context[Key.ResultSummary]).SetResultCode(ResultCode.UnknownError);
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Determines whether the deal must be claimed for the card.
        /// </summary>
        /// <param name="deal">
        /// The deal that may need to be claimed.
        /// </param>
        /// <param name="card">
        /// The card for which the deal may need to be claimed.
        /// </param>
        /// <returns>
        /// * True if the deal must be claimed for the card.
        /// * Else returns false.
        /// </returns>
        private bool MustClaim(Deal deal,
                               Card card)
        {
            bool result = true;

            // A deal only has to be claimed for the card if
            // the deal is registered with First Data,
            result = deal.PartnerDealInfoList.Any(info => info.PartnerId == Partner.FirstData);

            if (result == true)
            {
                // the card has been registered with First Data,
                result = card.PartnerCardInfoList.Any(info => info.PartnerId == Partner.FirstData);
            }

            if (result == true)
            {
                // the deal pays back currency, and
                if (deal.ReimbursementTender == ReimbursementTender.DeprecatedEarn)
                {
                    // the card is in the CLO offers program.
                    result = (card.RewardPrograms & RewardPrograms.CardLinkOffers) == RewardPrograms.CardLinkOffers;
                }
                // or the deal burns earned credits, and
                else if (deal.ReimbursementTender == ReimbursementTender.MicrosoftBurn)
                {
                    // the card is in the EarnBurn program.
                    result = (card.RewardPrograms & RewardPrograms.EarnBurn) == RewardPrograms.EarnBurn;
                }
                // Otherwise, the card and deal are not in compatible programs, so no claim should be made.
                else
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts required elements from the ClaimDealPayload.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode ExtractClaimDealPayload()
        {
            ResultCode result = ResultCode.Success;

            Context.Log.Verbose("Extracting ClaimDealPayload.");
            ClaimDealPayload claimDealPayload = Context[Key.ClaimDealPayload] as ClaimDealPayload;
            if (claimDealPayload != null)
            {
                if (claimDealPayload.UserId != Guid.Empty)
                {
                    Context[Key.GlobalUserId] = claimDealPayload.UserId;

                    if (claimDealPayload.ClaimDealInfo != null)
                    {
                        Context[Key.GlobalDealId] = claimDealPayload.ClaimDealInfo.DealId;
                        Context[Key.CardId] = claimDealPayload.ClaimDealInfo.CardId;
                    }
                    else
                    {
                        Context.Log.Verbose("ClaimDealInfo is null.");
                        result = ResultCode.ParameterCannotBeNull;
                    }
                }
                else
                {
                    Context.Log.Verbose("UserId is Guid.Empty.");
                    result = ResultCode.InvalidParameter;
                }
            }
            else
            {
                Context.Log.Verbose("ClaimDealPayload object is null.");
                result = ResultCode.ParameterCannotBeNull;
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}