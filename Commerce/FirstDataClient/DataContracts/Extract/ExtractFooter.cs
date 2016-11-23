//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;

    /// <summary>
    /// Represents the footer for a First Data extract file.
    /// </summary>
    public class ExtractFooter
    {
        /// <summary>
        /// Gets or sets the ID of the provider to which the extract file belongs.
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the extract file was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the number of header records in the extract file.
        /// </summary>
        public long NumberOfHeaderRecords { get; set; }

        /// <summary>
        /// Gets or sets the number of redemption records in the extract file.
        /// </summary>
        public long NumberOfRedemptionRecords { get; set; }

        /// <summary>
        /// Gets or sets the total amount for all redemption records.
        /// </summary>
        public decimal TotalRedemptionRecordAmount { get; set; }

        /// <summary>
        /// Gets or sets the number of settlement records in the extract file.
        /// </summary>
        public long NumberOfSettlementRecords { get; set; }

        /// <summary>
        /// Gets or sets the total amount for all settlement records.
        /// </summary>
        public decimal TotalSettlementRecordAmount { get; set; }

        /// <summary>
        /// Gets or sets the number of transaction notification records in the extract file.
        /// </summary>
        public long NumberOfTransactionNotificationRecords { get; set; }

        /// <summary>
        /// Gets or sets the total amount for all transaction notification records in the extract file.
        /// </summary>
        public decimal TotalTransactionNotificationAmount { get; set; }

        /// <summary>
        /// Gets or sets the number of footer records in the extract file.
        /// </summary>
        public long NumberOfFooterRecords { get; set; }
    }
}