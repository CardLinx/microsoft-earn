//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;

namespace Lomo.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToYearMonthDayFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string ToYearMonthDayHourMinuteSecondFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime SpecifyDateTimeKindUtc(this DateTime dateTime)
        {
            return dateTime.Kind != DateTimeKind.Utc ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime;
        }

        public static string ToStandardDateTimeString(this DateTime dateTime)
        {
            return dateTime.Kind == DateTimeKind.Utc
                       ? dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
                       : dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"); // ISO 8601
        }
    }
}