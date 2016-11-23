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
    /// Parses a First Data General Acknowledgment record.
    /// </summary>
    public class GeneralAcknowledgmentParser
    {
        /// <summary>
        /// Initializes a new instance of the DetailAcknowledgmentParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public GeneralAcknowledgmentParser(CommerceLog log)
        {
            Log = log;
        }

        /// <summary>
        /// Parses given record and creates instance of <see cref="GeneralAcknowledgment"/>
        /// </summary>
        /// <remarks>
        /// Some fields are Signed Values listed as S9(x) [Always positive]
        /// Here x is number of digits and least significant digit is in EBCDIC !!
        /// So S9(4) means the field has 4 digits with LSD in EBCDIC.
        /// One more example:
        /// S9(6)V99 means total 8 digits, of which last two are decimal places and LSD in EBCDIC.
        /// </remarks>
        /// <param name="record"></param>
        /// <param name="recordNumber"></param>
        /// <returns></returns>
        internal GeneralAcknowledgment Parse(string record, int recordNumber)
        {
            SignedValueConverter signedValueConverter = new SignedValueConverter(); ;
            GeneralAcknowledgment generalAcknowledgment = new GeneralAcknowledgment();
            RecordNumber = recordNumber;

            int recordPos = 0;
            bool recordValid = true;
            string stringField = null;
            string convertedString;
            long convertedNum;

            ParsingUtilities parsingUtilities = new ParsingUtilities(RecordTypeDescriptor, RecordNumber, FileName, AcknowledgmentConstants.TimeFieldLength, Log);

            // RecordId 
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Record ID", RecordType,
                                                        AcknowledgmentConstants.RecordTypeLength, recordValid);

            // SalesDepositAmount :  S9(7)V99
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.SalesDepositAmountLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            generalAcknowledgment.SalesDepositAmount = (decimal)convertedNum / 100;

            // CreditAmount : S9(7)V99
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.CreditAmountLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            generalAcknowledgment.CreditAmount = (decimal)convertedNum / 100;

            // Cash Advance Deposit Amount : S9(7)V99
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.CashAdvanceDepositAmountLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            generalAcknowledgment.CashAdvanceDepositAmount = (decimal)convertedNum / 100;

            // Acknowledgment Code : S9(4)
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.AcknowledgmentCodeLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            // We will not get overflow as ack code is at most 4 digits long
            generalAcknowledgment.AcknowledgementCode = Convert.ToInt32(convertedNum);

            // FiveSpaceFiller
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Filler", FiveSpaceFiller,
                                                        AcknowledgmentConstants.FiveSpaceFillerLength, recordValid);

            // Record Sequence Number : S9(6)
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                         AcknowledgmentConstants.RecordSequenceNumberLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            generalAcknowledgment.RecordSequenceNumber = convertedNum;

            // Submission Id : S9(9)
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                         AcknowledgmentConstants.SubmissionID, recordValid);
            recordValid = signedValueConverter.TryGetStringFromSignedData(stringField, out convertedString);
            generalAcknowledgment.SubmissionId = convertedString;

            // EightSpaceFiller
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Filler", EightSpaceFiller,
                                                        AcknowledgmentConstants.EightSpaceFillerLength, recordValid);

            // Deposit Authorization Request Amount : S9(7)V99
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.DepositAuthorizationRequestAmountLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            generalAcknowledgment.DepositAuthorizationRequestAmount = (decimal)convertedNum / 100;

            // Cash Advance Deposit Auth Request Amount : S9(7)V99
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.CashAdvanceDepositAuthRequestAmountLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            generalAcknowledgment.CashAdvanceDepositAuthAmount = (decimal)convertedNum / 100;

            // Follow-up Ack Indicator
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                         AcknowledgmentConstants.FollowupAckIndicatorLength, recordValid);
            generalAcknowledgment.FollowUpAcknowledgmentIndicator = stringField;

            // Single Space Filler
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Filler", SingleSpaceFiller,
                                                        AcknowledgmentConstants.SingleSpaceFillerLength, recordValid);
            
            // If the record is not valid, return a null value.
            if (recordValid == false)
            {
                generalAcknowledgment = null;
            }

            return generalAcknowledgment;
        }

        /// <summary>
        /// The record type for acknowledgment detail record.
        /// </summary>
        internal const string RecordType = "B";

        /// <summary>
        /// Gets or sets the name of the file being parsed.
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// Gets or sets the number of the record of this type from the acknowledgment file being parsed.
        /// </summary>
        private int RecordNumber { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// FiveSpaceFiller got General Ack Record - 5 spaces
        /// </summary>
        private const string FiveSpaceFiller = "     "; //5 spaces

        /// <summary>
        /// EightSpaceFiller got General Ack Record - 8 spaces
        /// </summary>
        private const string EightSpaceFiller = "        "; //8 spaces

        /// <summary>
        /// SingleSpaceFiller got General Ack Record - 1 space
        /// </summary>
        private const string SingleSpaceFiller = " "; //1 space

        /// <summary>
        /// The descriptor for this record type.
        /// </summary>
        private const string RecordTypeDescriptor = "General Acknowledgment Summary (B) Record";
    }
}