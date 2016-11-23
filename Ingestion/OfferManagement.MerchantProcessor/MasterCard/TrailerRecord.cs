//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Logging;
using OfferManagement.DataModel;

namespace OfferManagement.MerchantFileParser.MasterCard
{
    public static class TrailerRecord
    {
        internal const string RecordType = "30";        

        public static string Create(int merchantsCount, char delimiter)
        {
            return string.Join(delimiter.ToString(), 
                new string[] { RecordType, MerchantConstants.MasterCardProjectID, merchantsCount.ToString() });
        }

        public static int Parse(string record, char delimiter)
        {
            int recordCount = -1;
            if (string.IsNullOrWhiteSpace(record))
            {
                Log.Error("MasterCard Trailer record is empty");
                return recordCount;
            }

            string[] recordParts = record.Split(new char[] { delimiter });
            if (recordParts.Length != 3)
            {
                Log.Error("Invalid trailer record. Number of fields did not equate to 3");
                return recordCount;
            }
            if (!int.TryParse(recordParts[2], out recordCount))
            {
                Log.Error("Invalid data for record count in the trailer record");
            }

            return recordCount;
        }
    }
}