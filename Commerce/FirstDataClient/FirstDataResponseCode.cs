//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    /// <summary>
    /// The response codes for FirstData APIs.
    /// </summary>
    public static class FirstDataResponseCode
    {
        ///////////////////////////////////////////////////////////
        // General
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates the API call completed successfully.
        /// </summary>
        public const string Success = "000";

        /// <summary>
        /// Indicates that the request was invalid due to missing data.
        /// </summary>
        public const string RequestMissingData = "400";

        /// <summary>
        /// Indicates that the request was invalid due to bad data.
        /// </summary>
        public const string RequestContainsBadData = "422";

        ///////////////////////////////////////////////////////////
        // AddCard
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the card was added successfully.
        /// </summary>
        public const string AddCardSuccess = "700";

        /// <summary>
        /// Indicates that a token could not be generated for the card.
        /// </summary>
        public const string CouldNotGenerateCardToken = "710";

        /// <summary>
        /// Indicates that the card has already been registered.
        /// </summary>
        public const string CardAlreadyExists = "750";

        /// <summary>
        /// Indicates that a field in the add card request could not be parsed.
        /// </summary>
        public const string AddCardFieldParseError = "760";

        /// <summary>
        /// Indicates that an unknown card addition error occurred.
        /// </summary>
        public const string UnknownAddCardError = "799";

        ///////////////////////////////////////////////////////////
        // RemoveCard
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the card was removed successfully.
        /// </summary>
        public const string RemoveCardSuccess = "700";

        /// <summary>
        /// Indicates that the card does not exist.
        /// </summary>
        public const string CardDoesNotExist = "755";

        /// <summary>
        /// Indicates that a field in the remove card request could not be parsed.
        /// </summary>
        public const string RemoveCardFieldParseError = "760";

        /// <summary>
        /// Indicates that an unknown card removal error occurred.
        /// </summary>
        public const string UnknownRemoveCardError = "799";

        ///////////////////////////////////////////////////////////
        // RegisterDeal
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the deal was registered successfully.
        /// </summary>
        public const string RegisterDealSuccess = "800";

        /// <summary>
        /// Indicates that the deal was registered successfully with some of the specified merchants.
        /// </summary>
        public const string RegisterDealPartialSuccess = "805";

        /// <summary>
        /// Indicates that a token could not be generated for the deal.
        /// </summary>
        public const string CouldNotGenerateDealToken = "810";

        /// <summary>
        /// Indicates that none of the specified merchants were valid within First Data.
        /// </summary>
        public const string NoValidMerchantIds = "820";

        /// <summary>
        /// Indicates that the deal has already been registered.
        /// </summary>
        public const string DealAlreadyRegistered = "850";

        /// <summary>
        /// Indicates that a field in the register deal request could not be parsed.
        /// </summary>
        public const string RegisterDealFieldParseError = "860";

        /// <summary>
        /// Indicates that an unknown deal registration error occurred.
        /// </summary>
        public const string UnknownRegisterDealError = "899";

        ///////////////////////////////////////////////////////////
        // ClaimDeal
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the deal was claimed successfully.
        /// </summary>
        public const string ClaimDealSuccess = "800";

        /// <summary>
        /// Indicates that the deal has already been claimed.
        /// </summary>
        public const string DealAlreadyClaimed = "850";

        /// <summary>
        /// Indicates that the deal cannot be claimed because it has already expired.
        /// </summary>
        public const string DealExpired = "856";

        /// <summary>
        /// Indicates that a field in the claim deal request could not be parsed.
        /// </summary>
        public const string ClaimDealFieldParseError = "860";

        /// <summary>
        /// Indicates that an unknown deal registration error occurred.
        /// </summary>
        public const string UnknownClaimDealError = "899";

        ///////////////////////////////////////////////////////////
        // Ping
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the Ping API call encountered an error on the server.
        /// </summary>
        public const string ServerError = "500";
    }
}