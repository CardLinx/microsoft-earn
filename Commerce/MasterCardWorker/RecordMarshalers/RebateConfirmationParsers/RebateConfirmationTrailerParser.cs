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
    /// Parses a MasterCard RebateConfirmation trailer record.
    /// </summary>
    public class RebateConfirmationTrailerParser
    {
        /// <summary>
        /// Initializes a new instance of the RebateConfirmationTrailerParser class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public RebateConfirmationTrailerParser(CommerceLog log)
        {
            Log = log;
        }
        
        /// <summary>
        /// Parses the specified record text into a rebate confirmation trailer if possible.
        /// </summary>
        /// <param name="record">
        /// The record text to parse into a rebate confirmation trailer.
        /// </param>
        /// <returns>
        /// * The RebateConfirmationTrailer object if successful.
        /// * Else returns null.
        /// </returns>
        internal RebateConfirmationTrailer Parse(string record)
        {
            RebateConfirmationTrailer result = new RebateConfirmationTrailer();

            int recordPos = 0;
            bool recordValid = true;
            string stringField = null;
            long longField = Int64.MinValue;

            ParsingUtilities parsingUtilities = new ParsingUtilities("Rebate Confirmation File Trailer", 1, FileName, RebateConfirmationConstants.TimeFieldLength, Log);

            // RecordType
            recordValid = parsingUtilities.VerifyString(record, ref recordPos, "Record Type", RecordType,
                                                        RebateConfirmationConstants.RecordTypeLength, recordValid);

            // ExceptionRecordCount
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "Exception Record Count", out longField,
                                                        RebateConfirmationConstants.ExceptionRecordCountFieldLength, recordValid);
            result.ExceptionRecordCount = longField;

            // SuccessRecordCount
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "Success Record Count", out longField,
                                                        RebateConfirmationConstants.SuccessRecordCountFieldLength, recordValid);
            result.SuccessRecordCount = longField;

            // TotalProcessedRecordCount
            recordValid = parsingUtilities.PopulateLong(record, ref recordPos, "TotalProcessed Record Count", out longField,
                                                        RebateConfirmationConstants.TotalProcessedRecordCountFieldLength, recordValid);
            result.TotalProcessedRecordCount = longField;

            // MemberIca
            recordValid = parsingUtilities.PopulateString(record, ref recordPos, out stringField, RebateConfirmationConstants.MemberIcaLength, recordValid);
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
        /// The record type for rebate confirmation trailers.
        /// </summary>
        internal const string RecordType = "T";

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// The number of spaces that must appear at the end of an rebate confirmation trailer.
        /// </summary>
        private const int FillerLength = 152;
    }
}