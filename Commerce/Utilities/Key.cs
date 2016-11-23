//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Utilities
{
    /// <summary>
    /// Contains a reposity of context keys used to refer to context items unambiguously.
    /// </summary>
    public enum Key
    {
        /// <summary>
        /// The key under which authorization request can be found.
        /// </summary>
        Authorization,

        /// <summary>
        /// The key under which the authorization amount can be found.
        /// </summary>
        AuthorizationAmount,

        /// <summary>
        /// The key under which the API call timer can be found.
        /// </summary>
        CallTimer,

        /// <summary>
        /// The key under which the eligible user ID can be found.
        /// </summary>
        EligibleUserId,

        /// <summary>
        /// The key under which the card can be found.
        /// </summary>
        Card,

        /// <summary>
        /// The key under which the brand of the card can be found.
        /// </summary>
        CardBrand,

        /// <summary>
        /// The key under which the ID of the card can be found.
        /// </summary>
        CardId,

        /// <summary>
        /// The key under which the claim deal payload can be found.
        /// </summary>
        ClaimDealPayload,

        /// <summary>
        /// The key under which the claimed deal can be found.
        /// </summary>
        ClaimedDeal,

        /// <summary>
        /// The key under which the correlation ID can be found.
        /// </summary>
        CorrelationId,

        /// <summary>
        /// The key under which the create unauthenticated account flag can be found.
        /// </summary>
        CreateUnauthenticatedAccount,

        /// <summary>
        /// The key under which the credit status to use can be found.
        /// </summary>
        CreditStatus,

        /// <summary>
        /// The key under which the action to be performed on deal can be found.
        /// </summary>
        DealAction,

        /// <summary>
        /// The key under which the deal can be found.
        /// </summary>
        Deal,

        /// <summary>
        /// The key under which the deal data contract can be found.
        /// </summary>
        DealDataContract,

        /// <summary>
        /// The key under which the discount summary for a deal can be found.
        /// </summary>
        DealDiscountSummary,

        /// <summary>
        /// The key under which the deal reports query can be found.
        /// </summary>
        DealReportsQuery,

        /// <summary>
        /// The key under which the disallowed reason can be found.
        /// </summary>
        DisallowedReason,

        /// <summary>
        /// The key under which the discount start date can be found.
        /// </summary>
        DiscountStartDate,

        /// <summary>
        /// The key under which the discount end date can be found.
        /// </summary>
        DiscountEndDate,

        /// <summary>
        /// The key under which the distribution amount can be found.
        /// </summary>
        DistributionAmount,

        /// <summary>
        /// The key under which the deal ID can be found.
        /// </summary>
        DealId,

        /// <summary>
        /// The key under which the deals batch can be found.
        /// </summary>
        DealBatch,

        /// <summary>
        /// The key under which the detail acknowledgment record can be found.
        /// </summary>
        DetailAcknowledgment,

        /// <summary>
        /// The key under which the enumerable containing discount IDs can be found.
        /// </summary>
        DiscountIds,

        /// <summary>
        /// The key under which the e-mail address can be found.
        /// </summary>
        EmailAddress,

        /// <summary>
        /// The key under which the ending day for a query can be found.
        /// </summary>
        EndDay,

        /// <summary>
        /// The key under which the first Earn reward amount can be found.
        /// </summary>
        FirstEarnRewardAmount,

        /// <summary>
        /// The key under which the first earn reward explanation can be found.
        /// </summary>
        FirstEarnRewardExplanation,

        /// <summary>
        /// The key under which the deal ID can be found.
        /// </summary>
        GlobalDealId,

        /// <summary>
        /// The key under which the global merchant ID can be found.
        /// </summary>
        GlobalMerchantID,

        /// <summary>
        /// The key under which the global offer ID can be found.
        /// </summary>
        GlobalOfferID,

        /// <summary>
        /// The key under which the global provider ID can be found.
        /// </summary>
        GlobalProviderID,

        /// <summary>
        /// The key under which the user ID can be found.
        /// </summary>
        GlobalUserId,

        /// <summary>
        /// The key under which the value that indicates whether partner merchants are / should be included in the Merchant object can be found.
        /// </summary>
        IncludePartnerMerchants,

        /// <summary>
        /// The key under which the initial deal can be found.
        /// </summary>
        InitialDeal,

        /// <summary>
        /// The key under which the initial user can be found.
        /// </summary>
        InitialUser,

        /// <summary>
        /// The key under which the merchant can be found.
        /// </summary>
        Merchant,

        /// <summary>
        /// The key under which the merchant data contract can be found.
        /// </summary>
        MerchantDataContract,

        /// <summary>
        /// The key under which the merchant ID can be found.
        /// </summary>
        MerchantId,

        /// <summary>
        /// The key under which a list of merchant IDs can be found.
        /// </summary>
        MerchantIds,

        /// <summary>
        /// The key under which the merchant name can be found.
        /// </summary>
        MerchantName,

        /// <summary>
        /// The key under which the collection of merchant records can be found.
        /// </summary>
        MerchantRecords,

        /// <summary>
        /// The key under which the merchant report query can be found.
        /// </summary>
        MerchantReportQuery,

        /// <summary>
        /// The key under which the Microsoft Store voucher distribution history can be found.
        /// </summary>
        MssVoucherDistributionHistory,

        /// <summary>
        /// The key under which the new card info can be found.
        /// </summary>
        NewCardInfo,

        /// <summary>
        /// The key under which notes can be found.
        /// </summary>
        Notes,

        /// <summary>
        /// The key under which the NotifyClaimedDealPayload can be found.
        /// </summary>
        NotifyClaimedDealPayload,

        /// <summary>
        /// The key under which the offer can be found.
        /// </summary>
        Offer,

        /// <summary>
        /// The key under which the outlet partner merchant ID can be found.
        /// </summary>
        OutletPartnerMerchantId,

        /// <summary>
        /// The key under which outstanding redeemed deals can be found.
        /// </summary>
        OutstandingRedeemedDeals,

        /// <summary>
        /// The key under which the PAN token can be found.
        /// </summary>
        PanToken,

        /// <summary>
        /// The key under which the ID of a parent deal can be found.
        /// </summary>
        ParentDealId,

        /// <summary>
        /// The key under which the partner can be found.
        /// </summary>
        Partner,

        /// <summary>
        /// The key under which the partner card ID can be found.
        /// </summary>
        PartnerCardId,

        /// <summary>
        /// The key under which the partner claimed deal ID can be found.
        /// </summary>
        PartnerClaimedDealId,

        /// <summary>
        /// The key under which partner data can be found.
        /// </summary>
        PartnerData,

        /// <summary>
        /// They key under which the partner deal ID can be found.
        /// </summary>
        PartnerDealId,

        /// <summary>
        /// The key under which the partner merchant ID can be found.
        /// </summary>
        PartnerMerchantId,

        /// <summary>
        /// The key under which the partner merchant ID type can be found.
        /// </summary>
        PartnerMerchantIdType,

        /// <summary>
        /// The key under which the partner offer merchant ID can be found.
        /// </summary>
        PartnerOfferMerchantId,

        /// <summary>
        /// The key under which the partner redeemed deal ID can be found.
        /// </summary>
        PartnerRedeemedDealId,

        /// <summary>
        /// The key under which the partner reference number can be found.
        /// </summary>
        PartnerReferenceNumber,

        /// <summary>
        /// The key under which the partner user ID can be found.
        /// </summary>
        PartnerUserId,

        /// <summary>
        /// They key under which the value that indicates whether an entity was previously registered can be found.
        /// </summary>
        PreviouslyRegistered,

        /// <summary>
        /// The key under which the provider object can be found.
        /// </summary>
        Provider,

        /// <summary>
        /// The key under which the purchase date and time can be found.
        /// </summary>
        PurchaseDateTime,

        /// <summary>
        /// The key under which the directive to queue a job can be found.
        /// </summary>
        QueueJob,

        /// <summary>
        /// The key under which the referred user first Earn reward amount can be found.
        /// </summary>
        ReferredUserFirstEarnRewardAmount,

        /// <summary>
        /// The key under which the referred user first earn reward explanation can be found.
        /// </summary>
        ReferredUserFirstEarnRewardExplanation,

        /// <summary>
        /// The key under which remaining funds can be found.
        /// </summary>
        RemainingFunds,

        /// <summary>
        /// The key under which MasterCard rebate confirmation data can be found.
        /// </summary>
        RebateConfirmationData,

        /// <summary>
        /// The key under which the redeemed deal can be found.
        /// </summary>
        RedeemedDeal,

        /// <summary>
        /// The key under which the redeemed deal ID can be found.
        /// </summary>
        RedeemedDealId,
        
        /// <summary>
        /// The key under which information on the deal that was actually redeemed can be found.
        /// </summary>
        RedeemedDealInfo,

        /// <summary>
        /// The key under which the redemption event can be found.
        /// </summary>
        RedemptionEvent,

        /// <summary>
        /// The key under which the collection of reference numbers can be found.
        /// </summary>
        ReferenceNumbers,

        /// <summary>
        /// The key under which the referral added flag can be found.
        /// </summary>
        ReferralAdded,

        /// <summary>
        /// The key under which the referral data contract can be found.
        /// </summary>
        ReferralDataContract,

        /// <summary>
        /// The key under which the referral event can be found.
        /// </summary>
        ReferralEvent,

        /// <summary>
        /// The key under which the referral type can be found.
        /// </summary>
        ReferralType,

        /// <summary>
        /// The key under which the referral type code can be found.
        /// </summary>
        ReferralTypeCode,

        /// <summary>
        /// The key under which the referred user ID can be found.
        /// </summary>
        ReferredUserId,

        /// <summary>
        /// The key under which the referrer ID can be found.
        /// </summary>
        ReferrerId,

        /// <summary>
        /// The key under which the referrer type can be found.
        /// </summary>
        ReferrerType,

        /// <summary>
        /// The key under which the reimbursement tender can be found.
        /// </summary>
        ReimbursementTender,

        /// <summary>
        /// The key under which the API call request can be found.
        /// </summary>
        Request,

        /// <summary>
        /// The key under which the requested CRUD operation can be found.
        /// </summary>
        RequestedCrudOperation,

        /// <summary>
        /// The key under which the API call response can be found.
        /// </summary>
        Response,

        /// <summary>
        /// The key under which the API call result code can be found.
        /// </summary>
        ResultCode,

        /// <summary>
        /// The key under which the API call result summary can be found.
        /// </summary>
        ResultSummary,

        /// <summary>
        /// The key under which the reversed flag can be found.
        /// </summary>
        Reversed,

        /// <summary>
        /// The key under which the reverse redeemed deal info can be found.
        /// </summary>
        ReverseRedeemedDealInfo,

        /// <summary>
        /// The key under which the reward ID can be found.
        /// </summary>
        RewardId,

        /// <summary>
        /// The key under which the reward payout ID can be found.
        /// </summary>
        RewardPayoutId,

        /// <summary>
        /// The key under which the reward payout record can be found.
        /// </summary>
        RewardPayoutRecord,

        /// <summary>
        /// The key under which the reward payout status can be found.
        /// </summary>
        RewardPayoutStatus,

        /// <summary>
        /// The key under which the reward recipient can be found.
        /// </summary>
        RewardRecipient,

        /// <summary>
        /// The key under which the settlement detail record can be found.
        /// </summary>
        SettlementDetail,

        /// <summary>
        /// The key under which the service referral type data contract can be found.
        /// </summary>
        ServiceReferralTypeDataContract,

        /// <summary>
        /// The key under which the starting day for a query can be found.
        /// </summary>
        StartDay,

        /// <summary>
        /// The key under which the task completion source can be found.
        /// </summary>
        TaskCompletionSource,

        /// <summary>
        /// The key under which tracked redemption reward IDs can be found.
        /// </summary>
        TrackedRedemptionRewardsIds,

        /// <summary>
        /// The key under which the MasterCard transaction can be found.
        /// </summary>
        Transaction,

        /// <summary>
        /// The key under which the user can be found.
        /// </summary>
        User,

        /// <summary>
        /// The key under which the user ID can be found.
        /// </summary>
        UserId,

        /// <summary>
        /// The key under which the value indicating if the user if claiming his first deal can be found.
        /// </summary>
        UserFirstClaimedDeal,

        /// <summary>
        /// The key under which the value indicating if the user is claiming a deal for the first time can be found.
        /// </summary>
        UserNewDealClaimed,

        /// <summary>
        /// The key under which the user location data can be found.
        /// </summary>
        UserLocation,

        // The key under which the V2 claim deal info can be found.
        V2ClaimDealInfo,

        /// <summary>
        /// The key under which the voucher expiration date (in UTC) can be found.
        /// </summary>
        VoucherExpirationUtc,

        /// <summary>
        /// The key under which Sequence Name can be found
        /// </summary>
        SequenceName,

        /// <summary>
        /// Key under which card token can be found
        /// </summary>
        CardToken,

        /// <summary>
        /// Key under which the reward program making the request to commerce service can be found
        /// </summary>
        RewardProgramType,

        /// <summary>
        /// Key under which the last successful run date for reward network transaction report can be found
        /// </summary>
        RewardNetworkReportLastRunDate,

        /// <summary>
        /// Key under which the Batch size for getting outstanding partner redeemed deals can be found
        /// </summary>
        BatchSize,

        /// <summary>
        /// Key under which the Discount amount for deal can be found
        /// </summary>
        DealDiscountAmount,

        /// <summary>
        /// The key under which information on the outstanding reedeemed deal can be found.
        /// </summary>
        OutstandingRedeemedDealInfo,

        /// <summary>
        /// Key under which Reedemed deal process status can be found
        /// </summary>
        RedeemedDealsProcessStatus,

        /// <summary>
        /// Key under which transaction settlement date can be found
        /// </summary>
       TransactionSettlementDate,

        /// <summary>
        /// Key under which transaction credit approved date can be found
        /// </summary>
        TransactionCreditApprovedDate
    }
}