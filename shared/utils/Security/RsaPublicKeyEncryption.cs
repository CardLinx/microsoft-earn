//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Helper class for doing cryptographic operations using RSA.
    /// </summary>
    public class RSAPublicKeyEncryption : IDisposable
    {
        const int KeySize = 2048;

        /// <summary>
        /// Specifies this instance has only the public key (and hence can do only encryption).
        /// </summary>
        readonly bool publicKeyOnly;

        /// <summary>
        /// The RSA crypto service provider instance.
        /// </summary>
        readonly RSACryptoServiceProvider cryptoServiceProvider;

        /// <summary>
        /// Factory method for creating an instance which can do only encryption.
        /// This requires only the public key.
        /// </summary>
        /// <param name="publicKeyXml">The public key for encryption.</param>
        /// <returns>
        /// An instance of RSAPublicKeyEncryption.
        /// </returns>
        public static RSAPublicKeyEncryption CreateForEncryptionOnly(string publicKeyXml)
        {
            return new RSAPublicKeyEncryption(publicKeyXml, true);
        }

        /// <summary>
        /// Factory method for creating an instance which can do both encryption and decryption.
        /// The input needs to contain both the private and public keys.
        /// </summary>
        /// <param name="publicAndPrivateKeyXml">The public and private keys.</param>
        /// <returns></returns>
        public static RSAPublicKeyEncryption CreateForEncryptionAndDecryption(string publicAndPrivateKeyXml)
        {
            return new RSAPublicKeyEncryption(publicAndPrivateKeyXml, false);
        }

        /// <summary>
        /// Initializes a new instance of RSAPublicKeyEncryption.
        /// </summary>
        /// <param name="keyXml">The key in xml format.</param>
        /// <param name="publicKeyOnly">
        /// True if the keyXml parameter contains only the public key, false if it contains both private and public key.
        /// </param>
        private RSAPublicKeyEncryption(string keyXml, bool publicKeyOnly)
        {
            if (string.IsNullOrWhiteSpace(keyXml))
            {
                throw new ArgumentException("The key xml is invalid.");
            }

            this.publicKeyOnly = publicKeyOnly;
            this.cryptoServiceProvider = new RSACryptoServiceProvider(KeySize);
            this.cryptoServiceProvider.FromXmlString(keyXml);
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="input">The input data that needs to be encrypted.</param>
        /// <returns>
        /// The encrypted data.
        /// </returns>
        public byte[] Encrypt(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "The input data is null.");
            }

            return this.cryptoServiceProvider.Encrypt(input, true);
        }

        /// <summary>
        /// Encrypts the input string.
        /// </summary>
        /// <param name="input">An UTF-8 string.</param>
        /// <returns>
        /// The encrypted data in base64 format.
        /// </returns>
        public string Encrypt(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] encryptedBytes = this.Encrypt(inputBytes);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="encryptedBytes"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] encryptedBytes)
        {
            if (this.publicKeyOnly)
            {
                throw new InvalidOperationException(
                    "This instance of the RSAPublicKeyEncryption was created using only the public key. It cannot decrypt data.");
            }

            if (encryptedBytes == null)
            {
                throw new ArgumentNullException(nameof(encryptedBytes), "The input data is null.");
            }

            return this.cryptoServiceProvider.Decrypt(encryptedBytes, true);
        }

        /// <summary>
        /// Decrypts data that has been formatted as a Base64 encoded string.
        /// </summary>
        /// <param name="base64EncryptedString">Encrypted data in base64 format.</param>
        /// <returns>
        /// Decrypted data as a UTF8 string.
        /// </returns>
        public string Decrypt(string base64EncryptedString)
        {
            byte[] encryptedBytes = Convert.FromBase64String(base64EncryptedString);
            byte[] decryptedBytes = this.Decrypt(encryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Releases the resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            this.cryptoServiceProvider?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string CreateKeyPair()
        {
            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                rsa.PersistKeyInCsp = false;
                return rsa.ToXmlString(true);
            }
        }
    }
}