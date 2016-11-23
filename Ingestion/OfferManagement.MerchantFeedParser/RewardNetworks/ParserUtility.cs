//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.MerchantFeedParser.RewardNetworks
{
    using System;
    using System.Text;

    public class ParserUtility
    {
        public bool VerifyString(string record, ref int recordPosition, string expectedValue, int fieldLength)
        {
            bool recordValid = true;
            if (record == null)
            {
                throw new ArgumentNullException($"[{nameof(record)}]", $"[{nameof(VerifyString)}]Parameter record cannot be null.");
            }

            if (record.Length >= recordPosition + fieldLength)
            {
                string receivedValue = record.Substring(recordPosition, fieldLength);
                if (receivedValue == expectedValue)
                {
                    recordPosition += fieldLength;
                }
                else
                {
                    recordValid = false;
                }
            }
            else
            {
                recordValid = false;
            }

            return recordValid;
        }

        public bool PopulateString(string record, ref int recordPosition, out string value, int fieldLength, bool trimString = true)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"[{nameof(record)}]", $"[{nameof(PopulateString)}]Parameter record cannot be null.");
            }

            bool recordValid = true;
            value = null;
            if (record.Length >= recordPosition + fieldLength)
            {
                value = record.Substring(recordPosition, fieldLength);

                if (trimString)
                    value = value.Trim(' ');

                recordPosition += fieldLength;
            }
            else
            {
                recordValid = false;
            }

            return recordValid;
        }

        public bool PopulateDateTime(string record, ref int recordPosition, out DateTime field)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"[{nameof(record)}]", $"[{nameof(PopulateDateTime)}]Parameter record cannot be null.");
            }

            bool recordValid = true;
            field = DateTime.MinValue;
            const int fieldLength = DateFieldLength;

            if (record.Length >= recordPosition + fieldLength)
            {
                // Build a standard string representation of the date.
                string receivedValue = record.Substring(recordPosition, fieldLength);
                recordPosition += fieldLength;
                StringBuilder normalizedDateTime = new StringBuilder(receivedValue.Substring(0, 4));
                normalizedDateTime.Append("/");
                normalizedDateTime.Append(receivedValue.Substring(4, 2));
                normalizedDateTime.Append("/");
                normalizedDateTime.Append(receivedValue.Substring(6, 2));

                // Attempt to parse the date.
                if (!DateTime.TryParse(normalizedDateTime.ToString(), out field))
                {
                    recordValid = false;
                }
            }
            else
            {
                recordValid = false;
            }

            return recordValid;
        }


        public bool VerifyRecordLength(string record, ref int recordPosition, int expectedLength)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"[{nameof(record)}]", $"[{nameof(VerifyString)}]Parameter record cannot be null.");
            }
            bool recordValid = true;
            string sValue;
            recordValid = PopulateString(record, ref recordPosition, out sValue, expectedLength, false);
            if (recordValid)
            {
                recordValid = (sValue.Length == expectedLength);
            }

            return recordValid;
        }


        /// <summary>
        /// The length of a date field.
        /// </summary>
        private const int DateFieldLength = 8;
    }
}