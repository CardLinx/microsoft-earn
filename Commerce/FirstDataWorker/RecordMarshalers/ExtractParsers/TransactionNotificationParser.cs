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
    /// Parses a First Data Extract transaction notification record.
    /// </summary>
    public class TransactionNotificationParser
    {
        /// <summary>
        /// Initializes a new instance of the TransactionNotificationParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public TransactionNotificationParser(CommerceLog log)
        {
            Log = log;
        }
        
        /// <summary>
        /// Parses the specified record text into a transaction notification object if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into a transaction notification object.
        /// </param>
        /// <param name="recordNumber">
        /// The number of the record of this type from the extract file being parsed.
        /// </param>
        /// <returns>
        /// * The TransactionNotification object if successful.
        /// * Else returns null.
        /// </returns>
        internal TransactionNotification Parse(string record,
                                               int recordNumber)
        {
            TransactionNotification result = new TransactionNotification();
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
            
            // ConsumerId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.ConsumerIdLength, recordValid);
            result.ConsumerId = stringField;

            // TransactionNotificationType
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.TransactionNotificationTypeLength, recordValid);
            TransactionNotificationType transactionNotificationType;
            recordValid = DetermineTransactionNotificationType(stringField, "Transaction Type", out transactionNotificationType,
                                                               recordValid);
            result.TransactionNotificationType = transactionNotificationType;

            // TransactionDateTime
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos, "Transaction Date\" and \"Transaction Time",
                                                            out dateField, true, true, recordValid);
            result.TransactionDateTime = dateField;

            // TransactionAmount
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Transaction Amount", out decimalField,
                                                           ExtractConstants.TransactionAmountLength, recordValid);
            result.TransactionAmount = decimalField;

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
        /// Determines the transaction notification type from the specified string.
        /// </summary>
        /// <param name="receivedValue">
        /// The field from which the transaction notification type will be determined.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="transactionNotificationType">
        /// Receives the transaction notification type if possible.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the transaction notification type could be determined from the specified string.
        /// * Else returns false.
        /// </returns>
        internal bool DetermineTransactionNotificationType(string receivedValue,
                                                           string fieldName,
                                                           out TransactionNotificationType transactionNotificationType,
                                                           bool recordValid)
        {
            transactionNotificationType = TransactionNotificationType.Reversal;
            if (recordValid == true)
            {
                switch (receivedValue.ToUpperInvariant())
                {
                    case "SALE":
                        transactionNotificationType = TransactionNotificationType.Sale;
                        break;
                    case "REVERSAL":
                        transactionNotificationType = TransactionNotificationType.Reversal;
                        break;
                    case "CREDIT":
                        transactionNotificationType = TransactionNotificationType.Credit;
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
        /// Gets or sets the name of the file being parsed.
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// The record type for extract settlement records.
        /// </summary>
        internal const string RecordType = "020";

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// Gets or sets the number of the record of this type from the extract file being parsed.
        /// </summary>
        private int RecordNumber { get; set; }

        /// <summary>
        /// The descriptor for this record type.
        /// </summary>
        private const string RecordTypeDescriptor = "Transaction Notification Extract";

        /// <summary>
        /// The number of spaces that must appear at the end of an extract header.
        /// </summary>
        private const int FillerLength = 713;
    }
}