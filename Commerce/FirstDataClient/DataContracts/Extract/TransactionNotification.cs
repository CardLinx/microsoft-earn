//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;

    /// <summary>
    /// Represents a transaction notification record for a First Data extract file.
    /// </summary>
    public class TransactionNotification
    {
        /// <summary>
        /// Gets or sets the ID of the provider to which the extract file belongs.
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the MID of the location at which the transaction took place.
        /// </summary>
        public string LocationMid { get; set; }

        /// <summary>
        /// Gets or sets ID of the merchant chain.
        /// </summary>
        public string ChainId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant corporate entity.
        /// </summary>
        public string CorporateId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the terminal on which the transaction was processed.
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// Gets or sets the bank marker.
        /// </summary>
        public string BankMarker { get; set; }

        /// <summary>
        /// Gets or sets the suffix of the card used in the transaction.
        /// </summary>
        public string CardSuffix { get; set; }

        /// <summary>
        /// Gets or sets the ID of the consumer who made the transaction.
        /// </summary>
        public string ConsumerId { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction notification represented by the record.
        /// </summary>
        public TransactionNotificationType TransactionNotificationType { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the transaction in EST.
        /// </summary>
        public DateTime TransactionDateTime { get; set; }

        /// <summary>
        /// The amount of the transaction.
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets the code of the currency used in the transaction.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the ID of the invoice through which the merchant tracks the transaction.
        /// </summary>
        public string InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets the First Data outgoing reference number.
        /// </summary>
        public string AcquirerReferenceNumber { get; set; }
    }
}