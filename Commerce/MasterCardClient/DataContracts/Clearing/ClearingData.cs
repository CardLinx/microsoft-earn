//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;

    /// <summary>
    /// Represents a data record for a MasterCard transaction clearing file.
    /// </summary>
    public class ClearingData
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
        /// Gets or sets the DBA name of the merchant at which the transaction occurred.
        /// </summary>
        public string MerchantDbaName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant at which the transaction occurred.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the location at which the transaction occurred.
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets the issuer ICA for the card used within the transaction.
        /// </summary>
        public string IssuerIca { get; set; }

        /// <summary>
        /// Gets or set the bank net reference number for the merchant at which the transaction occurred.
        /// </summary>
        public string BankNetRefNumber { get; set; }

        /// <summary>
        /// Gets or sets the bank customer number for the card used within the transaction.
        /// </summary>
        public string BankCustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets the aggregate ID for the merchant at which the transaction occurred.
        /// </summary>
        public string AggregateMerchantId { get; set; }
    }
}