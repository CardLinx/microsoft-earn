//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.IO;
using System.Xml;

namespace Lomo.Core.Serialization
{
    /// <summary>
    /// Utility class for serializing and deserializing objects to and from xml.
    /// </summary>
    public static class XmlSerializer
    {
        public static string Serialize<T>(T data)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(data.GetType());
            
            using( var stringWriter = new StringWriter())
            using( var xmlWriter = XmlWriter.Create(stringWriter))
            {
                serializer.Serialize(xmlWriter, data);
                return stringWriter.ToString();
            }
        }

        public static T Deserialize<T>(string data) where T : class
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (var reader = new StringReader(data))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}