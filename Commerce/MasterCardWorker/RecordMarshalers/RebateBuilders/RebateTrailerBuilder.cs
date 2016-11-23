//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Text;

    /// <summary>
    /// Builds a MasterCard rebate trailer record.
    /// </summary>
    public static class RebateTrailerBuilder
    {
        /// <summary>
        /// Builds the rebate trailer record using the specified submission date.
        /// </summary>
        /// <param name="recordCount">
        /// The count of records in the file, not including header or trailer.
        /// </param>
        /// <returns>
        /// The rebate trailer record.
        /// </returns>
        internal static string Build(int recordCount)
        {
            StringBuilder result = new StringBuilder();

            // Record type.
            result.Append(RecordType);

            // Record count. Header and footer are NOT included.
            result.AppendFormat(RecordCountFormat, recordCount);

            // Member ICA.
            result.Append(RebateConstants.MemberIca);

            // Filler.
            result.Append(RebateConstants.AlphaOmittedCharacter, FillerFieldLength);

            return result.ToString();
        }

        /// <summary>
        /// The record type for rebate trailer records.
        /// </summary>
        private const string RecordType = "T";

        /// <summary>
        /// The format string to use when adding the record count.
        /// </summary>
        /// <remarks>
        /// Field is 12 characters in length, left padded with zeros.
        /// </remarks>
        private const string RecordCountFormat = "{0:D12}";

        /// <summary>
        /// The length of the filler field.
        /// </summary>
        private const int FillerFieldLength = 176;
    }
}