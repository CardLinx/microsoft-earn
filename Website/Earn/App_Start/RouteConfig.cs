//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Web.Mvc;
using System.Web.Routing;

namespace Earn
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(name: "FBRedirect", url: "facebook", defaults: new { controller = "Social", action = "Facebook", id = UrlParameter.Optional });
            routes.MapRoute(name: "ExpireCookie", url: "expirecookie", defaults: new { controller = "ExpireCookie", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "Learn", url: "learn", defaults: new { controller = "Learn", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "Join", url: "enroll/{action}", defaults: new { controller = "Enroll", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "Test", url: "test/{action}", defaults: new { controller = "Test", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "Legal", url: "Legal/{action}", defaults: new { controller = "Legal", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "Migrate", url: "migrate", defaults: new { controller = "Migrate", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "MyAnid", url: "myanid", defaults: new { controller = "MyAnid", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "Default", url: "{action}", defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "Merchant", url: "merchant/{action}", defaults: new { controller = "Merchant", action = "Index", id = UrlParameter.Optional });
        }
    }
}