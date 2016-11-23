//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Cipher class has methods to encrypt/decrypt the user Anid
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProfileService.Utility
{
    /// <summary>
    /// Cipher class has methods to encrypt/decrypt the user Anid
    /// </summary>
    public static class Cipher
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string InitVector = "tu89geji340t89u2";

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int Keysize = 256;

        /// <summary>
        ///  Encrypts the user Anid using the passPhrase as the key
        /// </summary>
        /// <param name="plainText">
        ///  User Anid to encrypt
        /// </param>
        /// <param name="passPhrase">
        ///  Key to encrypt the Anid
        /// </param>
        /// <returns>
        ///  The encrypted Anid
        /// </returns>
        public static string Encrypt(string plainText, string passPhrase)
        {
            string encryptedString;

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    byte[] cipherTextBytes = memoryStream.ToArray();
                    encryptedString = Convert.ToBase64String(cipherTextBytes);
                }
            }

            return encryptedString;
        }

        /// <summary>
        ///  Decrypts the user Anid
        /// </summary>
        /// <param name="cipherText">
        ///  Anid to decrypt
        /// </param>
        /// <param name="passPhrase">
        ///  Key to decrypt the Anid
        /// </param>
        /// <returns>
        ///  Decrypted Anid
        /// </returns>
        public static string Decrypt(string cipherText, string passPhrase)
        {
            string decryptedString;

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            var password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            using (var memoryStream = new MemoryStream(cipherTextBytes))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                    decryptedString = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                }
            }

            return decryptedString;
        }
    }
}