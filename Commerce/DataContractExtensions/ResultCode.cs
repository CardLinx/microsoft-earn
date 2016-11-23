//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts.Extensions
{
    /// <summary>
    /// Represents the possible results of a service invocation.
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// Indicates the service invocation has not yet been assigned a result.
        /// -OR-
        /// Indicates that an unknown error prevented a result code from being generated.
        /// </summary>
        None = 0,

        //////////////////////////////////////////////////////////////////////////
        // Successful states: 101..199.
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates the service invocation was successful.
        /// </summary>
        Success = 101,

        /// <summary>
        /// Indicates a resource was successfully created.
        /// </summary>
        Created = 102,

        /// <summary>
        /// Indicates a job to perform the requested operation has been added to the queue.
        /// </summary>
        JobQueued = 103,

        /// <summary>
        /// Indicates the calling user is not registered with the system but that this may be expected at this time.
        /// </summary>
        UnregisteredUser = 104,

        //////////////////////////////////////////////////////////////////////////
        // Warnings: 201..299.
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates the specified Deal ID does not correspond to the ID of a deal that can be used within the commerce system.
        /// </summary>
        UnactionableDealId = 201,

        /// <summary>
        /// Indicates the calling user is not registered with the system.
        /// </summary>
        UnexpectedUnregisteredUser = 202,

        /// <summary>
        /// Indicates the specified card has not been registered with all parties involved in the transaction.
        /// </summary>
        UnregisteredCard = 204,

        /// <summary>
        /// Indicates that a deal was not changed during a deal operation.
        /// </summary>
        DealNotChanged = 205,

        /// <summary>
        /// Indicates that a request to register a card with the system could not be fulfilled because card information was
        /// determined to be invalid.
        /// </summary>
        InvalidCard = 206,

        /// <summary>
        /// Indicates that the specified merchant is not registered with the system.
        /// </summary>
        UnregisteredMerchant = 207,

        /// <summary>
        /// Indicates that the specified card does not belong to the authenticated user.
        /// </summary>
        CardDoesNotBelongToUser = 208,

        /// <summary>
        /// Indicates that a request to register a merchant with the system could not be fulfilled because merchant information
        /// was determined to be invalid.
        /// </summary>
        InvalidMerchant = 209,

        /// <summary>
        /// Indicates the specified deal has not been registered with all parties involved in the transaction.
        /// </summary>
        UnregisteredDeal = 210,

        /// <summary>
        /// Indicates that a request to register a deal with the system could not be fulfilled because deal information was
        /// determined to be invalid.
        /// </summary>
        InvalidDeal = 211,

        /// <summary>
        /// Indicates that the merchant has already been registered with the system with another ID.
        /// </summary>
        MerchantAlreadyExists = 212,

        /// <summary>
        /// Indicates that the specified external merchant ID has already been associated with a different merchant by the
        /// external source.
        /// </summary>
        ExternalMerchantIdAlreadyExists = 213,

        /// <summary>
        /// Indicates that the specified deal has already been claimed by the specified user for the specified card.
        /// </summary>
        AlreadyClaimed = 214,

        /// <summary>
        /// Indicates that an operation cannot be completed because the specified card has been deactivated.
        /// </summary>
        CardDeactivated = 215,

        /// <summary>
        /// Indicates that a required parameter was not included in the request.
        /// </summary>
        ParameterCannotBeNull = 216,

        /// <summary>
        /// Indicates that a specified parameter contained an invalid value.
        /// </summary>
        InvalidParameter = 217,

        /// <summary>
        /// Indicates that a specified discount does not belong to the specified deal.
        /// </summary>
        SpecifiedDiscountDoesNotBelongToSpecifiedDeal = 219,

        /// <summary>
        /// Indicates that a batch operation encountered one or more errors.
        /// </summary>
        AggregateError = 220,

        /// <summary>
        /// Indicates that the user has already been registered with the system.
        /// </summary>
        UserAlreadyExists = 221,

        /// <summary>
        /// Indicates that the user has already been added using an unauthenticated account.
        /// </summary>
        UnauthenticatedUserAlreadyExists = 222,

        /// <summary>
        /// Indicates that a transaction has been disallowed due to business logic constraints.
        /// </summary>
        TransactionDisallowed = 223,

        /// <summary>
        /// Indicated that transaction is dupe of what is already registered.
        /// </summary>
        DuplicateTransaction = 224,

        /// <summary>
        /// Card number is not valid.
        /// </summary>
        InvalidCardNumber = 225,

        /// <summary>
        /// Prepaid and Corporate cards not allowed
        /// </summary>
        CorporateOrPrepaidCardError = 226,

        /// <summary>
        /// An attempt was made to register a card with an unsupported BIN.
        /// </summary>
        UnsupportedBin = 227,

        /// <summary>
        /// The caller is unauthorized to make the request.
        /// </summary>
        Unauthorized = 228,

        /// <summary>
        /// The transaction cannot complete due to insufficient funds.
        /// </summary>
        InsufficientFunds = 229,

        /// <summary>
        /// The expiration date specified is not valid for the operation.
        /// </summary>
        InvalidExpirationDate = 230,

        /// <summary>
        /// The requested distribution amount is not valid.
        /// </summary>
        InvalidDistributionAmount = 231,

        /// <summary>
        /// The card is registered to a different user.
        /// </summary>
        CardRegisteredToDifferentUser = 232,

        /// <summary>
        /// User attempted to add a card that's currently active in the account.
        /// </summary>
        CardStateUnchanged = 233,

        /// <summary>
        /// Visa allows only 5 credit card to be enrolled per user. If user try to add more than 5 Visa cards then this error code will be returned
        /// </summary>
        MaximumEnrolledCardsLimitReached = 234,

        /// <summary>
        /// The card has expired
        /// </summary>
        CardExpired = 235,

        /// <summary>
        /// If we try to enroll a user who is already enrolled with the partner like Visa
        /// </summary>
        UserAlreadyEnrolledWithPartner = 236,

        /// <summary>
        /// If user is not enrolled with Partner like Visa and we try to save a card for him
        /// </summary>
        UserNotEnrolledWithPartner = 237,

        //////////////////////////////////////////////////////////////////////////
        // Warnings that may require a higher level of scrutiny: 301..399.
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the client certificate presented in the request is not valid for the operation.
        /// </summary>
        InvalidClientCertificate = 301,

        /// <summary>
        /// Indicates that the purchase date and time specified in the redemption request is not valid.
        /// </summary>
        InvalidPurchaseDateTime = 302,

        /// <summary>
        /// Indicates that a deal that should be registered with the system could not be found.
        /// </summary>
        DealNotFound = 303,

        /// <summary>
        /// Indicates that the deal specified in the redemption request is not yet valid.
        /// </summary>
        DealNotYetValid = 304,

        /// <summary>
        /// Indicates that the authorization amount is not sufficient to meet the deal's minimum purchase requirement.
        /// </summary>
        DealThresholdNotMet = 305,

        /// <summary>
        /// Indicates that the specified partner merchant ID does not correspond to the merchant involved in the operation.
        /// </summary>
        InvalidPartnerMerchantId = 306,

        /// <summary>
        /// Indicates that the specified partner redeemed deal ID does not correspond to the redeemed deal involved in the
        /// operation.
        /// </summary>
        RedeemedDealNotFound = 307,

        /// <summary>
        /// Indicates that the specified partner event is different from the redemption event recorded for the redemption.
        /// </summary>
        RedemptionEventMismatch = 308,

        /// <summary>
        /// Indicates that the partner making the reversal call is not the same partner recorded as having redeemed the deal.
        /// </summary>
        PartnerMismatch = 309,

        /// <summary>
        /// Indicates that the specified partner deal ID does not correspond to the deal involved in the operation.
        /// </summary>
        PartnerDealIdMismatch = 310,

        /// <summary>
        /// Indicates that the partner card ID specified in the reversal request is not the same as the partner card ID recorded
        /// for the corresponding redemption.
        /// </summary>
        PartnerCardIdMismatch = 311,

        /// <summary>
        /// Indicates that the described redeemed deal did not correspond to any recorded redeemed deals.
        /// </summary>
        MatchingRedeemedDealNotFound = 312,

        /// <summary>
        /// Indicates that the described redeemed deal corresponded to multiple recorded redeemed deals.
        /// </summary>
        MultipleMatchingRedeemedDealsFound = 313,

        /// <summary>
        /// Indicates that a file used within a task was missing an expected record.
        /// </summary>
        FileMissingExpectedRecord = 314,

        /// <summary>
        /// Indicates that the specified partner user ID does not correspond to the user involved in the operation.
        /// </summary>
        PartnerUserIdMismatch = 315,

        /// <summary>
        /// Indicates that the specified partner card suffix does not correspond to the card involved in the operation.
        /// </summary>
        PartnerCardSuffixMismatch = 316,

        /// <summary>
        /// Indicates that the specified partner card ID did not correspond to a registered card.
        /// </summary>
        PartnerCardIdNotFound = 317,

        /// <summary>
        /// Indicates that no applicable deal was found during a partner's attempt to redeem a deal.
        /// </summary>
        NoApplicableDealFound = 318,

        /// <summary>
        /// Indicates that the record ended unexpectedly.
        /// </summary>
        UnexpectedEndOfRecord = 319,

        /// <summary>
        /// Indicates that an expected value was not found when parsing a file.
        /// </summary>
        ExpectedValueNotFound = 320,

        /// <summary>
        /// Indicates that a field was not properly formatted for the data type it must contain.
        /// </summary>
        InvalidValue = 321,

        /// <summary>
        /// Indicates that a record was found in a file out of its expected place.
        /// </summary>
        RecordOutOfPlace = 322,

        /// <summary>
        /// Indicates that a record type was found in a file more than once but should have been found only once.
        /// </summary>
        UnexpectedDuplicateRecordTypeFound = 323,

        /// <summary>
        /// Indciates that the reversal status specified during an operation did not match the reversal status of the
        /// corresponding redeemed deal.
        /// </summary>
        RedeemedDealReversalStateMismatch = 324,

        /// <summary>
        /// Indicates that the result of an operation would have set the credit status of a transaction to value previous to its
        /// current value. Because of this, the operation will result in no changes.
        /// </summary>
        CreditStatusTooAdvanced = 325,

        /// <summary>
        /// Indicates that a deal we expected to be successfully redeemed was rejected by our partner.
        /// </summary>
        RedeemedDealRejectedByPartner = 326,

        /// <summary>
        /// Indicates that the data submission (of file or anything else) to our partner from us was rejected.
        /// </summary>
        SubmissionRejected = 327,

        /// <summary>
        /// Indicates that the result of an operation would have set the payout status of a transaction to value previous to its
        /// current value. Because of this, the operation will result in no changes.
        /// </summary>
        PayoutStatusTooAdvanced = 328,

        /// <summary>
        /// Indicates that the deal has already expired within the partner's system.
        /// </summary>
        PartnerDealExpired = 329,

        /// <summary>
        /// Indicates the number of records indicated in the file did not match the number of records found in the file.
        /// </summary>
        RecordCountMismatch = 330,

        /// <summary>
        /// Indicates that the redeemed deal found was not an exact match for the specified criteria.
        /// </summary>
        RedeemedDealFoundIsInexactMatch = 331,

        /// <summary>
        /// Indicates that an incoming authorization or redemption call appears to refer to a transaction for which a record already exists.
        /// </summary>
        DuplicateRecordDetected = 332,

        /// <summary>
        /// Indicates that the card used in a transaction is not in the reward program that created the deal.
        /// </summary>
        CardInWrongRewardProgram = 333,

        /// <summary>
        /// Indicates that the user has exceeded the rolling year earn cap, and so is inelligible for the deal's earn.
        /// </summary>
        UserOverEarnLimit = 334,

        //////////////////////////////////////////////////////////////////////////
        // Errors that would not have caused application to crash: 401..499.
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates a task timed out and was aborted.
        /// </summary>
        TimedOut = 401,

        /// <summary>
        /// Indicates that a file involved in a task could not be found.
        /// </summary>
        FileNotFound = 402,

        /// <summary>
        /// Indicates that a job being run in the queue contains no payload.
        /// </summary>
        JobContainsNoPayload = 403,

        /// <summary>
        /// Indicates that a payload for the job being run in the queue is missing required data.
        /// </summary>
        JobPayloadMissingData = 404,

        /// <summary>
        /// Indicates that a scheduled job could not successfully execute
        /// </summary>
        JobExecutionError = 405,

        /// <summary>
        /// Indicates that a record processed during settlement is corrupt.
        /// </summary>
        CorruptSettlementRecord = 406,

        //////////////////////////////////////////////////////////////////////////
        // Errors that would have caused application to crash: 501..599.
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates the service invocation failed for an unknown reason.
        /// </summary>
        UnknownError = 501
    }
}