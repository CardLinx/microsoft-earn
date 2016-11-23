//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The utils.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace AnalyticsClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The utils.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToString(List<string> list)
        {
            string listStr = list == null ? string.Empty : string.Join(",", list);
            return listStr;
        }

        /// <summary>
        /// The to list.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <returns>
        /// the list
        /// </returns>
        public static List<string> ToList(string str)
        {
            if (str == null)
            {
                return new List<string>();
            }

            var list = str.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return list;
        }
    }
}