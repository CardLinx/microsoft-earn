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
    /// Parses a First Data Acknowledgment file.
    /// </summary>
    public class AcknowledgmentParser
    {
        /// <summary>
        /// Initializes a new instance of the AcknowledgmentParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public AcknowledgmentParser(CommerceLog log)
        {
            Log = log;
            DetailAcknowledgmentParser = new DetailAcknowledgmentParser(log);
            GeneralAcknowledgmentParser = new GeneralAcknowledgmentParser(log);
        }

        /// <summary>
        /// Parses the specified ack file into an acknowledgment object if possible.
        /// </summary>
        /// <param name="ackFileName">
        /// The ack File Name to parse into an acknowledgment object.
        /// </param>
        /// <param name="stream">
        /// File contents as stream of data
        /// </param>
        /// <returns>
        /// * The Acknowledgment object resulting from the parsing attempt if the specified acknowledgment file could be found.
        /// * Otherwise returns null.
        /// </returns>
        internal Acknowledgment Parse(string ackFileName, Stream stream)
        {
            Acknowledgment  = new Acknowledgment();
            if (stream != null)
            {
                FileName = Path.GetFileName(ackFileName);
                DetailAcknowledgmentParser.FileName = FileName;

                string recordType;
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    while ((Line = streamReader.ReadLine()) != null)
                    {
                        if (Line.Length > AcknowledgmentConstants.RecordTypeLength)
                        {
                            LineNumber++;
                            recordType = Line.Substring(0, AcknowledgmentConstants.RecordTypeLength);

                            switch (recordType)
                            {
                                case DetailAcknowledgmentParser.RecordType:
                                    ParseDetailAcknowledgmentRecord();
                                    break;

                                case GeneralAcknowledgmentParser.RecordType:
                                    ParseGeneralAcknowledgmentRecord();
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

                // Add entries to the log if the acknowledgment file did not have the expected number of records of each type.
                if (Acknowledgment.DetailAcknowledgments.Count == 0)
                {
                    Log.Information("Acknowledgment file \"{0}\" contained no Detail Acknowledgments Records.", ackFileName);
                }

                if (Acknowledgment.GeneralAcknowledgments.Count != 1)
                {
                    Log.Warning("Acknowledgment file \"{0}\" should have exactly 1 General Acknowledgments Record.",
                                (int)ResultCode.FileMissingExpectedRecord, 
                                ackFileName);
                }
            }
            else
            {
                Log.Error("Acknowledgment file \"{0}\" could not be found.", null, (int)ResultCode.FileNotFound, ackFileName);
                Acknowledgment = null;
            }

            return Acknowledgment;
        }

        /// <summary>
        /// Parses a Detail Acknowledgment Record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseDetailAcknowledgmentRecord()
        {
            Acknowledgment.DetailAcknowledgments.Add(DetailAcknowledgmentParser.Parse(Line, NumberOfDetailAcknowledgmentRecords++));
        }

        /// <summary>
        /// Parses a General Acknowledgment Record.
        /// </summary>
        /// <remarks>
        /// A null item may be added.
        /// </remarks>
        private void ParseGeneralAcknowledgmentRecord()
        {
            Acknowledgment.GeneralAcknowledgments.Add(GeneralAcknowledgmentParser.Parse(Line, NumberOfGeneralAcknowledgmentRecords++));
        }

        /// <summary>
        /// Gets or sets the Acknowledgment built by this parser instance.
        /// </summary>
        private Acknowledgment Acknowledgment { get; set; }

        /// <summary>
        /// Gets or sets the Detail Acknowledgment Parser to use when parsing Detail Acknowledgment Records.
        /// </summary>
        private DetailAcknowledgmentParser DetailAcknowledgmentParser { get; set; }

        /// <summary>
        /// Gets or sets the General Acknowledgment Parser to use when parsing General Acknowledgment Records.
        /// </summary>
        private GeneralAcknowledgmentParser GeneralAcknowledgmentParser { get; set; }

        /// <summary>
        /// Gets or sets the number of Detail Acknowledgment Records found in the extract file.
        /// </summary>
        private int NumberOfDetailAcknowledgmentRecords { get; set; }

        /// <summary>
        /// Gets or sets the number of General Acknowledgment Records found in the extract file.
        /// </summary>
        private int NumberOfGeneralAcknowledgmentRecords { get; set; }


        /// <summary>
        /// Gets or sets the line number currently being parsed.
        /// </summary>
        private int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the line currently being parsed.
        /// </summary>
        private string Line { get; set; }

        /// <summary>
        /// Gets or sets the name of the Acknowledgment file being parsed.
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}