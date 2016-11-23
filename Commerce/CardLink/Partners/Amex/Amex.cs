//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Lomo.Commerce.AmexClient;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;
    using Newtonsoft.Json;

    /// <summary>
    /// Operation interface implementation for the Amex partner.
    /// </summary>
    public class Amex : ICommercePartner
    {
        /// <summary>
        /// Initializes a new instance of the Amex class.
        /// </summary>
        /// <param name="context">
        /// The context of the Amex operation.
        /// </param>
        public Amex(CommerceContext context)
        {
            Context = context;
            AmexInvoker = AmexInvokerFactory.BuildAmexInvoker(Context.PerformanceInformation, Context.Log, AmexInvokerOverride);
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

            // Build request
            AmexCardSyncRequest amexCardSyncRequest = new AmexCardSyncRequest
            {
                CardToken1 = Guid.NewGuid().ToString("N"),
                CardNumber = ((NewCardInfo)Context[Key.NewCardInfo]).Number
            };

            Context.Log.Verbose("Amex AddCardAsync suggested card token: {0}", amexCardSyncRequest.CardToken1);

            HashSet<ResultCode> terminalCodes = new HashSet<ResultCode>
            {
                ResultCode.Created,
                ResultCode.Success,
                ResultCode.InvalidCardNumber,
                ResultCode.CorporateOrPrepaidCardError
            };

            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                Context.Log.Verbose("Invoking Amex add card partner API.");
                AmexCardResponse response = await AmexInvoker.AddCardAsync(amexCardSyncRequest);
                Context.Log.Verbose("Amex add card partner API returned.\r\n {0}", JsonConvert.SerializeObject(response));

                string code = null;
                string text = null;

                if (response != null && response.ResponseCode != null)
                {
                    code = response.ResponseCode;
                    text = response.ResponseDescription;

                    switch (code)
                    {
                        case AmexCardSyncResponseCode.AddCardSuccess:
                        case AmexCardSyncResponseCode.CardAndTokenPairAlreadyExists:
                            result = ResultCode.Created;
                            PartnerCardInfo partnerCardInfo = GetAmexCardInfo((Card)Context[Key.Card]);
                            partnerCardInfo.PartnerCardId = response.CardToken1;
                            partnerCardInfo.PartnerCardSuffix = "00";
                            break;
                        case AmexCardSyncResponseCode.CardExistsWithDifferentToken:
                            result = ResultCode.CardDoesNotBelongToUser;
                            break;
                        case AmexCardSyncResponseCode.CorporateOrPrepaidCardError:
                            result = ResultCode.CorporateOrPrepaidCardError;
                            break;
                        case AmexCardSyncResponseCode.InvalidCardNumber:
                        case AmexCardSyncResponseCode.NotAmexCard:
                            result = ResultCode.InvalidCardNumber;
                            break;
                        default:
                            result = ResultCode.UnknownError;
                            break;
                    }
                }
                else
                {
                    result = ResultCode.UnknownError;
                }

                // Log a warning if result was not a success.
                if (result != ResultCode.Created)
                {
                    Context.Log.Warning("Amex call failed. respCode: {0}. respText: {1}.",
                        (int)DefaultLogEntryEventId.PartnerErrorWarning, code, text);
                }

                return result;
            }, terminalCodes);

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

            Card card = (Card)Context[Key.Card];
            PartnerCardInfo amexCardInfo = card.PartnerCardInfoList.SingleOrDefault(partnerCardInfo =>
                                                                                 partnerCardInfo.PartnerId == Partner.Amex);
            if (amexCardInfo != null)
            {
                // Build request
                AmexCardUnSyncRequest amexCardUnSyncRequest = new AmexCardUnSyncRequest
                {
                    CardToken1 = amexCardInfo.PartnerCardId
                };

                result = await PartnerUtilities.InvokePartner(Context, async () =>
                {
                    Context.Log.Verbose("Invoking partner RemoveCardAsync API.");
                    AmexCardResponse response = await AmexInvoker.RemoveCardAsync(amexCardUnSyncRequest);
                    Context.Log.Verbose("Partner RemoveCardAsync API returned: {0}: {1}.", response.ResponseCode, response.ResponseDescription);

                    string code = null;
                    string text = null;

                    if (response != null && response.ResponseCode != null)
                    {
                        code = response.ResponseCode;
                        text = response.ResponseDescription;

                        switch (code)
                        {
                            case AmexCardUnSyncResponseCode.RemoveCardSuccess:
                            case AmexCardUnSyncResponseCode.CardDoesNotExist:
                            case AmexCardUnSyncResponseCode.NoLinkedCards:
                                result = ResultCode.Success;
                                break;
                            default:
                                result = ResultCode.UnknownError;
                                break;
                        }
                    }
                    else
                    {
                        result = ResultCode.UnknownError;
                    }

                    // Log a warning if result was not a success.
                    if (result != ResultCode.Success)
                    {
                        Context.Log.Warning("Amex Data call failed. respCode: {0}. respText: {1}.",
                                            (int)DefaultLogEntryEventId.PartnerErrorWarning, code, text);
                    }

                    return result;
                });
            }

            return result;

        }

        /// <summary>
        /// Registers the deal in the context with this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        public Task<ResultCode> RegisterDealAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResultCode> ClaimDealAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marshals information from the Amex Auth request into the Authorization object in the context.
        /// </summary>
        public void MarshalAuthorization()
        {
            AmexAuthRequest request = (AmexAuthRequest)Context[Key.Request];

            // Populate the Authorization.
            DataModels.Authorization authorization = (DataModels.Authorization)Context[Key.Authorization];
            authorization.AuthorizationAmount = AmexUtilities.ParseAuthAmount(request.TransactionAmount);
            authorization.Currency = "USD";
            authorization.TransactionId = request.TransactionId;
            authorization.PurchaseDateTime = AmexUtilities.ParseAuthDateTime(request.TransactionTime);

            // Populate the Amex Auth Info.
            Context[Key.PartnerDealId] = request.OfferId;
            Context[Key.PartnerCardId] = request.CardToken;
            Context[Key.PartnerMerchantId] = request.MerchantNumber;
        }

        /// <summary>
        /// Construct response for Amex Auth Calls.
        /// </summary>
        public void BuildAuthorizationResponse()
        {
            AmexAuthResponse response = (AmexAuthResponse)Context[Key.Response];
            ResultCode resultCode = (ResultCode)Context[Key.ResultCode];
            switch (resultCode)
            {
                case ResultCode.Success:
                case ResultCode.Created:
                case ResultCode.DuplicateTransaction:
                    response.ResponseCode = AmexAuthResponseCode.AmexAuthSuccess;
                    break;
                case ResultCode.DealNotFound:
                    response.ResponseCode = AmexAuthResponseCode.AmexAuthDealNotFound;
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the context of the FirstData operation.
        /// </summary>
        public CommerceContext Context { get; set; }

        /// <summary>
        /// Retrieves the Amex PartnerCardInfo object, creating one if necessary.
        /// </summary>
        /// <param name="card">
        /// The Card whose Amex PartnerCardInfo object to retrieve.
        /// </param>
        /// <returns>
        /// The Amex PartnerCardInfo object.
        /// </returns>
        internal static PartnerCardInfo GetAmexCardInfo(Card card)
        {
            // Get the PartnerCardInfo for FirstData.
            PartnerCardInfo result = card.PartnerCardInfoList.SingleOrDefault(partnerCardInfo =>
                                                                                 partnerCardInfo.PartnerId == Partner.Amex);

            // If no Amex PartnerCardInfo existed, add one.
            if (result == null)
            {
                result = new PartnerCardInfo()
                {
                    PartnerId = Partner.Amex
                };
                card.PartnerCardInfoList.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the object through which the CardLink layer is accessed.
        /// </summary>
        internal IAmexInvoker AmexInvoker { get; set; }

        /// <summary>
        /// Overrides creation of the AmexInvoker object to use a specific object.
        /// </summary>
        /// <remarks>
        /// This should only be set for testing purposes.
        /// </remarks>
        internal static IAmexInvoker AmexInvokerOverride { get; set; }
    }
}