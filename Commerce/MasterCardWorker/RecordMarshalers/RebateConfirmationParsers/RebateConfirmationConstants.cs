//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    /// <summary>
    /// Contains constants used throughout the rebate confirmation parsing classes.
    /// </summary>
    internal static class RebateConfirmationConstants
    {
        /// <summary>
        /// The length of time fields.
        /// </summary>
        internal const int TimeFieldLength = 6;

        /// <summary>
        /// The length of the record type field.
        /// </summary>
        internal const int RecordTypeLength = 1;

        /// <summary>
        /// The length of the member ICA field.
        /// </summary>
        internal const int MemberIcaLength = 11;

        /// <summary>
        /// The length of the file name field.
        /// </summary>
        internal const int FileNameLength = 60;

        /// <summary>
        /// The length of the exception record count field.
        /// </summary>
        internal const int ExceptionRecordCountFieldLength = 12;

        /// <summary>
        /// The length of the success record count field.
        /// </summary>
        internal const int SuccessRecordCountFieldLength = 12;

        /// <summary>
        /// The length of the total processed record count field.
        /// </summary>
        internal const int TotalProcessedRecordCountFieldLength = 12;

        /// <summary>
        /// The length of the bank customer number field.
        /// </summary>
        internal const int BankCustomerNumberLength = 30;

        /// <summary>
        /// The length of the bank account number field.
        /// </summary>
        internal const int BankAccountNumberLength = 19;

        /// <summary>
        /// The length of the bank product code field.
        /// </summary>
        internal const int BankProductCodeLength = 20;

        /// <summary>
        /// The length of the transaction description field.
        /// </summary>
        internal const int TransactionDescriptionLength = 22;

        /// <summary>
        /// The length of the rebate amount field.
        /// </summary>
        internal const int RebateAmountLength = 13;

        /// <summary>
        /// The length of the exception reason code field.
        /// </summary>
        internal const int ExceptionReasonCodeLength = 1;

        /// <summary>
        /// The length of the exception reason description field.
        /// </summary>
        internal const int ExceptionReasonDescriptionLength = 30;

        /// <summary>
        /// The length of the transaction sequence number field.
        /// </summary>
        internal const int TransactionSequenceNumberLength = 13;
    }
}