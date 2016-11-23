//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;

    /// <summary>
    /// Contains constants used throughout MasterCard partner client implementation.
    /// </summary>
    public static class MasterCardConstants
    {
        /// <summary>
        /// Unique id assigned to Miscrosoft.
        /// </summary>
        public const int SourceId = 0; // TODO_YOUR_ID_HERE

        /// <summary>
        /// MasterCard internal identifier for microsoft on completion of initial setup.
        /// </summary>
        public const string MemberIca = "TODO_YOUR_ICA_ID_HERE";

        /// <summary>
        /// MasterCard client identifier.
        /// </summary>
        public const string BankProductCode = "TODO_YOUR_BANK_PRODUCT_CODE_HERE";

        /// <summary>
        /// MRS program identifier.
        /// </summary>
        public const string ProgramIdentifier = "TODO_YOUR_PROGRAM_IDENTIFIER_HERE";

        /// <summary>
        /// Enrollment code.
        /// </summary>
        public const string EnrollmentTypeCode = "CCA";

        /// <summary>
        /// Account Status Code for active account.
        /// </summary>
        public const string AccountStatusActive= "001";

        /// <summary>
        /// Account Status Code for cancelled account.
        /// </summary>
        public const string AccountStatusCanceled = "003";

        /// <summary>
        /// Name of the bank account number field.
        /// </summary>
        public const string BankAccountNumberFieldName = "BANK_ACCOUNT_NUMBER";

        /// <summary>
        /// Name of the bank product code field.
        /// </summary>
        public const string BankProductCodeFieldName = "BANK_PRODUCT_CODE";

        /// <summary>
        /// Name of the account status code field.
        /// </summary>
        public const string AccountStatusCodeFieldName = "ACCOUNT_STATUS_CODE";

        /// <summary>
        /// Name of the program identifier field.
        /// </summary>
        public const string ProgramIdentifierFieldName = "PROGRAM_IDENTIFIER";

        /// <summary>
        /// Name of the bank customer number field.
        /// </summary>
        public const string BankCustomerNumberFieldName = "BANK_CUSTOMER_NUMBER";

        /// <summary>
        /// Name of the member ICA field.
        /// </summary>
        public const string MemberIcaFieldName = "MEMBER_ICA";
    }
}