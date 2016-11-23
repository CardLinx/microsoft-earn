//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Cryptography
{
	using System.IO;
	using System.Security.Cryptography;

	using Serialization;

	public static class Symmetric<T> where T : SymmetricAlgorithm, new()
	{
		private static readonly SymmetricAlgorithm _cryptoProvider = new T();

		public static byte[] Encrypt(byte[] key, byte[] initializationVector, byte[] plainText)
		{
			var encryptor = _cryptoProvider.CreateEncryptor(key, initializationVector);
			using (var stream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
				{
					using (var writer = new BinaryWriter(cryptoStream))
						writer.Write(plainText);
					return stream.ToArray();
				}
			}
		}

		public static byte[] Decrypt(byte[] key, byte[] initializationVector, byte[] cipher)
		{
			var decryptor = _cryptoProvider.CreateDecryptor(key, initializationVector);
			using (var stream = new MemoryStream(cipher))
				using (var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
					return cryptoStream.ReadAll();
		}
	}
}