//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The secure hash generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The secure hash generator.
    /// </summary>
    public class SecureHashGenerator
    {
        /// <summary>
        /// The generate hash.
        /// </summary>
        /// <param name="s"> The string s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>. secured hash string
        /// </returns>
        public static string Generate(string s)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sb = new StringBuilder();

            using (SHA256 sha256Hasher = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha256Hasher.ComputeHash(Encoding.Default.GetBytes(s));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }
            }

            // Return the hexadecimal string.
            return sb.ToString();
        }
    }
}