//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Parses a First Data Extract footer record.
    /// </summary>
    public class ExtractFooterParser
    {
        /// <summary>
        /// Initializes a new instance of the ExtractFooterParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ExtractFooterParser(CommerceLog log)
        {
            Log = log;
        }
        
        /// <summary>
        /// Parses the specified record text into an extract footer if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into an extract footer.
        /// </param>
        /// <returns>
        /// * The ExtractFooter object if successful.
        /// * Else returns null.
        /// </returns>
        internal ExtractFooter Parse(string record)
        {
            ExtractFooter result = new ExtractFooter();

            int recordPos = 0;
            bool recordValid = true;
            string stringField = null;
            DateTime dateField = DateTime.MinValue;
            long longField = Int64.MinValue;
            decimal decimalField = Decimal.MinValue;
            ParsingUtilities parsingUtilities = new ParsingUtilities("Extract File Trailer", 1, FileName, ExtractConstants.TimeFieldLength, Log);

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

            // CreationDate
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos, "File Creation Date", out dateField, false, true,
                                                            recordValid);
            result.CreationDate = dateField;

            // NumberOfHeaderRecords
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "001 Records - Count", out longField,
                                                        ExtractConstants.NumberOfHeaderRecordsLength, recordValid);
            result.NumberOfHeaderRecords = longField;

            // NumberOfRedemptionRecords
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "018 Records - Count", out longField,
                                                        ExtractConstants.NumberOfRedemptionRecordsLength, recordValid);
            result.NumberOfRedemptionRecords = longField;

            // TotalRedemptionRecordAmount
            recordValid = ParseAmountField(record, ref recordPos, parsingUtilities, "018 Records - Amount", recordValid,
                                           out decimalField);
            result.TotalRedemptionRecordAmount = decimalField;

            // NumberOfSettlementRecords
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "019 Records - Count", out longField,
                                                        ExtractConstants.NumberOfSettlementRecordsLength, recordValid);
            result.NumberOfSettlementRecords = longField;

            // TotalSettlementRecordAmount
            recordValid = ParseAmountField(record, ref recordPos, parsingUtilities, "019 Records - Amount", recordValid,
                                           out decimalField);
            result.TotalSettlementRecordAmount = decimalField;

            // NumberOfTransactionNotificationRecords
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "020 Records - Count", out longField,
                                                        ExtractConstants.NumberOfTransactionNotificationRecordsLength,
                                                        recordValid);
            result.NumberOfTransactionNotificationRecords = longField;

            // TotalTransactionNotificationAmount
            recordValid = ParseAmountField(record, ref recordPos, parsingUtilities, "020 Records - Amount", recordValid,
                                           out decimalField);
            result.TotalTransactionNotificationAmount = decimalField;
            
            // NumberOfFooterRecords
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "999 Records - Count", out longField,
                                                        ExtractConstants.NumberOfFooterRecordsLength, recordValid);
            result.NumberOfFooterRecords = longField;

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
        /// Gets or sets the name of the file being parsed.
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// The record type for extract footers.
        /// </summary>
        internal const string RecordType = "999";

        /// <summary>
        /// Parses an amount field.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into an extract footer.
        /// </param>
        /// <param name="recordPos">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="parsingUtilities">
        /// The ParsingUtilities object to use to parse the record.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <param name="amount">
        /// Receives the parsed amount if the operations succeeds.
        /// </param>
        /// <returns>
        /// * True if the string is valid.
        /// * Else returns false.
        /// </returns>
        private static bool ParseAmountField(string record,
                                             ref int recordPos,
                                             ParsingUtilities parsingUtilities,
                                             string fieldName,
                                             bool recordValid,
                                             out decimal amount)
        {
            amount = 0;
            if (recordValid == true)
            {
                // First, get the sign.
                string amountSign;
                recordValid = parsingUtilities.PopulateString(record, ref recordPos, out amountSign,
                                                              ExtractConstants.AmountSignLength, recordValid);

                // Next, get the amount absolute value.
                recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, fieldName, out amount,
                                                               ExtractConstants.AmountLength, recordValid);
                
                // Apply sign to amount.
                switch (amountSign)
                {
                    case "+":
                    case "0":
                        break;
                    case "-":
                        amount *= -1;
                        break;
                    default:
                        recordValid = false;
                        break;
                };
            }

            return recordValid;
        }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// The number of spaces that must appear at the end of an extract footer.
        /// </summary>
        private const int FillerLength = 844;
    }
}