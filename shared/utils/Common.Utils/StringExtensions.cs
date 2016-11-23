//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Common.Utils
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Contains Extension functions for String type
    /// </summary>
    public static class StringExtensions
    {
        #region Static Fields
        /// <summary>
        /// Regular Expression for all white spaces
        /// </summary>
        private static readonly Regex WhiteSpaces = new Regex("\\s+", RegexOptions.Compiled);

        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// Returns a new string in which all occurrences of a multiple white spaces in the current instance are replaced with another specified string
        /// </summary>
        /// <param name="inputString">The string to be replaced</param>
        /// <param name="replaceWith">The string to replace all occurrences of white spaces</param>
        /// <returns>A string that is equivalent to the current string except that all instances of white spaces are replaced with replaceWith</returns>
        public static string ReplaceMultipleWhiteSpaces(this string inputString, string replaceWith)
        {
            return WhiteSpaces.Replace(inputString, replaceWith);
        }

        /// <summary>
        /// Compares both strings, character by character, in a secure manner, ensuring time information leak does not occur.
        /// </summary>
        /// <param name="baseString">The base string, Base64 encoded.</param>
        /// <param name="otherString">The string to be compared, also Base64 encoded.</param>
        /// <returns>True if the strings are equal length and each byte (in the string) is equal; false otherwise.</returns>
        public static bool SecureCompare(this string baseString, string otherString)
        {
            // If both strings are null, that is considered equal.
            if (baseString == null && otherString == null)
            {
                return true;
            }

            // If only one of the strings is null, they are not equal and should return false.
            if (baseString == null || otherString == null)
            {
                return false;
            }

            bool isEqual = true;
            int arrayLength = 0;

            // Ensuring the strings are of the same length, otherwise the tokens are
            // not the same. However to ensure that the constant-time algorithm is 
            // still executed, the smaller string length is assigned, 
            // to avoid an IndexOutOfBoundsException.
            if (baseString.Length == otherString.Length)
            {
                arrayLength = baseString.Length;
            }
            else
            {
                arrayLength = (baseString.Length > otherString.Length)
                    ? otherString.Length : baseString.Length;
                isEqual = false;
            }

            // Comparing each character (instead of a string comparison)
            // to have a constant-time algorithm to avoid timing 
            // channel leaks, i.e. the time would vary in a string
            // comparison based on how early in the string the comparison
            // would fail. This time fluctuation in a response being sent
            // could potentially be used to determine the token.
            for (int i = 0; i < arrayLength; ++i)
            {
                if (baseString[i] != otherString[i])
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }

        /// <summary>
        /// Formats the string using InvariantCulture and replaces the format items in the
        /// string with the args.
        /// </summary>
        /// <param name="value">The string to be formatted.</param>
        /// <param name="args">The replacement values for the format items in the string.</param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public static string FormatInvariant(this string value, params object[] args)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return string.Format(CultureInfo.InvariantCulture, value, args);
        }

        #endregion
    }
}