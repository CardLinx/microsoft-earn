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
    /// Parses a MasterCard Clearing file.
    /// </summary>
    public class ClearingParser
    {
        /// <summary>
        /// Initializes a new instance of the ClearingParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ClearingParser(CommerceLog log)
        {
            Log = log;
            ClearingHeaderParser = new ClearingHeaderParser(log);
            ClearingDataParser = new ClearingDataParser(log);
            ClearingTrailerParser = new ClearingTrailerParser(log);
        }

        /// <summary>
        /// Parses the specified clearing file into a clearing object if possible.
        /// </summary>
        /// <param name="clearingFileName">
        /// The clearing file name to parse into an Clearing object.
        /// </param>
        /// <param name="stream">
        /// File contents as stream of data
        /// </param>
        /// <returns>
        /// * The Clearing object resulting from the parsing attempt if the specified clearing file could be found.
        /// * Otherwise returns null.
        /// </returns>
        internal Clearing Parse(string clearingFileName,
                                Stream stream)
        {
            Clearing = new Clearing();
            LineNumber = 0;
            NumberOfHeaderRecords = 0;
            NumberOfDataRecords = 0;
            NumberOfTrailerRecords = 0;

            if (stream != null)
            {
                FileName = Path.GetFileName(clearingFileName);
                ClearingHeaderParser.FileName = FileName;
                ClearingDataParser.FileName = FileName;
                ClearingTrailerParser.FileName = FileName;
                bool loggedTrailerOutOfPlace = false;

                // Read the clearing file one line at a time and parse each line as a record.
                string recordType;
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    while ((Line = streamReader.ReadLine()) != null)
                    {
                        if (Line.Length > ClearingConstants.RecordTypeLength)
                        {
                            LineNumber++;
                            recordType = Line.Substring(0, ClearingConstants.RecordTypeLength);

                            // Log a warning if the trailer record has already been parsed and a warning was not already logged.
                            if (TrailerFound == true && loggedTrailerOutOfPlace == false)
                            {
                                Log.Warning("Error parsing record in line #{0} from file \"{1}\". One or more records found  " +
                                            "after trailer record parsed.", (int)ResultCode.RecordOutOfPlace, LineNumber,
                                            FileName);
                                loggedTrailerOutOfPlace = true;
                            }

                            // Parse the record according to its type and add it to the Clearing object.
                            switch(recordType)
                            {
                                case ClearingHeaderParser.RecordType:
                                    ParseHeaderRecord();
                                    break;
                                case ClearingDataParser.RecordType:
                                    ParseDataRecord();
                                    break;
                                case ClearingTrailerParser.RecordType:
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

                // Add entries to the log if the clearing file did not have the expected number of records.
                if (Clearing.Header == null)
                {
                    Log.Warning("Header for clearing file \"{0}\" missing or corrupted.",
                                (int)ResultCode.FileMissingExpectedRecord, clearingFileName);
                }

                if (Clearing.DataRecords.Count == 0)
                {
                    Log.Information("Clearing file \"{0}\" contained no data records.", clearingFileName);
                }

                if (Clearing.Trailer == null)
                {
                    Log.Warning("Trailer for clearing file \"{0}\" missing or corrupted.",
                                (int)ResultCode.FileMissingExpectedRecord, clearingFileName);
                }
                else
                {
                    if (Clearing.Trailer.RecordCount != NumberOfDataRecords)
                    {
                        Log.Warning("Record count for clearing file \"{0}\" is {1} but trailer indicated it should be {2}.",
                                    (int)ResultCode.RecordCountMismatch, clearingFileName, NumberOfDataRecords, Clearing.Trailer.RecordCount);
                    }
                }
            }
            else
            {
                Log.Error("Clearing file \"{0}\" could not be found.", null, (int)ResultCode.FileNotFound, clearingFileName);
                Clearing = null;
            }

            return Clearing;
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

                Clearing.Header = ClearingHeaderParser.Parse(Line);
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
            ClearingData clearingData = ClearingDataParser.Parse(Line, NumberOfDataRecords++);
            if (clearingData == null)
            {
                Log.Error("Record in line #{0} from file \"{1}\" could not be parsed.", null,
                          (int)ResultCode.CorruptSettlementRecord, LineNumber, FileName);
            }

            Clearing.DataRecords.Add(clearingData);
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
                Clearing.Trailer = ClearingTrailerParser.Parse(Line);
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
        /// Gets or sets the Clearing built by this parser instance.
        /// </summary>
        private Clearing Clearing { get; set; }

        /// <summary>
        /// Gets or sets the name of the clearing file being parsed.
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Gets or sets the clearing header parser to use when parsing header records.
        /// </summary>
        private ClearingHeaderParser ClearingHeaderParser { get; set; }

        /// <summary>
        /// Gets or sets the clearing data parser to use when parsing data records.
        /// </summary>
        private ClearingDataParser ClearingDataParser { get; set; }

        /// <summary>
        /// Gets or sets the clearing trailer parser to use when parsing trailer records.
        /// </summary>
        private ClearingTrailerParser ClearingTrailerParser { get; set; }

        /// <summary>
        /// Gets or sets the line currently being parsed.
        /// </summary>
        private string Line { get; set; }

        /// <summary>
        /// Gets or sets the line number currently being parsed.
        /// </summary>
        private int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of header records found in the clearing file.
        /// </summary>
        private int NumberOfHeaderRecords { get; set; }

        /// <summary>
        /// Gets or sets the number of data records found in the clearing file.
        /// </summary>
        private int NumberOfDataRecords { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a trailer record has been found.
        /// </summary>
        private bool TrailerFound { get; set; }

        /// <summary>
        /// Gets or sets the number of trailer records found in the clearing file.
        /// </summary>
        private int NumberOfTrailerRecords { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}