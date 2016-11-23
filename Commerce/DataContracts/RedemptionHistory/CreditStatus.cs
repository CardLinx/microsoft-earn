//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the status of the credit associated with a redeemed deal.
    /// </summary>
    public enum CreditStatus
    {
        /// <summary>
        /// Indicates that an authorization event for the transaction was received.
        /// </summary>
        /// <remarks>
        /// This was previously Unprocessed = 0.
        /// </remarks>
        [XmlEnum("0")]
        AuthorizationReceived = 0,

        /// <summary>
        /// Indicates that a clearing event for the transaction was received.
        /// settled.
        /// </summary>
        /// <remarks>
        /// This was previously ReadyForSettlement = 1.
        /// </remarks>
        [XmlEnum("5")]
        ClearingReceived = 5,

        /// <summary>
        /// Indicates that a request to issue a statement credit to a consumer is being generated at this time.
        /// </summary>
        [XmlEnum("10")]
        GeneratingStatementCreditRequest = 10,

        /// <summary>
        /// Indicates that a request to issue a statement credit to a consumer is in the process of being sent.
        /// </summary>
        [XmlEnum("15")]
        SendingStatementCreditRequest = 15,

        /// <summary>
        /// Indicates that the request to issue a statement credit to a consumer has been sent.
        /// </summary>
        /// <remarks>   
        /// This was previously PendingSettledAsRedeemed = 2.
        /// </remarks>
        [XmlEnum("20")]
        StatementCreditRequested = 20,

        /// <summary>
        /// Indicates that an error within the Earn code base prevented the statement credit request for being generated.
        ///  Another attempt will be made.
        /// </summary>
        [XmlEnum("25")]
        RetryingAfterGeneratingStatementCreditRequestFailure = 25,
        
        /// <summary>
        /// Indicates that the credit should appear on the consumer's statement.
        /// </summary>
        /// <remarks>
        /// This was previously SettledAsRedeemed = 3.
        /// </remarks>
        [XmlEnum("500")]
        CreditGranted = 500,

        /// <summary>
        /// Indicates that a potential Burn transaction was not eligible for a statement credit because the consumer had no Earn
        ///  balance to burn.
        /// </summary>
        /// <remarks>
        /// This was previously SettledAsReversed = 4.
        /// </remarks>
        [XmlEnum("505")]
        NoEarnBalanceToBurn = 505,

        /// <summary>
        /// Indicates that the request to issue a statement credit to the consumer was rejected by our partner company.
        /// </summary>
        /// <remarks>
        /// This was previously RejectedByPartner = 5.
        /// </remarks>
        [XmlEnum("510")]
        RejectedByPartner = 510,

        /// <summary>
        /// Indicates that the settlement amount did not qualify for any offer.
        /// </summary>
        /// <remarks>
        /// * This State is not currently applicable to the Earn program, but it must be retained to keep the old CLO records intact.
        /// * This was previously SettlementAmountToSmall = 6.
        /// </remarks>
        [XmlEnum("515")]
        SettlementAmountTooSmall = 515,

        /// <summary>
        /// Indicates that the transaction was deemed suspicious and was rejected after being reviewed.
        /// </summary>
        /// <remarks>
        /// This was previously RejectedAfterReview = 7.
        /// </remarks>
        [XmlEnum("520")]
        RejectedAfterReview = 520,

        /// <summary>
        /// Indicates that an error within the Earn code base prevented the statement credit request for being generated.
        ///  No further attempts will be made.
        /// </summary>
        [XmlEnumAttribute("525")]
        GeneratingStatementCreditRequestFailed = 525,

        /// <summary>
        /// Indicates that an error occurred during the attempt to submit a statement credit request to the partner.
        /// </summary>
        [XmlEnumAttribute("530")]
        SendingStatementCreditRequestFailed = 530
    }
}