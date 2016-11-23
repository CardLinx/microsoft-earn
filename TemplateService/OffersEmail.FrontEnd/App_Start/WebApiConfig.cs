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
    using System.Web.Http;

    /// <summary>
    /// Register web apis
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register web api routes
        /// </summary>
        /// <param name="config">existing configuration</param>
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}