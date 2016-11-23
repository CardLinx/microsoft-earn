//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Runtime.Serialization;
    using Lomo.Commerce.DataContracts;

    /// <summary>
    /// Represents the transaction history for a user.
    /// </summary>
    public class TransactionHistory
    {
        /// <summary>
        /// Gets or sets the name of the business within this transaction.
        /// </summary>
        public string Business { get; set; }

        /// <summary>
        /// Gets or sets the status of the credit for this transaction.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be "Unprocessed".
        /// </remarks>
        public CreditStatus CreditStatus { get; set; }

        /// <summary>
        /// Gets or sets the date of the purchase within this transaction.
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount for this deal redemption.
        /// </summary>
        public int DiscountAmount { get; set; }
    }
}