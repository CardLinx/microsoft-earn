//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Amex's TLOG file representation
    /// </summary>
    public class TransactionLogFile
    {
        /// <summary>
        /// Gets or sets the Transaction Log File's Header
        /// </summary>
        public TransactionLogHeader Header { get; set; }

        /// <summary>
        /// Gets Transaction Log File's records
        /// </summary>
        public Collection<TransactionLogDetail> TransactionLogRecords
        {
            get
            {
                return transactionLogRecords;
            }
        }
        private Collection<TransactionLogDetail> transactionLogRecords = new Collection<TransactionLogDetail>();

        /// <summary>
        /// Gets or sets the Transaction Log File's Trailer
        /// </summary>
        public TransactionLogTrailer Trailer { get; set; }
    }
}