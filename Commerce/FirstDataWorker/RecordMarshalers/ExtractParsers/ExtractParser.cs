//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System.IO;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Parses a First Data Extract file.
    /// </summary>
    public class ExtractParser
    {
        /// <summary>
        /// Initializes a new instance of the ExtractParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ExtractParser(CommerceLog log)
        {
            Log = log;
            ExtractHeaderParser = new ExtractHeaderParser(log);
            RedemptionDetailParser = new RedemptionDetailParser(log);
            SettlementDetailParser = new SettlementDetailParser(log);
            TransactionNotificationParser = new TransactionNotificationParser(log);
            ExtractFooterParser = new ExtractFooterParser(log);
        }

        /// <summary>
        /// Parses the specified extract file into an extract object if possible.
        /// </summary>
        /// <param name="extractFileName">
        /// The extract file name to parse into an extract object.
        /// </param>
        /// <param name="stream">
        /// File contents as stream of data
        /// </param>
        /// <returns>
        /// * The Extract object resulting from the parsing attempt if the specified extract file could be found.
        /// * Otherwise returns null.
        /// </returns>
        internal Extract Parse(string extractFileName, Stream stream)
        {
            Extract = new Extract();
            LineNumber = 0;
            NumberOfHeaderRecords = 0;
            NumberOfRedemptionDetailRecords = 0;
            NumberOfSettlementDetailRecords = 0;
            NumberOfTransactionNotificationRecords = 0;
            NumberOfFooterRecords = 0;

            if (stream != null)
            {
                FileName = Path.GetFileName(extractFileName);
                ExtractHeaderParser.FileName = FileName;
                RedemptionDetailParser.FileName = FileName;
                SettlementDetailParser.FileName = FileName;
                TransactionNotificationParser.FileName = FileName;
                ExtractFooterParser.FileName = FileName;
                bool loggedFooterOutOfPlace = false;

                // Read the extract file one line at a time and parse each line as a record.
                string recordType;
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    while ((Line = streamReader.ReadLine()) != null)
                    {
                        if (Line.Length > ExtractConstants.RecordTypeLength)
                        {
                            LineNumber++;
                            recordType = Line.Substring(0, ExtractConstants.RecordTypeLength);

                            // Log a warning if the footer record has already been parsed and a warning was not already logged.
                            if (NumberOfFooterRecords == 1 && loggedFooterOutOfPlace == false)
                            {
                                Log.Warning("Error parsing record in line #{0} from file \"{1}\". One or more records found  " +
                                            "after footer record parsed.", (int)ResultCode.RecordOutOfPlace, LineNumber,
                                            FileName);
                                loggedFooterOutOfPlace = true;
                            }

                            // Parse the record according to its type and add it to the Extract object.
                            switch(recordType)
                            {
                                case ExtractHeaderParser.RecordType:
                                    ParseHeaderRecord();
                                    break;
                                case RedemptionDetailParser.RecordType:
                                    ParseRedemptionDetailRecord();
                                    break;
                                case SettlementDetailParser.RecordType:
                                    ParseSettlementDetailRecord();
                                    break;
                                case TransactionNotificationParser.RecordType:
                                    ParseTransactionNotificationRecord();
                                    break;
                                case ExtractFooterParser.RecordType:
                                    ParseFooterRecord();
                                    break;
                                default:
                                    Log.Warning("Error parsing record in line #{0} from file \"{1}\". Encountered invalid " +
                                                "record type \"{2}\".", (int)ResultCode.ExpectedValueNotFound, LineNumber,
                                                FileName, recordType);
                                    break;
                            }
                        }
                        else
                        {
                            Log.Warning("Error parsing record in line #{0} from file \"{1}\". Unexpected end of record detected.",
                                        (int)ResultCode.UnexpectedEndOfRecord, LineNumber, FileName);
                        }
                    }
                }

                // Add entries to the log if the extract file did not have the expected number of records of each type.
                if (Extract.Header == null)
                {
                    Log.Warning("Header for extract file \"{0}\" missing or corrupted.",
                                (int)ResultCode.FileMissingExpectedRecord, extractFileName);
                }

                if (Extract.RedemptionDetailRecords.Count == 0)
                {
                    Log.Information("Extract file \"{0}\" contained no redemption detail records.", extractFileName);
                }

                if (Extract.SettlementDetailRecords.Count == 0)
                {
                    Log.Information("Extract file \"{0}\" contained no settlement detail records.", extractFileName);
                }

                if (Extract.TransactionNotificationRecords.Count == 0)
                {
                    Log.Information("Extract file \"{0}\" contained no transaction notification records.", extractFileName);
                }

                if (Extract.Footer == null)
                {
                    Log.Warning("Footer for extract file \"{0}\" missing or corrupted.",
                                (int)ResultCode.FileMissingExpectedRecord, extractFileName);
                }
            }
            else
            {
                Log.Error("Extract file \"{0}\" could not be found.", null, (int)ResultCode.FileNotFound, extractFileName);
                Extract = null;
            }

            return Extract;
        }

        /// <summary>
        /// Parses a header record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseHeaderRecord()
        {
            if (NumberOfHeaderRecords == 0)
            {
                if (LineNumber > 1)
                {
                    Log.Warning("Error parsing record in line #{0} from file \"{1}\". One or more records found  " +
                                "before header record parsed.", (int)ResultCode.RecordOutOfPlace, LineNumber, FileName);
                }

                Extract.Header = ExtractHeaderParser.Parse(Line);
                NumberOfHeaderRecords = 1;
            }
            else
            {
                Log.Warning("Error parsing record in line #{0} from file \"{1}\". More than one header record found. All " +
                            "header records after the first one encountered will be ignored.",
                            (int)ResultCode.UnexpectedDuplicateRecordTypeFound, LineNumber, FileName);
            }
        }

        /// <summary>
        /// Parses a redemption detail record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseRedemptionDetailRecord()
        {
            RedemptionDetail redemptionDetail = RedemptionDetailParser.Parse(Line, NumberOfRedemptionDetailRecords++);
            if (redemptionDetail == null)
            {
                Log.Error("Record in line #{0} from file \"{1}\" could not be parsed.", null,
                          (int)ResultCode.CorruptSettlementRecord, LineNumber, FileName);
            }

            Extract.RedemptionDetailRecords.Add(redemptionDetail);
        }

        /// <summary>
        /// Parses a settlement detail record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseSettlementDetailRecord()
        {
            SettlementDetail settlementDetail = SettlementDetailParser.Parse(Line, NumberOfSettlementDetailRecords++);
            if (settlementDetail == null)
            {
                Log.Error("Record in line #{0} from file \"{1}\" could not be parsed.", null,
                          (int)ResultCode.CorruptSettlementRecord, LineNumber, FileName);
            }

            Extract.SettlementDetailRecords.Add(settlementDetail);
        }

        /// <summary>
        /// Parses a transaction notification record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseTransactionNotificationRecord()
        {
            TransactionNotification transactionNotification = TransactionNotificationParser.Parse(Line,
                                                                                       NumberOfTransactionNotificationRecords++);
            if (transactionNotification == null)
            {
                Log.Error("Record in line #{0} from file \"{1}\" could not be parsed.", null,
                          (int)ResultCode.CorruptSettlementRecord, LineNumber, FileName);
            }

            Extract.TransactionNotificationRecords.Add(transactionNotification);
        }

        /// <summary>
        /// Parses a footer record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseFooterRecord()
        {
            if (NumberOfFooterRecords == 0)
            {
                Extract.Footer = ExtractFooterParser.Parse(Line);
                NumberOfFooterRecords = 1;
            }
            else
            {
                Log.Warning("Error parsing record in line #{0} from file \"{1}\". More than one footer record found. All " +
                            "footer records after the first one encountered will be ignored.",
                            (int)ResultCode.UnexpectedDuplicateRecordTypeFound, LineNumber, FileName);
            }
        }

        /// <summary>
        /// Gets or sets the Extract built by this parser instance.
        /// </summary>
        private Extract Extract { get; set; }

        /// <summary>
        /// Gets or sets the name of the extract file being parsed.
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Gets or sets the extract header parser to use when parsing header records.
        /// </summary>
        private ExtractHeaderParser ExtractHeaderParser { get; set; }

        /// <summary>
        /// Gets or sets the redemption detail parser to use when parsing redemption detail records.
        /// </summary>
        private RedemptionDetailParser RedemptionDetailParser { get; set; }

        /// <summary>
        /// Gets or sets the settlement detail parser to use when parsing settlement detail records.
        /// </summary>
        private SettlementDetailParser SettlementDetailParser { get; set; }

        /// <summary>
        /// Gets or sets the transaction notification parser to use when parsing transaction notification records.
        /// </summary>
        private TransactionNotificationParser TransactionNotificationParser { get; set; }

        /// <summary>
        /// Gets or sets the extract footer parser to use when parsing footer records.
        /// </summary>
        private ExtractFooterParser ExtractFooterParser { get; set; }

        /// <summary>
        /// Gets or sets the line currently being parsed.
        /// </summary>
        private string Line { get; set; }

        /// <summary>
        /// Gets or sets the line number currently being parsed.
        /// </summary>
        private int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of header records found in the extract file.
        /// </summary>
        private int NumberOfHeaderRecords { get; set; }

        /// <summary>
        /// Gets or sets the number of redemption detail records found in the extract file.
        /// </summary>
        private int NumberOfRedemptionDetailRecords { get; set; }

        /// <summary>
        /// Gets or sets the number of settlement detail records found in the extract file.
        /// </summary>
        private int NumberOfSettlementDetailRecords { get; set; }

        /// <summary>
        /// Gets or sets the number of transaction notification records found in the extract file.
        /// </summary>
        private int NumberOfTransactionNotificationRecords { get; set; }

        /// <summary>
        /// Gets or sets the number of footer records found in the extract file.
        /// </summary>
        private int NumberOfFooterRecords { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}