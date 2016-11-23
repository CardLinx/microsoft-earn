//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System.IO;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.MasterCardClient;

    /// <summary>
    /// Parses a MasterCard Rebate Confirmation file.
    /// </summary>
    public class RebateConfirmationParser
    {
        /// <summary>
        /// Initializes a new instance of the RebateConfirmationParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public RebateConfirmationParser(CommerceLog log)
        {
            Log = log;
            RebateConfirmationHeaderParser = new RebateConfirmationHeaderParser(log);
            RebateConfirmationDataParser = new RebateConfirmationDataParser(log);
            RebateConfirmationTrailerParser = new RebateConfirmationTrailerParser(log);
        }

        /// <summary>
        /// Parses the specified rebate confirmation file into a rebate confirmation object if possible.
        /// </summary>
        /// <param name="rebateConfirmationFileName">
        /// The rebate confirmation file name to parse into a RebateConfirmation object.
        /// </param>
        /// <param name="stream">
        /// File contents as stream of data
        /// </param>
        /// <returns>
        /// * The RebateConfirmation object resulting from the parsing attempt if the specified rebate confirmation file could be found.
        /// * Otherwise returns null.
        /// </returns>
        internal RebateConfirmation Parse(string rebateConfirmationFileName,
                                          Stream stream)
        {
            RebateConfirmation = new RebateConfirmation();
            LineNumber = 0;
            NumberOfHeaderRecords = 0;
            NumberOfDataRecords = 0;
            NumberOfTrailerRecords = 0;

            if (stream != null)
            {
                FileName = Path.GetFileName(rebateConfirmationFileName);
                RebateConfirmationHeaderParser.FileName = FileName;
                RebateConfirmationDataParser.FileName = FileName;
                RebateConfirmationTrailerParser.FileName = FileName;
                bool loggedTrailerOutOfPlace = false;

                // Read the rebate confirmation file one line at a time and parse each line as a record.
                string recordType;
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    while ((Line = streamReader.ReadLine()) != null)
                    {
                        if (Line.Length > RebateConfirmationConstants.RecordTypeLength)
                        {
                            LineNumber++;
                            recordType = Line.Substring(0, RebateConfirmationConstants.RecordTypeLength);

                            // Log a warning if the trailer record has already been parsed and a warning was not already logged.
                            if (TrailerFound == true && loggedTrailerOutOfPlace == false)
                            {
                                Log.Warning("Error parsing record in line #{0} from file \"{1}\". One or more records found  " +
                                            "after trailer record parsed.", (int)ResultCode.RecordOutOfPlace, LineNumber,
                                            FileName);
                                loggedTrailerOutOfPlace = true;
                            }

                            // Parse the record according to its type and add it to the RebateConfirmation object.
                            switch(recordType)
                            {
                                case RebateConfirmationHeaderParser.RecordType:
                                    ParseHeaderRecord();
                                    break;
                                case RebateConfirmationDataParser.RecordType:
                                    ParseDataRecord();
                                    break;
                                case RebateConfirmationTrailerParser.RecordType:
                                    ParseTrailerRecord();
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

                // Add entries to the log if the rebate confirmation file did not have the expected number of records.
                if (RebateConfirmation.Header == null)
                {
                    Log.Warning("Header for RebateConfirmation file \"{0}\" missing or corrupted.",
                                (int)ResultCode.FileMissingExpectedRecord, rebateConfirmationFileName);
                }

                if (RebateConfirmation.DataRecords.Count == 0)
                {
                    Log.Information("RebateConfirmation file \"{0}\" contained no data records.", rebateConfirmationFileName);
                }

                if (RebateConfirmation.Trailer == null)
                {
                    Log.Warning("Trailer for rebate confirmation file \"{0}\" missing or corrupted.",
                                (int)ResultCode.FileMissingExpectedRecord, rebateConfirmationFileName);
                }
                else
                {
                    if (NumberOfDataRecords != RebateConfirmation.Trailer.ExceptionRecordCount)
                    {
                        Log.Warning("Exception record count for rebate confirmation file \"{0}\" is {1} but trailer indicated it should be {2}.",
                                    (int)ResultCode.RecordCountMismatch, rebateConfirmationFileName, NumberOfDataRecords, RebateConfirmation.Trailer.ExceptionRecordCount);
                    }
                }
            }
            else
            {
                Log.Error("RebateConfirmation file \"{0}\" could not be found.", null, (int)ResultCode.FileNotFound, rebateConfirmationFileName);
                RebateConfirmation = null;
            }

            return RebateConfirmation;
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

                RebateConfirmation.Header = RebateConfirmationHeaderParser.Parse(Line);
            }
            else
            {
                Log.Warning("Error parsing record in line #{0} from file \"{1}\". More than one header record found. All " +
                            "header records after the first one encountered will be ignored.",
                            (int)ResultCode.UnexpectedDuplicateRecordTypeFound, LineNumber, FileName);
            }

            NumberOfHeaderRecords++;
        }

        /// <summary>
        /// Parses a data record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseDataRecord()
        {
            RebateConfirmationData rebateConfirmationData = RebateConfirmationDataParser.Parse(Line, NumberOfDataRecords++);
            if (rebateConfirmationData == null)
            {
                Log.Error("Record in line #{0} from file \"{1}\" could not be parsed.", null,
                          (int)ResultCode.CorruptSettlementRecord, LineNumber, FileName);
            }

            RebateConfirmation.DataRecords.Add(rebateConfirmationData);
        }

        /// <summary>
        /// Parses a trailer record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseTrailerRecord()
        {
            if (NumberOfTrailerRecords == 0)
            {
                RebateConfirmation.Trailer = RebateConfirmationTrailerParser.Parse(Line);
                TrailerFound = true;
            }
            else
            {
                Log.Warning("Error parsing record in line #{0} from file \"{1}\". More than one trailer record found. All " +
                            "trailer records after the first one encountered will be ignored.",
                            (int)ResultCode.UnexpectedDuplicateRecordTypeFound, LineNumber, FileName);
            }

            NumberOfTrailerRecords++;
        }

        /// <summary>
        /// Gets or sets the RebateConfirmation built by this parser instance.
        /// </summary>
        private RebateConfirmation RebateConfirmation { get; set; }

        /// <summary>
        /// Gets or sets the name of the rebate confirmation file being parsed.
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Gets or sets the rebate confirmation header parser to use when parsing header records.
        /// </summary>
        private RebateConfirmationHeaderParser RebateConfirmationHeaderParser { get; set; }

        /// <summary>
        /// Gets or sets the rebate confirmation data parser to use when parsing data records.
        /// </summary>
        private RebateConfirmationDataParser RebateConfirmationDataParser { get; set; }

        /// <summary>
        /// Gets or sets the rebate confirmation trailer parser to use when parsing trailer records.
        /// </summary>
        private RebateConfirmationTrailerParser RebateConfirmationTrailerParser { get; set; }

        /// <summary>
        /// Gets or sets the line currently being parsed.
        /// </summary>
        private string Line { get; set; }

        /// <summary>
        /// Gets or sets the line number currently being parsed.
        /// </summary>
        private int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of header records found in the rebate confirmation file.
        /// </summary>
        private int NumberOfHeaderRecords { get; set; }

        /// <summary>
        /// Gets or sets the number of data records found in the rebate confirmation file.
        /// </summary>
        private int NumberOfDataRecords { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a trailer record has been found.
        /// </summary>
        private bool TrailerFound { get; set; }

        /// <summary>
        /// Gets or sets the number of trailer records found in the rebate confirmation file.
        /// </summary>
        private int NumberOfTrailerRecords { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}