//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public class ProtobufHelper
    {
        public static T Deserialize<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentNullException("data");
            }

            using (MemoryStream ms = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static byte[] Serialize<T>(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, data);
                ms.Position = 0;
                return ms.ToArray();
            }
        }
    }
}