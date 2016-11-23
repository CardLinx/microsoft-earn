//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provides utility methods used by the MasterCard partner plug-in.
    /// </summary>
    public static class MasterCardUtilities
    {
        /// <summary>
        /// Parses the specified MasterCard formatted date/time string into a DateTime.
        /// </summary>
        /// <param name="dateTime">
        /// The MasterCard date/time string to parse.
        /// </param>
        /// <returns>
        /// * The DateTime representation of the specified string if successful.
        /// * Else returns DateTime.MinValue.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter dateTime cannot be null.
        /// </exception>
        public static DateTime ParseDateTimeString(string dateTime)
        {
            if (dateTime == null)
            {
                throw new ArgumentNullException("dateTime", "Parameter dateTime cannot be null.");
            }

            DateTime result = DateTime.MinValue;

            if (dateTime.Length == 14)
            {
                // Build a standard string representation of the date.
                StringBuilder normalizedDateTime = new StringBuilder(dateTime.Substring(0, 2));
                normalizedDateTime.Append("/");
                normalizedDateTime.Append(dateTime.Substring(2, 2));
                normalizedDateTime.Append("/");
                normalizedDateTime.Append(dateTime.Substring(4, 4));
                normalizedDateTime.Append(" ");
                normalizedDateTime.Append(dateTime.Substring(8, 2));
                normalizedDateTime.Append(":");
                normalizedDateTime.Append(dateTime.Substring(10, 2));
                normalizedDateTime.Append(":");
                normalizedDateTime.Append(dateTime.Substring(12, 2));

                // Attempt to parse the date.
                if (DateTime.TryParse(normalizedDateTime.ToString(), out result) == false)
                {
                    result = DateTime.MinValue;
                }
            }

            return result;
        }
    }
}