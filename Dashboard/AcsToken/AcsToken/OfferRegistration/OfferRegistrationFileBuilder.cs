//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace AcsToken
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;
    using Lomo.Commerce.AmexClient;
    /// <summary>
    /// Builder for Offer Registration File for Amex
    /// </summary>
    public static class OfferRegistrationFileBuilder
    {
        /// <summary>
        /// Given  string representation of detail records, this creates a file with proper header and footer
        /// </summary>
        /// <param name="detailRecords">
        /// All the records that are part of the file
        /// </param>
        /// <param name="submissionSequenceNumber">
        /// Sequence number of submission
        /// </param>
        /// <param name="submissionDateTime">
        /// Submission Date
        /// </param>
        /// <returns>
        /// File Contents
        /// </returns>
        public static string Build(Collection<string> detailRecords, int submissionSequenceNumber, DateTime submissionDateTime)
        {
            if (detailRecords == null || detailRecords.Count == 0)
            {
                throw new ArgumentNullException("detailRecords", "detailRecords cannot be null or empty");
            }

            StringBuilder result = new StringBuilder();
            result.Append(ConstructHeader(submissionSequenceNumber, submissionDateTime));
            result.Append("\n");
            foreach (string detailRecord in detailRecords)
            {
                result.Append(detailRecord);
                result.Append("\n");
            }
            result.Append(ConstructTrailer(detailRecords.Count));
            return result.ToString();
        }

        /// <summary>
        /// Construct a header
        /// </summary>
        /// <param name="sequenceNumber">
        /// Sequence Number
        /// </param>
        /// <param name="submissionDate">
        /// Date of submission
        /// </param>
        /// <returns>
        /// String header
        /// </returns>
        private static string ConstructHeader(int sequenceNumber, DateTime submissionDate)
        {
            OfferRegistrationHeader header = new OfferRegistrationHeader()
            {
                Date = submissionDate,
                FileDescription = "Offer Merchant Registration",
                SequenceNumber = sequenceNumber
            };

            return header.BuildFileHeader();
        }

        /// <summary>
        /// Construct a Trailer
        /// </summary>
        /// <param name="numberOfRecords">
        /// Number of Records
        /// </param>
        /// <returns>
        /// String trailer
        /// </returns>
        private static string ConstructTrailer(int numberOfRecords)
        {
            OfferRegistrationTrailer trailer = new OfferRegistrationTrailer()
            {
                TrailerCount = numberOfRecords
            };

            return trailer.BuildFileTrailer();
        }
    }
}