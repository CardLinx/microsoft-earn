//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;

    /// <summary>
    /// Represents a detail record to be used when building a PTS file.
    /// </summary>
    public class DetailRecord
    {
        /// <summary>
        /// Gets or sets the offer ID for this detail record.
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets the acquirer reference number for this detail record.
        /// </summary>
        public string AcquirerReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the token for this detail record.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount for this detail record.
        /// </summary>
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the date on which the transaction within this detail record occurred.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the reference number for this detail record.
        /// </summary>
        public int ReferenceNumber { get; set; }
    }
}