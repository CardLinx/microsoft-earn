//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.VisaClient
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.XPath;

    /// <summary>
    /// Contains authorization/settlement data that can be used as keys to trace transactions over thier lifetimes.
    /// </summary>
    public class KeyTransactionData
    {
        /// <summary>
        /// Serializes this object into an XmlDocument.
        /// </summary>
        /// <returns>
        /// An XmlDocument containing the serialization of this object.
        /// </returns>
        public string XmlSerialize()
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, this);
                memoryStream.Position = 0;
                xmlDocument.Load(memoryStream);
            }

            return xmlDocument.OuterXml;
        }


        /// <summary>
        /// Deserializes the specified XML into a KeyAuthorizationData object.
        /// </summary>
        /// <param name="xml">
        /// The XML to deserialize into an object.
        /// </param>
        /// <returns>
        /// The object deserialized from the XML.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter xml cannot be null.
        /// </exception>
        public static KeyTransactionData Deserialize(IXPathNavigable xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml", "Parameter xml cannot be null.");
            }

            KeyTransactionData result;

            using (XmlReader xmlReader = xml.CreateNavigator().ReadSubtree())
            {
                result = (KeyTransactionData)Serializer.Deserialize(xmlReader);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets merchant city where Visa transaction occurrs.
        /// </summary>
        public string MerchantCity { get; set; }

        /// <summary>
        /// Gets or sets merchant state where Visa transaction occurrs.
        /// </summary>
        public string MerchantState { get; set; }


        /// <summary>
        /// Gets or sets merchant postal code where Visa transaction occurrs.
        /// </summary>
        public string MerchantPostalCode { get; set; }

        /// <summary>
        ///    Gets or sets the XmlSerializer with which objects of this class can be serialized.
        /// </summary>
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(KeyTransactionData));
    }
}