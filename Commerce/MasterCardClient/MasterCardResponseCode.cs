//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    /// <summary>
    /// The response codes for MasterCard APIs.
    /// </summary>
    public static class MasterCardResponseCode
    {
        ///////////////////////////////////////////////////////////
        // General
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates the API call completed successfully.
        /// </summary>
        public const int Success = 0;

        ///////////////////////////////////////////////////////////
        // AddCard
        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Indicates that the card has already been registered.
        /// </summary>
        public const int BankAccountNumberExists = -45;

        /// <summary>
        /// Indicates that the request was invalid due to bad data.
        /// Parameter improperly named or with incorrect type
        /// </summary>
        public const int InvalidParameters = -1;

        /// <summary>
        /// Indicates that card number specified is incorrect.
        /// - Undocumented
        /// </summary>
        public const int InvalidCard = -44;

        /// <summary>
        /// Indicates that a message was not found within MasterCard's system
        /// </summary>
        /// <remarks>
        /// At this time, MasterCard may send this instead of UnsupportedBin for cards from non-US banks.
        /// </remarks>
        public const int MessageNotFound = -52;

        /// <summary>
        /// Indicates that the card is from a BIN that MasterCard cannot support.
        /// </summary>
        /// <remarks>
        /// At this time, this precludes any BIN from a non-US bank.
        /// </remarks>
        public const int UnsupportedBin = -170;

        /// <summary>
        /// Indicates that an unknown card addition error occurred.
        /// </summary>
        public const int UnknownError = -1001;

        ///////////////////////////////////////////////////////////
        // RemoveCard
        ///////////////////////////////////////////////////////////

    }
}