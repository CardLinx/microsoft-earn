//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Text;

    /// <summary>
    /// Builds a MasterCard filtering header record.
    /// </summary>
    public static class FilteringHeaderBuilder
    {
        /// <summary>
        /// Builds the filtering header record using the specified submission date.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file being built.
        /// </param>
        /// <param name="date">
        /// The date to add to the header record.
        /// </param>
        /// <param name="time">
        /// The time to add to the header record.
        /// </param>
        /// <returns>
        /// The filtering header record.
        /// </returns>
        internal static string Build(string fileName,
                                     string date,
                                     string time)
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

            // Original file name.
            result.Append(FilteringConstants.AlphaOmittedCharacter, OriginalFileNameFieldLength);

            // Original date.
            result.Append(FilteringConstants.DateTimeOmittedCharacter, OriginalDateFieldLength);

            // MRS reserved.
            result.Append(FilteringConstants.AlphaOmittedCharacter, FilteringConstants.MrsReservedFieldLength);

            // Filler.
            result.Append(FilteringConstants.AlphaOmittedCharacter, FillerFieldLength);

            return result.ToString();
        }

        /// <summary>
        /// The record type for filtering header records.
        /// </summary>
        private const string RecordType = "10";

        /// <summary>
        /// The length of the original file name field.
        /// </summary>
        private const int OriginalFileNameFieldLength = 30;

        /// <summary>
        /// The length of the original date field.
        /// </summary>
        private const int OriginalDateFieldLength = 14;

        /// <summary>
        /// The length of the filler field.
        /// </summary>
        private const int FillerFieldLength = 704;
    }
}