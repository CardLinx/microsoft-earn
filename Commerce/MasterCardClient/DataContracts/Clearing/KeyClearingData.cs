//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.XPath;

    /// <summary>
    /// Contains clearing data that can be used as keys to trace transactions over thier lifetimes.
    /// </summary>
    public class KeyClearingData
    {
        /// <summary>
        /// Initializes static members of the KeyClearingData class.
        /// </summary>
        static KeyClearingData()
        {
            Serializer = new XmlSerializer(typeof(KeyClearingData));
        }

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
        /// Deserializes the specified XML into a KeyClearingData object.
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
        public static KeyClearingData Deserialize(IXPathNavigable xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml", "Parameter xml cannot be null.");
            }

            KeyClearingData result;

            using (XmlReader xmlReader = xml.CreateNavigator().ReadSubtree())
            {
                result = (KeyClearingData)Serializer.Deserialize(xmlReader);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the bank customer number for the card used within the transaction.
        /// </summary>
        public string BankCustomerNumber { get; set; }

        /// <summary>
        /// Gets or set the bank net reference number for the merchant at which the transaction occurred.
        /// </summary>
        public string BankNetRefNumber { get; set; }

        /// <summary>
        /// Gets or sets the ID of the merchant at which the transaction occurred.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the issuer ICA for the card used within the transaction.
        /// </summary>
        public string IssuerIca { get; set; }

        /// <summary>
        ///    Gets or sets the XmlSerializer with which objects of this class can be serialized.
        /// </summary>
        private static XmlSerializer Serializer { get; set; }
    }
}