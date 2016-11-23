//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Text;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Builds an acquirer reference number representation for a First Data PTS file.
    /// </summary>
    /// <remarks>
    /// This builds First Data's "Acquirer Reference Number" (PTS Specification ver. 2012-1 section 12.20.1.3.)
    /// </remarks>
    public static class AcquirerReferenceNumberPtsBuilder
    {
        /// <summary>
        /// Builds the PTS file representation for the specified record.
        /// </summary>
        /// <param name="record">
        /// The record for which to build a PTS file representation.
        /// </param>
        /// <param name="recordSequenceNumber">
        /// The record sequence number to place in the PTS file for the specified record.
        /// </param>
        /// <param name="forNonFirstDataPartners">
        /// Will be set to True if PTS being built is for Non-FDC (like Visa) partners
        /// </param>
        /// <returns>
        /// The PTS file representation for the specified record.
        /// </returns>
        internal static string Build(OutstandingRedeemedDealInfo record,
                                     int recordSequenceNumber,
                                     bool forNonFirstDataPartners)
        {
            StringBuilder result = new StringBuilder(PtsConstants.RecordLength);

            // AcquirerReferenceNumber record ID
            result.Append(RecordId);

            // Record sequence number
            string sequenceNumber = recordSequenceNumber.ToString();
            result.Append(PtsConstants.RecordSequenceNumberPad,
                          PtsConstants.RecordSequenceNumberLength - sequenceNumber.Length);
            result.Append(sequenceNumber);

            if (forNonFirstDataPartners)
            {
                BuildForNonFirstData(record, result);
            }
            else
            {
                BuildForFirstData(record, result);
            }

            // Filler (and unused fields).
            result.Append(PtsConstants.FillerPad, FillerLength);

            return result.ToString();
        }

        /// <summary>
        /// Append data for Non-FDC partners
        /// </summary>
        /// <param name="record">
        /// The record for which to build a PTS file representation.
        /// </param>
        /// <param name="result">
        /// Stringbuilder holding existing representation
        /// </param>
        private static void BuildForNonFirstData(OutstandingRedeemedDealInfo record, StringBuilder result)
        {
            result.Append("M");
            result.Append(record.TransactionDate.ToString("yyyyMMdd"));
            result.Append("REV");
            result.Append("0000000");
            result.Append("MSFT");
        }

        /// <summary>
        /// Append data for FDC
        /// </summary>
        /// <param name="record">
        /// The record for which to build a PTS file representation.
        /// </param>
        /// <param name="result">
        /// Stringbuilder holding existing representation
        /// </param>
        private static void BuildForFirstData(OutstandingRedeemedDealInfo record, StringBuilder result)
        {
            // Acquirer reference number.
            result.Append(record.AcquirerReferenceNumber);
        }

        /// <summary>
        /// The ID for an acquirer refrence number record in a PTS file.
        /// </summary>
        private const string RecordId = "XR03";

        /// <summary>
        /// The length of the filler in the PTS representation of the acquirer reference number record.
        /// </summary>
        private const int FillerLength = 47;
    }
}