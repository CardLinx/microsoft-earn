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
    using System.Web.Routing;

    /// <summary>
    /// Register route configuration
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Register global routes
        /// </summary>
        /// <param name="routes">Existing routes</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "EmailTemplate",
                url: "{controller}/{action}/{campaign}/{referrer}",
                defaults: new
                {
                    controller = "GetEmail",
                    action = "Index",
                    campaign = string.Empty,
                    referrer = "BO_EMAIL"
                },
                constraints: new { controller = "GetEmail|Seasonal|Reminder|Newsletter" });

            routes.MapRoute(
                name: "Mass Email",
                url: "outlook/{city}/{version}",
                defaults: new
                {
                    controller = "Reminder",
                    action = "OutlookMassEmail",
                    city = "S",
                    version = "A",
                });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}