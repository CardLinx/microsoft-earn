//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Helper class to encrypt and decrypt text using AES
    /// </summary>
    public class AesEncryption : IDisposable
    {
        private Aes algorithm;

        public string Base64EncodedKey => Convert.ToBase64String(this.Key);

        public string Base64EncodedIV => Convert.ToBase64String(this.IV);

        public byte[] Key => this.algorithm.Key;

        public byte[] IV => this.algorithm.IV;

        /// <summary>
        /// Default constructor, creates new key and IV.
        /// </summary>
        public AesEncryption()
        {
            this.algorithm = Aes.Create();
        }

        public AesEncryption(string base64EncodedKey)
        {
            if (string.IsNullOrWhiteSpace(base64EncodedKey))
            {
                throw new ArgumentException("The key is not valid.", nameof(base64EncodedKey));
            }

            this.Create(Convert.FromBase64String(base64EncodedKey), null);
        }

        public AesEncryption(string base64EncodedKey, string base64EncodedIv)
        {
            if (string.IsNullOrWhiteSpace(base64EncodedKey))
            {
                throw new ArgumentException("The key is not valid.", nameof(base64EncodedKey));
            }

            if (string.IsNullOrWhiteSpace(base64EncodedIv))
            {
                throw new ArgumentException("The initialization vector is not valid.", nameof(base64EncodedIv));
            }

            this.Create(Convert.FromBase64String(base64EncodedKey), Convert.FromBase64String(base64EncodedIv));
        }

        public AesEncryption(byte[] key, byte[] iv)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv == null)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            this.Create(key, iv);
        }

        public AesEncryption(byte[] key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            this.Create(key, null);
        }

        private void Create(byte[] key, byte[] iv)
        {
            this.algorithm = Aes.Create();

            if (key != null)
            {
                if (!this.algorithm.ValidKeySize(key.Length * 8))
                {
                    throw new ArgumentException("The key size is not valid", nameof(key));
                }

                this.algorithm.Key = (byte[])key.Clone();
            }

            if (iv != null)
            {
                this.algorithm.IV = (byte[])iv.Clone();
            }
        }

        public string Encrypt(string plainText)
        {
            using (var encryptor = this.algorithm.CreateEncryptor())
            {
                return Convert.ToBase64String(Crypt(Encoding.UTF8.GetBytes(plainText), encryptor));
            }
        }

        public string Encrypt(string plainText, string base64EncodedIv)
        {
            using (var encryptor = this.algorithm.CreateEncryptor(this.algorithm.Key, Convert.FromBase64String(base64EncodedIv)))
            {
                return Convert.ToBase64String(Crypt(Encoding.UTF8.GetBytes(plainText), encryptor));
            }
        }

        public string Encrypt(string plainText, string base64EncodedKey, string base64EncodedIv)
        {
            using (var encryptor = this.algorithm.CreateEncryptor(Convert.FromBase64String(base64EncodedKey), Convert.FromBase64String(base64EncodedIv)))
            {
                return Convert.ToBase64String(Crypt(Encoding.UTF8.GetBytes(plainText), encryptor));
            }
        }

        public string Decrypt(string encryptedText)
        {
            using (var decryptor = this.algorithm.CreateDecryptor())
            {
                return Encoding.UTF8.GetString(Crypt(Convert.FromBase64String(encryptedText), decryptor));
            }
        }

        public string Decrypt(string encryptedText, string base64EncodedIV)
        {
            using (var decryptor = this.algorithm.CreateDecryptor(this.algorithm.Key, Convert.FromBase64String(base64EncodedIV)))
            {
                return Encoding.UTF8.GetString(Crypt(Convert.FromBase64String(encryptedText), decryptor));
            }
        }

        public string Decrypt(string encryptedText, string base64EncodedKey, string base64EncodedIV)
        {
            using (var decryptor = this.algorithm.CreateDecryptor(Convert.FromBase64String(base64EncodedKey), Convert.FromBase64String(base64EncodedIV)))
            {
                return Encoding.UTF8.GetString(Crypt(Convert.FromBase64String(encryptedText), decryptor));
            }
        }

        static byte[] Crypt(byte[] data, ICryptoTransform cryptor)
        {
            using (var ms = new MemoryStream())
            {
                using (Stream c = new CryptoStream(ms, cryptor, CryptoStreamMode.Write))
                {
                    c.Write(data, 0, data.Length);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// This is a helper method to generate an IV for use during the encryption/decryption process.
        /// </summary>
        /// <returns></returns>
        public static string GenerateInitializationVector()
        {
            using (Aes algorithm = Aes.Create())
            {
                return Convert.ToBase64String(algorithm.IV);
            }
        }

        public static string GenerateKey(int keySizeInBits)
        {
            using (Aes algorithm = Aes.Create())
            {
                if (!algorithm.ValidKeySize(keySizeInBits))
                {
                    throw new ArgumentException("The key size is invalid.", nameof(keySizeInBits));
                }

                algorithm.KeySize = keySizeInBits;
                algorithm.GenerateKey();
                return Convert.ToBase64String(algorithm.Key);
            }
        }

        public void Dispose()
        {
            this.algorithm?.Dispose();
        }
    }
}