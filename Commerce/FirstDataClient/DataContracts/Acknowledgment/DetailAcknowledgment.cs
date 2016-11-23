//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;

    /// <summary>
    /// Represents a detail record for a First Data acknowledgement file.
    /// </summary>
    public class DetailAcknowledgment
    {
        /// <summary>
        /// Gets or sets the Token for this detail record.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount for this detail record.
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets the date of the transaction for this detail record.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the authorization code for this detail record.
        /// </summary>
        public string AuthorizationCode { get; set; }

        /// <summary>
        /// Gets or sets the authorization date for this detail record.
        /// </summary>
        public string AuthorizationDate { get; set; }

        /// <summary>
        /// Gets or sets the acknowledgement code for this detail record.
        /// </summary>
        public int AcknowledgementCode { get; set; }

        /// <summary>
        /// Gets or sets the reference number for this detail record.
        /// </summary>
        public long ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the record sequence number for this detail record as it was initially specified in the corresponding
        /// PTS record.
        /// </summary>
        public long RecordSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the descriptor for the merchant for this detail record.
        /// </summary>
        public string MerchantDescriptor { get; set; }
    }
}