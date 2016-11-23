//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System.Text;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Builds a special condition representation for a First Data PTS file.
    /// </summary>
    /// <remarks>
    /// This builds First Data's "Special Condition Record 'S' - 80 byte" (PTS Specification ver. 2012-1 section 9.1.1.66.)
    /// </remarks>
    public static class SpecialConditionPtsBuilder
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

            // Special condition record ID
            result.Append(RecordId);

            // Non-quasi-cash indicator
            result.Append(NonQuasiCashIndicator);

            // No special condition indicator
            result.Append(NoSpecialConditionIndicator);

            // Clearing sequence
            result.Append(ClearingSequence);

            // Cleaing count
            result.Append(ClearingCount);

            // Print customer service phone number
            result.Append(PrintCustomerServicePhoneNumberIndicator);

            // First filler
            result.Append(PtsConstants.FillerPad, FirstFillerLength);

            // Record sequence number
            string sequenceNumber = recordSequenceNumber.ToString();
            result.Append(PtsConstants.RecordSequenceNumberPad,
                          PtsConstants.RecordSequenceNumberLength - sequenceNumber.Length);
            result.Append(sequenceNumber);

            // Second filler
            result.Append(PtsConstants.FillerPad, SecondFillerLength);

            // Offerwise or not
            if (forNonFirstDataPartners)
            {
                result.Append(NonOfferWise);
            }
            else
            {
                result.Append(Offerwise);
            }

            // Partial offer ID
            // in non FDC path, it can be less than expected length, so pad it if such is the case
            if (record.OfferId.Length < PartialOfferIdLength)
            {
                record.OfferId = record.OfferId.PadRight(PartialOfferIdLength, '0');
            }

            // Partial offer ID
            result.Append(record.OfferId.Substring(0, PartialOfferIdLength).ToUpperInvariant());

            // Filler (and unused fields)
            result.Append(PtsConstants.FillerPad, ThirdFillerLength);

            return result.ToString();
        }

        /// <summary>
        /// The ID for a special condition record in a PTS file.
        /// </summary>
        private const string RecordId = "S";

        /// <summary>
        /// Indicates that the record did not involve quasi-cash.
        /// </summary>
        private const string NonQuasiCashIndicator = "N";

        /// <summary>
        /// Indicates that the record has no special condition indicator.
        /// </summary>
        private const string NoSpecialConditionIndicator = " ";

        /// <summary>
        /// The clearing sequence to specify in the record.
        /// </summary>
        private const string ClearingSequence = "00";

        /// <summary>
        /// The clearing count to specify in the record.
        /// </summary>
        private const string ClearingCount = "00";

        /// <summary>
        /// Indicates that the customer service phone number should be printed on the statement.
        /// </summary>
        private const string PrintCustomerServicePhoneNumberIndicator = "Y";

        /// <summary>
        /// The length of the first filler in the PTS representation of the special condition record.
        /// </summary>
        private const int FirstFillerLength = 34;

        /// <summary>
        /// The length of the second filler in the PTS representation of the special condition record.
        /// </summary>
        private const int SecondFillerLength = 6;

        /// <summary>
        /// Indicates the offer originated in the Offerwise platform.
        /// </summary>
        private const string Offerwise = "OW";

        /// <summary>
        /// Indicates the offer originated in the non FDC Partners.
        /// </summary>
        private const string NonOfferWise = "MS";

        /// <summary>
        /// The length of the partial offer ID.
        /// </summary>
        private const int PartialOfferIdLength = 10;

        /// <summary>
        /// The length of the third filler in the PTS representation of the special condition record.
        /// </summary>
        private const int ThirdFillerLength = 14;
    }
}