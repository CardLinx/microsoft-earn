//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Microsoft.HolMon.Security;

namespace OfferManagement
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Attribute routing.
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var authenticationHandlers = CreateAuthenticationHandlers();
            config.MessageHandlers.Add(new ClaimsAuthenticationMessageHandler(authenticationHandlers));

            //   config.Routes.MapHttpRoute(
            //    name: "ActionApi",
            //    routeTemplate: "api/{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }

        static IEnumerable<IAuthenticationHandler> CreateAuthenticationHandlers()
        {
            var tokenIssuerAndKeys = new Dictionary<TokenIssuer, KeyPair>
                                     {
                                         {
                                             TokenIssuer.EarnDashboard,
                                             new KeyPair(ConfigurationManager.AppSettings["SwtPrimaryKey"], ConfigurationManager.AppSettings["SwtSecondaryKey"])
                                         }
                                     };

            var audienceIssuerRolesMapping = new Dictionary<TokenAudience, IDictionary<TokenIssuer, IList<string>>>
                                             {
                                                 {
                                                     TokenAudience.EarnService,
                                                     new Dictionary<TokenIssuer, IList<string>>
                                                     {
                                                         {
                                                             TokenIssuer.EarnDashboard,
                                                             new List<string>
                                                             {
                                                                 Roles.AllApisAccessRole
                                                             }
                                                         }
                                                     }
                                                 }
                                             };

            IAuthenticationHandler[] authenticationHandlers =
            {
                new BearerAuthenticationHandler(tokenIssuerAndKeys, audienceIssuerRolesMapping)
            };

            return authenticationHandlers;
        }
    }
}