//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Extension methods for XElement
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient
{
    using System;
    using System.Xml.Linq;

    public static class XmlExtensionMethods
    {
        #region Public Methods and Operators

        /// <summary>
        /// Extends XElement 
        /// </summary>
        /// <param name="element">
        /// the xml node
        /// </param>
        /// <param name="attributeName">
        /// attribute name
        /// </param>
        /// <param name="defaultValue">
        /// default value
        /// </param>
        /// <returns>
        /// the attribute value
        /// </returns>
        public static string GetAttributeValue(this XElement element, string attributeName, string defaultValue)
        {
            XAttribute attribute = element.Attribute(attributeName);
            return (attribute == null) ? defaultValue : attribute.Value;
        }

        /// <summary>
        /// Extends XElement 
        /// </summary>
        /// <param name="element">
        /// the xml node
        /// </param>
        /// <param name="attributeName">
        /// attribute name
        /// </param>
        /// <param name="defaultValue">
        /// default value
        /// </param>
        /// <returns>
        /// the attribute value
        /// </returns>
        public static Guid GetAttributeValue(this XElement element, string attributeName, Guid defaultValue)
        {
            XAttribute attribute = element.Attribute(attributeName);
            return (attribute == null) ? defaultValue : Guid.Parse(attribute.Value);
        }

        /// <summary>
        /// Extends XElement 
        /// </summary>
        /// <param name="element">
        /// the xml node
        /// </param>
        /// <param name="attributeName">
        /// attribute name
        /// </param>
        /// <param name="defaultValue">
        /// default value
        /// </param>
        /// <returns>
        /// the attribute value
        /// </returns>
        public static DateTime GetAttributeValue(this XElement element, string attributeName, DateTime defaultValue)
        {
            XAttribute attribute = element.Attribute(attributeName);
            return (attribute == null) ? defaultValue : DateTime.Parse(attribute.Value);
        }

        /// <summary>
        /// Extends XElement 
        /// </summary>
        /// <param name="element">
        /// the xml node
        /// </param>
        /// <param name="attributeName">
        /// attribute name
        /// </param>
        /// <param name="defaultValue">
        /// default value
        /// </param>
        /// <returns>
        /// the attribute value
        /// </returns>
        public static long GetAttributeValue(this XElement element, string attributeName, long defaultValue)
        {
            XAttribute attribute = element.Attribute(attributeName);
            long value;
            return (attribute == null || !long.TryParse(attribute.Value, out value)) ? defaultValue : value;
        }


        #endregion
    }
}