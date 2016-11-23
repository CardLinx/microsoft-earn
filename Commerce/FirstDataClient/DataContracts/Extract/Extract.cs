//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a First Data extract file.
    /// </summary>
    public class Extract
    {
        /// <summary>
        /// Gets or sets the extract file's header.
        /// </summary>
        public ExtractHeader Header { get; set; }

        /// <summary>
        /// Gets the extract file's redemption detail records.
        /// </summary>
        public Collection<RedemptionDetail> RedemptionDetailRecords
        {
            get
            {
                return redemptionDetailRecords;
            }
        }
        private Collection<RedemptionDetail> redemptionDetailRecords = new Collection<RedemptionDetail>();

        /// <summary>
        /// Gets the extract file's settlement detail records.
        /// </summary>
        public Collection<SettlementDetail> SettlementDetailRecords
        {
            get
            {
                return settlementDetailRecords;
            }
        }
        private Collection<SettlementDetail> settlementDetailRecords = new Collection<SettlementDetail>();

        /// <summary>
        /// Gets the extract file's transaction notification records.
        /// </summary>
        public Collection<TransactionNotification> TransactionNotificationRecords
        {
            get
            {
                return transactionNotificationRecords;
            }
        }
        private Collection<TransactionNotification> transactionNotificationRecords = new Collection<TransactionNotification>();

        /// <summary>
        /// Gets or sets the extract file's footer.
        /// </summary>
        public ExtractFooter Footer { get; set; }
    }
}