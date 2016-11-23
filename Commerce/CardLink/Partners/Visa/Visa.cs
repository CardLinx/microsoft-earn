//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Commerce.VisaClient;
using Lomo.Commerce.CardLink.Partners;
using Newtonsoft.Json;
using Visa.Proxy;

namespace Lomo.Commerce.CardLink
{
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.VisaClient;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Operation interface implementation for the Visa partner.
    /// </summary>
    public class Visa : ICommercePartner, IVisaClient
    {
        //private static string[] PossibleVisaErrorsForEnrollAndSaveCard = { VisaCallErrorConstants.InvalidCardNumber, VisaCallErrorConstants.MaximumEnrolledCardsReached, VisaCallErrorConstants.MaximumPanUsageReached, VisaCallErrorConstants.Mod10CheckFailed, VisaCallErrorConstants.InvalidLastFourDigitOfCardNumber, VisaCallErrorConstants.CardAlreadyExpired};

        /// <summary>
        /// Initializes a new instance of the Visa class.
        /// </summary>
        /// <param name="context">
        /// The context of the Visa operation.
        /// </param>
        public Visa(CommerceContext context)
        {
            Context = context;
            VisaInvoker = VisaInvokerFactory.BuildVisaInvoker(Context.PerformanceInformation,
                                                                             VisaInvokerOverride, Context.Config);
            visaErrorUtility = VisaErrorUtility.Instance;
        }

        /// <summary>
        /// Adds the card in the context for the user in the context to this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        public async Task<ResultCode> AddCardAsync()
        {
            ResultCode result;

            // If the card has a PAN token, see if a Visa ID has already been allocated for that PAN token. We do this because Visa rejects
            //  registering a card that had previously been registered to a different account. This approach allows us to differentiate false
            //  positives (i.e. Visa rejects a card for which we already have the Visa partner card ID somewhere in our DB) from true positives
            //  (i.e. Visa rejects a card erroneously as far as we can tell.)
            
            ICardOperations cardOperations = CommerceOperationsFactory.CardOperations(Context);

            //Since we will comment FDC code, FDC PAN token will not be assigned to a card. So the code below is commented since RetrieveVisaPartnerCardId will never return any card
            //without FDC PAN token

            /*
            Card card = (Card)Context[Key.Card];
            string visaPartnerCardId = cardOperations.RetrieveVisaPartnerCardId();
            if (String.IsNullOrWhiteSpace(visaPartnerCardId) == false)
            {
                PartnerCardInfo partnerCardInfo = GetVisaCardInfo(card);
                partnerCardInfo.PartnerCardId = visaPartnerCardId;
                partnerCardInfo.PartnerCardSuffix = "00";
                result = ResultCode.Created;
                result = Task.FromResult(result).Result;
            }
            else
            */
            {
                var userEnrolledWithVisa = false;
                var cards = cardOperations.RetrieveCards();
                if (cards != null)
                {
                    userEnrolledWithVisa = cards.Any(c => c.CardBrand == CardBrand.Visa);
                }
                result = await InvokeVisaAddCard(userEnrolledWithVisa);
            }

            return result;
        }

        /// <summary>
        /// Adds the card in the context for the user in the context to this partner. If user is new user then it also enroll that user, othwerwise just add the card
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        private async Task<ResultCode> InvokeVisaAddCard(bool userEnrolledWithVisa)
        {
            ResultCode result;
            var userkey = GetVisaExternalUserId();

            if (userEnrolledWithVisa)
            {
                result = await SaveCardAsync(userkey);
                if (result == ResultCode.UserNotEnrolledWithPartner)
                {
                    Context.Log.Warning("Visa API tells us that UserId = {0} is not enrolled with Visa. As per our system user is enrolled. We will try to enroll user again.", (int)DefaultLogEntryEventId.PartnerErrorWarning, userkey);
                    result = await CreateEnrollment(userkey);
                    if (result == ResultCode.Created)
                    {
                        result = await SaveCardAsync(userkey);
                    }
                }
            }
            else
            {
                result = await CreateEnrollment(userkey);
                if (result == ResultCode.UserAlreadyEnrolledWithPartner)
                {
                    Context.Log.Warning("Visa API tells us that UserId = {0} is enrolled with Visa. As per our system user is not enrolled. We will try to save user card.", (int)DefaultLogEntryEventId.PartnerErrorWarning, userkey);
                    result = await SaveCardAsync(userkey);
                }
            }

            return result;

        }

