//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System;
    using System.Web.Http;

    /// <summary>
    /// Configures MVC Web API.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers MVC Web APIs.
        /// </summary>
        /// <param name="config">
        /// Configuration to register.
        /// </param>
        /// <remarks>
        /// Post-conditions:
        /// * MVC Web APIs have been registered.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Parameter config cannot be null.
        /// </exception>
        public static void Register(HttpConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // Service health APIs.
            config.Routes.MapHttpRoute("HbiPing", "api/commerce/ping", new { controller = "Ping" });

            // UI APIs.
            config.Routes.MapHttpRoute("V2FastHbiCards", "api/commerce/v2/cards/fast", new { controller = "V2FastCards" });
            config.Routes.MapHttpRoute("V2HbiCards", "api/commerce/v2/cards", new { controller = "V2Cards" });

            // UI pages.
            config.Routes.MapHttpRoute("clientApi", "api/client/{controller}");
        }
    }
}