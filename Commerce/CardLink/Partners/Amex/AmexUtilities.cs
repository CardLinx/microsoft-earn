//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides utility methods used by the Amex partner plug-in.
    /// </summary>
    public static class AmexUtilities
    {
        /// <summary>
        /// Amex transaction time zone id
        /// </summary>
        public const string TimeZoneId = "US Mountain Standard Time";

        /// <summary>
        /// Amex transaction date time format
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Parse the amount passed in Auth Request
        /// </summary>
        /// <param name="amount">
        /// Amount in string representation of decimal
        /// </param>
        /// <returns>
        /// Amount in cents
        /// </returns>
        public static int ParseAuthAmount(string amount)
        {
            decimal authAmount = decimal.Parse(amount);
            // currently currency is USD
            return (int)(authAmount * 100);
        }

        /// <summary>
        /// Parse the purchase date time in Auth Request
        /// </summary>
        /// <param name="purchaseDateTime">
        /// datetime string
        /// </param>
        /// <returns>
        /// Date time value
        /// </returns>
        public static DateTime ParseAuthDateTime(string purchaseDateTime)
        {
            // Amex Auth time zone is MST
            TimeZoneInfo mstInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            DateTime mstTime = DateTime.ParseExact(purchaseDateTime, DateTimeFormat, CultureInfo.InvariantCulture);
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(mstTime, mstInfo);
            return utcTime;
        }
    }
}