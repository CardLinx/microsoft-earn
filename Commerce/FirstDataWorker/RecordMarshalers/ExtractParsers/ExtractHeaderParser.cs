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
    /// Parses a First Data Extract header record.
    /// </summary>
    public class ExtractHeaderParser
    {
        /// <summary>
        /// Initializes a new instance of the ExtractHeaderParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ExtractHeaderParser(CommerceLog log)
        {
            Log = log;
        }
        
        /// <summary>
        /// Parses the specified record text into an extract header if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into an extract header.
        /// </param>
        /// <returns>
        /// * The ExtractHeader object if successful.
        /// * Else returns null.
        /// </returns>
        internal ExtractHeader Parse(string record)
        {
            ExtractHeader result = new ExtractHeader();

            int recordPos = 0;
            bool recordValid = true;
            string stringField = null;
            DateTime dateField = DateTime.MinValue;

            ParsingUtilities parsingUtilities = new ParsingUtilities("Extract File Header", 1, FileName, ExtractConstants.TimeFieldLength, Log);

            // RecordType
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Record Type", RecordType,
                                                        ExtractConstants.RecordTypeLength, recordValid);

            // ProviderId
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.ProviderIdLength, recordValid);
            result.ProviderId = stringField;

            // ProviderLevelNumber
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Hierarchy Level No.",
                                                        ExtractConstants.ProviderLevelNumber,
                                                        ExtractConstants.ProviderLevelNumberLength, recordValid);

            // CreationDate
            recordValid = parsingUtilities.PopulateDateTime(record, ref recordPos, "File Creation Date", out dateField, false, true,
                                                            recordValid);
            result.CreationDate = dateField;

            // ProviderName
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField,
                                                          ExtractConstants.ProviderNameLength, recordValid);
            result.ProviderName = stringField;

            // Record end
            parsingUtilities.VerifyRecordEnd(record, ref recordPos, FillerLength, true, recordValid);

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
        /// The record type for extract headers.
        /// </summary>
        internal const string RecordType = "001";

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// The number of spaces that must appear at the end of an extract header.
        /// </summary>
        private const int FillerLength = 899;
    }
}