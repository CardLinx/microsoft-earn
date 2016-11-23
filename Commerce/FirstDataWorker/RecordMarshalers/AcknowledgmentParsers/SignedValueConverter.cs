//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System.Collections.Generic;

    /// <summary>
    /// Signed Data is in EBCDIC.
    /// </summary>
    class SignedValueConverter
    {
        /// <summary>
        /// Converts signed EBCDIC data to decimal format number.
        /// Only +ve numbers are supported.
        /// Signed is in the Least Significant Digit (LSD) 
        /// </summary>
        /// <param name="signedData">Data to be converted, signed EBCDIC</param>
        /// <param name="value">converted value in decimal format</param>
        /// <returns>boolean status to signify if conversion was successful or not</returns>
        public bool TryGetNumberFromSignedData(string signedData, out long value)
        {
            // -ve value indicates error as all should be +ve
            value = -1;
            string strValue;
            if ( TryGetStringFromSignedData(signedData, out strValue))
            {
                return long.TryParse(strValue, out value);
            }

            return false;
        }

        /// <summary>
        /// Converts signed EBCDIC data to string, i.e don't interpret the number, leave it alone.
        /// Only +ve numbers are supported.
        /// Signed is in the Least Significant Digit (LSD) 
        /// </summary>
        /// <param name="signedData">Data to be converted, signed EBCDIC</param>
        /// <param name="value">converted value in string format</param>
        /// <returns>boolean status to signify if conversion was successful or not</returns>
        public bool TryGetStringFromSignedData(string signedData, out string value)
        {
            if (string.IsNullOrEmpty(signedData))
            {
                // Empty will denote invalid value
                value = string.Empty;
                return false;
            }

            char[] chars = signedData.ToCharArray();

            // Least Significant Digit is to be converted
            int lsd = chars.Length - 1;

            char convertedLsd;
            if (signedFieldDictionary.TryGetValue(chars[lsd], out convertedLsd))
            {
                chars[lsd] = convertedLsd;
            }

            value = new string(chars);
            return true;
        }

        // conversion map for +ve signed numbers
        readonly Dictionary<char, char> signedFieldDictionary = new Dictionary<char, char>()
            {
               {'{','0'},
               {'A','1'},
               {'B','2'},
               {'C','3'},
               {'D','4'},
               {'E','5'},
               {'F','6'},
               {'G','7'},
               {'H','8'},
               {'I','9'},
            };
    }
}