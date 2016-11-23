//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Linq;

namespace Lomo.Core.Enums
{
    public static class EnumHelper<T> where T : struct
    {
        // ReSharper disable once StaticFieldInGenericType
        private static readonly Enum[] Values;
        private static readonly T DefaultValue;
        
        static EnumHelper()
        {
            var type = typeof(T);
            if (type.IsSubclassOf(typeof(Enum)) == false)
            {
                throw new ArgumentException();
            }
            Values = Enum.GetValues(type).Cast<Enum>().ToArray();
            DefaultValue = default(T);
        }

        public static T[] MaskToList(Enum mask, bool ignoreDefault = true)
        {
            var q = Values.Where(mask.HasFlag);
            if (ignoreDefault)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                q = q.Where(v => !v.Equals(DefaultValue));
            }
            // ReSharper disable once SuspiciousTypeConversion.Global
            return q.Cast<T>().ToArray();
        }
    }
}