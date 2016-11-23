//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace OffersEmail.FrontEnd
{
    using System.Web.Mvc;

    /// <summary>
    /// Used to register fiters
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Register global filters
        /// </summary>
        /// <param name="filters">existing filters</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}