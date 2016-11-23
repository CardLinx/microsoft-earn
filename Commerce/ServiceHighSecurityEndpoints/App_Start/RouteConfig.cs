//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Contains configuration for MVC UI routes.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Registers MVC UI routes.
        /// </summary>
        /// <param name="routes">
        /// The routes to register.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}