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
    /// Parses a First Data Detail Acknowledgment record.
    /// </summary>
    public class DetailAcknowledgmentParser
    {
        /// <summary>
        /// Initializes a new instance of the DetailAcknowledgmentParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public DetailAcknowledgmentParser(CommerceLog log)
        {
            Log = log;
        }

        /// <summary>
        /// Parses given record and creates instance of <see cref="DetailAcknowledgment"/>
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
        internal DetailAcknowledgment Parse(string record, int recordNumber)
        {
            SignedValueConverter signedValueConverter = new SignedValueConverter(); ;
            DetailAcknowledgment detailAcknowledgment = new DetailAcknowledgment();
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

            // Card Token : S9(16)
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.TokenLength, recordValid);
            recordValid = signedValueConverter.TryGetStringFromSignedData(stringField, out convertedString);
            detailAcknowledgment.Token = convertedString;

            // TransactionCode
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Transaction Code", TransactionCode,
                                                        AcknowledgmentConstants.TransactionCodeLength, recordValid);

            // TransactionAmount :  S9(6)V99
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.TransactionAmountLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            detailAcknowledgment.TransactionAmount = (decimal) convertedNum/100;

            // TransactionDate : S9(4) format [MMDD]
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.TransactionDateLength, recordValid);
            recordValid = signedValueConverter.TryGetStringFromSignedData(stringField, out convertedString);
            detailAcknowledgment.TransactionDate = recordValid ? GetDateTimeFromString(convertedString) : DateTime.Now;

            // Authorization Code
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.AuthorizationCodeLength, recordValid);
            detailAcknowledgment.AuthorizationCode = stringField;

            // Authorization Date
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.AuthorizationDateLength, recordValid);
            detailAcknowledgment.AuthorizationDate = stringField;

            // Acknowledgment Code : S9(4)
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          AcknowledgmentConstants.AcknowledgmentCodeLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            // We will not get overflow as ack code is at most 4 digits long
            detailAcknowledgment.AcknowledgementCode = Convert.ToInt32(convertedNum);

            // Reference Number :  S9(8)
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                         AcknowledgmentConstants.ReferenceNumberLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            detailAcknowledgment.ReferenceNumber = convertedNum;

            // Record Sequence Number : S9(6)
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                         AcknowledgmentConstants.RecordSequenceNumberLength, recordValid);
            recordValid = signedValueConverter.TryGetNumberFromSignedData(stringField, out convertedNum);
            detailAcknowledgment.RecordSequenceNumber = convertedNum;

            // Merchant Descriptor
// TODO : Make this 21 and ignore the filler below. Side effect of using Bing Offers in merchant description
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                        AcknowledgmentConstants.MerchantDescriptorLength, recordValid);
            detailAcknowledgment.MerchantDescriptor = stringField;

            // Filler
//            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Filler", Filler,
//                                                        AcknowledgmentConstants.FillerLength, recordValid);
            
            // Card Type
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Card Type", CardType,
                                                        AcknowledgmentConstants.CardTypeLength, recordValid);

            // If the record is not valid, return a null value.
            if (recordValid == false)
            {
                detailAcknowledgment = null;
            }

            return detailAcknowledgment;
        }

        /// <summary>
        /// Get DateTime from string date
        /// </summary>
        /// <param name="input">date in string format</param>
        /// <returns>a formatted Date</returns>
        private static DateTime GetDateTimeFromString(string input)
        {
            // incoming string will be of format Mdd, convert it to M-dd so that C# can understand it.
            return DateTime.Parse(input.Insert(input.Length - 2, "-"));
        }

        /// <summary>
        /// The record type for acknowledgment detail record.
        /// </summary>
        internal const string RecordType = "A";

        /// <summary>
        /// Transaction Code for acknowledgment detail record
        /// </summary>
        internal const string TransactionCode = "6";

        /// <summary>
        /// Card Type for acknowledgment detail record
        /// </summary>
        internal const string CardType = "T";

        /// <summary>
        /// Filler for acknowledgment detail record -  2 spaces
        /// </summary>
        internal const string Filler = "  ";

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
        /// The descriptor for this record type.
        /// </summary>
        private const string RecordTypeDescriptor = "Detail Acknowledgment (A) Record";
    }
}