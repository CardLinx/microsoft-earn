//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;
    using Lomo.Commerce.MasterCardClient;
    
    /// <summary>
    /// Builds a MasterCard filtering file.
    /// </summary>
    public static class FilteringBuilder
    {
        /// <summary>
        /// Builds the filtering file representation for the specified records.
        /// </summary>
        /// <param name="records">
        /// The records for which to build a filtering file.
        /// </param>
        /// <param name="submissionDate">
        /// The submission date to specify within the filtering file.
        /// </param>
        /// <returns>
        /// The filtering file representation for the specified records.
        /// </returns>
        /// <remarks>
        /// MasterCard uses Unix style end of lines, so \n is manually added instead of using AppendLine.
        /// </remarks>
        internal static string Build(Collection<FilteringRecord> records,
                                     DateTime submissionDate)
        {
            StringBuilder result = new StringBuilder();

            // Parse date information and build the filename.
            string date = submissionDate.ToString("yyyyMMdd");
            string time = submissionDate.ToString("HHmmss");
            string fileName = String.Concat(FilteringFileDecoration, date, time, FileExtension).PadRight(FileNameFieldLength);

            // Add the header.
            result.Append(FilteringHeaderBuilder.Build(fileName, date, time));
            result.Append("\n");

            // Build filtering records for each record object.
            int mappingId = 0;
            foreach (FilteringRecord record in records)
            {
                // Add the authorization filtering record.
                result.Append(FilteringAuthorizationRecordBuilder.Build(record, date, ++mappingId));
                result.Append("\n");

                // Add the clearing filtering record.
                result.Append(FilteringClearingRecordBuilder.Build(record, date, ++mappingId));
                result.Append("\n");
            }

            // Add the trailer.
            result.Append(FilteringTrailerBuilder.Build(fileName, date, time, records.Count));
            result.Append("\n");

            return result.ToString();
        }

        /// <summary>
        /// The decoration to add to filtering filenames.
        /// </summary>
        private const string FilteringFileDecoration = "Filtering";

        /// <summary>
        /// The length of the file name field.
        /// </summary>
        private const int FileNameFieldLength = 30;

        /// <summary>
        /// The extension to add to filenames.
        /// </summary>
        private const string FileExtension = ".txt";
    }
}