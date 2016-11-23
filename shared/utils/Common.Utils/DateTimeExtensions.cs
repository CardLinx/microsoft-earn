//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Extension methods for DateTime class.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets a string representation of the DateTime value in Round-trip pattern (ISO 8601).
        /// </summary>
        /// <param name="dateTime">The DateTime value.</param>
        /// <returns>
        /// A string instance containing the DateTime in Round-trip pattern.
        /// </returns>
        public static string ToRoundtripFormatString(this DateTime dateTime)
        {
            return dateTime.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}