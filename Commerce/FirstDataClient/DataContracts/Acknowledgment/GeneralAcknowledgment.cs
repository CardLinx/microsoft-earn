//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    public class GeneralAcknowledgment
    {
        /// <summary>
        /// Gets or sets the Sales Deposit Amount 
        /// </summary>
        public decimal SalesDepositAmount { get; set; }

        /// <summary>
        /// Gets or sets the Credit Amount
        /// </summary>
        public decimal CreditAmount { get; set; }

        /// <summary>
        /// Gets or sets the Cash Advance Deposit Amount
        /// </summary>
        public decimal CashAdvanceDepositAmount { get; set; }

        /// <summary>
        /// Gets or sets the Acknowledgement Code
        /// </summary>
        public int AcknowledgementCode { get; set; }

        /// <summary>
        /// Gets or sets the Record Sequence number
        /// </summary>
        public long RecordSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the Submission Id
        /// </summary>
        public string SubmissionId { get; set; }

        /// <summary>
        /// Gets or sets Deposit Authorization Request Amount
        /// </summary>
        public decimal DepositAuthorizationRequestAmount { get; set; }

        /// <summary>
        /// Gets or sets the Cash Advance Deposit/Auth Amount
        /// </summary>
        public decimal CashAdvanceDepositAuthAmount { get; set; }

        /// <summary>
        /// Gets or sets the Followup Acknowledgment Indicator
        /// </summary>
        public string FollowUpAcknowledgmentIndicator { get; set; }
    }
}