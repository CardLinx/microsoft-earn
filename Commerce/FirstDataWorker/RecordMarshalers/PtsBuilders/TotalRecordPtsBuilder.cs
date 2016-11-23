//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System.Text;

    /// <summary>
    /// Builds a total record representation for a First Data PTS file.
    /// </summary>
    /// <remarks>
    /// This builds First Data's "Total 'T' Record - 80 Byte" (PTS Specification ver. 2012-1 section 9.1.1.92.)
    /// </remarks>
    public static class TotalRecordPtsBuilder
    {
        /// <summary>
        /// Builds the PTS file representation for a total record.
        /// </summary>
        /// <param name="totalDiscountAmount">
        /// The total amount of all discounts within the PTS file.
        /// </param>
        /// <param name="recordSequenceNumber">
        /// The record sequence number to place in the PTS file for the specified record.
        /// </param>
        /// <returns>
        /// The PTS file representation for a total record.
        /// </returns>
        public static string Build(int totalDiscountAmount,
                                     int recordSequenceNumber)
        {
            StringBuilder result = new StringBuilder(PtsConstants.RecordLength);

            // Total record ID
            result.Append(RecordId);

            // First filler
            result.Append(PtsConstants.AlternateFillerPad, FirstFillerLength);

            // Total discount amount.
            string discountAmount = totalDiscountAmount.ToString();
            result.Append(TotalDiscountAmountPad, TotalDiscountAmountLength - discountAmount.Length);
            result.Append(discountAmount);

            // Second filler
            result.Append(PtsConstants.AlternateFillerPad, SecondFillerLength);

            // Record sequence number
            string sequenceNumber = recordSequenceNumber.ToString();
            result.Append(PtsConstants.RecordSequenceNumberPad,
                          PtsConstants.RecordSequenceNumberLength - sequenceNumber.Length);
            result.Append(sequenceNumber);

            // Third filler
            result.Append(PtsConstants.FillerPad, ThirdFillerLength);

            return result.ToString();
        }

        /// <summary>
        /// The ID for a total record in a PTS file.
        /// </summary>
        private const string RecordId = "T";

        /// <summary>
        /// The length of the first filler in the PTS representation of the total record.
        /// </summary>
        private const int FirstFillerLength = 12;

        /// <summary>
        /// The character to use the left-pad the total discount amount.
        /// </summary>
        private const char TotalDiscountAmountPad = '0';

        /// <summary>
        /// The length of the total discount amount field.
        /// </summary>
        private const int TotalDiscountAmountLength = 12;

        /// <summary>
        /// The length of the second filler in the PTS representation of the total record.
        /// </summary>
        private const int SecondFillerLength = 36;

        /// <summary>
        /// The length of the third filler in the PTS representation of the total record.
        /// </summary>
        private const int ThirdFillerLength = 13;
    }
}