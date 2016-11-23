//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    /// <summary>
    /// Contains constants used throughout the clearing parsing classes.
    /// </summary>
    internal static class ClearingConstants
    {
        /// <summary>
        /// The length of the aggregate merchant ID field.
        /// </summary>
        internal const int AggregateMerchantIdLength = 6;

        /// <summary>
        /// The length of the bank account number field.
        /// </summary>
        internal const int BankAccountNumberLength = 19;

        /// <summary>
        /// The length of the bank customer number field.
        /// </summary>
        internal const int BankCustomerNumberLength = 30;

        /// <summary>
        /// The length of the bank net ref number field.
        /// </summary>
        internal const int BankNetRefNumberLength = 9;

        /// <summary>
        /// The length of the issuer ICA field.
        /// </summary>
        internal const int IssuerIcaLength = 6;

        /// <summary>
        /// The length of the location ID field.
        /// </summary>
        internal const int LocationIdLength = 9;

        /// <summary>
        /// The length of the member ICA field.
        /// </summary>
        internal const int MemberIcaLength = 11;

        /// <summary>
        /// The length of the merchant DBA name field.
        /// </summary>
        internal const int MerchantDbaNameLength = 60;

        /// <summary>
        /// The length of the merchant ID field.
        /// </summary>
        internal const int MerchantIdLength = 22;

        /// <summary>
        /// The length of the record count field.
        /// </summary>
        internal const int RecordCountFieldLength = 12;

        /// <summary>
        /// The length of the record type field.
        /// </summary>
        internal const int RecordTypeLength = 1;

        /// <summary>
        /// The length of short time fields.
        /// </summary>
        internal const int ShortTimeFieldLength = 4;

        /// <summary>
        /// The length of time fields.
        /// </summary>
        internal const int TimeFieldLength = 6;

        /// <summary>
        /// The length of the transaction amount field.
        /// </summary>
        internal const int TransactionAmountLength = 13;

        /// <summary>
        /// The length of the transaction sequence number field.
        /// </summary>
        internal const int TransactionSequenceNumberLength = 13;
    }
}