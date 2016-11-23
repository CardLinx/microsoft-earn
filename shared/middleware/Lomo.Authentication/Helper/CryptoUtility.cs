//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication.Helper
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides utility functions to perform crypto operations.
    /// </summary>
    public static class CryptoUtility
    {
        /// <summary>
        /// Provides symmetric key based encryption and decryption operations using Rijndael symmetric encryption algorithm.
        /// </summary>
        public static class RijndaelEncryptionProvider
        {
            /// <summary>
            ///  Encrypts plain text data .
            /// </summary>
            /// <param name="dataToEncrypt">Plain text data to encrypt.</param>
            /// <param name="encryptionPassword">The encryption password to derive encryption keys from.</param>
            /// <param name="salt">The salt to use to derive encryption key and IV.</param>
            /// <returns>Returns the encrypted data.</returns>
            public static byte[] EncryptData(string dataToEncrypt, string encryptionPassword, string salt)
            {
                if (string.IsNullOrEmpty(dataToEncrypt))
                {
                    throw new ArgumentNullException("dataToEncrypt");
                }

                if (string.IsNullOrEmpty(salt))
                {
                    throw new ArgumentNullException("salt");
                }

                if (string.IsNullOrEmpty(encryptionPassword))
                {
                    throw new ArgumentNullException("encryptionPassword");
                }

                byte[] encryptedData = null;
                byte[] saltedBytes = Encoding.ASCII.GetBytes(salt);
                Rfc2898DeriveBytes encryptionKey = new Rfc2898DeriveBytes(encryptionPassword, saltedBytes);

                using (AesCryptoServiceProvider rijndael = new AesCryptoServiceProvider())
                {
                    rijndael.Key = encryptionKey.GetBytes(rijndael.KeySize / 8);
                    rijndael.IV = encryptionKey.GetBytes(rijndael.BlockSize / 8);

                    ICryptoTransform encryptor = rijndael.CreateEncryptor();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cryptoStream))
                            {
                                sw.Write(dataToEncrypt);
                            }

                            encryptedData = memoryStream.ToArray();
                        }
                    }
                }

                return encryptedData;
            }

            /// <summary>
            /// Decrypts the encrypted data to plain text.
            /// </summary>
            /// <param name="encryptedData">Encrypted data that needs to be decrypted.</param>
            /// <param name="decryptionPassword">The decryption password to derive the decryption key from.</param>
            /// <param name="salt">The salt to use to derive decryption key and IV.</param>
            /// <returns>Returns the decrypted text.</returns>
            public static string DecryptData(byte[] encryptedData, string decryptionPassword, string salt)
            {
                if (encryptedData == null || encryptedData.Length <= 0)
                {
                    throw new ArgumentNullException("encryptedData");
                }

                if (string.IsNullOrEmpty(decryptionPassword))
                {
                    throw new ArgumentNullException("decryptionPassword");
                }

                if (string.IsNullOrEmpty(salt))
                {
                    throw new ArgumentNullException("salt");
                }

                string plaintext = null;
                byte[] saltedBytes = Encoding.ASCII.GetBytes(salt);
                Rfc2898DeriveBytes encryptionKey = new Rfc2898DeriveBytes(decryptionPassword, saltedBytes);

                using (AesCryptoServiceProvider rijndael = new AesCryptoServiceProvider())
                {
                    rijndael.Key = encryptionKey.GetBytes(rijndael.KeySize / 8);
                    rijndael.IV = encryptionKey.GetBytes(rijndael.BlockSize / 8);

                    ICryptoTransform decryptor = rijndael.CreateDecryptor();
                    using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cryptoStream))
                            {
                                plaintext = sr.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
        }

        /// <summary>
        /// Provides symmetric key based signature operations using HMACSHA256 signature algorithm.
        /// </summary>
        public static class HMACSHA256SignatureProvider
        {
            /// <summary>
            /// Signs the data using the provided key.
            /// </summary>
            /// <param name="data">The data to be signed.</param>
            /// <param name="signingKey">The signing key to be used.</param>
            /// <returns>Returns the signature.</returns>
            public static byte[] GetSignature(byte[] data, string signingKey)
            {
                if (data == null || data.Length <= 0)
                {
                    throw new ArgumentNullException("data");
                }

                if (string.IsNullOrEmpty(signingKey))
                {
                    throw new ArgumentNullException("signingKey");
                }

                byte[] signingKeyBytes = Encoding.ASCII.GetBytes(signingKey);

                using (HMACSHA256 hmac = new HMACSHA256(signingKeyBytes))
                {
                    return hmac.ComputeHash(data);
                }
            }

            /// <summary>
            ///  Verifies the signature over a given data.
            /// </summary>
            /// <param name="data">The data that is signed.</param>
            /// <param name="signature">The signature over the data that needs verification.</param>
            /// <param name="signingKey">The signingKey used to verify the signature.</param>
            /// <returns>Returns true if the signature over the data is valid else returns false.</returns>
            public static bool VerifySignature(byte[] data, byte[] signature, string signingKey)
            {
                if (string.IsNullOrEmpty(signingKey))
                {
                    throw new ArgumentNullException("signingKey");
                }

                try
                {
                    byte[] newSignature = GetSignature(data, signingKey);
                    return (newSignature != null) && (newSignature.Length > 0) && newSignature.SequenceEqual(signature);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}