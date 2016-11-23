//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Common
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Extension methods for System.Object class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Throws ArgumentNullException if the object is null. 
        /// This can be used to validate arguments of a function.
        /// </summary>
        /// <param name="value">The value that cannot be null.</param>
        /// <param name="valueName">The name of the value.</param>
        public static void ThrowIfNull(this object value, string valueName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(
                    valueName,
                    string.Format(CultureInfo.InvariantCulture, "The parameter '{0}' is null.", valueName));
            }
        }

        /// <summary>
        /// Returns hash code for objects including those which can be null.
        /// A value of 0 is returned as the hash code if the object is null.
        /// </summary>
        /// <param name="value">The object whose hash code needs to be retrieved.</param>
        /// <returns>
        /// 0 if the object is null, other the hash code of the object is returned.
        /// </returns>
        public static int GetNullAwareHashCode(this object value)
        {
            if (value == null) return 0;

            return value.GetHashCode();
        }
    }
}