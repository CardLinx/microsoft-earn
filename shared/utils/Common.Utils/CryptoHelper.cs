//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utils
{
    public class CryptoHelper
    {
        public enum SerializationType
        {
            JsonDotNet = 0,
            ProtoBuf = 1,
            JsonMicrosoft
        }

        public static class Encryptor
        {
            public static byte[] EncryptData(string data, byte[] key, byte[] initializationVector)
            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    throw new ArgumentNullException("data is null inside EncryptData");
                }

                if (key == null || key.Length == 0)
                {
                    throw new ArgumentNullException("key is null inside EncryptData");
                }

                if (initializationVector == null || initializationVector.Length == 0)
                {
                    throw new ArgumentNullException("initializationVector is null inside EncryptData");
                }


                byte[] encrypted;

                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.Key = key;
                    aesProvider.IV = initializationVector;
                    ICryptoTransform encryptor = aesProvider.CreateEncryptor(aesProvider.Key, aesProvider.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(data);
                            }

                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                return encrypted;
            }

            public static string DecryptData(byte[] data, byte[] key, byte[] initializationVector)
            {
                if (data == null || data.Length == 0)
                {
                    throw new ArgumentNullException("data is null inside EncryptData");
                }

                if (key == null || key.Length == 0)
                {
                    throw new ArgumentNullException("key is null inside EncryptData");
                }

                if (initializationVector == null || initializationVector.Length == 0)
                {
                    throw new ArgumentNullException("initializationVector is null inside EncryptData");
                }

                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.Key = key;
                    aesProvider.IV = initializationVector;
                    ICryptoTransform decryptor = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
                    using (MemoryStream msDecrypt = new MemoryStream(data))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }

                        }
                    }
                }
            }

            public static byte[] EncryptObject<T>(T data, byte[] key, byte[] initializationVector, SerializationType serializationType)
            {
                string dataString = string.Empty;

                switch (serializationType)
                {
                    case SerializationType.JsonDotNet:
                        dataString =JsonConvert.SerializeObject(data);
                        break;

                    case SerializationType.JsonMicrosoft:
                        dataString = JsonHelpers.SerializeJson<T>(data);
                        break;

                    case SerializationType.ProtoBuf:
                        dataString = Convert.ToBase64String(ProtobufHelper.Serialize<T>(data));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("serializationType is not understood.");

                }

                return EncryptData(dataString, key, initializationVector);
            }

            public static T DecryptObject<T>(byte[] data, byte[] key, byte[] initializationVector, SerializationType serializationType)
            {
                string decryptedText = DecryptData(data, key, initializationVector);
                switch (serializationType)
                {
                    case SerializationType.JsonDotNet:
                        return JsonConvert.DeserializeObject<T>(decryptedText);

                    case SerializationType.JsonMicrosoft:
                        return JsonConvert.DeserializeObject<T>(decryptedText);

                    case SerializationType.ProtoBuf:
                        return ProtobufHelper.Deserialize<T>(Convert.FromBase64String(decryptedText));

                    default:
                        throw new ArgumentOutOfRangeException("serializationType is not understood.");
                }
            }
        }
    }
}