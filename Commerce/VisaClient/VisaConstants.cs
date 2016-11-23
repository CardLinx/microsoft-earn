//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.DataContracts.Extensions;
using System;
using System.Collections.Generic;

namespace Lomo.Commerce.VisaClient
{
    /// <summary>
    /// Visa Constants
    /// </summary>
    public class VisaConstants
    {
        /// <summary>
        /// Community name
        /// </summary>
        public const string CommunityName = "TODO_VISA_COMMUNITY_NAME_HERE";

        /// <summary>
        /// Community code at group level
        /// </summary>
        public const string CommunityCodeGroupLevel = "TODO_COMMUNITY_CODE_GROUP_LEVEL_HERE";

        /// <summary>
        /// Community code at CL level
        /// </summary>
        public const string CommunityCodeClLevel = "TODO_COMMUNITY_CL_LEVEL_HERE";

        /// <summary>
        /// Commmunity Terms Version
        /// </summary>
        public const string CommunityTermsVersion = "1.1";

        /// <summary>
        /// Culture info
        /// </summary>
        public const string LanguageId = "en-US";

        /// <summary>
        ///  User status
        /// </summary>
        public const string VisaUserStatusActive = "Active";

        /// <summary>
        ///  OfferId field used in SaveOfferActivation
        /// </summary>
        public const string OfferIdString = "OfferID";

        /// <summary>
        /// Clientname field key used in SaveOfferActivation
        /// </summary>
        public const string ClientNameKey = "ClientName";


        /// <summary>
        /// ClientName field value used in SaveOfferActivation
        /// </summary>
        public const string ClientNameValue = "TODO_COMMUNITY_CLIENT_NAME_HERE";

        /// <summary>
        ///  a string used in SaveOfferActivation
        /// </summary>
        public const string TargetedInterfaceString = "OnOfferActivation";

        /// <summary>
        /// US currency
        /// </summary>
        public const string CurrencyUSD = "USD";

        /// <summary>
        /// a blob containing the bing-visa deal mapping
        /// </summary>
        public const string VisaDealMappingUrl = "http://TODO_YOUR_AZURE_URL_HERE/microsoft/VisaDealMapping.json";
    }

    /// <summary>
    /// Visa Api call error codes
    /// </summary>
    public class VisaCallErrorConstants
    {

        public const string MaximumEnrolledCardsReached = "RTMENRE0032";

        public const string MaximumPanUsageReached = "RTMENRE0042";

        public const string InvalidCardNumber = "RTMENRE0044";

        public const string TheUserkeyProvidedIsAlreadyInUse = "RTMENRE0025";

        public const string CardAlreadyExpired = "RTMENRE0014";

        public const string EnrollUserNotFound = "RTMENRE0026";

        public const string EnrollmentRecordIsNull = "RTMENRE0029";

        public const string Mod10CheckFailed = "RTMENRE0047";

        public const string InvalidLastFourDigitOfCardNumber = "RTMENRE0038";

        public const string InvalidUserStatus = "RTMENRE0050";

        public static Dictionary<string, ResultCode> VisaErrorToResultCodeMapping = new Dictionary<string, ResultCode>(StringComparer.OrdinalIgnoreCase)
        {
            {MaximumEnrolledCardsReached, ResultCode.MaximumEnrolledCardsLimitReached},
            {MaximumPanUsageReached, ResultCode.CardRegisteredToDifferentUser},
            {InvalidCardNumber, ResultCode.InvalidCardNumber},
            {Mod10CheckFailed, ResultCode.InvalidCardNumber},
            {InvalidLastFourDigitOfCardNumber, ResultCode.InvalidCardNumber},
            {CardAlreadyExpired, ResultCode.CardExpired},
            {TheUserkeyProvidedIsAlreadyInUse, ResultCode.UserAlreadyEnrolledWithPartner},
            {EnrollUserNotFound, ResultCode.UserNotEnrolledWithPartner},
            {EnrollmentRecordIsNull, ResultCode.UserNotEnrolledWithPartner},
            {InvalidUserStatus, ResultCode.UserNotEnrolledWithPartner}
        };
    }

    /// <summary>
    /// VisaConstants in their notifications
    /// </summary>
    public class VisaEPMConstants
    {
        /// <summary>
        ///  OnVIPCardSwipe or OnClear
        /// </summary>
        public const string EventEventType = "Event.EventType";

        /// <summary>
        ///   EventType string for onAuth
        /// </summary>
        public const string OnAuthEventTypeValue = "OnVIPCardSwipe";

        /// <summary>
        ///  EventType string for onClearing
        /// </summary>
        public const string OnClearEventTypeValue = "OnClearing";

        /// <summary>
        ///  Merchant Id
        /// </summary>
        public const string TransactionVisaMerchantId = "Transaction.VisaMerchantId";

        /// <summary>
        ///   Visa Offer Id?
        /// </summary>
        public const string OfferOfferId = "Offer.OfferId";

        /// <summary>
        ///  Transaction Amount
        /// </summary>
        public const string TransactionTransactionAmount = "Transaction.TransactionAmount";


        /// <summary>
        /// Clearing Amount
        /// </summary>
        public const string TransactionClearingAmount = "Transaction.ClearingAmount";

        /// <summary>
        /// VIP Transaction Id
        /// </summary>
        public const string TransactionVipTransactionId = "Transaction.VipTransactionId";

        /// <summary>
        ///   Transaction Id
        /// </summary>
        public const string TransactionTransactionID = "Transaction.TransactionID";

        /// <summary>
        ///  Transaction DateTime
        /// </summary>
        public const string TransactionTimeStampYYMMDD = "Transaction.TimeStampYYMMDD";

        /// <summary>
        ///  Store Id
        /// </summary>
        public const string TransactionVisaStoreId = "Transaction.VisaStoreId";
        
        /// <summary>
        /// Bing Offer Id passed back from Visa
        /// </summary>
        public const string BingOfferIdString = "BingOfferID";

        /// <summary>
        /// Merchant City
        /// </summary>
        public const string MerchantCityString = "Transaction.MerchantCity";

        /// <summary>
        /// Merchant State
        /// </summary>
        public const string MerchantStateString = "Transaction.MerchantState";

        /// <summary>
        /// Merchant PostalCode
        /// </summary>
        public const string MerchantPostalCodeString = "Transaction.MerchantPostalCode";

        /// <summary>
        /// StatementCredit status
        /// </summary>
        public const string FulfillmentStatus = "Fulfillment.Status";

        /// <summary>
        /// StatementCredit message
        /// </summary>
        public const string FulfillmentStatusMessage = "Fulfillment.StatusMessage";

        /// <summary>
        /// StatementCredit amount
        /// </summary>
        public const string FulfillmentAmount = "Fulfillment.Amount";

        /// <summary>
        /// DateTime on StatementCredit is processed
        /// </summary>
        public const string FulfillmentProcessedDateTime = "Fulfillment.ProcessedDateTime";
    }
}