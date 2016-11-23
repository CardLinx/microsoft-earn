//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   Represents a partner transaction 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.Storage.PartnerTransactions
{
    using System;
    using System.Globalization;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Represents a partner transaction 
    /// </summary>
    public class PartnerTransactionsEntity : TableEntity
    {
        #region Constructors

        public PartnerTransactionsEntity()
        {
        }


        public PartnerTransactionsEntity(string transactionId, DateTime postingDate)
        {
            PartitionKey = transactionId;
            var inverseTimeKey = DateTime
                                  .MaxValue
                                  .Subtract(postingDate)
                                  .TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            PartitionKey = string.Format("{0}-{1}", transactionId, inverseTimeKey);
            RowKey = string.Format("{0}-{1}", inverseTimeKey, Guid.NewGuid());
            PostingDate = postingDate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Status of the request reporting transaction to partner
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Date of posting the request
        /// </summary>
        public DateTime PostingDate { get; set; }

        /// <summary>
        /// Request sent to partner
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// Response received from partner
        /// </summary>
        public string PartnerResponse { get; set; }

        #endregion

    }
}
