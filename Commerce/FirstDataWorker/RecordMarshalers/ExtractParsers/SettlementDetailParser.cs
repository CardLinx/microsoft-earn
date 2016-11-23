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
    /// Parses a First Data Extract settlement detail record.
    /// </summary>
    public class SettlementDetailParser : DetailParser
    {
        /// <summary>
        /// Initializes a new instance of the SettlementDetailParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public SettlementDetailParser(CommerceLog log)
        {
            Log = log;
            LogRecordType = RecordType;
        }
        
        /// <summary>
        /// Parses the specified record text into a settlement detail object if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into a settlement detail object.
        /// </param>
        /// <param name="recordNumber">
        /// The number of the record of this type from the extract file being parsed.
        /// </param>
        /// <returns>
        /// * The SettlementDetail object if successful.
        /// * Else returns null.
        /// </returns>
        internal SettlementDetail Parse(string record,
                                        int recordNumber)
        {
            SettlementDetail result = new SettlementDetail();
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

            // LocationMid
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.LocationMidLength, recordValid);
            result.LocationMid = stringField;

            // ChainId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ExtractConstants.ChainIdLength,
                                                          recordValid);
            result.ChainId = stringField;

            // CorporateId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.CorporateIdLength, recordValid);
            result.CorporateId = stringField;

            // TerminalId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.TerminalIdLength, recordValid);
            result.TerminalId = stringField;

            // BankMarker
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.BankMarkerLength, recordValid);
            result.BankMarker = stringField;

            // CardSuffix
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.CardSuffixLength, recordValid);
            result.CardSuffix = stringField;
            
            // OfferId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ExtractConstants.OfferIdLength,
                                                          recordValid);
            result.OfferId = stringField;

            // ConsumerId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.ConsumerIdLength, recordValid);
            result.ConsumerId = stringField;

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

            // TotalTransactionAmount
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Total Transaction Amount", out decimalField,
                                                           ExtractConstants.TotalTransactionAmountLength, recordValid);
            result.TotalTransactionAmount = decimalField;

            // RedemptionDiscountAmount
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Redemption Discount Amount", out decimalField,
                                                           ExtractConstants.RedemptionDiscountAmountLength, recordValid);
            result.RedemptionDiscountAmount = decimalField;

            // CurrencyCode
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.CurrencyCodeLength, recordValid);
            result.CurrencyCode = stringField;

            // InvoiceId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.InvoiceIdLength, recordValid);
            result.InvoiceId = stringField;

            // AcquirerReferenceNumber
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, 
                                                          ExtractConstants.AcquirerReferenceNumberLength, recordValid);
            result.AcquirerReferenceNumber = stringField;

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
        /// Gets or sets the type of record being parsed.
        /// </summary>
        internal const string RecordType = "019";

        /// <summary>
        /// The descriptor for this record type.
        /// </summary>
        private const string RecordTypeDescriptor = "Settlement Redemption Extract";

        /// <summary>
        /// The number of spaces that must appear at the end of an extract header.
        /// </summary>
        private const int FillerLength = 644;
    }
}