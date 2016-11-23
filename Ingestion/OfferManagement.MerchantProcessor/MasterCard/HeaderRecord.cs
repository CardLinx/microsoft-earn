//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using Lomo.Logging;
using OfferManagement.DataModel;

namespace OfferManagement.MerchantFileParser.MasterCard
{
    public static class HeaderRecord
    {
        internal const string RecordType = "10";  

        public static string Create(Provider provider, char delimiter)
        {
            string headerData = null;
            if (!provider.ExtendedAttributes.ContainsKey(MerchantConstants.MCProvisioningDate))
            {
                throw new Exception($"Provider is missing {MerchantConstants.MCProvisioningDate} value. Cannot generate the header record for provider {provider.Id}");                
            }

            string originalDate = provider.ExtendedAttributes[MerchantConstants.MCProvisioningDate];
            if (string.IsNullOrWhiteSpace(originalDate))
            {
                throw new Exception($"Invalid value for mastercard original filedate. Cannot generate the header record for provider {provider.Id}");               
            }

            headerData = string.Join(delimiter.ToString(), new string[] { RecordType, originalDate, MerchantConstants.MasterCardProjectID });

            return headerData;
        }

        public static string Parse(string record, char delimiter)
        {
            string fileCreatedDate = null;
            if (string.IsNullOrWhiteSpace(record))
            {
                Log.Error("MasterCard Header record is empty");

                return fileCreatedDate;
            }

            string[] recordParts = record.Split(new char[] { delimiter });
            if (recordParts.Length != 3)
            {
                Log.Error("Invalid header record. Number of fields did not equate to 3");

                return fileCreatedDate;
            }
            try
            {
                //Check to make sure the date coming in the file matches the schema
                DateTime fileDate = DateTime.ParseExact(recordParts[1], "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                fileCreatedDate = fileDate.ToString("yyyyMMddHHmmss");
            }
            catch (Exception)
            {
                Log.Error("Invalid header record. Original Date field has an invalid value");
            }

            return fileCreatedDate;
        }
    }
}