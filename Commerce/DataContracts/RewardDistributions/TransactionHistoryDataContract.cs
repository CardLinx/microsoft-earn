//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the transaction history for a user.
    /// </summary>
    [DataContract]
    public class TransactionHistoryDataContract
    {
        /// <summary>
        /// Gets or sets the name of the business within this transaction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "business")]
        public string Business { get; set; }

        /// <summary>
        /// Gets or sets the status of the credit for this transaction.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be "Unprocessed".
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "credit_status")]
        public string CreditStatus { get; set; }

        /// <summary>
        /// Gets or sets the date of the purchase within this transaction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "purchase_date")]
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount for this deal redemption.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_amount")]
        public decimal DiscountAmount { get; set; }
    }
}