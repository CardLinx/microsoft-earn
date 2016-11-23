//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    internal static class AcknowledgmentConstants
    {
        /// <summary>
        /// The length of the record type field.
        /// </summary>
        internal const int RecordTypeLength = 1;

        /// <summary>
        /// The length of Token Field 
        /// </summary>
        internal const int TokenLength = 16;

        /// <summary>
        /// The length of Transaction Code field
        /// </summary>
        internal const int TransactionCodeLength = 1;

        /// <summary>
        /// The length of Transaction Amount field
        /// </summary>
        internal const int TransactionAmountLength = 8;

        /// <summary>
        /// The length of Sales Deposit Amount field
        /// </summary>
        internal const int SalesDepositAmountLength = 9;

        /// <summary>
        /// The length of Credit Amount field
        /// </summary>
        internal const int CreditAmountLength = 9;

        /// <summary>
        /// The length of Cash Advance Deposit Amount field
        /// </summary>
        internal const int CashAdvanceDepositAmountLength = 9;

        /// <summary>
        /// The length of Deposit Authorization Request Amount field
        /// </summary>
        internal const int DepositAuthorizationRequestAmountLength = 9;

        /// <summary>
        /// The length of Cash Advance Deposit Auth Request Amount field
        /// </summary>
        internal const int CashAdvanceDepositAuthRequestAmountLength = 9;

        /// <summary>
        /// The length of Transaction Date field
        /// </summary>
        internal const int TransactionDateLength = 4;

        /// <summary>
        /// The length of Authorization Code field
        /// </summary>
        internal const int AuthorizationCodeLength = 6;

        /// <summary>
        /// The length of Authorization Date field
        /// </summary>
        internal const int AuthorizationDateLength = 4;

        /// <summary>
        /// The length of Acknowledgment Code field
        /// </summary>
        internal const int AcknowledgmentCodeLength = 4;

        /// <summary>
        /// The length of Reference Number field
        /// </summary>
        internal const int ReferenceNumberLength = 8;

        /// <summary>
        /// The length of Record Sequence Number field
        /// </summary>
        internal const int RecordSequenceNumberLength = 6;

        /// <summary>
        /// The length of Submission ID field
        /// </summary>
        internal const int SubmissionID = 9;

        /// <summary>
        /// The length of Followup Ack Indicator field
        /// </summary>
        internal const int FollowupAckIndicatorLength = 1;

        /// <summary>
        /// The length of Merchant Descriptor field
        /// </summary>
        internal const int MerchantDescriptorLength = 21;

        /// <summary>
        /// The length of Filler field
        /// </summary>
        internal const int FillerLength = 2;

        /// <summary>
        /// The length of Five Space Filler field
        /// </summary>
        internal const int FiveSpaceFillerLength = 5;

        /// <summary>
        /// The length of Eight Space Filler field
        /// </summary>
        internal const int EightSpaceFillerLength = 8;

        /// <summary>
        /// The length of Single Space Filler field
        /// </summary>
        internal const int SingleSpaceFillerLength = 1;

        /// <summary>
        /// The length of Card Type field
        /// </summary>
        internal const int CardTypeLength = 1;

        /// <summary>
        /// The ack code for type B general ack records, which signifies successful submission.
        /// </summary>
        internal const int SuccessfulSubmissionAckCode = 98;

        /// <summary>
        /// The ack code for type A detail ack records, which signifies successful redemption.
        /// </summary>
        internal const int SuccessfulRedemptionAckCode = 8;

        /// <summary>
        /// The length of the time field.
        /// </summary>
        internal const int TimeFieldLength = 10;
    }
}