//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Parses a MasterCard RebateConfirmation data record.
    /// </summary>
    public class RebateConfirmationDataParser
    {
        /// <summary>
        /// Initializes a new instance of the RebateConfirmationDataParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public RebateConfirmationDataParser(CommerceLog log)
        {
            Log = log;
        }
        
        /// <summary>
        /// Parses the specified record text into a rebate confirmation data record if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into a rebate confirmation data record.
        /// </param>
        /// <param name="recordNumber">
        /// The number of the record of this type from the extract file being parsed.
        /// </param>
        /// <returns>
        /// * The RebateConfirmationData object if successful.
        /// * Else returns null.
        /// </returns>
        internal RebateConfirmationData Parse(string record,
                                              int recordNumber)
        {
            RebateConfirmationData result = new RebateConfirmationData();

            int recordPos = 0;
            bool recordValid = true;
            string stringField = null;
            decimal decimalField = Decimal.MinValue;
            DateTime dateField = DateTime.MinValue;

            ParsingUtilities parsingUtilities = new ParsingUtilities("Rebate Confirmation File Data Record", recordNumber, FileName,
                                                                     RebateConfirmationConstants.TimeFieldLength, Log);

            // RecordType
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Record Type", RecordType, RebateConfirmationConstants.RecordTypeLength, recordValid);

            // Bank customer number.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, RebateConfirmationConstants.BankCustomerNumberLength, recordValid);
            result.BankCustomerNumber = stringField;

            // Bank account number.
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Bank Account Number", EmptyBankAccountNumber,
                                                        RebateConfirmationConstants.BankAccountNumberLength, false, recordValid);

            // Bank product code.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, RebateConfirmationConstants.BankProductCodeLength, recordValid);
            result.BankProductCode = stringField;

            // Transaction description.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, RebateConfirmationConstants.TransactionDescriptionLength, recordValid);
            result.TransactionDescription = stringField;

            // Rebate amount.
            recordValid = parsingUtilities.PopulateDecimal(record, ref recordPos, "Rebate Amount", out decimalField, RebateConfirmationConstants.RebateAmountLength,
                                                           recordValid);
            result.RebateAmount = decimalField;

            // Exception reason code.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, RebateConfirmationConstants.ExceptionReasonCodeLength, recordValid);
            ExceptionReasonCode exceptionReasonCode;
            switch (stringField)
            {
                case "A":
                    exceptionReasonCode = ExceptionReasonCode.AccountNotFound;
                    break;
                case "C":
                    exceptionReasonCode = ExceptionReasonCode.CustomerNotFound;
                    break;
                case "I":
                    exceptionReasonCode = ExceptionReasonCode.InvalidAccount;
                    break;
                case "M":
                    exceptionReasonCode = ExceptionReasonCode.MultipleAccountsFound;
                    break;
                case "O":
                    exceptionReasonCode = ExceptionReasonCode.Others;
                    break;
                case "R":
                    exceptionReasonCode = ExceptionReasonCode.InvalidAccountCountry;
                    break;
                default:
                    Log.Warning("Error parsing record in line #{0} from file \"{1}\". Encountered unrecognized exception reason code \"{2}\".",
                                (int)ResultCode.ExpectedValueNotFound, recordNumber, FileName, stringField);
                    exceptionReasonCode = ExceptionReasonCode.Unrecognized;
                    recordValid = false;
                    break;
            }
            result.ExceptionReasonCode = exceptionReasonCode;

            // Exception reason description.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, RebateConfirmationConstants.ExceptionReasonDescriptionLength, recordValid);
            result.ExceptionReasonDescription = stringField;

            // Rebate file send date.
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos, "Rebate File Send Date", out dateField, false, false, recordValid);
            result.RebateFileSendDate = dateField;

            // Transaction sequence number.
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, RebateConfirmationConstants.TransactionSequenceNumberLength, recordValid);
            result.TransactionSequenceNumber = stringField;

            // Record end
            parsingUtilities.VerifyRecordEnd(record, ref recordPos, FillerLength, false, recordValid);

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
        /// The record type for rebate confirmation data records.
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

        /// <summary>
        /// The number of spaces that must appear at the end of an rebate confirmation data record.
        /// </summary>
        private const int FillerLength = 43;
    }
}