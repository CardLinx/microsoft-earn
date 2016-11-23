//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System.Text;
    using Lomo.Commerce.MasterCardClient;

    /// <summary>
    /// Builds a MasterCard filtering record.
    /// </summary>
    public static class FilteringRecordBuilder
    {
        /// <summary>
        /// Builds the filtering record of the specified type for the specified record.
        /// </summary>
        /// <param name="record">
        /// The record for which to build a filtering record.
        /// </param>
        /// <param name="date">
        /// The date to add to the filtering record.
        /// </param>
        /// <param name="mappingTypeCode">
        /// The mapping type code to embed within the filtering record.
        /// </param>
        /// <param name="setId">
        /// The set ID to specify for the record.
        /// </param>
        /// <param name="mappingId">
        /// The mapping ID for the filtering record.
        /// </param>
        /// <returns>
        /// The filtering record for the specified record.
        /// </returns>
        internal static string Build(FilteringRecord record,
                                     string date,
                                     string mappingTypeCode,
                                     string setId,
                                     int mappingId)
        {
            StringBuilder result = new StringBuilder();

            // Record type.
            result.Append(RecordType);

            // Record date.
            result.Append(date);

            // Member ICA.
            result.Append(FilteringConstants.MemberIca);

            // Bank customer number.
            result.Append(record.BankCustomerNumber.PadRight(BankCustomerNumberFieldLength));

            // Bank account number.
            result.Append(FilteringConstants.AlphaOmittedCharacter, BankAccountNumberFieldLength);

            // Bank product code.
            result.Append(BankProductCode.PadRight(BankProductCodeFieldLength));

            // Mapping action code.
            result.Append(MappingActionCode);

            // Mapping type code.
            result.Append(mappingTypeCode);

            // Mapping ID.
            result.Append(mappingId.ToString().PadRight(MappingIdFieldLength));

            // Status.
            result.Append(Status);

            // Expiration date.
            result.Append(FilteringConstants.DateTimeOmittedCharacter, ExpirationDateFieldLength);

            // Enrollment date.
            result.Append(record.EffectiveDate.ToString("yyyyMMdd"));

            // Merchant set ID (User defined 1).
            result.Append(setId.PadRight(UserDefinedFieldLength));

            // Transaction threshold (User defined 2).
            result.Append(record.Threshold.ToString("F2").PadRight(UserDefinedFieldLength));

            // User defined 3.
            result.Append(FilteringConstants.AlphaOmittedCharacter, UserDefinedFieldLength);

            // User defined 4.
            result.Append(FilteringConstants.AlphaOmittedCharacter, UserDefinedFieldLength);

            // User defined 5.
            result.Append(FilteringConstants.AlphaOmittedCharacter, UserDefinedFieldLength);

            // Points total sign.
            result.Append(FilteringConstants.AlphaOmittedCharacter);

            // Point total.
            result.Append(FilteringConstants.NumericOmittedCharacter, PointTotalFieldLength);

            // Filler.
            result.Append(FilteringConstants.AlphaOmittedCharacter, FillerFieldLength);

            return result.ToString();
        }

        /// <summary>
        /// The record type for filtering header records.
        /// </summary>
        private const string RecordType = "22";

        /// <summary>
        /// The length of the bank customer number field.
        /// </summary>
        private const int BankCustomerNumberFieldLength = 30;

        /// <summary>
        /// The length of the bank account number field.
        /// </summary>
        private const int BankAccountNumberFieldLength = 30;

        /// <summary>
        /// The length of the bank product code field.
        /// </summary>
        private const int BankProductCodeFieldLength = 20;

        /// <summary>
        /// The bank product code.
        /// </summary>
        private const string BankProductCode = "MCCMSFT";

        /// <summary>
        /// The mapping action code.
        /// </summary>
        private const char MappingActionCode = 'A';

        /// <summary>
        /// The length of the mapping ID field.
        /// </summary>
        private const int MappingIdFieldLength = 100;

        /// <summary>
        /// The status to specify.
        /// </summary>
        private const string Status = "A  ";

        /// <summary>
        /// The length of the expiration date field.
        /// </summary>
        private const int ExpirationDateFieldLength = 8;

        /// <summary>
        /// The length of a user defined field.
        /// </summary>
        private const int UserDefinedFieldLength = 40;

        /// <summary>
        /// The length of the point total field.
        /// </summary>
        private const int PointTotalFieldLength = 11;

        /// <summary>
        /// The length of the filler field.
        /// </summary>
        private const int FillerFieldLength = 417;
    }
}