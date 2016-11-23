//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    /// <summary>
    /// Response Codes for Amex APIs
    /// </summary>
    public static class AmexCardSyncResponseCode
    {
        /// <summary>
        /// Indicates that the card was added successfully.
        /// </summary>
        public const string AddCardSuccess = "RCCMP000";
        
        /// <summary>
        /// Invalid Card Number.
        /// </summary>
        public const string InvalidCardNumber = "RCCMP002";

        /// <summary>
        /// Indicates that the card has already been registered.
        /// </summary>
        public const string CardExistsWithDifferentToken = "RCCMP004";

        /// <summary>
        /// Indicates that the card token has already been registered for another card.
        /// </summary>
        public const string TokenUsedByAnotherCard = "RCCMP005";
        
        /// <summary>
        /// Indicates that the card and token pair have already been registered.
        /// </summary>
        public const string CardAndTokenPairAlreadyExists = "RCCMP007";
        
        /// <summary>
        /// Indicates that the card is not an Amex card.
        /// </summary>
        public const string NotAmexCard = "RCCMP014";

        /// <summary>
        /// Indicates that the card token 1 is invalid.
        /// </summary>
        public const string InvalidToken = "RCCMP016";

        /// <summary>
        /// Corporate or PrePaid Cards not allowed
        /// </summary>
        public const string CorporateOrPrepaidCardError = "RCCMP025";

        /// <summary>
        /// Indicates that the distribution channel is invalid.
        /// </summary>
        public const string InvalidDistributionChannel = "RCCMP031";

        /// <summary>
        /// Indicates that the Card doesn't have MR feature.
        /// </summary>
        public const string RewardsNotSupported = "RCCMP036";

        /// <summary>
        /// Indicates that the partner id is invalid.
        /// </summary>
        public const string InvalidPartnerId = "RCCMP050";

        /// <summary>
        /// Indicates that the message id is invalid.
        /// </summary>
        public const string InvalidMessageId = "RCCMP051";

        /// <summary>
        /// Indicates that the system cannot process the request. Try later.
        /// </summary>
        public const string SystemBusy1 = "RCCMP900";

        /// <summary>
        /// Indicates that the system cannot process the request. Try later.
        /// </summary>
        public const string SystemBusy2 = "RCCMP901";

        /// <summary>
        /// Indicates that the system cannot process the request. Try later.
        /// </summary>
        public const string SystemBusy3 = "RCCMP904";
    }
}