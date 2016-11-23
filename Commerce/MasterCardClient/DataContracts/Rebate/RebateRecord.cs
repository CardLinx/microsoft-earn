//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;

    /// <summary>
    /// Represents a record for a MasterCard transaction rebate file.
    /// </summary>
    public class RebateRecord
    {
        /// <summary>
        /// Gets or sets the sequence of the transaction within a calendar day for the card involved in the transaction.
        /// </summary>
        public string TransactionSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the amount of the transaction.
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the transaction occurred.
        /// </summary>
        /// <remarks>
        /// Nominally, this is expressed in the time zone at which the transaction occurred.
        /// </remarks>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the rebate amount for the transaction.
        /// </summary>
        public decimal RebateAmount { get; set; }

        /// <summary>
        /// Gets or sets the description for the transaction that will appear on customer statements.
        /// </summary>
        public string TransactionDescription { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant at which the transaction occurred.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the issuer ICA for the card used within the transaction.
        /// </summary>
        public string IssuerIca { get; set; }

        /// <summary>
        /// Gets or sets the bank customer number (i.e. the PartnerCardId) for the card to associate with the merchant set ID.
        /// </summary>
        public string BankCustomerNumber { get; set; }
    }
}