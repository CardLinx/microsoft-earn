//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System.Text;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Builds a transaction detail representation for a First Data PTS file.
    /// </summary>
    /// <remarks>
    /// This builds First Data's "(Credit Card) Detail 'D' Record - 80 Byte" (PTS Specification ver. 2012-1 section 9.1.1.90.)
    /// </remarks>
    public static class TransactionDetailPtsBuilder
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

            // Transaction detail record ID
            result.Append(RecordId);

            // Filler
            result.Append(PtsConstants.AlternateFillerPad, FirstFillerLength);

            // Transaction code
            result.Append(TransactionCode);

            // Discount amount.
            string discountAmount = record.DiscountAmount.ToString();
            result.Append(DiscountAmountPad, DiscountAmountLength - discountAmount.Length);
            result.Append(discountAmount);

            // Transaction date.
            result.Append(record.TransactionDate.ToString("MMdd"));

            // Filler
            result.Append(PtsConstants.FillerPad, SecondFillerLength);

            // Filler
            result.Append(PtsConstants.AlternateFillerPad, ThirdFillerLength);

            // Reference number. Limit to eight places. (Overflow is okay because numbers do not have to be absolutely unique).
            string referenceNumber = record.ReferenceNumber.ToString().PadLeft(Int32MaxLength, ReferenceNumberPad);
            referenceNumber = referenceNumber.Substring(Int32MaxLength - ReferenceNumberLength);
            result.Append(referenceNumber);

            // Record sequence number
            string sequenceNumber = recordSequenceNumber.ToString();
            result.Append(PtsConstants.RecordSequenceNumberPad,
                          PtsConstants.RecordSequenceNumberLength - sequenceNumber.Length);
            result.Append(sequenceNumber);

            // Filler
            result.Append(PtsConstants.FillerPad, FourthFillerLength);

            // Filler
            result.Append(PtsConstants.AlternateFillerPad, FifthFillerLength);

            // Filler
            result.Append(PtsConstants.FillerPad, SixthFillerLength);

            return result.ToString();
        }

        /// <summary>
        /// The ID for a transaction detail record in a PTS file.
        /// </summary>
        private const string RecordId = "D";
        
        /// <summary>
        /// The length of the first filler in the PTS representation of the transaction detail.
        /// </summary>
        private const int FirstFillerLength = 16;

        /// <summary>
        /// The transaction code to use for settlement-based redemption.
        /// </summary>
        private const string TransactionCode = "6";

        /// <summary>
        /// The length of the discount amount field.
        /// </summary>
        private const int DiscountAmountLength = 8;

        /// <summary>
        /// The character to use to left-pad the discount amount.
        /// </summary>
        private const char DiscountAmountPad = '0';

        /// <summary>
        /// The length of the second filler in the PTS representation of the transaction detail.
        /// </summary>
        private const int SecondFillerLength = 6;

        /// <summary>
        /// The length of the third filler in the PTS representation of the transaction detail.
        /// </summary>
        private const int ThirdFillerLength = 8;

        /// <summary>
        /// The maximum length of an Int32 as a string.
        /// </summary>
        private const int Int32MaxLength = 10;

        /// <summary>
        /// The length of the reference number field.
        /// </summary>
        private const int ReferenceNumberLength = 8;

        /// <summary>
        /// The character to use to left-pad the reference number.
        /// </summary>
        private const char ReferenceNumberPad = '0';

        /// <summary>
        /// The length of the fourth filler in the PTS representation of the transaction detail.
        /// </summary>
        private const int FourthFillerLength = 2;

        /// <summary>
        /// The length of the fifth filler in the PTS representation of the transaction detail.
        /// </summary>
        private const int FifthFillerLength = 15;

        /// <summary>
        /// The length of the sixth filler in the PTS representation of the transaction detail.
        /// </summary>
        private const int SixthFillerLength = 5;
    }
}