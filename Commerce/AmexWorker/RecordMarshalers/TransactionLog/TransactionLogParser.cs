//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using System.Collections.ObjectModel;
    using System.IO;
    using Lomo.Commerce.AmexClient;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Class to parse Amex Transaction Log File
    /// </summary>
    public class TransactionLogParser
    {
        /// <summary>
        /// Initializes a new instance of the TransactionLogParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public TransactionLogParser(CommerceLog log)
        {
            Log = log;
        }

        /// <summary>
        /// Parse the file with given name and stream
        /// </summary>
        /// <param name="transactionLogFileName">
        /// Name of the file to parse
        /// </param>
        /// <param name="stream">
        /// File stream 
        /// </param>
        /// <returns>
        /// Marshalled TransactionLogFile object
        /// </returns>
        public TransactionLogFile Parse(string transactionLogFileName, Stream stream)
        {
            TransactionLogFile transactionLogFile = new TransactionLogFile();
            TransactionLogHeader header = null;
            TransactionLogTrailer trailer = null;
            Collection<TransactionLogDetail> detailRecords = new Collection<TransactionLogDetail>();

            if (stream != null)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string recordType = line.Substring(0, 1);
                        switch (recordType)
                        {
                            case "H":
                                header = new TransactionLogHeader(line);
                                break;
                            case "D":
                                detailRecords.Add(new TransactionLogDetail(line));
                                break;
                            case "T":
                                trailer = new TransactionLogTrailer(line);
                                break;
                        }
                    }
                }
            }

            // verify integrity
            if (trailer != null && trailer.TrailerCount != detailRecords.Count)
            {
                Log.Warning("Number of Records suggested by trailer Amex TransactionLog file \"{0}\" do not match.",
                             (int)ResultCode.FileMissingExpectedRecord, transactionLogFileName);
            }

            transactionLogFile.Header = header;
            transactionLogFile.Trailer = trailer;
            foreach (TransactionLogDetail detailRecord in detailRecords)
            {
                transactionLogFile.TransactionLogRecords.Add(detailRecord);
            }

            return transactionLogFile;
        }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}