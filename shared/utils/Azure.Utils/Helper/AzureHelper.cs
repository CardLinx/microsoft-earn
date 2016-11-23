//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Azure.Utils
{
    /// <summary>
    ///     Static helper class for Azure.Utils.
    /// </summary>
    public static class AzureHelper
    {
        #region Static Fields

        /// <summary>
        ///     Field for the regular expression of table name.
        /// </summary>
        private static readonly Regex TableNameRegex = new Regex("^[A-Za-z][A-Za-z0-9]{2,62}$", RegexOptions.Compiled);

        /// <summary>
        ///     Field for the regular expression of key value.
        /// </summary>
        private static readonly Regex ValidKeyFieldRegex = new Regex(@"^[^#/\\?]{0,1024}$", RegexOptions.Compiled);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Reads the entity property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The object.</returns>
        public static object ReadEntityProperty(EntityProperty value)
        {
            switch (value.PropertyType)
            {
                // WCF Data Services type
                case EdmType.Binary:
                    return value.BinaryValue;
                case EdmType.Boolean:
                    return value.BooleanValue;
                case EdmType.DateTime:
                    return value.DateTimeOffsetValue;
                case EdmType.Double:
                    return value.DoubleValue;
                case EdmType.Guid:
                    return value.GuidValue;
                case EdmType.Int32:
                    return value.Int32Value;
                case EdmType.Int64:
                    return value.Int64Value;
                case EdmType.String:
                    return value.StringValue;
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Convert to the dynamic entity.
        /// </summary>
        /// <param name="dynamicTableEntity">The dynamic table entity.</param>
        /// <returns>The DynamicEntity.</returns>
        public static DynamicEntity ToDynamicEntity(this DynamicTableEntity dynamicTableEntity)
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            foreach (var property in dynamicTableEntity.Properties)
            {
                properties.Add(property.Key, ReadEntityProperty(property.Value));
            }

            var dynamicEntity = new DynamicEntity(properties);
            dynamicEntity["PartitionKey"] = dynamicTableEntity.PartitionKey;
            dynamicEntity["RowKey"] = dynamicTableEntity.RowKey;
            dynamicEntity["Timestamp"] = dynamicTableEntity.Timestamp;
            dynamicEntity["ETag"] = dynamicTableEntity.ETag;
            return dynamicEntity;
        }

        /// <summary>
        ///     Function member to validate key field.
        /// </summary>
        /// <param name="keyField">key field.</param>
        /// <exception cref="System.ArgumentException">Invalid key.</exception>
        public static void ValidateEntityKey(string keyField)
        {
            if (!ValidKeyFieldRegex.IsMatch(keyField))
            {
                throw new ArgumentException(string.Format("{0} is not valid", keyField));
            }
        }

        /// <summary>
        ///     Function member to validate table name.
        /// </summary>
        /// <param name="tableName">table name.</param>
        /// <exception cref="System.ArgumentException">Invalid table name.</exception>
        public static void ValidateTableName(string tableName)
        {
            if (!TableNameRegex.IsMatch(tableName))
            {
                throw new ArgumentException("Invalid table name.");
            }
        }

        /// <summary>
        ///     Function member to validate field type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>EntityProperty converted from object</returns>
        /// <exception cref="System.ArgumentException">Type not supported</exception>
        public static EntityProperty WriteEntityProperty(object value)
        {
            ////******************* WCF Data Services type ************************/
            ////Edm.Binary ->     byte[], An array of bytes up to 64 KB in size.
            ////Edm.Boolean ->    bool, A Boolean value.
            ////Edm.DateTime ->   DateTime, A 64-bit value expressed as Coordinated Universal Time (UTC). 
            ////The supported DateTime range begins from 12:00 midnight, January 1, 1601 A.D. (C.E.), UTC. The range ends at December 31, 9999.
            ////Edm.Double ->     double, A 64-bit floating point value.
            ////Edm.Guid ->       Guid, A 128-bit globally unique identifier.
            ////Edm.Int32 ->      Int32 or int, A 32-bit integer.
            ////Edm.Int64 ->      Int64 or long, A 64-bit integer.
            ////Edm.String ->     String, A UTF-16-encoded value. String values may be up to 64 KB in size.

            Type type = value.GetType();
            switch (type.Name)
            {
                // WCF Data Services type
                case "Byte[]":
                    return EntityProperty.GeneratePropertyForByteArray((byte[])value);
                case "Boolean":
                    return EntityProperty.GeneratePropertyForBool((bool)value);
                case "DateTime":
                    return EntityProperty.GeneratePropertyForDateTimeOffset(new DateTimeOffset((DateTime)value));
                case "DateTimeOffset":
                    return EntityProperty.GeneratePropertyForDateTimeOffset((DateTimeOffset)value);
                case "Double":
                    return EntityProperty.GeneratePropertyForDouble((double)value);
                case "Guid":
                    return EntityProperty.GeneratePropertyForGuid((Guid)value);
                case "Int32":
                    return EntityProperty.GeneratePropertyForInt((int)value);
                case "Int64":
                    return EntityProperty.GeneratePropertyForLong((long)value);
                case "String":
                    return EntityProperty.GeneratePropertyForString((string)value);
                default:
                    throw new ArgumentException(string.Format("Type \"{0}\" is not supported.", type.FullName));
            }
        }

        #endregion
    }
}