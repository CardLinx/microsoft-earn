//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Operation interface implementation for the MasterCard partner.
    /// </summary>
    public class MasterCard : ICommercePartner
    {
        /// <summary>
        /// Initializes a new instance of the MasterCard class.
        /// </summary>
        /// <param name="context">
        /// The context of the MasterCard operation.
        /// </param>
        public MasterCard(CommerceContext context)
        {
            Context = context;
            MasterCardInvoker = MasterCardInvokerFactory.BuildMasterCardInvoker(Context.PerformanceInformation,
                                                                                MasterCardInvokerOverride);
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

            // Build customer fields.
            string partnerCardId = General.GenerateShortGuid();
            HashStringElement[] customerFieldsElements = new HashStringElement[2]
            {
                new HashStringElement
                {
                    fieldName = MasterCardConstants.BankCustomerNumberFieldName,
                    data = partnerCardId
                },
                new HashStringElement
                {
                    fieldName = MasterCardConstants.MemberIcaFieldName,
                    data = MasterCardConstants.MemberIca
                }
            };

            // Build the customer account fields.
            HashStringElement[] customerAccountFieldsElements = new HashStringElement[4]
            {
                new HashStringElement
                {
                    fieldName = MasterCardConstants.BankAccountNumberFieldName,
                    data = ((NewCardInfo) Context[Key.NewCardInfo]).Number
                },
                new HashStringElement
                {
                    fieldName = MasterCardConstants.BankProductCodeFieldName,
                    data = MasterCardConstants.BankProductCode
                },
                new HashStringElement
                {
                    fieldName = MasterCardConstants.AccountStatusCodeFieldName,
                    data = MasterCardConstants.AccountStatusActive
                },
                new HashStringElement
                {
                    fieldName = MasterCardConstants.ProgramIdentifierFieldName,
                    data = MasterCardConstants.ProgramIdentifier
                }
            };

            // Build a card register request object.
            doEnrollment cardEnrollment = new doEnrollment
            {
                sourceId = MasterCardConstants.SourceId,
                enrollmentTypeCode = MasterCardConstants.EnrollmentTypeCode,
                customerFields = customerFieldsElements,
                customerAccountFields = customerAccountFieldsElements
            };
            doEnrollmentRequest cardEnrollmentRequest = new doEnrollmentRequest
            {
                doEnrollment = cardEnrollment

            };
            LogAddCardRequestParameters(cardEnrollmentRequest);

            // Invoke the partner to add the card.
            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                Context.Log.Verbose("Invoking partner AddCard API.");
                DoEnrollmentResp response = await MasterCardInvoker.AddCard(cardEnrollment).ConfigureAwait(false);
                Context.Log.Verbose("Partner AddCard API returned: {0}: {1}.", response.returnCode, response.returnMsg);
                LogAddCardResponseParameters(response);

                // Determine the ResultCode from the response code.
                PartnerCardInfo partnerCardInfo = GetMasterCardCardInfo((Card)Context[Key.Card]);
//TODO: Move PartnerCardSuffix into First Data-specific construct.
                partnerCardInfo.PartnerCardSuffix = "00";
                switch (response.returnCode)
                {
                    case MasterCardResponseCode.Success:
                        result = ResultCode.Created;
                        partnerCardInfo.PartnerCardId = partnerCardId;
                        break;
                    case MasterCardResponseCode.BankAccountNumberExists:
                        result = ResultCode.Created;
                        partnerCardInfo.PartnerCardId = response.bankCustomerNumber;
                        break;
                    case MasterCardResponseCode.UnsupportedBin:
                    case MasterCardResponseCode.MessageNotFound:
                        result = ResultCode.UnsupportedBin;
                        break;
                    case MasterCardResponseCode.InvalidCard:
                        result = ResultCode.InvalidCard;
                        break;
                    case MasterCardResponseCode.InvalidParameters:
                    case MasterCardResponseCode.UnknownError:
                        result = ResultCode.UnknownError;
                        break;
                }

                // Log a warning if result was not a success.
                if (result != ResultCode.Created)
                {
                    Context.Log.Warning("MasterCard call failed. returnCode: {0}. returnMsg: {1}.",
                                        (int)DefaultLogEntryEventId.PartnerErrorWarning, response.returnCode, response.returnMsg);
                }

                return result;
            }).ConfigureAwait(false);

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

            // Build a account update request object to cancel card registration.
            CustomerAccountField[] customerAccountFieldFields = new CustomerAccountField[1]
            {
                new CustomerAccountField
                {
                    fieldName = MasterCardConstants.AccountStatusCodeFieldName,
                    data = MasterCardConstants.AccountStatusCanceled
                }
            };

            doCustomerAccountUpdate accountUpdate = new doCustomerAccountUpdate
            {
                sourceId = MasterCardConstants.SourceId,
                bankCustomerNumber = ((User)Context[Key.User]).GetPartnerUserId(Partner.MasterCard),
                customerAccountFields = customerAccountFieldFields
            };
            doCustomerAccountUpdateRequest accountUpdateRequest = new doCustomerAccountUpdateRequest
            {
                doCustomerAccountUpdate = accountUpdate
            };
            LogRemoveCardRequestParameters(accountUpdateRequest);

            // Invoke the partner to remove the card.
            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                Context.Log.Verbose("Invoking partner RemoveCardAsync API.");
                CustomerAccountUpdateResp response = await MasterCardInvoker.RemoveCard(accountUpdate).ConfigureAwait(false);
                Context.Log.Verbose("Partner RemoveCardAsync API returned: {0}: {1}.", response.returnCode, response.returnMsg);
                LogUpdateCardResponseParameters(response);

                // Determine the ResultCode from the response code.
                switch (response.returnCode)
                {
                    case MasterCardResponseCode.Success:
                    case MasterCardResponseCode.InvalidCard:
                        result = ResultCode.Success;
                        break;
                    case MasterCardResponseCode.UnknownError:
                        result = ResultCode.UnknownError;
                        break;
                }

                // Log a warning if result was not a success.
                if (result != ResultCode.Success)
                {
                    Context.Log.Warning("MasterCard call failed. returnCode: {0}. returnMsg: {1}.",
                                        (int)DefaultLogEntryEventId.PartnerErrorWarning, response.returnCode, response.returnMsg);
                }

                return result;
            }).ConfigureAwait(false);

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
            return Task.Factory.StartNew(() =>
            {
                ResultCode result = ResultCode.None;

                // Get the PartnerDealInfo for MasterCard.
                Deal deal = (Deal)Context[Key.Deal];
                PartnerDealInfo masterCardDealInfo = deal.PartnerDealInfoList.SingleOrDefault(partnerDealInfo =>
                                                                                     partnerDealInfo.PartnerId == Partner.MasterCard);

                // If no MasterCard PartnerDealInfo exists, the deal cannot be registered with MasterCard.
                if (masterCardDealInfo == null)
                {
                    // Only mark operation successful if another partner will register the deal.
                    if (deal.PartnerDealInfoList.Count > 0)
                    {
                        result = ResultCode.Success;
                    }
                }
                else
                {
                    // MasterCard does not have a concept of registering a deal, so just mark status as completed successfully.
                    if (String.IsNullOrWhiteSpace(masterCardDealInfo.PartnerDealId) == true)
                    {
                        masterCardDealInfo.PartnerDealId = Guid.NewGuid().ToString();
                    }
                    masterCardDealInfo.PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Complete;
                    result = ResultCode.Success;
                }

                return result;
            });
        }

        /// <summary>
        /// Claims the deal in the context for redemption with the card in the context with this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        /// <remarks>
        /// MasterCard does not have a concept of claiming a deal, so this will succeed automatically.
        /// </remarks>
        public Task<ResultCode> ClaimDealAsync()
        {
            return Task.Factory.StartNew(() =>
            { 
                ResultCode result = ResultCode.None;

                // Get the PartnerCardInfo for MasterCard.
                Card card = (Card)Context[Key.Card];
                if (card.Id == 0)
                {
                    throw new InvalidOperationException("Unexpected value in card.Id");
                }

                PartnerCardInfo masterCardCardInfo = card.PartnerCardInfoList.SingleOrDefault(partnerCardInfo =>
                                                                                     partnerCardInfo.PartnerId == Partner.MasterCard);
                if (masterCardCardInfo == null)
                {
                    result = ResultCode.UnregisteredCard;
                }

                // Get the PartnerDealInfo for MasterCard.
                Deal deal = (Deal)Context[Key.Deal];
                if (deal.Id == 0)
                {
                    throw new InvalidOperationException("Unexpected value in deal.Id");
                }

                PartnerDealInfo MaterCardDealInfo = deal.PartnerDealInfoList.SingleOrDefault(partnerDealInfo =>
                                                                                     partnerDealInfo.PartnerId == Partner.MasterCard);
                if (MaterCardDealInfo == null)
                {
                    result = ResultCode.UnregisteredDeal;
                }

                // Only proceed if MasterCard has both the card and the deal registered.
                if (result == ResultCode.None)
                {
                    // MasterCard does not have a concept of claiming a deal, so just assign an ID that will be used
                    // when searching for the best deal for a transaction.
                    User user = (User)Context[Key.User];
                    Context[Key.ClaimedDeal] = new ClaimedDeal()
                                    {
                                        CardId = card.Id,
                                        GlobalDealId = deal.GlobalId,
                                        GlobalUserId = user.GlobalId,
                                        Partner = Partner.MasterCard
                                    };
                    result = ResultCode.Success;
                }
                
                return result;            
            });
        }

        /// <summary>
        /// Marshals information from the MasterCard authorization request into the Authorization object in the context.
        /// </summary>
        public void MarshalAuthorization()
        {
            Transaction transaction = (Transaction)Context[Key.Transaction];
            LogAuthorizationRequestParameters(transaction);
            Authorization authorization = (Authorization)Context[Key.Authorization];
            authorization.AuthorizationAmount = (int)(transaction.transAmt * 100);
            authorization.Currency = "USD";
            authorization.TransactionId = Guid.NewGuid().ToString();
            authorization.PurchaseDateTime = MasterCardUtilities.ParseDateTimeString(String.Concat(transaction.transDate, transaction.transTime));
            authorization.TransactionScopeId = transaction.refNum;

            KeyAuthorizationData keyAuthorizationData = new KeyAuthorizationData
            {
                BankCustomerNumber = transaction.bankCustNum,
                BankNetRefNumber = transaction.refNum,
                Timestamp = transaction.timestamp,
                AcquirerIca = transaction.acquirerIca,
                MerchantId = transaction.merchId
            };
            Context[Key.PartnerData] = keyAuthorizationData.XmlSerialize();

            Context[Key.PartnerCardId] = transaction.bankCustNum;
            Context[Key.PartnerMerchantId] = String.Concat(transaction.acquirerIca, ";", transaction.merchId);
        }

        /// <summary>
        /// Construct response for MasterCard Authorization Calls.
        /// </summary>
        /// <remarks>
        /// MasterCard only pays attention to the HTTP status code, but does record information sent back to them.
        /// Because of this, the result code's int value will be sent for bookkeeping purposes.
        /// </remarks>
        public void BuildAuthorizationResponse()
        {
            MasterCardAuthorizationResponse response = (MasterCardAuthorizationResponse)Context[Key.Response];
            ResultCode resultCode = (ResultCode)Context[Key.ResultCode];
            response.ResponseCode = ((int)resultCode).ToString();
            LogAuthorizationResponseParameters(response);
        }

        /// <summary>
        /// Marshals information from the MasterCard clearing data record into the RedeemedDeal object in the context.
        /// </summary>
        /// <param name="clearingData">
        /// The clearing data record to marshal.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter clearingData cannot be null.
        /// </exception>
        public void MarshalRedeemDeal(ClearingData clearingData)
        {
            if (clearingData == null)
            {
                throw new ArgumentNullException("clearingData", "Parameter clearingData cannot be null.");
            }

            RedeemedDeal redeemedDeal = (RedeemedDeal)Context[Key.RedeemedDeal];
            redeemedDeal.CallbackEvent = RedemptionEvent.Settlement;
            redeemedDeal.PurchaseDateTime = clearingData.TransactionDate;
            redeemedDeal.AuthorizationAmount = (int)(clearingData.TransactionAmount * 100);
            redeemedDeal.Currency = "USD";
            redeemedDeal.PartnerRedeemedDealScopeId = clearingData.BankNetRefNumber;

            KeyClearingData keyClearingData = new KeyClearingData
            {
                BankCustomerNumber = clearingData.BankCustomerNumber,
                BankNetRefNumber = clearingData.BankNetRefNumber,
                MerchantId = clearingData.MerchantId,
                IssuerIca = clearingData.IssuerIca
            };
            Context[Key.PartnerData] = keyClearingData.XmlSerialize();

            Context[Key.PartnerCardId] = clearingData.BankCustomerNumber;
            Context[Key.PartnerMerchantId] = clearingData.LocationId;
            Context[Key.PartnerMerchantIdType] = PartnerMerchantIdType.SettlementOnly;
            Context[Key.CreditStatus] = CreditStatus.ClearingReceived;
            Context[Key.PartnerReferenceNumber] = clearingData.TransactionSequenceNumber;
        }

        /// <summary>
        /// Marshals information about a collection of outstanding MasterCard redeemed deals into a collection of MasterCard rebate records.
        /// </summary>
        /// <param name="outstandingRedeemedDeals">
        /// The outstanding redeemed deals to marshal.
        /// </param>
        /// <returns>
        /// The corresponding collection of rebate records.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter outstandingRedeemedDeals cannot be null.
        /// </exception>
        public static Collection<RebateRecord> MarshalOutstandingRedeemedDeals(Collection<OutstandingRedeemedDealInfo> outstandingRedeemedDeals)
        {
            if (outstandingRedeemedDeals == null)
            {
                throw new ArgumentNullException("outstandingRedeemedDeals", "Parameter outstandingRedeemedDeals cannot be null.");
            }

            Collection<RebateRecord> result = new Collection<RebateRecord>();

            foreach (OutstandingRedeemedDealInfo outstandingRedeemedDeal in outstandingRedeemedDeals)
            {
                KeyClearingData keyClearingData = KeyClearingData.Deserialize(outstandingRedeemedDeal.PartnerData);
                RebateRecord rebateRecord = new RebateRecord
                {
                    TransactionSequenceNumber = outstandingRedeemedDeal.AcquirerReferenceNumber,
                    TransactionAmount = ((decimal)outstandingRedeemedDeal.SettlementAmount / 100),
                    TransactionDate = outstandingRedeemedDeal.TransactionDate,
                    RebateAmount = ((decimal)outstandingRedeemedDeal.DiscountAmount / 100),
                    MerchantId = keyClearingData.MerchantId,
                    IssuerIca = keyClearingData.IssuerIca,
                    BankCustomerNumber = keyClearingData.BankCustomerNumber,
                    TransactionDescription = MicrosoftEarnRebateSource
                };

                result.Add(rebateRecord);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context of the MasterCard operation.
        /// </summary>
        public CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the object through which the MasterCard API is invoked.
        /// </summary>
        internal IMasterCardInvoker MasterCardInvoker { get; set; }

        /// <summary>
        /// Overrides creation of the MasterCardInvoker object to use a specific object.
        /// </summary>
        /// <remarks>
        /// This should only be set for testing purposes.
        /// </remarks>
        internal static IMasterCardInvoker MasterCardInvokerOverride { get; set; }

        /// <summary>
        /// Retrieves the MasterCard PartnerCardInfo object, creating one if necessary.
        /// </summary>
        /// <param name="card">
        /// The Card whose MasterCard PartnerCardInfo object to retrieve.
        /// </param>
        /// <returns>
        /// The MasterCard PartnerCardInfo object.
        /// </returns>
        private static PartnerCardInfo GetMasterCardCardInfo(Card card)
        {
            // Get the PartnerCardInfo for MasterCard.
            PartnerCardInfo result = card.PartnerCardInfoList.SingleOrDefault(partnerCardInfo =>
                                                                                 partnerCardInfo.PartnerId == Partner.MasterCard);

            // If no MasterCard PartnerCardInfo existed, add one.
            if (result == null)
            {
                result = new PartnerCardInfo()
                {
                    PartnerId = Partner.MasterCard
                };
                card.PartnerCardInfoList.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Logs the add card request parameters sent to MasterCard.
        /// </summary>
        /// <param name="request">
        /// The request whose parameters to log.
        /// </param>
        private void LogAddCardRequestParameters(doEnrollmentRequest request)
        {
            HashStringElement[] customerFields = request.doEnrollment.customerFields;
            HashStringElement[] customerAccounts = request.doEnrollment.customerAccountFields;

            StringBuilder parameters = new StringBuilder();
            foreach (HashStringElement hashStringElement in customerFields)
            {
                parameters.Append(hashStringElement.fieldName);
                parameters.Append(": ");
                parameters.Append(hashStringElement.data);
                parameters.Append("\r\n");
            }

            foreach (HashStringElement hashStringElement in customerAccounts)
            {
                // Log every field sent to MasterCard _except_ the PAN.
                if (hashStringElement.fieldName != MasterCardConstants.BankAccountNumberFieldName)
                {
                    parameters.Append(hashStringElement.fieldName);
                    parameters.Append(": ");
                    parameters.Append(hashStringElement.data);
                    parameters.Append("\r\n");
                }
            }
            parameters.Append("sourceId: ");
            parameters.Append(request.doEnrollment.sourceId);
            parameters.Append("\r\n");
            parameters.Append("enrollmentType: ");
            parameters.Append(request.doEnrollment.enrollmentTypeCode);
            Context.Log.Verbose("MasterCard Add Card request parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Logs the add card response parameters received from MasterCard.
        /// </summary>
        /// <param name="response">
        /// The response whose parameters to log.
        /// </param>
        private void LogAddCardResponseParameters(DoEnrollmentResp response)
        {
            StringBuilder parameters = new StringBuilder("accountStatusId: ");
            parameters.Append(response.accountStatusId);
            parameters.Append("\r\n");
            parameters.Append("bankCustomerNumber: ");
            parameters.Append(response.bankCustomerNumber);
            parameters.Append("\r\n");
            parameters.Append("virtualId: ");
            parameters.Append(response.virtualId);
            parameters.Append("\r\n");
            parameters.Append("returnCode: ");
            parameters.Append(response.returnCode);
            parameters.Append("\r\n");
            parameters.Append("returnMsg: ");
            parameters.Append(response.returnMsg);
            Context.Log.Verbose("MasterCard Add Card response parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Logs the remove card request parameters sent to MasterCard.
        /// </summary>
        /// <param name="request">
        /// The request whose parameters to log.
        private void LogRemoveCardRequestParameters(doCustomerAccountUpdateRequest request)
        {
            CustomerAccountField[] customerAccounts = request.doCustomerAccountUpdate.customerAccountFields;

            StringBuilder parameters = new StringBuilder();
            foreach (CustomerAccountField hashStringElement in customerAccounts )
            {
                parameters.Append(hashStringElement.fieldName);
                parameters.Append(": ");
                parameters.Append(hashStringElement.data);
                parameters.Append("\r\n");
            }

            parameters.Append("bankCustomerNumber: ");
            parameters.Append(request.doCustomerAccountUpdate.bankCustomerNumber);
            parameters.Append("\r\n");
            parameters.Append("memberIca: ");
            parameters.Append(request.doCustomerAccountUpdate.memberIca);
            parameters.Append("\r\n");
            parameters.Append("programId: ");
            parameters.Append(request.doCustomerAccountUpdate.programId);
            parameters.Append("\r\n");
            parameters.Append("sourceId: ");
            parameters.Append(request.doCustomerAccountUpdate.sourceId);
            Context.Log.Verbose("MasterCard Remove Card request parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Logs the register card response parameters received from MasterCard.
        /// </summary>
        /// <param name="response">
        /// The response whose parameters to log.
        /// </param>
        private void LogUpdateCardResponseParameters(CustomerAccountUpdateResp response)
        {
            StringBuilder parameters = new StringBuilder("returnCode: ");
            parameters.Append(response.returnCode);
            parameters.Append("\r\n");
            parameters.Append("returnMsg: ");
            parameters.Append(response.returnMsg);
            parameters.Append("\r\n");
            Context.Log.Verbose("MasterCard Remove Card request parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Logs the authorization request parameters received from MasterCard.
        /// </summary>
        /// <param name="transaction">
        /// The authorization request parameters.
        /// </param>
        private void LogAuthorizationRequestParameters(Transaction transaction)
        {
            StringBuilder parameters = new StringBuilder("timestamp: ");
            parameters.Append(transaction.timestamp);
            parameters.Append("\r\n");
            parameters.Append("transAmt: ");
            parameters.Append(transaction.transAmt);
            parameters.Append("\r\n");
            parameters.Append("bankCustNum: ");
            parameters.Append(transaction.bankCustNum);
            parameters.Append("\r\n");
            parameters.Append("refNum: ");
            parameters.Append(transaction.refNum);
            parameters.Append("\r\n");
            parameters.Append("transDate: ");
            parameters.Append(transaction.transDate);
            parameters.Append("\r\n");
            parameters.Append("acquirerIca: ");
            parameters.Append(transaction.acquirerIca);
            parameters.Append("\r\n");
            parameters.Append("merchId: ");
            parameters.Append(transaction.merchId);
            Context.Log.Verbose("MasterCard Authorization request parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// Logs the authorization response parameters sent to MasterCard.
        /// </summary>
        /// <param name="response">
        /// The authorization response object.
        /// </param>
        private void LogAuthorizationResponseParameters(MasterCardAuthorizationResponse response)
        {
            StringBuilder parameters = new StringBuilder("ResponseCode: ");
            parameters.Append(response.ResponseCode);
            Context.Log.Verbose("MasterCard Authorization response parameters:\r\n{0}", parameters.ToString());
        }

        /// <summary>
        /// The transaction source to add to statement.
        /// </summary>
        private const string MicrosoftEarnRebateSource = "MSFT EARN REDEMPTION";
    }
}