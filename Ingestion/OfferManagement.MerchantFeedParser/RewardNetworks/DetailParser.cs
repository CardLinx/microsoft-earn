//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Logging;
using OfferManagement.DataModel;
using System;

namespace OfferManagement.MerchantFeedParser.RewardNetworks
{
    public static class DetailParser
    {
        internal const string RecordType = "D";

        internal const int McLocationIdLength = 9;

        internal const int McAcquirerMidLength = 15;

        internal const int RnMerchantAcquirerIDLength = 20;

        internal const int VisaMidLength = 10;

        internal const int VisaSidLength = 10;

        private const int FillerLength = 26;

        internal static Merchant Parse(string record, int lineNumber)
        {
            Merchant merchant = null;
            int recordPos = 0;
            ParserUtility parser = new ParserUtility();
            string sValue;

            bool recordValid = parser.VerifyString(record, ref recordPos, RecordType, RecordLengthConstants.RecordTypeLength);
            if (!recordValid)
            {
                Log.Error($"Invalid RecordType for merchant in {lineNumber}");
                return merchant;
            }

            recordValid = parser.PopulateString(record, ref recordPos, out sValue,
                RecordLengthConstants.MerchantIdLength);
            if (!recordValid)
            {
                Log.Error($"Invalid MerchantID for merchant in {lineNumber}");
                return merchant;
            }
            string partnerMerchantId = sValue;

            recordValid = parser.PopulateString(record, ref recordPos, out sValue,
                RecordLengthConstants.MerchantNameLength);
            if (!recordValid)
            {
                Log.Error($"Invalid MerchantName for merchant in {lineNumber}");
                return merchant;
            }
            string merchantName = sValue;

            recordValid = parser.PopulateString(record, ref recordPos, out sValue,
                RecordLengthConstants.MerchantAddressLength, recordValid);
            if (!recordValid)
            {
                Log.Error($"Invalid Address for merchant in {lineNumber}");
                return merchant;
            }
            string address = sValue;

            recordValid = parser.PopulateString(record, ref recordPos, out sValue,
                RecordLengthConstants.MerchantCityLength, recordValid);
            if (!recordValid)
            {
                Log.Error($"Invalid City for merchant in {lineNumber}");
                return merchant;
            }
            string city = sValue;

            recordValid = parser.PopulateString(record, ref recordPos, out sValue,
                RecordLengthConstants.MerchantStateLength, recordValid);
            if (!recordValid)
            {
                Log.Error($"Invalid State for merchant in {lineNumber}");
                return merchant;
            }
            string state = sValue;

            recordValid = parser.PopulateString(record, ref recordPos, out sValue,
                RecordLengthConstants.MerchantZipLength, recordValid);
            if (!recordValid)
            {
                Log.Error($"Invalid Zip for merchant in {lineNumber}");
                return merchant;
            }
            string zip = sValue;

            recordValid = parser.PopulateString(record, ref recordPos, out sValue,
                RecordLengthConstants.MerchantUrlLength, recordValid);
            if (!recordValid)
            {
                Log.Error($"Invalid Url for merchant in {lineNumber}");
                return merchant;
            }
            string url = sValue;

            recordValid = parser.VerifyRecordLength(record, ref recordPos, VisaMidLength);
            if (!recordValid)
            {
                Log.Error($"Invalid VisaMidLength for merchant in {lineNumber}");
                return merchant;
            }

            recordValid = parser.VerifyRecordLength(record, ref recordPos, VisaSidLength);
            if (!recordValid)
            {
                Log.Error($"Invalid VisaSidLength for merchant in {lineNumber}");
                return merchant;
            }

            recordValid = parser.VerifyRecordLength(record, ref recordPos, McLocationIdLength);
            if (!recordValid)
            {
                Log.Error($"Invalid McLocationIdLength for merchant in {lineNumber}");
                return merchant;
            }

            recordValid = parser.VerifyRecordLength(record, ref recordPos, McAcquirerMidLength);
            if (!recordValid)
            {
                Log.Error($"Invalid McAcquirerMidLength for merchant in {lineNumber}");
                return merchant;
            }
            recordValid = parser.VerifyRecordLength(record, ref recordPos, RnMerchantAcquirerIDLength);
            if (!recordValid)
            {
                Log.Error($"Invalid RnMerchantAcquirerIDLength for merchant in {lineNumber}");
                return merchant;
            }

            recordValid = parser.VerifyRecordLength(record, ref recordPos, FillerLength);
            if (!recordValid)
            {
                Log.Error($"Invalid FillerLength for merchant in {lineNumber}");
                return merchant;
            }

            merchant = new Merchant
            {
                Id = Guid.NewGuid().ToString(),
                PartnerMerchantId = partnerMerchantId,
                Name = merchantName,
                Location = new Location
                {
                    Address = address,
                    City = city,
                    State = state,
                    Zip = zip
                },
                Url = url
            };            

            return merchant;
        }       
    }
}