//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Parses a First Data Extract redemption detail record.
    /// </summary>
    public class RedemptionDetailParser : DetailParser
    {
        /// <summary>
        /// Initializes a new instance of the RedemptionDetailParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public RedemptionDetailParser(CommerceLog log)
        {
            Log = log;
            LogRecordType = RecordType;
        }
        
        /// <summary>
        /// Parses the specified record text into a redemption detail object if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into a redemption detail object.
        /// </param>
        /// <param name="recordNumber">
        /// The number of the record of this type from the extract file being parsed.
        /// </param>
        /// <returns>
        /// * The RedemptionDetail object if successful.
        /// * Else returns null.
        /// </returns>
        internal RedemptionDetail Parse(string record,
                                        int recordNumber)
        {
            RedemptionDetail result = new RedemptionDetail();
            RecordNumber = recordNumber;

            int recordPos = 0;
            bool recordValid = true;
            string stringField = null;
            DateTime dateField = DateTime.MinValue;
            decimal decimalField = Decimal.MinValue;

            ParsingUtilities parsingUtilities = new ParsingUtilities(RecordTypeDescriptor, RecordNumber, FileName, ExtractConstants.TimeFieldLength, Log);

            // RecordType
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Record Type", RecordType,
                                                        ExtractConstants.RecordTypeLength, recordValid);

            // ProviderId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.ProviderIdLength, recordValid);
            result.ProviderId = stringField;

            // ProviderLevelNumber
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Hierarchy Level No.",
                                                        ExtractConstants.ProviderLevelNumber,
                                                        ExtractConstants.ProviderLevelNumberLength, recordValid);

            // TransactionId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.TransactionIdLength, recordValid);
            result.TransactionId = stringField;

            // TransactionType
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.TransactionTypeLength, recordValid);
            TransactionType transactionType;
            recordValid = DetermineTransactionType(stringField, "Transaction Type", out transactionType, recordValid);
            result.TransactionType = transactionType;

            // TransactionDateTime
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos, "Transaction Date\" and \"Transaction Time",
                                                            out dateField, true, true, recordValid);
            result.TransactionDateTime = dateField;

            // LocalDateTime
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos, "Local Date\" and \"Local Time",
                                                            out dateField, true, true, recordValid);
            result.LocalDateTime = dateField;

            // PublisherName
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.PublisherNameLength, recordValid);
            result.PublisherName = stringField;

            // PublisherId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.PublisherIdLength, recordValid);
            result.PublisherId = stringField;

            // ConsumerId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.ConsumerIdLength, recordValid);
            result.ConsumerId = stringField;

            // CardSuffix
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.CardSuffixLength, recordValid);
            result.CardSuffix = stringField;

            // OfferId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ExtractConstants.OfferIdLength,
                                                          recordValid);
            result.OfferId = stringField;

            // OfferType
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.OfferTypeLength, recordValid);
            OfferType offerType;
            recordValid = DetermineOfferType(stringField, "Offer Type", out offerType, recordValid);
            result.OfferType = offerType;

            // OfferAcceptanceId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.OfferAcceptanceIdLength, recordValid);
            result.OfferAcceptanceId = stringField;

            // OfferAcceptanceDateTime
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos,
                                                            "Offer Acceptance Date\" and \"Offer Acceptance Time", out dateField, true,
                                                            true, recordValid);
            result.OfferAcceptanceDateTime = dateField;

            // RegistrationMid
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.RegistrationMidLength, recordValid);
            result.RegistrationMid = stringField;

            // RedemptionMid
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.RedemptionMidLength, recordValid);
            result.RedemptionMid = stringField;

            // OfferStartDate
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos,
                                                            "Offer Start Date\" and \"Offer Start Time", out dateField, true,
                                                            true, recordValid);
            result.OfferStartDate = dateField;

            // OfferEndDate
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos,
                                                            "Offer End Date\" and \"Offer End Time", out dateField, true,
                                                            true, recordValid);
            result.OfferEndDate = dateField;

            // PurchasePrice
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Purchase Price", out decimalField,
                                                           ExtractConstants.PurchasePriceLength, recordValid);
            result.PurchasePrice = decimalField;

            // MinimumTransactionAmount
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Minimum Transaction Amount", out decimalField,
                                                           ExtractConstants.MinimumTransactionAmountLength, recordValid);
            result.MinimumTransactionAmount = decimalField;

            // DiscountPercentage
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Discount Percentage", out decimalField,
                                                           ExtractConstants.DiscountPercentageLength, recordValid);
            result.DiscountPercentage = decimalField;

            // DiscountAmount
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Discount Amount", out decimalField,
                                                           ExtractConstants.DiscountAmountLength, recordValid);
            result.DiscountAmount = decimalField;

            // TotalTransactionAmount
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Total Transaction Amount", out decimalField,
                                                           ExtractConstants.TotalTransactionAmountLength, recordValid);
            result.TotalTransactionAmount = decimalField;

            // RedemptionDiscountAmount
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Redemption Discount Amount", out decimalField,
                                                           ExtractConstants.RedemptionDiscountAmountLength, recordValid);
            result.RedemptionDiscountAmount = decimalField;

            // Record end
            parsingUtilities.VerifyRecordEnd(record, ref recordPos, FillerLength, true, recordValid);

            // If the record is not valid, return a null value.
            if (recordValid == false)
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Determines the offer type from the specified string.
        /// </summary>
        /// <param name="receivedValue">
        /// The field from which the offer type will be determined.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="offerType">
        /// Receives the offer type if possible.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the offer type could be determined from the specified string.
        /// * Else returns false.
        /// </returns>
        internal bool DetermineOfferType(string receivedValue,
                                         string fieldName,
                                         out OfferType offerType,
                                         bool recordValid)
        {
            offerType = OfferType.ConsumerClipped;
            if (recordValid == true)
            {
                switch (receivedValue.ToUpperInvariant())
                {
                    case "PURCHASED":
                        offerType = OfferType.Purchased;
                        break;
                    case "CONSUMER CLIPPED":
                        offerType = OfferType.ConsumerClipped;
                        break;
                    case "MERCHANT FUNDED":
                        offerType = OfferType.MerchantFunded;
                        break;
                    default:
                        Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Received value \"{3}\" in field " +
                                    "\"{4}\" is not properly formatted.", (int)ResultCode.InvalidValue, RecordTypeDescriptor,
                                    RecordNumber, FileName, receivedValue, fieldName);
                        recordValid = false;
                        break;
                };
            }

            return recordValid;
        }

        /// <summary>
        /// Gets or sets the type of record being parsed.
        /// </summary>
        internal const string RecordType = "018";

        /// <summary>
        /// The descriptor for this record type.
        /// </summary>
        private const string RecordTypeDescriptor = "Provider Redemption Extract";

        /// <summary>
        /// The number of spaces that must appear at the end of a redemption detail record.
        /// </summary>
        private const int FillerLength = 493;
    }
}