//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;

    /// <summary>
    /// Represents a data record for a MasterCard transaction rebate confirmation file.
    /// </summary>
    public class RebateConfirmationData
    {
        /// <summary>
        /// Gets or sets the bank customer number for the card used within the transaction.
        /// </summary>
        public string BankCustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets the bank product code for the transaction.
        /// </summary>
        public string BankProductCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the transaction.
        /// </summary>
        public string TransactionDescription { get; set; }

        /// <summary>
        /// Gets or sets the amount of the rebate.
        /// </summary>
        public decimal RebateAmount { get; set; }

        /// <summary>
        /// Gets or sets the reason code for the exception.
        /// </summary>
        public ExceptionReasonCode ExceptionReasonCode { get; set; }

        /// <summary>
        /// Gets or sets the explanation for the exception reason code.
        /// </summary>
        public string ExceptionReasonDescription { get; set; }

        /// <summary>
        /// Gets or sets the date the rebate file was sent.
        /// </summary>
        public DateTime RebateFileSendDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction sequence number.
        /// </summary>
        public string TransactionSequenceNumber { get; set; }
    }
}