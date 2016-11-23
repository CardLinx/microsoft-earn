//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Azure.Utils
{
    /// <summary>
    ///     The following entity is a dynamic entity for azure table.
    /// </summary>
    public class DynamicEntity : DynamicObject
    {
        #region Fields

        /// <summary>
        ///     Field to hold the properties of the entity
        /// </summary>
        private readonly IDictionary<string, object> properties =
            new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicEntity" /> class.
        /// </summary>
        public DynamicEntity()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicEntity" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentNullException">Object is null.</exception>
        public DynamicEntity(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // Allow only anonymous and dynamic types
            string ns = value.GetType().Namespace;
            bool supportedTypes = ns == null || ns.Equals("System.Dynamic");

            if (!supportedTypes)
            {
                throw new ArgumentException(string.Format("{0} type is not supported", value.GetType()), "value");
            }

            // get object name/values via reflection
            IEnumerable<KeyValuePair<string, object>> fields = value
                .GetType()
                .GetProperties()
                .Select(property => new KeyValuePair<string, object>(property.Name, property.GetValue(value, null)));
            this.AddProperties(fields);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicEntity" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <exception cref="System.ArgumentNullException">Dictionary is null.</exception>
        public DynamicEntity(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            this.AddProperties(dictionary);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicEntity" /> class.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <exception cref="System.ArgumentNullException">Enumerator is null.</exception>
        public DynamicEntity(IEnumerable<KeyValuePair<string, object>> enumerator)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException("enumerator");
            }

            this.AddProperties(enumerator);
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets or sets the <see cref="System.Object" /> with the specified key.
        /// </summary>
        /// <value>
        ///     The <see cref="System.Object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     value of the property.
        /// </returns>
        public object this[string key]
        {
            get
            {
                return this.properties[key];
            }

            set
            {
                this.properties[key] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether the specified property name contains property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///     <c>true</c> if the specified property name contains property; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsProperty(string propertyName)
        {
            return this.properties.ContainsKey(propertyName);
        }

        /// <summary>
        ///     Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        ///     A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.properties.Keys.ToList();
        }

        /// <summary>
        ///     Gets the properties.
        /// </summary>
        /// <returns>properties name and value.</returns>
        public IEnumerable<KeyValuePair<string, object>> GetProperties()
        {
            return this.properties.ToList();
        }

        /// <summary>
        ///     Removes the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>true if removed.</returns>
        public bool RemoveProperty(string propertyName)
        {
            if (this.properties.ContainsKey(propertyName))
            {
                return this.properties.Remove(propertyName);
            }

            return false;
        }

        /// <summary>
        ///     Builds the dynamic table entity.
        /// </summary>
        /// <param name="autoGenerateKeys">
        ///     if set to <c>true</c> [auto generate keys].
        /// </param>
        /// <returns>
        ///     The DynamicTableEntity
        /// </returns>
        /// <exception cref="System.ArgumentException">The Columns Limit</exception>
        /// <exception cref="System.ArgumentNullException">Dynamic Entity</exception>
        public DynamicTableEntity ToDynamicTableEntity(bool autoGenerateKeys = false)
        {
            var dynamicTableEntity = new DynamicTableEntity();

            foreach (var field in this.properties)
            {
                switch (field.Key.ToLowerInvariant())
                {
                    case "partitionkey":
                        dynamicTableEntity.PartitionKey = field.Value as string;
                        break;
                    case "rowkey":
                        dynamicTableEntity.RowKey = field.Value as string;
                        break;
                    case "etag":
                        dynamicTableEntity.ETag = field.Value as string;
                        break;
                    default:
                        if (field.Value != null)
                        {
                            dynamicTableEntity.Properties[field.Key] = AzureHelper.WriteEntityProperty(field.Value);
                        }

                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(dynamicTableEntity.PartitionKey))
            {
                if (autoGenerateKeys)
                {
                    dynamicTableEntity.PartitionKey = DateTime.UtcNow.Date.ToString("yyyy-MM-dd-HH-mm");
                }
                else
                {
                    throw new ArgumentException("PartitionKey is missing or not of type \"System.String\".");
                }
            }
            else
            {
                AzureHelper.ValidateEntityKey(dynamicTableEntity.PartitionKey);
            }

            if (string.IsNullOrWhiteSpace(dynamicTableEntity.RowKey))
            {
                if (autoGenerateKeys)
                {
                    dynamicTableEntity.RowKey = Guid.NewGuid().ToString();
                }
                else
                {
                    throw new ArgumentException("RowKey is missing or not of type \"System.String\".");
                }
            }
            else
            {
                AzureHelper.ValidateEntityKey(dynamicTableEntity.RowKey);
            }

            return dynamicTableEntity;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            int count = 0;
            if (this.properties != null)
            {
                foreach (var kvp in this.properties)
                {
                    if (count > 0)
                    {
                        stringBuilder.Append("|");
                    }

                    string value = (kvp.Value == null) ? string.Empty : kvp.Value.ToString();
                    stringBuilder.Append(kvp.Key + "=" + value);
                    count++;
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Provides the implementation for operations that get member values. Derived from the
        ///     <see
        ///         cref="T:System.Dynamic.DynamicObject" />
        ///     class.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="result">The result of the get operation.</param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false.
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.properties.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        ///     Provides the implementation for operations that set member values. Derived from the
        ///     <see
        ///         cref="T:System.Dynamic.DynamicObject" />
        ///     class.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="value">The value to set to the member.</param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false.
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.properties[binder.Name] = value;
            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the properties.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        private void AddProperties(IEnumerable<KeyValuePair<string, object>> enumerator)
        {
            foreach (var field in enumerator)
            {
                this.properties.Add(field.Key, field.Value);
            }
        }

        #endregion
    }
}