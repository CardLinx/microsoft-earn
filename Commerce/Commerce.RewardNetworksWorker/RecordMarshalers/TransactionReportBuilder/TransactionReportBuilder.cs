//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Data;

namespace Commerce.RewardsNetworkWorker.RecordMarshalers.TransactionReportBuilder
{
    using System;
    using System.Text;
    using System.Collections.ObjectModel;

    public static class TransactionReportBuilder
    {
        /// <summary>
        /// Builds the reward network transaction file containing all the transactions that got cleared on the specified date
        /// </summary>
        /// <param name="transactionRecords">List of all cleared reward network transactions</param>
        /// <param name="reportDateTime">Report generation date</param>
        /// <returns>Reward network transaction file name and the file contents</returns>
        public static Tuple<String, String> Build(Collection<TransactionRecord> transactionRecords, DateTime reportDateTime)
        {
            StringBuilder sbFileContents = new StringBuilder();
            string reportDate = reportDateTime.ToString("yyyyMMdd");

            // Add the transaction file header.
            sbFileContents.AppendLine(ReportHeaderBuilder.Build(reportDateTime, transactionRecords.Count));

            //Sequence number of transaction record begins with 2.
            int transactionRecordSeqNo = 2;

            // Add the individual transaction records.
            for (int i = 0; i < transactionRecords.Count; i++)
            {
                transactionRecords[i].RecordSequenceNumber = transactionRecordSeqNo;
                if (i == transactionRecords.Count - 1)
                {
                    sbFileContents.Append(ReportRecordBuilder.Build(transactionRecords[i]));
                }
                else
                {
                    sbFileContents.AppendLine(ReportRecordBuilder.Build(transactionRecords[i]));
                }

                transactionRecordSeqNo += 1;
            }

            string transactionReportFileName = String.Concat(TransactionFileDecoration, reportDate, FileExtension);
            Tuple<string, string> buildResult = new Tuple<string, string>(transactionReportFileName, sbFileContents.ToString());

            return buildResult;
        }

        /// <summary>
        /// The decoration to add to rebate filenames.
        /// </summary>
        private const string TransactionFileDecoration = "MSFTTransactions";

        /// <summary>
        /// The extension to add to filenames.
        /// </summary>
        private const string FileExtension = ".txt";

    }
}