//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Linq;
using services.visa.com.realtime.realtimeservice.datacontracts.v6;

namespace Lomo.Commerce.VisaClient
{
    using System.Collections.Generic;
    
    /// <summary>
    ///  encapsulate all the calls to generate Visa requests
    /// </summary>
    public class VisaRtmDataManager
    {
        /// <summary>
        ///     CreateEnrollment request
        /// </summary>
        /// <param name="userkey">user id</param>
        /// <param name="community">community id</param>
        /// <param name="cardnumbers">card number</param>
        /// <returns>request</returns>
        public static EnrollRequest GetCreateEnrollmentRequest(string userkey, string community,
                                                                         List<string> cardnumbers)
        {
            var request = new EnrollRequest();
            request.AuthenticationDetails = GetExternalAuthenticationRecord(userkey, community);
            request.CardDetails = cardnumbers.Select(cardnumber => new CardInfoRequest { CardNumber = cardnumber }).ToArray();

            request.CardHolderDetails = new UserInfoRequest
            {
                ExternalUserId = userkey
            };

            request.CommunityTermsVersion = VisaConstants.CommunityTermsVersion;

            request.UserPreferences = new UserPreferences
            {
                LanguageId = VisaConstants.LanguageId
            };

            request.UserStatus = VisaConstants.VisaUserStatusActive;
            return request;
        }
        

        /// <summary>
        ///  Get SaveCardRequest
        /// </summary>
        /// <param name="userkey">user id</param>
        /// <param name="community">community id</param>
        /// <param name="cardnumber">card number</param>
        /// <returns>request</returns>
        public static SaveCardRequest GetSaveCardRequest(string userkey, string community, string cardnumber)
        {
            var request = new SaveCardRequest();
            request.AuthenticationDetails = GetExternalAuthenticationRecord(userkey, community);
            request.CardInfoRequest = new CardInfoRequest
            {
                CardNumber = cardnumber
            };

            return request;
        }

        /// <summary>
        ///  Get DeleteCardRequest
        /// </summary>
        /// <param name="userkey">user id</param>
        /// <param name="community">community </param>
        /// <param name="cardid">guid obtained from Visa</param>
        /// <returns></returns>
        public static DeleteCardRequest GetDeleteCardRequest(string userkey, string community, string cardid)
        {
            var request = new DeleteCardRequest();
            request.AuthenticationDetails = GetExternalAuthenticationRecord(userkey, community);
            request.CardId = Guid.Parse(cardid);
            return request;
        }

        /// <summary>
        ///  remove user reqeust
        /// </summary>
        /// <param name="userkey">user id</param>
        /// <param name="community">community id</param>
        /// <returns>request</returns>
        public static UnenrollRequest GetUnenrollRequest(string userkey, string community)
        {
            var request = new UnenrollRequest();
            request.AuthenticationDetails = GetExternalAuthenticationRecord(userkey, community);
            return request;
        }

        /// <summary>
        /// Get SaveStatementCreditRequest request
        /// </summary>
        /// <param name="cardId">Token returned by partner for credit card</param>
        /// <param name="creditAmount">Amount to credit</param>
        /// <param name="transactionId">Transaction Id</param>
        /// <param name="transactionSettlementDate">Transaction settlement date.</param>
        /// <param name="partnerUserId">Partner user Id</param>
        /// <param name="community">Community Code</param>
        /// <returns>request</returns>
        public static SaveStatementCreditRequest GetSaveStatementCreditAsync(string cardId, float creditAmount, string transactionId, DateTime transactionSettlementDate, string partnerUserId, string community)
        {
            const string currentCodeUs = "840";
            var request = new SaveStatementCreditRequest();
            var externalStatementCredit  = new ExternalStatementCredit();
            externalStatementCredit.CardId = cardId;
            externalStatementCredit.CommunityCode = community;
            externalStatementCredit.StatementCreditAmount = new MonetaryAmount
            {
                Currency = currentCodeUs,
                Value = creditAmount
            };

            //TODO: Check if the value for StatementCreditDescription and StatementCreditSourceId is correct
            externalStatementCredit.StatementCreditDescription = "MSFT EARN REDEMPTION";
            externalStatementCredit.StatementCreditSourceId = "earn.microsoft.com";
            externalStatementCredit.TransactionId = transactionId;
            externalStatementCredit.TransactionSettlementDate = transactionSettlementDate.ToString("s");  //format like 2016-03-07T21:13:53
            externalStatementCredit.UserId = partnerUserId;

            request.ExternalStatementCredit = externalStatementCredit;
            return request;
        }

        /// <summary>
        ///     Authentication record is used in multiple requests
        /// </summary>
        /// <param name="userkey"> user id</param>
        /// <param name="community">community id</param>
        /// <returns>auth record </returns>
        private static AuthenticationDetails GetExternalAuthenticationRecord(string userkey, string community)
        {
            var authenticationDetails = new AuthenticationDetails
            {
                CommunityCode = community,
                UserKey = userkey

            };

            return authenticationDetails;
        }

        /// <summary>
        ///  Mock the register deal request
        /// </summary>
        /// <param name="bingofferid">bing offer deal id</param>
        /// <returns>reqeust</returns>
        public static RegisterDealRequest GetRegisterOfferRequest(string bingofferid)
        {
            RegisterDealRequest request = new RegisterDealRequest()
            {
                BingOfferDealId = bingofferid
            };
            return request;
        }
    }
}