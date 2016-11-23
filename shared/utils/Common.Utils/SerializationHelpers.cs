//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The serialization helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Common.Utils
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Lomo.Logging;

    /// <summary>
    ///     The serialization helper.
    /// </summary>
    public static class SerializationHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="xmlString">The xml string.</param>
        /// <typeparam name="T">The object type</typeparam>
        /// <returns>The T.</returns>
        public static T Deserialize<T>(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                return default(T);
            }

            var serializer = new XmlSerializer(typeof(T));

            // Per SDL requirements: Harden or Disable XML Entity Resolution 
            var settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Prohibit;

            using (var input = new StringReader(xmlString))
            using (var reader = XmlReader.Create(input, settings))
            {
                try
                {
                    return (T)serializer.Deserialize(reader);
                }
                catch (InvalidOperationException ex)
                {
                    Log.Error("Deserialize Error. Input [{0}] With Exception [{1}]", xmlString, ex.ToString());
                    return default(T);
                }
            }
        }

        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="addNamespaces">Parameter indicatiing whether to add name spaces.</param>
        /// <returns>The System.String.</returns>
        public static string Serialize<T>(T obj, bool addNamespaces = true)
        {
            if (obj == null)
            {
                return null;
            }

            var serializer = new XmlSerializer(obj.GetType());
            using (var writer = new StringWriter())
            {
                if (addNamespaces)
                {
                    serializer.Serialize(writer, obj);
                }
                else
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    serializer.Serialize(writer, obj, ns);
                }
                
                return writer.ToString();
            }
        }

        #endregion
    }
}