//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Concurrent;

namespace Lomo.Core.Extensions
{
    public static class TypeExtension
    {
        private static readonly ConcurrentDictionary<Type, object> TypeDefaults = new ConcurrentDictionary<Type, object>();
        private static readonly Type NullableType = typeof(Nullable<>);
        private static readonly Type StringType = typeof(string);

        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? TypeDefaults.GetOrAdd(type, Activator.CreateInstance) : null;
        }

        public static object GetTypedValue(this Type objectType, object inputObject)
        {
            var targetType = objectType;
            if (IsNullableType(objectType))
            {
                targetType = Nullable.GetUnderlyingType(objectType);
            }

            var returnValue = objectType.GetDefaultValue();
            if (inputObject != null)
            {
                returnValue = Convert.ChangeType(inputObject, targetType);
            }
            return returnValue;
        }

        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == NullableType;
        }

        public static bool IsStringType(this Type type)
        {
            return type == StringType;
        }
    }
}