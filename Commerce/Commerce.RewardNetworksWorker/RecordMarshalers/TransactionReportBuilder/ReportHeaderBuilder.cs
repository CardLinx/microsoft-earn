//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Commerce.RewardsNetworkWorker.RecordMarshalers.TransactionReportBuilder
{
    using System;
    using System.Text;

    public class ReportHeaderBuilder
    {
        /// <summary>
        /// Builds the transaction report header record
        /// </summary>
        /// <param name="reportDateTime">Transaction report date</param>
        /// <param name="detailRecordCount">Total number of transaction records in this report</param>
        /// <returns>Header record string</returns>
        public static string Build(DateTime reportDateTime, int detailRecordCount)
        {
            StringBuilder result = new StringBuilder();

            // Record type.
            result.Append(RecordType);
            
            // Record Sequence number
            result.Append(RecordSequenceNo);

            // Report date.
            result.Append(reportDateTime.ToString("MMddyy"));

            //Transaction report file name
            result.Append(FileNameInHeader.PadRight(FileNameFieldLength,TransactionReportConstants.AlphaPaddingCharacter));

            // Sequence number of the first transaction record begins with 2 since the sequence number of the header record is 1
            // so, the sequence number of the final transaction record will be 1 more than the total record count
            int finalRecordSequenceNumber = detailRecordCount + 1;

            // Total number of transaction records contained in this file.
            result.Append(finalRecordSequenceNumber.ToString().PadLeft(FinalSequenceNumberFieldLength, TransactionReportConstants.NumericPaddingCharacter));

            // File transmission id.
            result.Append(TransmissionId);

            // Filler.
            result.Append(TransactionReportConstants.AlphaPaddingCharacter, FillerFieldLength);

            return result.ToString();
        }

        /// <summary>
        /// The record type for transaction report header records.
        /// </summary>
        private const string RecordType = "H";

        /// <summary>
        /// Record sequence number for the transaction report.
        /// </summary>
        private const string RecordSequenceNo = "000001";

        /// <summary>
        /// Transaction file name to be used in the header record.
        /// </summary>
        private const string FileNameInHeader = "MSFTTransactions";

        /// <summary>
        /// Length of the file name field
        /// </summary>
        private const int FileNameFieldLength = 22;

        /// <summary>
        /// Length of the final record sequence number field
        /// </summary>
        private const int FinalSequenceNumberFieldLength = 6;

        /// <summary>
        /// File transmission id provided by reward networks.
        /// </summary>
        private const string TransmissionId = "MSFT";

        /// <summary>
        /// The length of the filler field.
        /// </summary>
        private const int FillerFieldLength = 211;
    }
}