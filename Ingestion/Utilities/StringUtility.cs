//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class StringUtility
    {
        public static string StripAllButDigits(string input)
        {
            return (input == null) ? string.Empty : Regex.Replace(input, @"\D", string.Empty);
        }
    }
}