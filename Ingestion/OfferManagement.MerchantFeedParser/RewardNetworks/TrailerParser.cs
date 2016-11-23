//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.MerchantFeedParser.RewardNetworks
{
    using System;

    public static class TrailerParser
    {
        internal const string RecordType = "T";

        private const string FileDescription = "RNMerchants    ";

        private const int FileSequenceNumberLength = 5;

        private const int FillerLength = 264;

        internal static Tuple<bool, DateTime, string> Parse(string record)
        {
            DateTime fileCreationDate = DateTime.MinValue;
            int recordPos = 0;
            string sValue;
            ParserUtility parser = new ParserUtility();
            bool recordValid = parser.VerifyString(record, ref recordPos, RecordType, RecordLengthConstants.RecordTypeLength);
            recordValid = parser.VerifyString(record, ref recordPos, FileDescription, RecordLengthConstants.FileDescriptionLength);
            recordValid = parser.PopulateDateTime(record, ref recordPos, out fileCreationDate);
            recordValid = parser.VerifyRecordLength(record, ref recordPos, FileSequenceNumberLength);
            recordValid = parser.PopulateString(record, ref recordPos, out sValue, RecordLengthConstants.RecordCountFieldLength, recordValid);
            recordValid = parser.VerifyRecordLength(record, ref recordPos, FillerLength);

            return new Tuple<bool, DateTime, string>(recordValid, fileCreationDate, sValue);
        }
    }
}