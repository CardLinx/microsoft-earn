//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Parses a MasterCard Clearing data record.
    /// </summary>
    public class ClearingDataParser
    {
        /// <summary>
        /// Initializes a new instance of the ClearingDataParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ClearingDataParser(CommerceLog log)
        {
            Log = log;
        }
        
        /// <summary>
        /// Parses the specified record text into a clearing data record if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into a clearing data record.
        /// </param>
        /// <param name="recordNumber">
        /// The number of the record of this type from the extract file being parsed.
        /// </param>
        /// <returns>
        /// * The ClearingData object if successful.
        /// * Else returns null.
        /// </returns>
        internal ClearingData Parse(string record,
                                    int recordNumber)
        {
            ClearingData result = new ClearingData();

            int recordPos = 0;
            bool recordValid = true;
            string stringField = null;
            decimal decimalField = Decimal.MinValue;
            DateTime dateField = DateTime.MinValue;

            ParsingUtilities parsingUtilities = new ParsingUtilities("Clearing File Data Record", recordNumber, FileName, ClearingConstants.ShortTimeFieldLength, Log);

            // RecordType
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Record Type", RecordType, ClearingConstants.RecordTypeLength, recordValid);

            // Transaction sequence number.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.TransactionSequenceNumberLength, recordValid);
            result.TransactionSequenceNumber = stringField;

            // Bank account number.
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Bank Account Number", EmptyBankAccountNumber, ClearingConstants.BankAccountNumberLength,
                                                        false, recordValid);

            // Transaction amount.
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Transaction Amount", out decimalField, ClearingConstants.TransactionAmountLength,
                                                           recordValid);
            result.TransactionAmount = decimalField;

            // Transaction date. Save this for later combination with transaction time.
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos, "Transaction Date", out dateField, false, false, recordValid);
            DateTime transactionDate = dateField;

            // Merchant DBA name.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.MerchantDbaNameLength, recordValid);
            result.MerchantDbaName = stringField;

            // Merchant ID.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.MerchantIdLength, recordValid);
            result.MerchantId = stringField;
            
            // Location ID.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.LocationIdLength, recordValid);
            result.LocationId = stringField;

            // Issuer ICA.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.IssuerIcaLength, recordValid);
            result.IssuerIca = stringField;

            // Transaction time.
            recordValid = parsingUtilities.PopulateTimeForExistingDate(record, ref recordPos, "Transaction Time", ref transactionDate, recordValid);
            result.TransactionDate = transactionDate;

            // Bank Net ref number.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.BankNetRefNumberLength, recordValid);
            result.BankNetRefNumber = stringField;

            // Bank customer number.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.BankCustomerNumberLength, recordValid);
            result.BankCustomerNumber = stringField;

            // Aggregate merchant ID.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.AggregateMerchantIdLength, recordValid);
            result.AggregateMerchantId = stringField;

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
        /// The record type for clearing data records.
        /// </summary>
        internal const string RecordType = "D";

        /// <summary>
        /// The string that appears when the bank account is ommitted.
        /// </summary>
        private const string EmptyBankAccountNumber = "                   ";

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}