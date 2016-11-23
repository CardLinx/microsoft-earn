//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using Lomo.Commerce.DataModels;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Builds a descriptor record representation for a First Data PTS file.
    /// </summary>
    /// <remarks>
    /// This builds First Data's "Expanded Descriptor 'N' Record - 80 bytes" (PTS Specification ver. 2012-1 section 9.1.1.3.)
    /// </remarks>
    public static class DescriptorRecordPtsBuilder
    {
        /// <summary>
        /// Builds the PTS file representation for a descriptor record.
        /// </summary>
        /// <param name="record">
        /// The record for which to build a PTS file representation.
        /// </param>
        /// <param name="recordSequenceNumber">
        /// The record sequence number to place in the PTS file for the specified record.
        /// </param>
        /// <returns>
        /// The PTS file representation for a descriptor record.
        /// </returns>
        internal static string Build(PtsMerchantInfo record, 
            int recordSequenceNumber)
        {
            StringBuilder result = new StringBuilder(PtsConstants.RecordLength);

            // Descriptor record ID
            result.Append(RecordId);

            // Transaction source
            result.Append(GetMerchantDescription(record));

            // First filler
            result.Append(PtsConstants.FillerPad, FirstFillerLength);

            // Customer service phone number
            result.Append(CustomerServicePhoneNumber);

            // Provider state.
            result.Append(ProviderState);

            // Record sequence number
            string sequenceNumber = recordSequenceNumber.ToString();
            result.Append(PtsConstants.RecordSequenceNumberPad,
                          PtsConstants.RecordSequenceNumberLength - sequenceNumber.Length);
            result.Append(sequenceNumber);

            // Second filler
            result.Append(PtsConstants.FillerPad, SecondFillerLength);

            return result.ToString();
        }

        /// <summary>
        /// Given a merchant name, this method builds a bing offers specific name exactly 25 characters long.
        /// </summary>
        /// <param name="merchantInfo">
        /// Merchant information needed to build the merchant description.
        /// </param>
        /// <returns>
        /// result description
        /// </returns>
        private static string GetMerchantDescription(PtsMerchantInfo merchantInfo)
        {
            StringBuilder merchantDescriptionBuilder = new StringBuilder();

            // Return the merchant description for the reward program indicated by the specified reimbursement tender.
            if ((merchantInfo.ReimbursementTender & ReimbursementTender.MicrosoftBurn) == ReimbursementTender.MicrosoftBurn)
            {
                merchantDescriptionBuilder.Append(EarnSource);
            }
            else
            {
                // BING OFFERS- is 12 character long. 
                // So we need to get 13 more characters from merchant name
                string merchantName = merchantInfo.MerchantName.ToUpper(CultureInfo.InvariantCulture);
                int numberOfCharactersNeeded = MaxTransactionSourceLength - BingOffersSource.Length;
                merchantDescriptionBuilder.Append(BingOffersSource);
                if (!string.IsNullOrEmpty(merchantName))
                {
                    merchantDescriptionBuilder.Append(merchantName.Length > numberOfCharactersNeeded ? merchantName.Substring(0, numberOfCharactersNeeded) : merchantName);
                }
            }

            int diff = MaxTransactionSourceLength - merchantDescriptionBuilder.Length;
            if (diff > 0)
            {
                merchantDescriptionBuilder.Append(PtsConstants.FillerPad, diff);
            }

            return merchantDescriptionBuilder.ToString();
        }

        /// <summary>
        /// The ID for a descriptor record in a PTS file.
        /// </summary>
        private const string RecordId = "N";

        /// <summary>
        /// Max lenght of transaction source.
        /// </summary>
        private const int MaxTransactionSourceLength = 25;

        /// <summary>
        /// The tranaction source to add to the statement for CLO reward program transactions.
        /// </summary>
        private const string BingOffersSource = "BING OFFERS-";

        /// <summary>
        /// The transaction source to add to the statement for EarnBurn reward program Burn transactions.
        /// </summary>
        private const string EarnSource = "MSFT EARN REDEMPTION";

        /// <summary>
        /// The length of the first filler in the PTS representation of the descriptor record.
        /// </summary>
        private const int FirstFillerLength = 1;

        /// <summary>
        /// The customer service phone number to add to statement.
        /// </summary>
        private const string CustomerServicePhoneNumber = "8006427676  ";

        /// <summary>
        /// The string to place on the statement to represent the state these offers originate in, i.e. Microsoft's home state.
        /// </summary>
        private const string ProviderState = "WA ";

        /// <summary>
        /// The length of the second filler in the PTS representation of the descriptor record.
        /// </summary>
        private const int SecondFillerLength = 32;
    }
}