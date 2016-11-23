//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;

namespace Lomo.Core.Extensions
{
    using System.Globalization;

    public static class DecimalExtensions
    {
        public static string FormatAsCurrency(this decimal value)
        {
            return String.Format((value == Math.Floor(value)) ? "{0:C0}" : "{0:C2}", value);
        }

        //NOTE - this is not the best implementation to format currency. We should use
        //.NET library with cultureInfo. 
        public static string FormatAsCurrency(this decimal value, string currencySymbol)
        {
            return String.Format("{0}{1}", currencySymbol, value.FormatAsNumeric());
        }

        public static string FormatAsCurrency(this decimal value, string currencySymbol, CultureInfo cultureInfo)
        {
            if (cultureInfo != null)
            {
                var format = (value == Math.Floor(value)) ? "C0" :"C2";
                return value.ToString(format, cultureInfo);
            }

            if (!string.IsNullOrEmpty(currencySymbol))
            {
                return value.FormatAsCurrency(currencySymbol);
            }

            return value.FormatAsNumeric();
        }

        public static string FormatAsNumeric(this decimal value)
        {
            return String.Format((value == Math.Floor(value)) ? "{0:F0}" : "{0:F2}", value);
        }

        public static string FormatAsPercent(this decimal value)
        {
            return String.Format(((value) == Math.Floor(value)) ? "{0:0}%" : "{0:2}%", value);
        }
    }
}