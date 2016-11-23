//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    /// <summary>
    /// The different types of transactions a record may represent.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Indicates the transaction record represents a real-time redemption event.
        /// </summary>
        RealTimeRedemption,

        /// <summary>
        /// Indicates the transaction record represents a settlement redemption event.
        /// </summary>
        SettlementRedemption,

        /// <summary>
        /// Indicates the transaction record represents the reversal of a real-time redemption due to a timeout during the
        /// redemption attempt.
        /// </summary>
        RealTimeTimeoutReversal,

        /// <summary>
        /// Indicates the transaction record represents the reversal of a real-time redemption due to reasons other than a
        /// timeout during the redemption attempt.
        /// </summary>
        RealTimeNonTimeoutReversal,

        /// <summary>
        /// Indicates the transaction record represents the reversal of a settlement redemption due to a timeout during the
        /// redemption attempt.
        /// </summary>
        SettlementTimeoutReversal,

        /// <summary>
        /// Indicates the transaction record represents the reversal of a settlement redemption due to reasons other than a
        /// timeout during the redemption attempt.
        /// </summary>
        SettlementNonTimeoutReversal
    }
}