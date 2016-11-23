//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    /// <summary>
    /// Extension methods for DateTime class.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Converts DateTime object to Epoch time.
        /// </summary>
        /// <param name="dateTime">DateTime object.</param>
        /// <returns>Epoch time representation.</returns>
        public static ulong ConvertToEpochTime(this DateTime dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ts = date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToUInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// Converts epoch time representation to DateTime object.
        /// </summary>
        /// <param name="epochInterval">EpochTime representation.</param>
        /// <returns>DateTime object.</returns>
        public static DateTime ConvertFromEpochTime(this ulong epochInterval)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epochInterval);
        }
    }
}