        /// <summary>
        ///  Enroll new user to Visa
        /// </summary>
        /// <returns></returns>
        private async Task<ResultCode> CreateEnrollment(string userKey)
        {
            ResultCode result = ResultCode.None;

            var newCardNumber = ((NewCardInfo)Context[Key.NewCardInfo]).Number;
            var lastFourOfNewCard = newCardNumber.Substring(12);
            
            // Build a card register request object.
            var request = VisaRtmDataManager.GetCreateEnrollmentRequest(
                community: VisaConstants.CommunityName,
                userkey: userKey,
                cardnumbers: new List<string> {newCardNumber}
                );

            LogRequest("CreateEnrollment", request, newCardNumber, lastFourOfNewCard);
            // Invoke the partner to add the card.
            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                
                var response = await VisaInvoker.CreateEnrollment(request).ConfigureAwait(false);
                LogRequestResponse("CreateEnrollment", request, response, response.Success, newCardNumber, lastFourOfNewCard);
                
                result = ResultCode.UnknownError;
                if (response.Success)
                    result = ResultCode.Created;
                else if (response.HasError())
                {
                    result = visaErrorUtility.GetResultCode(response, null);
                }

                if (result == ResultCode.Created)
                {
                    PartnerCardInfo partnerCardInfo = GetVisaCardInfo((Card)Context[Key.Card]);
                    partnerCardInfo.PartnerCardId = response.EnrollmentRecord.CardDetails[0].CardId.ToString();
                    partnerCardInfo.PartnerCardSuffix = "00";

                    var partnerUserId = response.EnrollmentRecord.UserProfileId;
                    User user = (User)Context[Key.User];
                    user.AddOrUpdatePartnerUserId(Partner.Visa, partnerUserId.ToString(), true);

                    //add partner user information to the database
                    var userOperations = CommerceOperationsFactory.UserOperations(Context);
                    userOperations.AddOrUpdateUser();
                }

                return result;

            }, null, Partner.None, true).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Add a card to Visa
        /// </summary>
        /// <param name="userkey"></param>
        /// <returns></returns>
        private async Task<ResultCode> SaveCardAsync(string userkey)
        {
            ResultCode result = ResultCode.None;

            var newCardNumber = ((NewCardInfo)Context[Key.NewCardInfo]).Number;
            var lastFourOfNewCard = newCardNumber.Substring(12);

            // Build a card register request object.
            var request = VisaRtmDataManager.GetSaveCardRequest(userkey, VisaConstants.CommunityName, newCardNumber);
            LogRequest("AddCard", request, newCardNumber, lastFourOfNewCard);

            // Invoke the partner to add the card.
            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                var response = await VisaInvoker.AddCard(request).ConfigureAwait(false);
                LogRequestResponse("AddCard", request, response, response.Success, newCardNumber, lastFourOfNewCard);

                result = ResultCode.UnknownError;
                if (response.Success)
                    result = ResultCode.Created;
                else if (response.HasError())
                {
                    result = visaErrorUtility.GetResultCode(response, null);
                }

                if (result == ResultCode.Created)
                {
                    PartnerCardInfo partnerCardInfo = GetVisaCardInfo((Card)Context[Key.Card]);
                    partnerCardInfo.PartnerCardId = response.CardInfoResponse.CardId.ToString();
                    partnerCardInfo.PartnerCardSuffix = "00";
                }

                return result;

            }, null, Partner.None, true).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Remove a card from Visa
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCode> RemoveCardAsync()
        {
            ResultCode result = ResultCode.None;
            var card = (Card)Context[Key.Card];
            var partnerCardId = card.PartnerCardInfoList.Where(p => p.PartnerId == Partner.Visa).Select(p => p.PartnerCardId).First();
            // Build a card register request object.
            var request = VisaRtmDataManager.GetDeleteCardRequest(GetVisaExternalUserId(), VisaConstants.CommunityName, partnerCardId);
            LogRequest("RemoveCard", request);

            // Invoke the partner to add the card.
            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                var response = await VisaInvoker.RemoveCard(request);
                LogRequestResponse("RemoveCard", request, response, response.Success);

                // Determine the ResultCode from the response code.
                result = response.Success ? ResultCode.Success : ResultCode.UnknownError;

                return result;

            }, null, Partner.None, true).ConfigureAwait(false);

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

            // Get the PartnerDealInfo for Visa.
            Deal deal = (Deal)Context[Key.Deal];
            PartnerDealInfo visaDealInfo = deal.PartnerDealInfoList.SingleOrDefault(partnerDealInfo => partnerDealInfo.PartnerId == Partner.Visa);

            // If no Visa PartnerDealInfo exists, the deal cannot be registered with Visa.
            if (visaDealInfo == null)
            {
                // Only mark operation successful if another partner will register the deal.
                if (deal.PartnerDealInfoList.Count > 0)
                {
                    result = ResultCode.Success;
                }
            }
            else
            {
                // Visa does not have a concept of registering a deal, so just mark status as completed successfully.
                if (String.IsNullOrWhiteSpace(visaDealInfo.PartnerDealId))
                {
                    visaDealInfo.PartnerDealId = Guid.NewGuid().ToString();
                }

                visaDealInfo.PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Complete;
                result = ResultCode.Success;
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Claims the deal in the context for redemption with the card in the context with this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        public Task<ResultCode> ClaimDealAsync()
        {
            return Task.Run(() =>
            {
                ResultCode result = ResultCode.None;

                // Get the PartnerCardInfo for Visa.
                Card card = (Card)Context[Key.Card];
                if (card.Id == 0)
                {
                    throw new InvalidOperationException("Unexpected value in card.Id");
                }

                PartnerCardInfo visaCardInfo = card.PartnerCardInfoList.SingleOrDefault(partnerCardInfo => partnerCardInfo.PartnerId == Partner.Visa);

                if (visaCardInfo == null)
                {
                    result = ResultCode.UnregisteredCard;
                    return result;
                }

                // Get the PartnerDealInfo for Visa.
                Deal deal = (Deal)Context[Key.Deal];
                if (deal.Id == 0)
                {
                    throw new InvalidOperationException("Unexpected value in deal.Id");
                }

                PartnerDealInfo visaDealInfo = deal.PartnerDealInfoList.SingleOrDefault(partnerDealInfo => partnerDealInfo.PartnerId == Partner.Visa);
                if (visaDealInfo == null)
                {
                    result = ResultCode.UnregisteredDeal;
                    return result;
                }

                if (result == ResultCode.None)
                {
                    User user = (User)Context[Key.User];
                    Context[Key.ClaimedDeal] = new ClaimedDeal()
                    {
                        CardId = card.Id,
                        GlobalDealId = deal.GlobalId,
                        GlobalUserId = user.GlobalId,
                        Partner = Partner.Visa
                    };
                    result = ResultCode.Success;
                }

                return result;
            });

        }


        public async Task<ResultCode> SaveStatementCreditAsync()
        {
            var result = ResultCode.None;
            var cardId = (string) Context[Key.CardToken];
            var creditAmount = (float)((int) Context[Key.DealDiscountAmount] /100.0);
            var transactionId = (string) Context[Key.Transaction];
            var transactionSettlementDate = (DateTime) Context[Key.TransactionSettlementDate];
            var partnerUserId = (string) Context[Key.PartnerUserId];

            var request = VisaRtmDataManager.GetSaveStatementCreditAsync(cardId, creditAmount, transactionId, transactionSettlementDate, partnerUserId, VisaConstants.CommunityCodeClLevel);
            LogRequest("SaveStatementCreditAsync", request);

            result = await PartnerUtilities.InvokePartner(Context, async () =>
            {
                var response = await VisaInvoker.SaveStatementCreditAsync(request).ConfigureAwait(false);
                LogRequestResponse("SaveStatementCreditAsync", request, response, response.StatementCreditSubmitStatus);

                result = ResultCode.UnknownError;
                if (response.StatementCreditSubmitStatus)
                    result = ResultCode.Created;
                else if (response.HasError())
                {
                    //TODO: once we are ready to log these error messages in Db - map the error returned by Visa
                    //into our error codes and log in Db
                    result = visaErrorUtility.GetResultCode(response, null);
                }
                
                return result;
            }, null, Partner.None, true).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Retrieves the Visa External User Id. This is userId in Commerce system which Visa treats as External User Id
        /// </summary>
        /// <returns>
        /// The Visa Partner User Id.
        /// </returns>
        private string GetVisaExternalUserId()
        {
            return VisaInvoker.GenerateExternalUserId((User)Context[Key.User]);
        }

        /// <summary>
        /// Retrieves the Visa PartnerCardInfo object, creating one if necessary.
        /// </summary>
        /// <param name="card">
        /// The Card whose Visa PartnerCardInfo object to retrieve.
        /// </param>
        /// <returns>
        /// The Visa PartnerCardInfo object.
        /// </returns>
        private static PartnerCardInfo GetVisaCardInfo(Card card)
        {
            // Get the PartnerCardInfo for FirstData.
            PartnerCardInfo result = card.PartnerCardInfoList.SingleOrDefault(partnerCardInfo => partnerCardInfo.PartnerId == Partner.Visa);

            // If no FirstData PartnerCardInfo existed, add one.
            if (result == null)
            {
                result = new PartnerCardInfo
                {
                    PartnerId = Partner.Visa
                };
                card.PartnerCardInfoList.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context of the Visa operation.
        /// </summary>
        public CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the object through which the CardLink layer is accessed.
        /// </summary>
        internal IVisaInvoker VisaInvoker { get; set; }

        /// <summary>
        /// Overrides creation of the VisaInvoker object to use a specific object.
        /// </summary>
        /// <remarks>
        /// This should only be set for testing purposes.
        /// </remarks>
        internal static IVisaInvoker VisaInvokerOverride { get; set; }

        /// <summary>
        /// helps transform Visa errors into Commerce Result Code
        /// </summary>
        private readonly VisaErrorUtility visaErrorUtility;


        /// <summary>
        /// Logs Visa Request
        /// </summary>
        /// <param name="methodName">Visa API method name which is called</param>
        /// <param name="request">Request object</param>
        /// <param name="findToken">Any token in request which need to be replaced</param>
        /// <param name="replaceWith">Any token in request which need to be replaced with</param>
        private void LogRequest(string methodName, object request, string findToken = null, string replaceWith = null)
        {
            var serializedRequest = Serialize(request, findToken, replaceWith);
            Context.Log.Verbose($"Invoking Visa API:{methodName}. Request:{serializedRequest}");
        }

        /// <summary>
        /// Logs Visa Request and Response
        /// </summary>
        /// <param name="methodName">Visa API method name which is called</param>
        /// <param name="request">Request object</param>
        /// <param name="response">Response object</param>
        /// <param name="responseStatus">Response status</param>
        /// <param name="findToken">Any token in request which need to be replaced</param>
        /// <param name="replaceWith">Any token in request which need to be replaced with</param>
        private void LogRequestResponse(string methodName, object request, object response, bool responseStatus, string findToken = null, string replaceWith = null)
        {
            var serializedRequest = Serialize(request, findToken, replaceWith);
            var serializedResponse = Serialize(response);
            if (responseStatus)
            {
                Context.Log.Verbose($"Visa API:{methodName}. Response status IsSuccess:true. Request:{serializedRequest} Response:{serializedResponse}");
            }
            else
            {
                //Context.Log.Warning("Visa API:{0}. Response status IsSuccess:false. Request:{2} Response:{3}", methodName, serializedRequest, serializedResponse);
                Context.Log.Error("Visa API:{0}. Response status IsSuccess:false. Request:{1} Response:{2}",null, methodName, serializedRequest, serializedResponse);
            }
        }

        /// <summary>
        /// Serialize object in json
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize</param>
        /// <param name="findToken">Any token in request which need to be replaced</param>
        /// <param name="replaceWith">Any token in request which need to be replaced with</param>
        /// <returns></returns>
        private string Serialize(object objectToSerialize, string findToken = null, string replaceWith = null)
        {
            string serializedValue = null;
            if (objectToSerialize != null)
            {
                serializedValue = JsonConvert.SerializeObject(objectToSerialize, new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Ignore});
                if (findToken != null && replaceWith != null)
                {
                    serializedValue = serializedValue.Replace(findToken, replaceWith);
                }
            }
            return serializedValue;
        }
    }
}