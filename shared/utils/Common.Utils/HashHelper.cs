//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class HashHelper
    {
        #region public members

        /// <summary>
        /// Validate the hash
        /// </summary>
        /// <param name="message">The url</param>
        /// <param name="hash">The hash</param>
        /// <param name="key"></param>
        /// <returns>true if hash matches</returns>
        public static bool ValidateHmacsha1Hash(string message, string hash, string key)
        {
            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                key = string.Empty;
            }

            return hash.Equals(GenerateHmacsha1Hash(message, key));
        }

        #endregion

        #region private members

        /// <summary>
        /// Compute the HMAC SHA1 hash
        /// </summary>
        /// <param name="message">The url</param>
        /// <param name="key">The hash key</param>
        /// <returns>The hash</returns>
        public static string GenerateHmacsha1Hash(string message, string key)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("message", "The specified string is not valid");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                key = string.Empty;
            }

            byte[] textBytes = Encoding.UTF8.GetBytes(message);
            //// using (HMACSHA1 hashAlgorithm = new HMACSHA1(HexToByteArray(key)))
            using (HMACSHA1 hashAlgorithm = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
            {
                byte[] hash = hashAlgorithm.ComputeHash(textBytes);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Convert Hex string to byte array
        /// </summary>
        /// <param name="hexString">The hex string</param>
        /// <returns>The byte array</returns>
        private static byte[] HexToByteArray(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
            {
                throw new ArgumentNullException("hexString", "The specified string is not valid");
            }

            if ((hexString.Length % 2) != 0)
            {
                throw new ApplicationException("Hex string must be multiple of 2 in length");
            }

            int byteCount = hexString.Length / 2;
            byte[] byteValues = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
            {
                byteValues[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return byteValues;
        }

        #endregion
    }
}