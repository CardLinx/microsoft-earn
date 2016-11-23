//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Text;

    /// <summary>
    /// Builds a MasterCard filtering trailer record.
    /// </summary>
    public static class FilteringTrailerBuilder
    {
        /// <summary>
        /// Builds the filtering trailer record using the specified submission date.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file being built.
        /// </param>
        /// <param name="date">
        /// The date to add to the trailer record.
        /// </param>
        /// <param name="time">
        /// The time to add to the trailer record.
        /// </param>
        /// <param name="recordCount">
        /// The count of records in the file, not including header or trailer.
        /// </param>
        /// <returns>
        /// The filtering trailer record.
        /// </returns>
        internal static string Build(string fileName,
                                     string date,
                                     string time,
                                     int recordCount)
        {
            StringBuilder result = new StringBuilder();

            // Record type.
            result.Append(RecordType);

            // Submission date.
            result.Append(date);

            // Submission time.
            result.Append(time);

            // Member ICA.
            result.Append(FilteringConstants.MemberIca);

            // File name.
            result.Append(fileName);

            // Record count. Each record is added for both authorization and clearing filtering, so count is doubled, and header and footer are included.
            result.AppendFormat(RecordCountFormat, recordCount * 2 + 2);

            // Adjustment record count.
            result.Append(FilteringConstants.NumericOmittedCharacter, AdjustmentRecordCountFieldLength);

            // Adjustment record amount.
            result.Append(FilteringConstants.NumericOmittedCharacter, AdjustmentRecordAmountFieldLength);

            // Transaction detail record count.
            result.Append(FilteringConstants.NumericOmittedCharacter, TransactionDetailRecordCountFieldLength);

            // Transaction detail record amount.
            result.Append(FilteringConstants.NumericOmittedCharacter, TransactionDetailRecordAmountFieldLength);

            // MRS reserved.
            result.Append(FilteringConstants.AlphaOmittedCharacter, FilteringConstants.MrsReservedFieldLength);

            // Filler.
            result.Append(FilteringConstants.AlphaOmittedCharacter, FillerFieldLength);

            return result.ToString();
        }

        /// <summary>
        /// The record type for filtering trailer records.
        /// </summary>
        private const string RecordType = "90";

        /// <summary>
        /// The format string to use when adding the record count.
        /// </summary>
        /// <remarks>
        /// Field is 9 characters in length, left padded with zeros.
        /// </remarks>
        private const string RecordCountFormat = "{0:D9}";

        /// <summary>
        /// The length of the adjustment record count field.
        /// </summary>
        private const int AdjustmentRecordCountFieldLength = 8;

        /// <summary>
        /// The length of the adjustment record amount field.
        /// </summary>
        private const int AdjustmentRecordAmountFieldLength = 15;

        /// <summary>
        /// The length of the transaction detail record count field.
        /// </summary>
        private const int TransactionDetailRecordCountFieldLength = 8;

        /// <summary>
        /// The length of the transaction detail record amount field.
        /// </summary>
        private const int TransactionDetailRecordAmountFieldLength = 15;

        /// <summary>
        /// The length of the filler field.
        /// </summary>
        private const int FillerFieldLength = 693;
    }
}