//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Operation interface implementation for the First Data partner.
    /// </summary>
    public class FirstData : ICommercePartner
    {
        /// <summary>
        /// Initializes a new instance of the FirstData class.
        /// </summary>
        /// <param name="context">
        /// The context of the First Data operation.
        /// </param>
        public FirstData(CommerceContext context)
        {
            Context = context;
            FirstDataInvoker = FirstDataInvokerFactory.BuildFirstDataInvoker(Context.PerformanceInformation,
                                                                             FirstDataInvokerOverride);
        }

        /// <summary>
        /// Adds the card in the context for the user in the context to this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        public async Task<ResultCode> AddCardAsync()
        {
            ResultCode result = ResultCode.None;

            // Build a card register request object.
            CardRegisterRequest cardRegisterRequest = new CardRegisterRequest
            {
                provID = FirstDataConstants.MicrosoftProviderId,
                pubID = FirstDataConstants.MicrosoftPubId,
                pubName = FirstDataConstants.MicrosoftPubName,
                reqType = CardRegisterRequestReqType.A, // "A" means "Add" request type.
                consumerID = ((User)Context[Key.User]).GetPartnerUserId(Partner.FirstData),
                PAN = ((NewCardInfo)Context[Key.NewCardInfo]).Number
            };
            LogRegisterCardRequestParameters(cardRegisterRequest, "Add");

            // Invoke the partner to add the card.
            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                Context.Log.Verbose("Invoking partner AddCard API.");
                CardRegisterResponse response = await FirstDataInvoker.AddCard(cardRegisterRequest);
                Context.Log.Verbose("Partner AddCard API returned: {0}: {1}.", response.respCode, response.respText);
                LogRegisterCardResponseParameters(response, "Add");

                // Determine the ResultCode from the response code.
                switch (response.respCode)
                {
                    case FirstDataResponseCode.AddCardSuccess:
                    case FirstDataResponseCode.CardAlreadyExists:
                        result = ResultCode.Created;
                        PartnerCardInfo partnerCardInfo = GetFirstDataCardInfo((Card)Context[Key.Card]);
                        partnerCardInfo.PartnerCardId = response.token;
                        partnerCardInfo.PartnerCardSuffix = response.cardSuffix;
                        break;
                    case FirstDataResponseCode.CouldNotGenerateCardToken:
                        result = ResultCode.InvalidCard;
                        break;
                    case FirstDataResponseCode.AddCardFieldParseError:
                    case FirstDataResponseCode.UnknownAddCardError:
                        result = ResultCode.UnknownError;
                        break;
                }

                // Log a warning if result was not a success.
                if (result != ResultCode.Created)
                {
                    Context.Log.Warning("First Data call failed. respCode: {0}. respText: {1}.",
                                        (int)DefaultLogEntryEventId.PartnerErrorWarning, response.respCode, response.respText);
                }

                return result;
            }, null, Partner.FirstData);

            return result;
        }

        /// <summary>
        /// Removes the card in the context for the user in the context from this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        public async Task<ResultCode> RemoveCardAsync()
        {
            ResultCode result = ResultCode.None;

            // Build a card register request object.
            CardRegisterRequest cardRegisterRequest = new CardRegisterRequest
            {
                provID = FirstDataConstants.MicrosoftProviderId,
                pubID = FirstDataConstants.MicrosoftPubId,
                pubName = FirstDataConstants.MicrosoftPubName,
                reqType = CardRegisterRequestReqType.D, // "D" means "Delete" request type.
                consumerID = ((User)Context[Key.User]).GetPartnerUserId(Partner.FirstData),
//TODO: IF we decide to delete the card in First Data at all, switch to new First Data WSDL and send consumerID / token instead of PAN.
//TODO: If we decide not to delete the card in First Data, remove all relevant code and revert DeactivateCard to a synchronous op.
                PAN = ((Card)Context[Key.Card]).LastFourDigits
            };
            LogRegisterCardRequestParameters(cardRegisterRequest, "Remove");

            // Invoke the partner to remove the card.
            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                Context.Log.Verbose("Invoking partner RemoveCardAsync API.");
                CardRegisterResponse response = await FirstDataInvoker.RemoveCard(cardRegisterRequest);
                Context.Log.Verbose("Partner RemoveCardAsync API returned: {0}: {1}.", response.respCode, response.respText);
                LogRegisterCardResponseParameters(response, "Remove");

                // Determine the ResultCode from the response code.
                switch (response.respCode)
                {
                    case FirstDataResponseCode.RemoveCardSuccess:
                    case FirstDataResponseCode.CardDoesNotExist:
//TODO: If the above is done, afterward move RemoveCardFieldParseError back to UnknownError. For now, this can never succeed.
                    case FirstDataResponseCode.RemoveCardFieldParseError:
                        result = ResultCode.Success;
                        break;
                    case FirstDataResponseCode.UnknownRemoveCardError:
                        result = ResultCode.UnknownError;
                        break;
                }

                // Log a warning if result was not a success.
                if (result != ResultCode.Success)
                {
                    Context.Log.Warning("First Data call failed. respCode: {0}. respText: {1}.",
                                        (int)DefaultLogEntryEventId.PartnerErrorWarning, response.respCode, response.respText);
                }

                return result;
            }, null, Partner.FirstData);

            return result;
        }

        /// <summary>
        /// Registers the deal in the context with this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        public async Task<ResultCode> RegisterDealAsync()
        {
            ResultCode result = ResultCode.None;

            // Get the PartnerDealInfo for First Data.
            Deal deal = (Deal)Context[Key.Deal];
            PartnerDealInfo firstDataDealInfo = deal.PartnerDealInfoList.SingleOrDefault(partnerDealInfo =>
                                                                                 partnerDealInfo.PartnerId == Partner.FirstData);

            if (firstDataDealInfo != null)
            {

                // Build the register offer request offer object.
                OfferRegisterRequestOffer offerRegisterRequestOffer = new OfferRegisterRequestOffer
                    {
                        BECode = FirstDataConstants.BECode,
                        startDateTime = FirstDataUtilities.GenerateDateTimeString(deal.StartDate),
                        endDateTime = FirstDataUtilities.GenerateDateTimeString(deal.EndDate),
                        minTxnAmt = deal.MinimumPurchase.ToString(),
                        maxRedCount = deal.Count,
                        maxRedCountSpecified = true, // Must always be set to true.
                        offerMode = FirstDataConstants.SettlementBasedRedemptionMode
                    };
                offerRegisterRequestOffer.MIDGroup = new string[firstDataDealInfo.PartnerMerchantLocations.Count];
                for (int ct = 0; ct < firstDataDealInfo.PartnerMerchantLocations.Count; ct++)
                {
                    offerRegisterRequestOffer.MIDGroup[ct] = firstDataDealInfo.PartnerMerchantLocations[ct].PartnerMerchantId;
                }
                if (deal.Percent > 0)
                {
                    offerRegisterRequestOffer.discountPercentage = deal.Percent;
                    offerRegisterRequestOffer.discountPercentageSpecified = true;
                }
                else
                {
                    offerRegisterRequestOffer.discountAmt = deal.Amount.ToString();
                }

                // Update previously registered flag if registration hasn't yet completed.
                bool previouslyRegistered = (bool)Context[Key.PreviouslyRegistered];
                if (firstDataDealInfo.PartnerDealRegistrationStatusId != PartnerDealRegistrationStatus.Complete)
                {
                    previouslyRegistered = false;
                    Context[Key.PreviouslyRegistered] = false;
                }

                // Build the register offer request object.
                OfferRegisterRequest offerRegisterRequest = new OfferRegisterRequest
                    {
                        provID = FirstDataConstants.MicrosoftProviderId,
                        pubID = FirstDataConstants.MicrosoftPubId,
                        pubName = FirstDataConstants.MicrosoftPubName,
                        offer = offerRegisterRequestOffer
                    };

                // Use the existing partner deal ID or generate one if needed.
                offerRegisterRequest.offerID = firstDataDealInfo.PartnerDealId;
                if (String.IsNullOrWhiteSpace(offerRegisterRequest.offerID) == true)
                {
                    offerRegisterRequest.offerID = General.GenerateShortGuid();
                }

                if (previouslyRegistered == false)
                {
                    offerRegisterRequest.reqType = OfferRegisterRequestReqType.O; // "O" means "Register Offer" request type.
                    offerRegisterRequestOffer.offerType = FirstDataConstants.OffTypeConsumerClipped;
                    offerRegisterRequestOffer.offerTypeSpecified = true;
                }
                else
                {
                    offerRegisterRequest.reqType = OfferRegisterRequestReqType.U; // "U" means "Update Offer" request type.
                }
                LogRegisterDealRequestParameters(offerRegisterRequest, "Register");

                // Invoke the partner to register the deal.
                result = await PartnerUtilities.InvokePartner(Context, async () =>
                    {
                        Context.Log.Verbose("Invoking partner RegisterDeal API.");
                        OfferRegisterResponse response = await FirstDataInvoker.RegisterDeal(offerRegisterRequest);
                        Context.Log.Verbose("Partner RegisterDeal API returned: {0}: {1}.", response.respCode,
                                            response.respText);
                        LogRegisterOfferResponseParameters(response, "Register");

                        // Determine the ResultCode from the response code.
                        switch (response.respCode)
                        {
                            case FirstDataResponseCode.RegisterDealSuccess:
                            case FirstDataResponseCode.RegisterDealPartialSuccess:
                            case FirstDataResponseCode.DealAlreadyRegistered:
                                result = ResultCode.Success;
                                firstDataDealInfo.PartnerDealId = response.offerID;
                                firstDataDealInfo.PartnerDealRegistrationStatusId =
                                    PartnerDealRegistrationStatus.Complete;
                                break;
                            case FirstDataResponseCode.CouldNotGenerateDealToken:
                            case FirstDataResponseCode.NoValidMerchantIds:
                            case FirstDataResponseCode.RegisterDealFieldParseError:
                            case FirstDataResponseCode.UnknownRegisterDealError:
                                firstDataDealInfo.PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Error;
                                result = ResultCode.UnknownError;
                                break;
                        }

                        // Log a warning if result was not a success.
                        if (result != ResultCode.Success)
                        {
                            Context.Log.Warning("First Data call failed. respCode: {0}. respText: {1}.",
                                                (int) DefaultLogEntryEventId.PartnerErrorWarning, response.respCode,
                                                response.respText);
                        }

                        return result;
                    }, null, Partner.FirstData);
            }

            return result;
        }

        /// <summary>
        /// Claims the deal in the context for redemption with the card in the context with this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        public async Task<ResultCode> ClaimDealAsync()
        {
            ResultCode result = ResultCode.None;

            // Get the PartnerCardInfo for First Data.
            Card card = (Card)Context[Key.Card];
            if (card.Id == 0)
            {
                throw new InvalidOperationException("Unexpected value in card.Id");
            }

            PartnerCardInfo firstDataCardInfo = card.PartnerCardInfoList.SingleOrDefault(partnerCardInfo =>
                                                                                 partnerCardInfo.PartnerId == Partner.FirstData);
            if (firstDataCardInfo == null)
            {
                result = ResultCode.UnregisteredCard;
            }

            // Get the PartnerDealInfo for First Data.
            Deal deal = (Deal)Context[Key.Deal];
            if (deal.Id == 0)
            {
                throw new InvalidOperationException("Unexpected value in deal.Id");
            }

            PartnerDealInfo firstDataDealInfo = deal.PartnerDealInfoList.SingleOrDefault(partnerDealInfo =>
                                                                                 partnerDealInfo.PartnerId == Partner.FirstData);
            if (firstDataDealInfo == null)
            {
                result = ResultCode.UnregisteredDeal;
            }

            // Only proceed if First Data has both the card and the deal registered.
            if (result == ResultCode.None)
            {
                User user = (User)Context[Key.User];

                // Build the claim offer request consumer object.
                OfferRegisterRequestConsumer offerRegisterRequestConsumer = new OfferRegisterRequestConsumer
                {
                    consumerID = user.GetPartnerUserId(Partner.FirstData),
                    tokenType = FirstDataConstants.TokenType,
                    token = firstDataCardInfo.PartnerCardId,
                    purDateTime = FirstDataUtilities.GenerateDateTimeString(DateTime.UtcNow),
                    offerAcceptID = General.TwoIntegersToHexString(card.Id, deal.Id),
                    currCode = FirstDataUtilities.GetCurrencyNumberCode(deal.Currency).ToString()
                };

                // Build the claim offer request object.
                OfferRegisterRequest offerRegisterRequest = new OfferRegisterRequest
                {
                    provID = FirstDataConstants.MicrosoftProviderId,
                    pubID = FirstDataConstants.MicrosoftPubId,
                    pubName = FirstDataConstants.MicrosoftPubName,
                    reqType = OfferRegisterRequestReqType.A, // "A" means user "Accepted" the offer.
                    offerID = firstDataDealInfo.PartnerDealId,
                    consumer = offerRegisterRequestConsumer
                };
                LogClaimDealRequestParameters(offerRegisterRequest);

                // Invoke the partner to claim the deal.
                result = await PartnerUtilities.InvokePartner(Context, async () =>
                {
                    Context.Log.Verbose("Invoking partner ClaimDealAsync API.");
                    OfferRegisterResponse response = await FirstDataInvoker.ClaimDeal(offerRegisterRequest);
                    Context.Log.Verbose("Partner ClaimDealAsync API returned {0}: {1}.", response.respCode, response.respText);
                    LogRegisterOfferResponseParameters(response, "Claim");

                    // Determine the ResultCode from the response code.
                    switch (response.respCode)
                    {
                        case FirstDataResponseCode.ClaimDealSuccess:
                        case FirstDataResponseCode.DealAlreadyClaimed:
                            result = ResultCode.Success;
                            Context[Key.ClaimedDeal] = new ClaimedDeal()
                            {
                                CardId = card.Id,
                                GlobalDealId = deal.GlobalId,
                                GlobalUserId = user.GlobalId,
                                Partner = Partner.FirstData
                            };
                            break;
                        case FirstDataResponseCode.DealExpired:
                            result = ResultCode.PartnerDealExpired;
                            Context.Log.Warning("Unable to claim deal {0} because it has expired in First Data's system.",
                                                (int)result, deal.GlobalId);
                            break;
                        case FirstDataResponseCode.ClaimDealFieldParseError:
                        case FirstDataResponseCode.UnknownClaimDealError:
                            result = ResultCode.UnknownError;
                            break;
                    }

                    // Log a warning if result was not a success.
                    if (result != ResultCode.Success)
                    {
                        Context.Log.Warning("First Data call failed. respCode: {0}. respText: {1}.",
                                        (int)DefaultLogEntryEventId.PartnerErrorWarning, response.respCode, response.respText);
                    }

                    return result;
                }, null, Partner.FirstData);
            }

            return result;
        }

        /// <summary>
        /// Marshals information from the First Data redeem deal request into the RedeemedDeal object in the context.
        /// </summary>
        public void MarshalRedeemDeal()
        {
            pubRedemptionRequest request = (pubRedemptionRequest)Context[Key.Request];
            LogRedemptionRequestParameters(request);

            // Populate the RedeemedDeal.
            RedeemedDeal redeemedDeal = (RedeemedDeal)Context[Key.RedeemedDeal];
            redeemedDeal.CallbackEvent = (RedemptionEvent)request.redemptionType;
            redeemedDeal.PurchaseDateTime = FirstDataUtilities.ParseDateTimeString(request.dateTime);
            redeemedDeal.AuthorizationAmount = request.authAmt;
            redeemedDeal.Currency = request.currCode;

            // Populate the FirstData RedeemedDeal info.
            Context[Key.PartnerDealId] = request.offerID;
            Context[Key.PartnerCardId] = request.token;
            AssignPartnerClaimedDealId(request.offerAcceptID);
            Context[Key.PartnerMerchantId] = request.offerMID;
            Context[Key.OutletPartnerMerchantId] = request.MID;
            if (String.Equals(FirstDataConstants.PinDebitTenderType, request.tenderType,
                              StringComparison.OrdinalIgnoreCase) == true)
            {
                Context[Key.DisallowedReason] = request.tenderType;
            }
        }

        /// <summary>
        /// Builds the response to a redeem deal request.
        /// </summary>
        public void BuildRedeemedDealResponse()
        {
            pubRedemptionResponse response = (pubRedemptionResponse)Context[Key.Response];
            pubRedemptionRequest request = (pubRedemptionRequest)Context[Key.Request];
            response.reqID = request.reqID;
            response.provID = FirstDataConstants.MicrosoftProviderId;
            ResultCode resultCode = (ResultCode)Context[Key.ResultCode];
            RedeemedDealInfo redeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];

            switch (resultCode)
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Approve offer redemption.
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case ResultCode.Success:
                case ResultCode.Created:
                    if (redeemedDealInfo == null)
                    {
                        throw new InvalidOperationException("Parameter redeemedDealInfo cannot be null when parameter " +
                                                            "resultCode is ResultCode.Success or ResultCode.Created.");
                    }

                    response.respCode = FirstDataResponseCode.Success;
                    AppendPubAndOfferInfo(request, response, redeemedDealInfo);
                    AppendDealValuationInfo(response, redeemedDealInfo);

                    if (response.offerID == request.offerID)
                    {
                        response.reasonCode = FirstDataReasonCode.ApprovedRetain;
                    }
                    else
                    {
                        response.reasonCode = FirstDataReasonCode.UseAlternateOfferThisTime;
                    }
                    break;

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Specified deal does not exist. Unlink the offer from the card.
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case ResultCode.DealNotFound:
                    response.respCode = FirstDataResponseCode.Success;
                    response.reasonCode = FirstDataReasonCode.RedeemDealDeclinedDelete;
                    AppendPubAndOfferInfo(request, response, redeemedDealInfo);
                    break;

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Offer cannot be redeemed yet / for this particular transaction. Leave the offer linked to the card.
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case ResultCode.CardDeactivated:
                case ResultCode.DealNotYetValid:
                case ResultCode.DealThresholdNotMet:
                case ResultCode.NoApplicableDealFound:
                case ResultCode.TransactionDisallowed:
                    response.respCode = FirstDataResponseCode.Success;
                    response.reasonCode = FirstDataReasonCode.RedeemDealDeclinedRetain;
                    AppendPubAndOfferInfo(request, response, redeemedDealInfo);
                    break;

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Malformed redemption request.
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case ResultCode.InvalidPurchaseDateTime:
                    response.respCode = FirstDataResponseCode.RequestContainsBadData;
                    response.reasonCode = FirstDataReasonCode.RedeemDealInvalidPurchaseDateTime;
                    break;
                case ResultCode.PartnerCardIdNotFound:
                    response.respCode = FirstDataResponseCode.RequestContainsBadData;
                    response.reasonCode = FirstDataReasonCode.RedeemDealInvalidCard;
                    break;
                case ResultCode.InvalidPartnerMerchantId:
                    response.respCode = FirstDataResponseCode.RequestContainsBadData;
                    response.reasonCode = FirstDataReasonCode.RedeemDealOfferMidMismatch;
                    break;
            }

            LogRedemptionResponseParameters(response);
        }

        /// <summary>
        /// Marshals information from the First Data reverse redeemed deal request into the context.
        /// </summary>
        public void MarshalReverseRedeemedDeal()
        {
            pubReversalRequest request = (pubReversalRequest)Context[Key.Request];
            LogReversalRequestParameters(request);

            // Populate the context.
            Context[Key.Partner] = Partner.FirstData;
            Context[Key.PartnerRedeemedDealId] = request.rdmptTranID;
            Context[Key.RedemptionEvent] = (RedemptionEvent)request.redemptionType;
            Context[Key.PartnerDealId] = request.offerID;
            Context[Key.PartnerCardId] = request.token;
            Context[Key.PartnerMerchantId] = request.MID;
        }

        /// <summary>
        /// Builds the response to a reverse redeemed deal request.
        /// </summary>
        public void BuildReverseRedeemedDealResponse()
        {
            pubReversalResponse response = (pubReversalResponse)Context[Key.Response];
            pubReversalRequest request = (pubReversalRequest)Context[Key.Request];
            ReverseRedeemedDealInfo reverseRedeemedDealInfo = (ReverseRedeemedDealInfo)Context[Key.ReverseRedeemedDealInfo];
            ResultCode resultCode = (ResultCode)Context[Key.ResultCode];
            response.reqID = request.reqID;
            response.provID = FirstDataConstants.MicrosoftProviderId;
            response.rdmptTranID = request.rdmptTranID;

            switch (resultCode)
            {
                case ResultCode.Success:
                case ResultCode.Created:
                    if (reverseRedeemedDealInfo == null)
                    {
                        throw new InvalidOperationException("Parameter reverseRedeemedDealInfo cannot be null when parameter " +
                                                            "resultCode is ResultCode.Success or ResultCode.Created.");
                    } 

                    response.respCode = FirstDataResponseCode.Success;
                    response.revAmt = reverseRedeemedDealInfo.DiscountAmount;
                    response.revAmtSpecified = true;
//TODO: Log warning if FD's reversal amount doesn't match.
                    break;
                case ResultCode.RedeemedDealNotFound:
                    response.respCode = FirstDataResponseCode.Success;
                    response.reasonCode = FirstDataReasonCode.RedeemedDealNotFound;
                    break;
                case ResultCode.RedemptionEventMismatch:
                    response.respCode = FirstDataResponseCode.RequestContainsBadData;
                    response.reasonCode = FirstDataReasonCode.RedemptionTypeMismatch;
                    break;
                case ResultCode.PartnerMismatch:
                    response.respCode = FirstDataResponseCode.RequestMissingData;
                    response.reasonCode = FirstDataReasonCode.UnrecognizedReversalAction;
                    break;
                case ResultCode.PartnerDealIdMismatch:
                    response.respCode = FirstDataResponseCode.RequestMissingData;
                    response.reasonCode = FirstDataReasonCode.NoValidOfferIdSpecified;
                    break;
                case ResultCode.PartnerCardIdMismatch:
                    response.respCode = FirstDataResponseCode.RequestContainsBadData;
                    response.reasonCode = FirstDataReasonCode.PartnerCardIdMismatch;
                    break;
                case ResultCode.InvalidPartnerMerchantId:
                    response.respCode = FirstDataResponseCode.RequestContainsBadData;
                    response.reasonCode = FirstDataReasonCode.PartnerMerchantIdMismatch;
                    break;
            }

            LogReversalResponseParameters(response);
        }

        /// <summary>
        /// Marshals information from the First Data reverse timed out redemption request into the context.
        /// </summary>
        public void MarshalRedemptionTimeout()
        {
            pubReversalRequest request = (pubReversalRequest)Context[Key.Request];
            LogReversalRequestParameters(request);

            // Populate the context.
            Context[Key.Partner] = Partner.FirstData;
            Context[Key.RedemptionEvent] = (RedemptionEvent)request.redemptionType;
            AssignPartnerClaimedDealId(request.offerAcceptID);
            Context[Key.PartnerDealId] = request.offerID;
            Context[Key.AuthorizationAmount] = request.authAmt;
            Context[Key.PartnerCardId] = request.token;
            Context[Key.PartnerMerchantId] = request.MID;
            Context[Key.PartnerOfferMerchantId] = request.offerMID;
            Context[Key.PurchaseDateTime] = FirstDataUtilities.ParseDateTimeString(request.dateTime);
        }

        /// <summary>
        /// Builds the response to a reverse timed out redemption request.
        /// </summary>
        public void BuildRedemptionTimeoutResponse()
        {
            pubReversalRequest request = (pubReversalRequest)Context[Key.Request];
            pubReversalResponse response = (pubReversalResponse)Context[Key.Response];
            ReverseRedeemedDealInfo reverseRedeemedDealInfo = (ReverseRedeemedDealInfo)Context[Key.ReverseRedeemedDealInfo];
            ResultCode resultCode = (ResultCode)Context[Key.ResultCode];
            response.reqID = request.reqID;
            response.provID = FirstDataConstants.MicrosoftProviderId;

            switch (resultCode)
            {
                case ResultCode.Success:
                case ResultCode.Created:
                    if (reverseRedeemedDealInfo == null)
                    {
                        throw new InvalidOperationException("Parameter reverseRedeemedDealInfo cannot be null when parameter " +
                                                            "resultCode is ResultCode.Success or ResultCode.Created.");
                    } 

                    response.respCode = FirstDataResponseCode.Success;
                    response.revAmt = reverseRedeemedDealInfo.DiscountAmount;
                    response.revAmtSpecified = true;
                    response.rdmptTranID = reverseRedeemedDealInfo.PartnerRedeemedDealId;
//TODO: Log warning if FD's reversal amount doesn't match.
                    break;
                case ResultCode.MatchingRedeemedDealNotFound:
                    response.respCode = FirstDataResponseCode.Success;
                    response.reasonCode = FirstDataReasonCode.RedeemedDealNotFound;
                    break;
                case ResultCode.MultipleMatchingRedeemedDealsFound:
                    response.respCode = FirstDataResponseCode.RequestMissingData;
                    response.reasonCode = FirstDataReasonCode.UnrecognizedReversalAction;
                    break;
                case ResultCode.InvalidPartnerMerchantId:
                    response.respCode = FirstDataResponseCode.RequestContainsBadData;
                    response.reasonCode = FirstDataReasonCode.PartnerMerchantIdMismatch;
                    break;
            }

            LogReversalResponseParameters(response);
        }

        /// <summary>
        /// Responds to a ping request from First Data.
        /// </summary>
        public void BuildPingResponse()
        {
            pubPingRequest request = (pubPingRequest)Context[Key.Request];
            pubPingResponse response = (pubPingResponse)Context[Key.Response];
            response.reqID = request.reqID;
            response.respCode = FirstDataResponseCode.Success;
        }

        /// <summary>
        /// Gets or sets the context of the FirstData operation.
        /// </summary>
        public CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the object through which the First Data API is invoked.
        /// </summary>
        internal IFirstDataInvoker FirstDataInvoker { get; set; }

        /// <summary>
        /// Overrides creation of the FirstDataInvoker object to use a specific object.
        /// </summary>
        /// <remarks>
        /// This should only be set for testing purposes.
        /// </remarks>
        internal static IFirstDataInvoker FirstDataInvokerOverride { get; set; }


        /// <summary>
        /// Assigns context keys related to partner claimed deal id 
        /// </summary>
        /// <param name="partnerClaimedDealId">
        /// partner claimed deal id
        /// </param>
        private void AssignPartnerClaimedDealId(string partnerClaimedDealId)
        {
            Context[Key.PartnerClaimedDealId] = partnerClaimedDealId;
            Context[Key.CardId] = 0;
            Context[Key.DealId] = 0;

            // try parse card and deal components
            Tuple<int, int> claimedDeal = General.HexStringToTwoIntegers(partnerClaimedDealId);
            if (claimedDeal != null)
            {
                Context[Key.CardId] = claimedDeal.Item1;
                Context[Key.DealId] = claimedDeal.Item2;
            }
        }

        /// <summary>
        /// Retrieves the FirstData PartnerCardInfo object, creating one if necessary.
        /// </summary>
        /// <param name="card">
        /// The Card whose FirstData PartnerCardInfo object to retrieve.
        /// </param>
        /// <returns>
        /// The FirstData PartnerCardInfo object.
        /// </returns>
        private static PartnerCardInfo GetFirstDataCardInfo(Card card)
        {
            // Get the PartnerCardInfo for FirstData.
            PartnerCardInfo result = card.PartnerCardInfoList.SingleOrDefault(partnerCardInfo =>
                                                                                 partnerCardInfo.PartnerId == Partner.FirstData);

            // If no FirstData PartnerCardInfo existed, add one.
            if (result == null)
            {
                result = new PartnerCardInfo()
                {
                    PartnerId = Partner.FirstData
                };
                card.PartnerCardInfoList.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Append publisher and offer information to the pubRedemption response.
        /// <param name="request">
        /// The request describing the redemption of a deal.
        /// </param>
        /// <param name="response">
        /// The pubRedemptionResponse to populate with the response for the deal redemption event.
        /// </param>
        /// <param name="redeemedDealInfo">
        /// Information on the deal being redeemed.
        /// </param>
        private static void AppendPubAndOfferInfo(pubRedemptionRequest request,
                                                  pubRedemptionResponse response,
                                                  RedeemedDealInfo redeemedDealInfo)
        {
            response.pubID = FirstDataConstants.MicrosoftPubId;
            response.pubName = FirstDataConstants.MicrosoftPubName;
            if (String.IsNullOrWhiteSpace(redeemedDealInfo.PartnerDealId) == false)
            {
                response.offerID = redeemedDealInfo.PartnerDealId;
            }
            else
            {
                response.offerID = request.offerID;
            }

            if (String.IsNullOrWhiteSpace(redeemedDealInfo.PartnerClaimedDealId) == false)
            {
                response.offerAcceptID = redeemedDealInfo.PartnerClaimedDealId;
            }
            else
            {
                response.offerAcceptID = request.offerAcceptID;
            }
        }

        /// <summary>
        /// Appends deal valuation info to the pubRedemption response.
        /// </summary>
        /// <param name="response">
        /// The pubRedemptionResponse to populate with the response for the deal redemption event.
        /// </param>
        /// <param name="redeemedDealInfo">
        /// Information on the deal being redeemed.
        /// </param>
        private static void AppendDealValuationInfo(pubRedemptionResponse response,
                                                    RedeemedDealInfo redeemedDealInfo)
        {
//TODO: When we expand to other currencies, hard coded symbol, division, precision, etc., will all vary by currency. Localization may also be a concern.
            response.redemptionAmt = redeemedDealInfo.DiscountAmount;
            double actualSavings = (double)response.redemptionAmt / 100;
            string offeredSavings;
            if (redeemedDealInfo.Percent > 0)
            {
                offeredSavings = String.Format("{0}%", redeemedDealInfo.Percent.ToString("###.##"));
            }
            else
            {
                offeredSavings = String.Format("${0:F2}", actualSavings);
            }

            redeemedDealInfo.DiscountText = String.Format("${0:F2}", actualSavings);
            response.offerDesc = String.Format("Save {0} on purchases of ${1:F2} or more.", offeredSavings,
                                               (double)redeemedDealInfo.MinimumPurchase / 100);
            response.receiptTxt = String.Format("You saved {0} with Bing Offers!", redeemedDealInfo.DiscountText);
            response.redemptionAmtSpecified = true;
            response.rdmptTranID = redeemedDealInfo.PartnerRedeemedDealId;
            response.consumerID = redeemedDealInfo.PartnerUserId;
        }

        /// <summary>
        /// Logs the register card request parameters sent to First Data.
        /// </summary>
        /// <param name="request">
        /// The request whose parameters to log.
        /// </param>
        /// <param name="callType">
        /// The type of the call to specify in the log.
        /// </param>
        private void LogRegisterCardRequestParameters(CardRegisterRequest request,
                                                      string callType)
        {
            StringBuilder parameters = new StringBuilder("consumerID: ");
            parameters.Append(request.consumerID);
            parameters.Append("\r\n");
            parameters.Append("provID: ");
            parameters.Append(request.provID);
            parameters.Append("\r\n");
            parameters.Append("pubID: ");
            parameters.Append(request.pubID);
            parameters.Append("\r\n");
            parameters.Append("pubName: ");
            parameters.Append(request.pubName);
            parameters.Append("\r\n");
            parameters.Append("reqType: ");
            parameters.Append(request.reqType);
            parameters.Append("\r\n");
            parameters.Append("coalitionList: ");
            parameters.Append(request.coalitionList);
            parameters.Append("\r\n");
            parameters.Append("encryptPAN: ");
            parameters.Append(request.encryptPAN);
            parameters.Append("\r\n");
            parameters.Append("keyID: ");
            parameters.Append(request.keyID);
            Context.Log.Verbose("First Data {0} Card request parameters:\r\n{1}", callType, parameters.ToString());
        }

        /// <summary>
        /// Logs the register card response parameters received from First Data.
        /// </summary>
        /// <param name="response">
        /// The response whose parameters to log.
        /// </param>
        /// <param name="callType">
        /// The type of the call to specify in the log.
        /// </param>
        private void LogRegisterCardResponseParameters(CardRegisterResponse response,
                                                       string callType)
        {
            StringBuilder parameters = new StringBuilder("cardSuffix: ");
            parameters.Append(response.cardSuffix);
            parameters.Append("\r\n");
            parameters.Append("consumerID: ");
            parameters.Append(response.consumerID);
            parameters.Append("\r\n");
            parameters.Append("provID: ");
            parameters.Append(response.provID);
            parameters.Append("\r\n");
            parameters.Append("reqType: ");
            parameters.Append(response.reqType);
            parameters.Append("\r\n");
            parameters.Append("respCode: ");
            parameters.Append(response.respCode);
            parameters.Append("\r\n");
            parameters.Append("respText: ");
            parameters.Append(response.respText);
            parameters.Append("\r\n");
            parameters.Append("token: ");
            parameters.Append(response.token);
            parameters.Append("\r\n");
            parameters.Append("tokenType: ");
            parameters.Append(response.tokenType);
            parameters.Append("\r\n");
            parameters.Append("errorCoalitionList: ");
            parameters.Append(response.errorCoalitionList);
            Context.Log.Verbose("First Data {0} Card response parameters:\r\n{1}", callType, parameters.ToString());
        }

        /// <summary>
        /// Logs the register deal request parameters sent to First Data.
        /// </summary>
        /// <param name="request">
        /// The request whose parameters to log.
        /// </param>
        /// <param name="callType">
        /// The type of the call to specify in the log.
        /// </param>
        private void LogRegisterDealRequestParameters(OfferRegisterRequest request,
                                                      string callType)
        {
            StringBuilder parameters = new StringBuilder("BECode: ");
            parameters.Append(request.offer.BECode);
            parameters.Append("\r\n");
            parameters.Append("discountAmt: ");
            parameters.Append(request.offer.discountAmt);
            parameters.Append("\r\n");
            parameters.Append("discountPercentage: ");
            parameters.Append(request.offer.discountPercentage);
            parameters.Append("\r\n");
            parameters.Append("discountPercentageSpecified: ");
            parameters.Append(request.offer.discountPercentageSpecified);
            parameters.Append("\r\n");
            parameters.Append("endDateTime: ");
            parameters.Append(request.offer.endDateTime);
            parameters.Append("\r\n");
            parameters.Append("maxRedCount: ");
            parameters.Append(request.offer.maxRedCount);
            parameters.Append("\r\n");
            parameters.Append("maxRedCountSpecified: ");
            parameters.Append(request.offer.maxRedCountSpecified);
            parameters.Append("\r\n");
            parameters.Append("MIDGroup:");
            foreach(string mid in request.offer.MIDGroup)
            {
                parameters.Append("\t");
                parameters.Append(mid);
                parameters.Append("\r\n");
            }
            parameters.Append("\r\n");
            parameters.Append("minTxnAmt: ");
            parameters.Append(request.offer.minTxnAmt);
            parameters.Append("\r\n");
            parameters.Append("offerType: ");
            parameters.Append(request.offer.offerType);
            parameters.Append("\r\n");
            parameters.Append("offerTypeSpecified: ");
            parameters.Append(request.offer.offerTypeSpecified);
            parameters.Append("\r\n");
            parameters.Append("startDateTime: ");
            parameters.Append(request.offer.startDateTime);
            parameters.Append("\r\n");
            parameters.Append("offerMode: ");
            parameters.Append(request.offer.offerMode);
            parameters.Append("\r\n");
            AppendRegisterOfferRequestParameters(request, parameters);
            Context.Log.Verbose("First Data {0} Deal request parameters:\r\n{1}", callType, parameters.ToString());
        }

        /// <summary>
        /// Logs the claim deal request parameters sent to First Data.
        /// </summary>
        /// <param name="request">
        /// The request whose parameters to log.
        /// </param>
        /// <param name="callType">
        /// The type of the call to specify in the log.
        /// </param>
        /// </param>
        private void LogClaimDealRequestParameters(OfferRegisterRequest request)
        {
            StringBuilder parameters = new StringBuilder("cardSuffix: ");
            parameters.Append(request.consumer.cardSuffix);
            parameters.Append("\r\n");
            parameters.Append("consumerID: ");
            parameters.Append(request.consumer.consumerID);
            parameters.Append("\r\n");
            parameters.Append("currCode: ");
            parameters.Append(request.consumer.currCode);
            parameters.Append("\r\n");
            parameters.Append("offerAcceptID: ");
            parameters.Append(request.consumer.offerAcceptID);
            parameters.Append("\r\n");
            parameters.Append("\r\n");
            parameters.Append("purDateTime: ");
            parameters.Append(request.consumer.purDateTime);
            parameters.Append("\r\n");
            parameters.Append("purPrice: ");
            parameters.Append(request.consumer.purPrice);
            parameters.Append("\r\n");
            parameters.Append("token: ");
            parameters.Append(request.consumer.token);
            parameters.Append("\r\n");
            parameters.Append("tokenType: ");
            parameters.Append(request.consumer.tokenType);
            parameters.Append("\r\n");
            parameters.Append("encryptPAN: ");
            parameters.Append(request.consumer.encryptPAN);
            parameters.Append("\r\n");
            parameters.Append("keyID: ");
            parameters.Append(request.consumer.keyID);
            parameters.Append("\r\n");
            parameters.Append("optOut: ");
            parameters.Append(request.consumer.optOut);
            parameters.Append("\r\n");
            parameters.Append("optOutSpecified: ");
            parameters.Append(request.consumer.optOutSpecified);
            parameters.Append("\r\n");
            AppendRegisterOfferRequestParameters(request, parameters);
            Context.Log.Verbose("First Data Claim Deal request parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Appends parameters from the specified OfferRegisterRequest object to the specified StringBuilder.
        /// </summary>
        /// <param name="request">
        /// The offer register request object from which to append parameters.
        /// </param>
        /// <param name="parameters">
        /// The StringBuilder in which to append register offer request parameters.
        /// </param>
        private static void AppendRegisterOfferRequestParameters(OfferRegisterRequest request,
                                                                 StringBuilder parameters)
        {
            parameters.Append("offerID: ");
            parameters.Append(request.offerID);
            parameters.Append("\r\n");
            parameters.Append("provID: ");
            parameters.Append(request.provID);
            parameters.Append("\r\n");
            parameters.Append("pubID: ");
            parameters.Append(request.pubID);
            parameters.Append("\r\n");
            parameters.Append("pubName: ");
            parameters.Append(request.pubName);
            parameters.Append("\r\n");
            parameters.Append("reqType: ");
            parameters.Append(request.reqType);
            parameters.Append("\r\n");
            parameters.Append("coalitionID: ");
            parameters.Append(request.coalitionID);
            parameters.Append("\r\n");
            parameters.Append("offerName: ");
            parameters.Append(request.offerName);
        }

        /// <summary>
        /// Logs the register offer response parameters received from First Data.
        /// </summary>
        /// <param name="response">
        /// The response whose parameters to log.
        /// </param>
        /// <param name="callType">
        /// The type of the call to specify in the log.
        /// </param>
        private void LogRegisterOfferResponseParameters(OfferRegisterResponse response,
                                                       string callType)
        {
            StringBuilder parameters = new StringBuilder("cardSuffix: ");
            parameters.Append(response.cardSuffix);
            parameters.Append("\r\n");
            parameters.Append("consumerID: ");
            parameters.Append(response.consumerID);
            parameters.Append("\r\n");
            parameters.Append("offerID: ");
            parameters.Append(response.offerID);
            parameters.Append("\r\n");
            parameters.Append("provID: ");
            parameters.Append(response.provID);
            parameters.Append("\r\n");
            parameters.Append("reqType: ");
            parameters.Append(response.reqType);
            parameters.Append("\r\n");
            parameters.Append("respCode: ");
            parameters.Append(response.respCode);
            parameters.Append("\r\n");
            parameters.Append("respText: ");
            parameters.Append(response.respText);
            parameters.Append("\r\n");
            parameters.Append("token: ");
            parameters.Append(response.token);
            parameters.Append("\r\n");
            parameters.Append("tokenType: ");
            parameters.Append(response.tokenType);
            parameters.Append("\r\n");
            parameters.Append("coalitionID: ");
            parameters.Append(response.coalitionID);
            Context.Log.Verbose("First Data {0} Deal response parameters:\r\n{1}", callType, parameters.ToString());
        }

        /// <summary>
        /// Logs the redemption request parameters received from First Data.
        /// </summary>
        /// <param name="request">
        /// The request whose parameters to log.
        /// </param>
        /// </param>
        private void LogRedemptionRequestParameters(pubRedemptionRequest request)
        {
            StringBuilder parameters = new StringBuilder("redemptionType: ");
            parameters.Append((RedemptionEvent)request.redemptionType);
            parameters.Append("\r\n");
            parameters.Append("dateTime: ");
            parameters.Append(request.dateTime);
            parameters.Append("\r\n");
            parameters.Append("authAmt: ");
            parameters.Append(request.authAmt);
            parameters.Append("\r\n");
            parameters.Append("currCode: ");
            parameters.Append(request.currCode);
            parameters.Append("\r\n");
            parameters.Append("offerID: ");
            parameters.Append(request.offerID);
            parameters.Append("\r\n");
            parameters.Append("token: ");
            parameters.Append(request.token);
            parameters.Append("\r\n");
            parameters.Append("offerAcceptID: ");
            parameters.Append(request.offerAcceptID);
            parameters.Append("\r\n");
            parameters.Append("reqID: ");
            parameters.Append(request.reqID);
            parameters.Append("\r\n");
            parameters.Append("MID: ");
            parameters.Append(request.MID);
            parameters.Append("\r\n");
            parameters.Append("offerMID: ");
            parameters.Append(request.offerMID);
            parameters.Append("\r\n");
            parameters.Append("BECode: ");
            parameters.Append(request.BECode);
            parameters.Append("\r\n");
            parameters.Append("tenderType: ");
            parameters.Append(request.tenderType);
            Context.Log.Verbose("First Data Redemption request parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Logs the redemption response parameters sent to First Data.
        /// </summary>
        /// <param name="response">
        /// The response whose parameters to log.
        /// </param>
        private void LogRedemptionResponseParameters(pubRedemptionResponse response)
        {
            StringBuilder parameters = new StringBuilder("respCode: ");
            parameters.Append(response.respCode);
            parameters.Append("\r\n");
            parameters.Append("reasonCode: ");
            parameters.Append(response.reasonCode);
            parameters.Append("\r\n");
            parameters.Append("reqID: ");
            parameters.Append(response.reqID);
            parameters.Append("\r\n");
            parameters.Append("consumerID: ");
            parameters.Append(response.consumerID);
            parameters.Append("\r\n");
            parameters.Append("errorMsg: ");
            parameters.Append(response.errorMsg);
            parameters.Append("\r\n");
            parameters.Append("offerAcceptID: ");
            parameters.Append(response.offerAcceptID);
            parameters.Append("\r\n");
            parameters.Append("offerDesc: ");
            parameters.Append(response.offerDesc);
            parameters.Append("\r\n");
            parameters.Append("offerID: ");
            parameters.Append(response.offerID);
            parameters.Append("\r\n");
            parameters.Append("provID: ");
            parameters.Append(response.provID);
            parameters.Append("\r\n");
            parameters.Append("pubID: ");
            parameters.Append(response.pubID);
            parameters.Append("\r\n");
            parameters.Append("pubName: ");
            parameters.Append(response.pubName);
            parameters.Append("\r\n");
            parameters.Append("rdmptTranID: ");
            parameters.Append(response.rdmptTranID);
            parameters.Append("\r\n");
            parameters.Append("receiptTxt: ");
            parameters.Append(response.receiptTxt);
            parameters.Append("\r\n");
            parameters.Append("redemptionAmt: ");
            parameters.Append(response.redemptionAmt);
            parameters.Append("\r\n");
            parameters.Append("redemptionAmtSpecified: ");
            parameters.Append(response.redemptionAmtSpecified);
            Context.Log.Verbose("First Data Redemption response parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Logs the reversal request parameters received from First Data.
        /// </summary>
        /// <param name="request">
        /// The request whose parameters to log.
        /// </param>
        private void LogReversalRequestParameters(pubReversalRequest request)
        {
            StringBuilder parameters = new StringBuilder("redemptionType: ");
            parameters.Append((RedemptionEvent)request.redemptionType);
            parameters.Append("\r\n");
            parameters.Append("dateTime: ");
            parameters.Append(request.dateTime);
            parameters.Append("\r\n");
            parameters.Append("authAmt: ");
            parameters.Append(request.authAmt);
            parameters.Append("\r\n");
            parameters.Append("currCode: ");
            parameters.Append(request.currCode);
            parameters.Append("\r\n");
            parameters.Append("offerID: ");
            parameters.Append(request.offerID);
            parameters.Append("\r\n");
            parameters.Append("token: ");
            parameters.Append(request.token);
            parameters.Append("\r\n");
            parameters.Append("offerAcceptID: ");
            parameters.Append(request.offerAcceptID);
            parameters.Append("\r\n");
            parameters.Append("reqID: ");
            parameters.Append(request.reqID);
            parameters.Append("\r\n");
            parameters.Append("MID: ");
            parameters.Append(request.MID);
            parameters.Append("\r\n");
            parameters.Append("offerMID: ");
            parameters.Append(request.offerMID);
            parameters.Append("\r\n");
            parameters.Append("BECode: ");
            parameters.Append(request.BECode);
            parameters.Append("\r\n");
            parameters.Append("tenderType: ");
            parameters.Append(request.tenderType);
            parameters.Append("\r\n");
            parameters.Append("authAmtSpecified: ");
            parameters.Append(request.authAmtSpecified);
            parameters.Append("\r\n");
            parameters.Append("rdmptTranID: ");
            parameters.Append(request.rdmptTranID);
            parameters.Append("\r\n");
            parameters.Append("revAmt: ");
            parameters.Append(request.revAmt);
            parameters.Append("\r\n");
            parameters.Append("revAmtSpecified: ");
            parameters.Append(request.revAmtSpecified);
            parameters.Append("\r\n");
            parameters.Append("revReason: ");
            parameters.Append(request.revReason);
            Context.Log.Verbose("First Data Reversal request parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Logs the reversal response parameters sent to First Data.
        /// </summary>
        /// <param name="response">
        /// The response whose parameters to log.
        /// </param>
        private void LogReversalResponseParameters(pubReversalResponse response)
        {
            StringBuilder parameters = new StringBuilder("respCode: ");
            parameters.Append(response.respCode);
            parameters.Append("\r\n");
            parameters.Append("reasonCode: ");
            parameters.Append(response.reasonCode);
            parameters.Append("\r\n");
            parameters.Append("reqID: ");
            parameters.Append(response.reqID);
            parameters.Append("\r\n");
            parameters.Append("errorMsg: ");
            parameters.Append(response.errorMsg);
            parameters.Append("\r\n");
            parameters.Append("provID: ");
            parameters.Append(response.provID);
            parameters.Append("\r\n");
            parameters.Append("rdmptTranID: ");
            parameters.Append(response.rdmptTranID);
            parameters.Append("\r\n");
            parameters.Append("revAmt: ");
            parameters.Append(response.revAmt);
            parameters.Append("\r\n");
            parameters.Append("revAmtSpecified: ");
            parameters.Append(response.revAmtSpecified);
            Context.Log.Verbose("First Data Reversal response parameters:\r\n{0}", parameters.ToString());
        }
    }
}