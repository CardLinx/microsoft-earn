//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System.Text;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Builds a tokenization representation for a First Data PTS file.
    /// </summary>
    /// <remarks>
    /// This builds First Data's "Tokenization XD67 Record - 80 Byte" (PTS Specification ver. 2012-1 section 9.1.1.95.)
    /// </remarks>
    public static class TokenizationPtsBuilder
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
        /// <returns>
        /// The PTS file representation for the specified record.
        /// </returns>
        internal static string Build(OutstandingRedeemedDealInfo record,
                                     int recordSequenceNumber)
        {
            StringBuilder result = new StringBuilder(PtsConstants.RecordLength);

            // Tokenization record ID
            result.Append(RecordId);

            // Record sequence number
            string sequenceNumber = recordSequenceNumber.ToString();
            result.Append(PtsConstants.RecordSequenceNumberPad,
                          PtsConstants.RecordSequenceNumberLength - sequenceNumber.Length);
            result.Append(sequenceNumber);

            // Token
            result.Append(record.Token);
            result.Append(TokenPad, TokenLength - record.Token.Length);

            // Token type
            result.Append(TokenType);

            // Filler
            result.Append(PtsConstants.FillerPad, FillerLength);

            return result.ToString();
        }

        /// <summary>
        /// The ID for a tokenization record in a PTS file.
        /// </summary>
        private const string RecordId = "XD67";

        /// <summary>
        /// The length of the token field.
        /// </summary>
        private const int TokenLength = 19;
        
        /// <summary>
        /// The character to use to pad the right side of the token field if necessary.
        /// </summary>
        internal const char TokenPad = ' ';

        /// <summary>
        /// The token type for the tokenization record.
        /// </summary>
        private const string TokenType = "PUB0";

        /// <summary>
        /// The length of the filler in the PTS representation of the tokenization record.
        /// </summary>
        private const int FillerLength = 47;
    }
}