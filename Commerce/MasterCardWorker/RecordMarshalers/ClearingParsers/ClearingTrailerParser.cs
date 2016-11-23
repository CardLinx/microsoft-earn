//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Parses a MasterCard Clearing trailer record.
    /// </summary>
    public class ClearingTrailerParser
    {
        /// <summary>
        /// Initializes a new instance of the ClearingTrailerParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ClearingTrailerParser(CommerceLog log)
        {
            Log = log;
        }
        
        /// <summary>
        /// Parses the specified record text into a clearing trailer if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into a clearing trailer.
        /// </param>
        /// <returns>
        /// * The ClearingTrailer object if successful.
        /// * Else returns null.
        /// </returns>
        internal ClearingTrailer Parse(string record)
        {
            ClearingTrailer result = new ClearingTrailer();

            int recordPos = 0;
            bool recordValid = true;
            string stringField = null;
            long longField = Int64.MinValue;

            ParsingUtilities parsingUtilities = new ParsingUtilities("Clearing File Trailer", 1, FileName, ClearingConstants.TimeFieldLength, Log);

            // RecordType
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Record Type", RecordType,
                                                        ClearingConstants.RecordTypeLength, recordValid);

            // RecordCount
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "Record Count", out longField, ClearingConstants.RecordCountFieldLength, recordValid);
            result.RecordCount = longField;

            // MemberIca
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, ClearingConstants.MemberIcaLength, recordValid);
            result.MemberIca = stringField;

            // Record end
            parsingUtilities.VerifyRecordEnd(record, ref recordPos, FillerLength, false, recordValid);

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
        /// The record type for clearing trailers.
        /// </summary>
        internal const string RecordType = "T";

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// The number of spaces that must appear at the end of an clearing trailer.
        /// </summary>
        private const int FillerLength = 176;
    }
}