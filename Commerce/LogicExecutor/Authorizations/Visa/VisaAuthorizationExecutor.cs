//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Notifications;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.VisaClient;
    using Lomo.Core.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    
    /// <summary>
    ///     Contains logic necessary to process the Visa OnAuth EndpointMessageRequest 
    /// </summary>
    public class VisaAuthorizationExecutor
    {
        /// <summary>
        ///     Initializes a new instance of the VisaAuthorizationExecutor class.
        /// </summary>
        /// <param name="context">
        ///     The context for the API being invoked.
        /// </param>
        public VisaAuthorizationExecutor(CommerceContext context)
        {
            Context = context;
            Context[Key.Partner] = Partner.Visa;
        }

        /// <summary>
        ///     Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        ///     Executes processing of the request.
        /// </summary>
        public ResultCode Execute()
        {
            ResultCode result = ResultCode.None; 
            EndPointMessageRequest request = (EndPointMessageRequest) Context[Key.Request];
            Dictionary<String, String> messageElementCollectionDictionary = new Dictionary<string, string>();
            foreach(MessageElementsCollection c in request.MessageElementsCollection)
                messageElementCollectionDictionary.Add(c.Key, c.Value);
            
            String requestType = messageElementCollectionDictionary[VisaEPMConstants.EventEventType];
            if (!string.Equals(requestType, VisaEPMConstants.OnAuthEventTypeValue, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Wrong request message");
            
            Dictionary<String, String> userDefinedFieldsCollectionDictionary = new Dictionary<string, string>(); 
            foreach(UserDefinedFieldsCollection c in request.UserDefinedFieldsCollection)
                userDefinedFieldsCollectionDictionary.Add(c.Key, c.Value);

            String cardId = request.CardId;
            String merchantId = messageElementCollectionDictionary[VisaEPMConstants.TransactionVisaMerchantId];
            String storeId = messageElementCollectionDictionary[VisaEPMConstants.TransactionVisaStoreId];

            SharedAuthorizationLogic sharedAuthorizationLogic = new SharedAuthorizationLogic(Context,
                                                                               CommerceOperationsFactory.AuthorizationOperations(Context));

            Authorization authorization = new Authorization();
            Context[Key.Authorization] = authorization;

            // Populate the Authorization.
            String amount = messageElementCollectionDictionary[VisaEPMConstants.TransactionTransactionAmount];
            authorization.AuthorizationAmount = AmexUtilities.ParseAuthAmount(amount);
            authorization.Currency = VisaConstants.CurrencyUSD;
            authorization.TransactionScopeId = messageElementCollectionDictionary[VisaEPMConstants.TransactionVipTransactionId];
            authorization.TransactionId =
                messageElementCollectionDictionary[VisaEPMConstants.TransactionTransactionID];
            String time = messageElementCollectionDictionary[VisaEPMConstants.TransactionTimeStampYYMMDD];
            // UTC time: 2013-12-05T07:25:06
            authorization.PurchaseDateTime = DateTime.Parse(time);
            authorization.PurchaseDateTime = DateTime.SpecifyKind(authorization.PurchaseDateTime, DateTimeKind.Utc);

            // Populate the Auth Info.
            Context[Key.PartnerCardId] = cardId;
            Context[Key.PartnerMerchantId] = string.Format("{0};{1}", merchantId, storeId);

            string merchantCity = messageElementCollectionDictionary.NullIfNotExist(VisaEPMConstants.MerchantCityString);
            string merchantState = messageElementCollectionDictionary.NullIfNotExist(VisaEPMConstants.MerchantStateString);
            string merchantPostalCode = messageElementCollectionDictionary.NullIfNotExist(VisaEPMConstants.MerchantPostalCodeString);

            KeyTransactionData keyTransactionData = new KeyTransactionData
            {
                MerchantCity = merchantCity,
                MerchantState = merchantState,
                MerchantPostalCode = merchantPostalCode
            };

            Context[Key.PartnerData] = keyTransactionData.XmlSerialize();
            
            LogAuthorizationRequest(authorization, Context);

            result = sharedAuthorizationLogic.AddAuthorization();
            Context[Key.ResultCode] = result;

            if (result == ResultCode.Created)
            {
                // Send notification.
                var notifyAuthorization = new NotifyAuthorization(Context);
                Context[Key.CardBrand] = CardBrand.Visa;
                Task.Run(new Action(notifyAuthorization.SendNotification));

            }
            return result;
        }

        /// <summary>
        /// Log the parameters to add auth call
        /// </summary>
        /// <param name="authorization">authorication data</param>
        /// <param name="context">context also have some parameters</param>
        private void LogAuthorizationRequest(Authorization authorization, CommerceContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("AddAuthorization Parameters");
            stringBuilder.Append("authorizationId: ");
            stringBuilder.AppendLine(authorization.Id.ToString());
            stringBuilder.Append("partnerId: ");
            stringBuilder.AppendLine("" + (int)context[Key.Partner]);
            stringBuilder.Append("recommendedPartnerDealId: ");
            stringBuilder.AppendLine((string)context[Key.PartnerDealId]);
            stringBuilder.Append("partnerCardId: ");
            stringBuilder.AppendLine((string)context[Key.PartnerCardId]);
            stringBuilder.Append("partnerMerchantId: ");
            stringBuilder.AppendLine((string) context[Key.PartnerMerchantId]);
            stringBuilder.Append("purchaseDateTime: ");
            stringBuilder.AppendLine(authorization.PurchaseDateTime.ToString());
            stringBuilder.Append("authAmount: ");
            stringBuilder.AppendLine("" + authorization.AuthorizationAmount);
            stringBuilder.Append("transactionId: ");
            stringBuilder.AppendLine(authorization.TransactionId);
            stringBuilder.Append("currency: ");
            stringBuilder.AppendLine(authorization.Currency);
            String logString = stringBuilder.ToString(); 
            Context.Log.Verbose(logString);
        }
    }
}