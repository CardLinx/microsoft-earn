//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Configures MVC filters.
    /// </summary>
    public static class FilterConfig
    {
        /// <summary>
        /// Registers global MVC filters.
        /// </summary>
        /// <param name="filters">
        /// The filters to register.
        /// </param>
        /// <remarks>
        /// Post-conditions:
        /// * Filters have been registered.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Parameter filters cannot be null.
        /// </exception>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException("filters");
            }

            filters.Add(new HandleErrorAttribute());
            filters.Add(new AiHandleErrorAttribute());
        }
    }
}