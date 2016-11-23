//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Common.Utils
{
    using System.IO;
    using System.Json;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public static class JsonHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The deserialize json.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the json string to.</typeparam>
        /// <param name="json">The json.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeJson<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return (T)serializer.ReadObject(memoryStream);
            }
        }

        /// <summary>
        ///     The get key value.
        /// </summary>
        /// <param name="properties">
        ///     The properties.
        /// </param>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The System.String.
        /// </returns>
        public static string GetKeyValue(JsonArray properties, string key)
        {
            foreach (JsonValue jv in properties)
            {
                dynamic jvd = jv.AsDynamic();
                if (jvd.Key.Value == key)
                {
                    return (string)jvd.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Serialize object to json
        /// </summary>
        /// <typeparam name="T">type of object to serialize</typeparam>
        /// <param name="objectToSerialize">object to serialize</param>
        /// <returns>json string</returns>
        public static string SerializeJson<T>(T objectToSerialize)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, objectToSerialize);
                string json = Encoding.Default.GetString(memoryStream.ToArray());
                return json;
            }
        }

        #endregion
    }
}