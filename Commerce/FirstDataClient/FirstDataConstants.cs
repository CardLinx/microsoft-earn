//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;

    /// <summary>
    /// Contains constants used throughout First Data partner client implementation.
    /// </summary>
    public static class FirstDataConstants
    {
        /// <summary>
        /// Code for the First Data north data center.
        /// </summary>
        public const string BECode = "0001";

        /// <summary>
        /// The token type to use in First Data calls.
        /// </summary>
        public const string TokenType = "PUB0";

        /// <summary>
        /// Microsoft's provider ID in First Data's system.
        /// </summary>
        public const string MicrosoftProviderId = "MSFT";

        /// <summary>
        /// Microsoft's publisher ID in First Data's system.
        /// </summary>
        /// <remarks>
        /// Maximum of 4 characters.
        /// </remarks>
        public const string MicrosoftPubId = "0000";

        /// <summary>
        /// Microsoft's publisher name in First Data's system.
        /// </summary>
        /// <remarks>
        /// Maximum of 20 characters.
        /// </remarks>
        public const string MicrosoftPubName = "YOUR_PUB_NAME_HERE";

        /// <summary>
        /// The Purchased consumer offer type.
        /// </summary>
        public const string OffTypeConsumerClipped = "03";

        /// <summary>
        /// The value that indicates a reversal request was to reverse an already redeemed deal.
        /// </summary>
        public const string ReverseRedeemedDealIndicator = "000";

        /// <summary>
        /// The value that indicates a reversal request was to process a time out received by First Data when previously
        /// attempting to redeem a deal.
        /// </summary>
        public const string ProcessRedemptionTimeoutIndicator = "001";

        /// <summary>
        ///  The value that indicates an offer must be redeemed as a settlement-based redemption only.
        /// </summary>
        public const string SettlementBasedRedemptionMode = "SBR";

        /// <summary>
        /// The value that indicates that the transaction was attempted using a debit card / PIN combination.
        /// </summary>
        public const string PinDebitTenderType = "PIN DEBIT";
    }
}