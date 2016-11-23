//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Text;

    /// <summary>
    /// Builds a MasterCard rebate header record.
    /// </summary>
    public static class RebateHeaderBuilder
    {
        /// <summary>
        /// Builds the rebate header record using the specified submission date.
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
        /// The rebate header record.
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
            result.Append(RebateConstants.MemberIca);

            // File name.
            result.Append(fileName);

            // Filler.
            result.Append(RebateConstants.AlphaOmittedCharacter, FillerFieldLength);

            return result.ToString();
        }

        /// <summary>
        /// The record type for rebate header records.
        /// </summary>
        private const string RecordType = "H";

        /// <summary>
        /// The length of the filler field.
        /// </summary>
        private const int FillerFieldLength = 114;
    }
}