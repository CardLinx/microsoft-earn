//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    /// <summary>
    /// The reason codes for FirstData APIs.
    /// </summary>
    public static class FirstDataReasonCode
    {
        ///////////////////////////////////////////////////////////
        // RedeemDeal
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the deal may not be redeemed, but the claim should be retained.
        /// </summary>
        public const string RedeemDealDeclinedRetain = "103";

        /// <summary>
        /// Indicates tha the deal may not be redeemed, and the claim should be deleted.
        /// </summary>
        public const string RedeemDealDeclinedDelete = "104";

        /// <summary>
        /// Indicates that the deal can be redeemed, and the claim should be retained.
        /// </summary>
        public const string ApprovedRetain = "105";

        /// <summary>
        /// Indicates that the specified deal cannot be redeemed, but the response contains an alternate deal that should be
        /// redeemed instead.
        /// </summary>
        public const string UseAlternateOfferThisTime = "106";

        /// <summary>
        /// Indicates the specified purchase date time was not valid.
        /// </summary>
        public const string RedeemDealInvalidPurchaseDateTime = "302";

        /// <summary>
        /// Indicates that an invalid card was specified in the redeem deal request.
        /// </summary>
        public const string RedeemDealInvalidCard = "309";

        /// <summary>
        /// Indicates that the redeem deal request specified an offer MID that does not match a merchant ID making the offer.
        /// </summary>
        public const string RedeemDealOfferMidMismatch = "316";

        ///////////////////////////////////////////////////////////
        // ReverseDeal
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the redeemed deal described in a reverse deal request could not be found.
        /// </summary>
        public const string RedeemedDealNotFound = "002";

        /// <summary>
        /// Indicates that no valid offer ID was specified in the reversal request.
        /// </summary>
        public const string NoValidOfferIdSpecified = "209";

        /// <summary>
        /// Indicates that the deal redemption described in the reversal request was for some reason not recognized.
        /// </summary>
        public const string UnrecognizedReversalAction = "210";

        /// <summary>
        /// Indicates that the merchant ID specified in the reversal request was not among the merchants offering the deal that
        /// was redeemed.
        /// </summary>
        public const string PartnerMerchantIdMismatch = "312";

        /// <summary>
        /// Indicates that the deal redemption specified a card that was not used during the redemption of the deal.
        /// </summary>
        public const string PartnerCardIdMismatch = "313";

        /// <summary>
        /// Indicates that the redemption type specified in the reversal request did not match the redemption type specified at
        /// the time of the redemption.
        /// </summary>
        public const string RedemptionTypeMismatch = "314";
    }
}