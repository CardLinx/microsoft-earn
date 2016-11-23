//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Text;

    /// <summary>
    /// Builds a merchant record representation for a First Data PTS file.
    /// </summary>
    /// <remarks>
    /// This builds First Data's "Merchant Record 80 Byte Records" (PTS Specification ver. 2012-1 section 9.1.1.1.)
    /// </remarks>
    public static class MerchantRecordPtsBuilder
    {
        /// <summary>
        /// Builds the PTS file representation for the specified record.
        /// </summary>
        /// <param name="record">
        /// The record for which to build a PTS file representation.
        /// </param>
        /// <param name="submissionDate">
        /// The submission date to specify within the PTS representation of the record.
        /// </param>
        /// <param name="submissionSequenceNumber">
        /// The submission sequence number to place in the PTS file
        /// </param>
        /// <param name="recordSequenceNumber">
        /// The record sequence number to place in the PTS file for the specified record.
        /// </param>
        /// <returns>
        /// The PTS file representation for the specified record.
        /// </returns>
        internal static string Build(PtsMerchantInfo record,
                                     DateTime submissionDate,
                                     int submissionSequenceNumber,
                                     int recordSequenceNumber)
        {
            StringBuilder result = new StringBuilder(PtsConstants.RecordLength);

            // Merchant record ID
            result.Append(RecordId);

            // Merchant ID
            result.Append(MerchantIdPad, MerchantIdLength - record.PartnerMerchantId.Length);
            result.Append(record.PartnerMerchantId);

            // Reference ID
            result.Append(ReferenceId);

            // Security code (instance 1)
            result.Append(SecurityCode);

            // Submission type
            result.Append(SubmissionType);

            // Submission date, Julian-style
            result.Append(submissionDate.ToString("yy"));
            int dayOfYear = submissionDate.DayOfYear;
            if (dayOfYear < 100)
            {
                result.Append("0");
            }
            if (dayOfYear < 10)
            {
                result.Append("0");
            }
            result.Append(dayOfYear);

            // Submission sequence number
            result.Append(submissionSequenceNumber);

            // Security code (instance 2)
            result.Append(SecurityCode);

            // Submission date, Gregorian-style
            result.Append(submissionDate.ToString("MMddyy"));

            // Record sequence number
            string sequenceNumber = recordSequenceNumber.ToString();
            result.Append(PtsConstants.RecordSequenceNumberPad,
                          PtsConstants.RecordSequenceNumberLength - sequenceNumber.Length);
            result.Append(sequenceNumber);

            // Filler (and unused fields)
            result.Append(PtsConstants.FillerPad, FillerLength);

            return result.ToString();
        }

        /// <summary>
        /// The ID for a merchant record in a PTS file.
        /// </summary>
        private const string RecordId = "M";

        /// <summary>
        /// The length of the merchant ID field.
        /// </summary>
        private const int MerchantIdLength = 12;

        /// <summary>
        /// The character to use to pad the left side of the merchant ID if necessary.
        /// </summary>
        private const char MerchantIdPad = '0';

        /// <summary>
        /// The reference ID for a merchant record in a PTS file.
        /// </summary>
        private const string ReferenceId = "RR";

        /// <summary>
        /// The security code for a merchant record in a PTS file.
        /// </summary>
        private const string SecurityCode = "5628";

        /// <summary>
        /// The submission type for a merchant record in a PTS file.
        /// </summary>
        private const string SubmissionType = "7";

        /// <summary>
        /// The length of the filler in the PTS representation of the merchant record.
        /// </summary>
        private const int FillerLength = 38;
    }
}