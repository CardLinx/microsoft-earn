//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Collections.Generic;

    /// <summary>
    /// Provides explanation strings for each ResultCode.
    /// </summary>
    public static class ResultCodeExplanation
    {
        /// <summary>
        /// Retrieves the explanation for the given result code.
        /// </summary>
        /// <param name="resultCode">
        /// The ResultCode whose explanation to retrieve.
        /// </param>
        /// <returns>
        /// The explanation for the result code.
        /// </returns>
        public static string Get(ResultCode resultCode)
        {
            return Mappings[resultCode];
        }

        /// <summary>
        /// Gets or sets the dictionary that contains the ResultCode to Explanation mappings.
        /// </summary>
        private static readonly Dictionary<ResultCode, string> Mappings = new Dictionary<ResultCode, string>()
        {
            { ResultCode.None, "An unknown error prevented a result code from being generated." },

            { ResultCode.Success, "The operation completed successfully." },

            { ResultCode.Created, "The resource was created successfully." },

            { ResultCode.JobQueued, "A job to perform the requested operation has been added to the queue." },

            { ResultCode.UnregisteredUser, "User should first be registered with the system before making this call. " +
                                           "Under some circumstances, this may be expected." },

            { ResultCode.UnactionableDealId, "Specified Deal ID cannot be used in this context." },

            { ResultCode.UnexpectedUnregisteredUser, "User must first be registered with the system before making this call." },

            { ResultCode.UnregisteredCard,
                                  "Specified Card has not been registered or must be re-registered to perform this operation." },

            { ResultCode.DealNotChanged, "No partner service added registration information to the Deal." },

            { ResultCode.InvalidCard, "Supplied card information is not valid." },

            { ResultCode.UnregisteredMerchant, "Specified merchant is not registered with the system." },

            { ResultCode.CardDoesNotBelongToUser, "Specified card does not belong to the authenticated user." },

            { ResultCode.InvalidMerchant, "Supplied merchant information is not valid."  },

            { ResultCode.UnregisteredDeal,
                          "Indicates the specified deal has not been registered with all parties involved in the transaction." },

            { ResultCode.InvalidDeal, "Supplied deal information is not valid." },

            { ResultCode.MerchantAlreadyExists, "Specified Merchant name has already been registered with a different ID." },

            { ResultCode.ExternalMerchantIdAlreadyExists, "One or more external merchant Ids has already been associated with " +
                                                          "a different merchant by the same external source." },

            { ResultCode.AlreadyClaimed,
                                       "The authenticated user has already claimed the specified deal for the specified card." },

            { ResultCode.CardDeactivated, "The operation cannot be completed because the specified card has been deactivated." },

            { ResultCode.ParameterCannotBeNull, "Required parameters cannot have null values." },

            { ResultCode.InvalidParameter, "Parameter contains an invalid value." },

            { ResultCode.SpecifiedDiscountDoesNotBelongToSpecifiedDeal, "A specified discount does not belong to the " +
                                                                        "specified deal." },

            { ResultCode.AggregateError, "A batch operation encountered one or more errors. See individual operation results." },

            { ResultCode.UserAlreadyExists, "The specified user has already been registered with the system." },

            { ResultCode.UnauthenticatedUserAlreadyExists, "The specified user has already been been added as an unauthenticated account." },

            { ResultCode.TransactionDisallowed, "Transaction has been disallowed due to business logic constraints." },

            { ResultCode.InvalidClientCertificate,
                                             "The client certificate presented in the request is not valid for the operation." },

            { ResultCode.InvalidPurchaseDateTime,
                                                "The purchase date and time specified in the redemption request is not valid." },

            { ResultCode.DealNotFound, "A deal that should be registered with the system could not be found." },

            { ResultCode.DealNotYetValid, "The specified deal cannot be redeemed because it is not yet valid." },

            { ResultCode.DuplicateTransaction, "The specified transaction is dupe of an earlier recorded transaction." },

            { ResultCode.UnsupportedBin, "An attempt was made to register a card with an unsupported BIN." },

            { ResultCode.Unauthorized, "The caller is unauthorized to make the request." },

            { ResultCode.InsufficientFunds, "The transaction cannot complete due to insufficient funds." },

            { ResultCode.InvalidExpirationDate, "The expiration date specified is not valid for the operation." },

            { ResultCode.InvalidDistributionAmount, "The requested distribution amount is not valid." },

            { ResultCode.DealThresholdNotMet,
                       "The specified authorization amount is not sufficient to meet the deal's minimum purchase requirement." },

            { ResultCode.InvalidPartnerMerchantId,
                            "The specified partner merchant ID does not correspond to the merchant involved in the operation." },

            { ResultCode.RedeemedDealNotFound,
                  "The specified partner redeemed deal ID does not correspond to the redeemed deal involved in the operation." },

            { ResultCode.RedemptionEventMismatch,
                            "The specified partner event is not the same as the redemption event recorded for the redemption." },

            { ResultCode.PartnerMismatch,
                          "The partner making the reversal call is not the same partner recorded as having redeemed the deal." },

            { ResultCode.PartnerDealIdMismatch,
                                    "The specified partner deal ID does not correspond to the deal involved in the operation." },

            { ResultCode.PartnerCardIdMismatch,
                   "The partner card ID specified in the reversal request is not the same as the partner card ID recorded for " +
                   "the corresponding redemption." },

            { ResultCode.MatchingRedeemedDealNotFound,
                                              "The described redeemed deal did not correspond to any recorded redeemed deals." },

            { ResultCode.MultipleMatchingRedeemedDealsFound,
                                               "The described redeemed deal corresponded to multiple recorded redeemed deals." },

            { ResultCode.FileMissingExpectedRecord, "A file used within a task was missing an expected record." },

            { ResultCode.PartnerUserIdMismatch,
                                    "The specified partner user ID does not correspond to the user involved in the operation." },

            { ResultCode.PartnerCardSuffixMismatch,
                                "The specified partner card suffix does not correspond to the card involved in the operation." },

            { ResultCode.PartnerCardIdNotFound, "The specified partner card ID did not correspond to a registered card." },

            { ResultCode.NoApplicableDealFound, "No applicable deal was found during a partner's attempt to redeem a deal." },

            { ResultCode.CardInWrongRewardProgram, "The card used in the transaction is not enrolled in the correct reward program for the offer." },

            { ResultCode.UserOverEarnLimit, "The user has exceeded the rolling year Earn limit, and so is inelligible to Earn from this offer today." },

            { ResultCode.UnexpectedEndOfRecord, "The record ended unexpectedly." },

            { ResultCode.ExpectedValueNotFound, "An expected value was not found when parsing a file." },

            { ResultCode.InvalidValue, "A field was not properly formatted for the data type it must contain." },

            { ResultCode.RecordOutOfPlace, "A record was found in a file out of its expected place." },

            { ResultCode.UnexpectedDuplicateRecordTypeFound,
                                      "A record type was found in a file more than once but should have been found only once." },

            { ResultCode.RedeemedDealReversalStateMismatch,
                                      "The reversal status specified during an operation did not match the reversal status of " +
                                      "the corresponding redeemed deal." },

            { ResultCode.CreditStatusTooAdvanced,
                                    "The result of an operation would have set the credit status of a transaction to a value " +
                                    "previous to its current value. Because of this, the operation will result in no changes." },


            { ResultCode.RedeemedDealRejectedByPartner, "A Redeemed deal was rejected by our partner. This will result in " +
                                                        " RejectedByPartner CreditStatus." },

            { ResultCode.SubmissionRejected, "A data submission (a file for example) was rejected by the partner." },

            { ResultCode.PayoutStatusTooAdvanced,
                                    "The result of an operation would have set the payout status of a reward to a value " +
                                    "previous to its current value. Because of this, the operation will result in no changes." },

            { ResultCode.JobContainsNoPayload, "A job being run in the queue contains no payload." },

            { ResultCode.CorruptSettlementRecord, "Record processed during settlement is corrupt." },

            { ResultCode.PartnerDealExpired, "The deal has already expired within the partner's system." },

            { ResultCode.RecordCountMismatch, "The number of records indicated in the file did not match the number of records found in the file." },

            { ResultCode.RedeemedDealFoundIsInexactMatch, "The redeemed deal found was not an exact match for the specified criteria." },

            { ResultCode.DuplicateRecordDetected, "An incoming authorization or redemption call appears to refer to a transaction for which a record already exists." },

            { ResultCode.TimedOut, "A task timed out and was aborted." },

            { ResultCode.FileNotFound, "A file involved in a task could not be found." },

            { ResultCode.UnknownError, "An unknown error occurred." },

            {ResultCode.CorporateOrPrepaidCardError, "Prepaid and Corporate Amex cards are not allowed."},

            {ResultCode.InvalidCardNumber, "The Credit Card Number is not valid."},

            {ResultCode.CardExpired, "The Credit Card is expired."},

            {ResultCode.CardRegisteredToDifferentUser, "The specified credit card is registered to a different user."},

            {ResultCode.CardStateUnchanged, "User attempted to add a card that's currently active in the account."},
            
            {ResultCode.MaximumEnrolledCardsLimitReached, "User reached the maximum card limit that can be enrolled with Visa."},

            {ResultCode.UserAlreadyEnrolledWithPartner, "User already enrolled with Visa. Cannot enroll again."},

            {ResultCode.UserNotEnrolledWithPartner, "User not enrolled with Visa. Cannot add card/delete card without enrolling."},
        };
    }
}