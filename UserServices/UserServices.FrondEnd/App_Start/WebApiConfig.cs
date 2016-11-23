//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The web api config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UserServices.FrondEnd
{
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Routing;

    /// <summary>
    /// The web api config.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            config.Routes.MapHttpRoute(
                name: "GetUserApi", 
                routeTemplate: "api/userinfo/", 
                defaults: new { controller = "UserInfo", action = "Get" }, 
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            
            config.Routes.MapHttpRoute(
                name: "ConfirmApi",
                routeTemplate: "api/confirm/{action}",
                defaults: new { controller = "ConfirmApi" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            


            config.Routes.MapHttpRoute(name: "ActionApi", routeTemplate: "api/{controller}/{action}");

            config.Routes.MapHttpRoute(name: "DefaultApi", routeTemplate: "api/{controller}");
        }
    }
}