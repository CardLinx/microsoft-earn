//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System.Text;
    using Lomo.Commerce.MasterCardClient;

    /// <summary>
    /// Builds a MasterCard rebate record.
    /// </summary>
    public static class RebateRecordBuilder
    {
        /// <summary>
        /// Builds the rebate record of the specified type for the specified record.
        /// </summary>
        /// <param name="record">
        /// The record for which to build a rebate record.
        /// </param>
        /// <returns>
        /// The rebate record for the specified record.
        /// </returns>
        internal static string Build(RebateRecord record)
        {
            StringBuilder result = new StringBuilder();

            // Record type.
            result.Append(RecordType);

            // Transaction sequence number.
            result.Append(record.TransactionSequenceNumber);

            // Bank account number.
            result.Append(RebateConstants.AlphaOmittedCharacter, BankAccountNumberFieldLength);

            // Transaction amount.
            result.Append(record.TransactionAmount.ToString("F2").PadLeft(TransactionAmountFieldLength, RebateConstants.NumericOmittedCharacter));

            // Transaction date.
            result.Append(record.TransactionDate.ToString("yyyyMMdd"));

            // Rebate amount.
            result.Append(record.RebateAmount.ToString("F2").PadLeft(RebateAmountFieldLength, RebateConstants.NumericOmittedCharacter));

            // Merchant category code.
            result.Append(MerchantCategoryCode);

            // Transaction description.
            result.Append(record.TransactionDescription.PadRight(TransactionDescriptionFieldLength));

            // Reversal indicator.
            result.Append(NoReversalIndicator);
            
            // Merchant ID.
            result.Append(record.MerchantId.PadRight(MerchantIdFieldLength));

            // Issuer ICA.
            result.Append(record.IssuerIca);

            // Program code.
            result.Append(ProgramCode);

            // Bank product code.
            result.Append(BankProductCode.PadRight(BankProductCodeFieldLength));

            // Bank customer number.
            result.Append(record.BankCustomerNumber.PadRight(BankCustomerNumberFieldLength));

            // Filler.
            result.Append(RebateConstants.AlphaOmittedCharacter, FillerFieldLength);

            return result.ToString();
        }

        /// <summary>
        /// The record type for rebate header records.
        /// </summary>
        private const string RecordType = "D";

        /// <summary>
        /// The length of the bank account number field.
        /// </summary>
        private const int BankAccountNumberFieldLength = 19;

        /// <summary>
        /// The length of the transaction amount field.
        /// </summary>
        private const int TransactionAmountFieldLength = 13;

        /// <summary>
        /// The length of the rebate amount field.
        /// </summary>
        private const int RebateAmountFieldLength = 13;

        /// <summary>
        /// The static merchant category code.
        /// </summary>
        public const string MerchantCategoryCode = "6555";

        /// <summary>
        /// The length of the transaction description field.
        /// </summary>
        private const int TransactionDescriptionFieldLength = 22;

        /// <summary>
        /// The character to use to indicate the rebate record does NOT indicate a rebate should be reversed.
        /// </summary>
        private const char NoReversalIndicator = 'N';

        /// <summary>
        /// The length of the merchant ID field.
        /// </summary>
        private const int MerchantIdFieldLength = 22;

        /// <summary>
        /// The product code.
        /// </summary>
        private const string ProgramCode = "MS";

        /// <summary>
        /// The bank product code.
        /// </summary>
        private const string BankProductCode = "MCCMSFT";

        /// <summary>
        /// The length of the bank product code field.
        /// </summary>
        private const int BankProductCodeFieldLength = 20;

        /// <summary>
        /// The length of the bank customer number field.
        /// </summary>
        private const int BankCustomerNumberFieldLength = 30;

        /// <summary>
        /// The length of the filler field.
        /// </summary>
        private const int FillerFieldLength = 26;
    }
}