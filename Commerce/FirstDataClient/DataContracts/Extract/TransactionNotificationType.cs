//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    /// <summary>
    /// The different types of transaction notifications a record may represent.
    /// </summary>
    public enum TransactionNotificationType
    {
        /// <summary>
        /// Indicates the transaction notification record represents a sale.
        /// </summary>
        Sale,

        /// <summary>
        /// Indicates the transaction notification record represents a reversal of a sale.
        /// </summary>
        Reversal,

        /// <summary>
        /// Indicates the transaction notification record represents a credit.
        /// </summary>
        Credit
    }
}