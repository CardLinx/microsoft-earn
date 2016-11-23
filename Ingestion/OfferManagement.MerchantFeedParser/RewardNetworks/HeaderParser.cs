//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.MerchantFeedParser.RewardNetworks
{
    using System;

    public static class HeaderParser
    {
        internal const string RecordType = "H";

        private const string FileDescription = "RNMerchants    ";

        private const int FileSequenceNumberLength = 5;

        private const int FillerLength = 271;

        internal static Tuple<bool,DateTime> Parse(string record)
        {
            int recordPos = 0;
            ParserUtility parser = new ParserUtility();
            bool recordValid = parser.VerifyString(record, ref recordPos, RecordType, RecordLengthConstants.RecordTypeLength);
            recordValid = parser.VerifyString(record, ref recordPos, FileDescription, RecordLengthConstants.FileDescriptionLength);

            DateTime fileCreationDate = DateTime.MinValue;
            recordValid = parser.PopulateDateTime(record, ref recordPos, out fileCreationDate);
            recordValid = parser.VerifyRecordLength(record, ref recordPos, FileSequenceNumberLength);
            recordValid = parser.VerifyRecordLength(record, ref recordPos, FillerLength);

            return new Tuple<bool, DateTime>(recordValid,fileCreationDate);
        }
    }
}