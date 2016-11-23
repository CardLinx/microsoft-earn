//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Configuration
{
    using System.Configuration;

    /// <summary>
    /// A simple string configuration element.
    /// </summary>
    public class StringConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the value of the string.
        /// </summary>
        [ConfigurationProperty(ValueAttribute, IsKey = true, IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)this[ValueAttribute];
            }
            set
            {
                this[ValueAttribute] = value;
            }
        }

        /// <summary>
        /// The string's value attribute name.
        /// </summary>
        private const string ValueAttribute = "value";
    }
}