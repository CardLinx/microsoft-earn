//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Commerce.RewardsNetworkWorker.RecordMarshalers.TransactionReportBuilder
{
    using System.Text;

    using Lomo.Commerce.DataModels;

    public class ReportRecordBuilder
    {
        /// <summary>
        /// Builds the transaction report detail record
        /// </summary>
        /// <param name="record">Transaction record to build</param>
        /// <returns>Detailed record string</returns>
        internal static string Build(TransactionRecord record)
        {
            StringBuilder result = new StringBuilder();

            // Record type.
            result.Append(RecordType);

            // Transaction sequence number.
            result.Append(record.RecordSequenceNumber.ToString().PadLeft(RecordSequenceFieldLength, TransactionReportConstants.NumericPaddingCharacter));

            // Merchant Id
            result.Append(record.MerchantId.PadRight(MerchantIdFieldLength, TransactionReportConstants.AlphaPaddingCharacter));

            // Merchant name
            if (record.MerchantName.Length > MerchantNameFieldLength)
            {
                result.Append(record.MerchantName.Remove(MerchantNameFieldLength));
            }
            else if (record.MerchantName.Length < MerchantNameFieldLength)
            {
                result.Append(record.MerchantName.PadRight(MerchantNameFieldLength,
                    TransactionReportConstants.AlphaPaddingCharacter));
            }
            else
            {
                result.Append(record.MerchantName);
            }

            // Transaction date.
            result.Append(record.TransactionDate.ToString("MMddyyyy"));

            // Transaction amount.
            result.Append(record.TransactionAmount.ToString().PadLeft(TransactionAmountFieldLength, TransactionReportConstants.NumericPaddingCharacter));

            // Last Four digits (PAN).
            result.Append(record.CardLastFourDigits);

            // Card type.
            switch (record.CardType)
            {
                case CardBrand.Visa:
                    result.Append(VisaCard);
                    break;
                case CardBrand.MasterCard:
                    result.Append(MasterCard);
                    break;
            }

            // Transaction time.
            result.Append(record.TransactionDate.ToString("HHmmss"));

            // New/Repeat dine indicator. For now, just set it to New for everyone
            result.Append(NewDineIndicator);

            // Transaction identifier.
            result.Append(record.TransactionIdentifier.Remove(TransactionIdentifierFieldLength));

            // Card Member city.. This is empty for now
            result.Append(TransactionReportConstants.AlphaPaddingCharacter, MemberCityFieldLength);

            // Card Member state.. This is empty for now
            result.Append(TransactionReportConstants.AlphaPaddingCharacter, MemberStateFieldLength);

            // Card Member zip code.. This is empty for now
            result.Append(record.CardMemberZip.PadLeft(MemberZipFieldLength, TransactionReportConstants.NumericPaddingCharacter));            

            // Filler.
            result.Append(TransactionReportConstants.AlphaPaddingCharacter, FillerFieldLength);

            return result.ToString();
        }

        /// <summary>
        /// The record type for transaction report detail record.
        /// </summary>
        private const string RecordType = "S";

        /// <summary>
        /// The length of the record sequence number field.
        /// </summary>
        private const int RecordSequenceFieldLength = 6;

        /// <summary>
        /// The length of the merchant id field.
        /// </summary>
        private const int MerchantIdFieldLength = 15;

        /// <summary>
        /// The length of the merchant name field.
        /// </summary>
        private const int MerchantNameFieldLength = 25;

        /// <summary>
        /// The length of the transaction amount field.
        /// </summary>
        private const int TransactionAmountFieldLength = 7;

        /// <summary>
        /// Representation of visa card brand in the transaction report.
        /// </summary>
        private const string VisaCard = "VI";

        /// <summary>
        /// Representation of Mastercard brand in the transaction report.
        /// </summary>
        private const string MasterCard = "MC";

        /// <summary>
        /// Indicator for a new dining member.
        /// </summary>
        private const string NewDineIndicator = "N";

        /// <summary>
        ///  Indicator for a repeat dining member.
        /// </summary>
        private const string RepeatDineIndicator = "R";

        /// <summary>
        /// The length of the transaction identifier field.
        /// </summary>
        private const int TransactionIdentifierFieldLength = 15;

        /// <summary>
        /// The length of the card member city field.
        /// </summary>
        private const int MemberCityFieldLength = 20;

        /// <summary>
        /// The length of the card member state field.
        /// </summary>
        private const int MemberStateFieldLength = 2;

        /// <summary>
        /// The length of the card member zip field.
        /// </summary>
        private const int MemberZipFieldLength = 5;

        /// <summary>
        /// The length of the filler field.
        /// </summary>
        private const int FillerFieldLength = 139;
    }
}