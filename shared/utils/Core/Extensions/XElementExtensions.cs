//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Lomo.Core.Extensions
{
	public static class XElementExtensions
	{
		public static T? GetNullableValueFromElement<T>(this XElement element, string xpath) where T : struct
		{
			var elementValueString = element.GetValueFromElement<string>(xpath);
			var returnValue = GetNullableValue<T>(elementValueString);
			return returnValue;
		}

		public static T GetValueFromElement<T>(this XElement element, string xpath)
		{

			return GetValueFromElement<T>(element, xpath, null, null);
		}

		public static T? GetNullableValueFromElement<T>(this XElement element, string xpath, string namespacePrefix,
													   string namespacuri) where T : struct
		{
			var elementValueString = element.GetValueFromElement<string>(xpath, namespacePrefix, namespacuri);
			var returnValue = GetNullableValue<T>(elementValueString);
			return returnValue;
		}

		public static T GetValueFromElement<T>(this XElement element, string xpath, string namespacePrefix, string namespacuri)
		{
			XmlNamespaceManager namespaceManager = null;
			if (!string.IsNullOrEmpty(namespacuri))
			{
				namespaceManager = new XmlNamespaceManager(new NameTable());
				namespaceManager.AddNamespace(namespacePrefix, namespacuri);
			}
			return element.GetValueFromElement<T>(xpath, namespaceManager);
		}

		public static T GetValueFromElement<T>(this XElement element, string elementName, XNamespace namespaceName)
		{
			T returnValue = default(T);
			var elementFound = element.Descendants(namespaceName + elementName).FirstOrDefault();
			if (elementFound != null)
			{
				string resultString = elementFound.Value;
				returnValue = (T)Convert.ChangeType(resultString, typeof(T));
			}
			return returnValue;
		}

		public static XAttribute GetAttribute(this XElement element, string name, bool required = false)
		{
			var result = element.Attribute(name);
			if (required && result == null)
				throw new Exception("Could not find attribute [{0}]".ExpandWith(name));
			return result;
		}

		public static XElement GetElement(this XElement element, string xpath, bool required = false)
		{
			var result = GetElement(element, xpath, null, null);
			if (required && result == null)
				throw new Exception("Could not find element at path [{0}]".ExpandWith(xpath));
			return result;
		}

		public static T GetValueFromElement<T>(this XElement element, string xpath, bool useDefaultNamespace)
		{
			if (!useDefaultNamespace)
				return GetValueFromElement<T>(element, xpath);
			const string PREFIX = "x";
			xpath = xpath.Replace("/", "/{0}:".ExpandWith(PREFIX));
			if (!xpath.StartsWith("/"))
				xpath = "{0}:{1}".ExpandWith(PREFIX, xpath);
			var defaultNamespace = element.GetDefaultNamespace().ToString();
			return GetValueFromElement<T>(element, xpath, PREFIX, defaultNamespace);
		}

		public static IEnumerable<XElement> GetElements(this XElement element, string xpath, string namespacePrefix, string namespacuri)
		{
			XmlNamespaceManager namespaceManager = null;
			if (!string.IsNullOrEmpty(namespacuri))
			{
				namespaceManager = new XmlNamespaceManager(new NameTable());
				namespaceManager.AddNamespace(namespacePrefix, namespacuri);
			}
			var results = element.XPathSelectElements(xpath, namespaceManager);
			return results;
		}

		public static XElement GetElement(this XElement element, string xpath, string namespacePrefix, string namespacuri)
		{
			XmlNamespaceManager namespaceManager = null;
			if (!string.IsNullOrEmpty(namespacuri))
			{
				namespaceManager = new XmlNamespaceManager(new NameTable());
				namespaceManager.AddNamespace(namespacePrefix, namespacuri);
			}
			XElement result = element.XPathSelectElement(xpath, namespaceManager);
			return result;
		}

		public static T GetValueFromElement<T>(this XElement element, string xpath, XmlNamespaceManager namespaceManager)
		{
			T returnValue = default(T);

			XElement result = element.XPathSelectElement(xpath, namespaceManager);
			if (result != null)
			{
				string resultString = result.Value;
				returnValue = (T)Convert.ChangeType(resultString, typeof(T));
			}

			return returnValue;
		}

		public static T GetValueFromElementAttribute<T>(this XElement element, string xpath, string attributeName)
		{
			return element.GetValueFromElementAttribute<T>(xpath, attributeName, new XmlNamespaceManager(new NameTable()));
		}

		public static T GetValueFromElementAttribute<T>(this XElement element, string xpath, string attributeName, XmlNamespaceManager namespaceManager)
		{
			T returnValue = default(T);

			XElement result = element.XPathSelectElement(xpath, namespaceManager);
			if (result != null && result.Attribute(attributeName) != null)
			{
				string resultString = result.Attribute(attributeName).Value;
				returnValue = (T)Convert.ChangeType(resultString, typeof(T));
			}

			return returnValue;
		}

		public static T GetValueFromElementAttributeThrows<T>(this XElement element, string xpath, string attributeName)
		{
			return element.GetValueFromElementAttribute<T>(xpath, attributeName, new XmlNamespaceManager(new NameTable()));
		}

		public static T GetValueFromElementAttributeThrows<T>(this XElement element, string xpath, string attributeName, XmlNamespaceManager namespaceManager)
		{
			XElement result = element.XPathSelectElement(xpath, namespaceManager);
			if (result != null && result.Attribute(attributeName) != null)
			{
				string resultString = result.Attribute(attributeName).Value;
				return (T)Convert.ChangeType(resultString, typeof(T));
			}

			throw new Exception(string.Format("Attribute {0} not found from xpath {1}", attributeName, xpath));
		}

		private static T? GetNullableValue<T>(string inputValue) where T : struct
		{
			T? returnValue = null;
			if (!string.IsNullOrEmpty(inputValue))
			{
				try
				{
					returnValue = (T)Convert.ChangeType(inputValue, typeof(T));
				}
				catch
				{
				}
			}
			return returnValue;
		}

	}
}