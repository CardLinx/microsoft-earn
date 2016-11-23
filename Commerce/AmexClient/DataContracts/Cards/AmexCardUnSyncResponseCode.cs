//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    /// <summary>
    /// Response Codes for Amex APIs
    /// </summary>
    public static class AmexCardUnSyncResponseCode
    {
        /// <summary>
        /// Indicates that the card was removed successfully.
        /// </summary>
        public const string RemoveCardSuccess = "RCCMU000";

        /// <summary>
        /// Indicates that the user do not have any linked cards.
        /// </summary>
        public const string NoLinkedCards = "RCCMU001";

        /// <summary>
        /// Indicates that distribution id is invalid.
        /// </summary>
        public const string InvalidDistributionChannel = "RCCMU004";

        /// <summary>
        /// Indicates card does not exist
        /// </summary>
        public const string CardDoesNotExist = "RCCMU006";

        /// <summary>
        /// Indicates that token 1 is invalid.
        /// </summary>
        public const string InvalidToken1 = "RCCMU009";

        /// <summary>
        /// Indicates that token 2 is invalid.
        /// </summary>
        public const string InvalidToken2 = "RCCMU010";

        /// <summary>
        /// Indicates that the partner id is invalid.
        /// </summary>
        public const string InvalidPartnerId = "RCCMU050";

        /// <summary>
        /// Indicates that the message id is invalid.
        /// </summary>
        public const string InvalidMessageId = "RCCMU051";

        /// <summary>
        /// Indicates that the system cannot process the request. Try later.
        /// </summary>
        public const string SystemBusy1 = "RCCMU900";
    }
}