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
    /// Builds a MasterCard rebate file.
    /// </summary>
    public static class RebateBuilder
    {
        /// <summary>
        /// Builds the rebate file representation for the specified records.
        /// </summary>
        /// <param name="records">
        /// The records for which to build a rebate file.
        /// </param>
        /// <param name="submissionDate">
        /// The submission date to specify within the rebate file.
        /// </param>
        /// <returns>
        /// The rebate file representation for the specified records.
        /// </returns>
        /// <remarks>
        /// MasterCard uses Unix style end of lines, so \n is manually added instead of using AppendLine.
        /// </remarks>
        internal static string Build(Collection<RebateRecord> records,
                                     DateTime submissionDate)
        {
            StringBuilder result = new StringBuilder();

            // Parse date information and build the filename.
            string date = submissionDate.ToString("yyyyMMdd");
            string time = submissionDate.ToString("HHmmss");
            string fileName = String.Concat(RebateFileDecoration, date, time, FileExtension).PadRight(FileNameFieldLength);

            // Add the header.
            result.Append(RebateHeaderBuilder.Build(fileName, date, time));
            result.Append("\n");

            // Build Rebate records for each record object.
            foreach (RebateRecord record in records)
            {
                result.Append(RebateRecordBuilder.Build(record));
                result.Append("\n");
            }

            // Add the trailer. 
            result.Append(RebateTrailerBuilder.Build(records.Count));
            result.Append("\n");

            return result.ToString();
        }

        /// <summary>
        /// The decoration to add to rebate filenames.
        /// </summary>
        private const string RebateFileDecoration = "Rebate";

        /// <summary>
        /// The length of the file name field.
        /// </summary>
        private const int FileNameFieldLength = 60;

        /// <summary>
        /// The extension to add to filenames.
        /// </summary>
        private const string FileExtension = ".txt";
    }
}