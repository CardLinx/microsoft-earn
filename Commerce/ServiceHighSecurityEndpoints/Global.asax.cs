//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using Lomo.Authorization;
    using Lomo.Commerce.Configuration;
    using System.Web.Optimization;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Users.Dal;
    using System.Web.Routing;

    /// <summary>
    /// Performs startup actions for this High Security Web API application.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// Performs application startup actions.
        /// </summary>
        /// <remarks>
        /// Post-conditions:
        /// * Application startup actions have been performed.
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
         Justification = "This method cannot be static, because MVC requires this signature.")]
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterExternalServices();
        }

        /// <summary>
        /// Registers external services for commerce like MVC constructs, security providers and Analytics
        /// </summary>
        internal static void RegisterExternalServices()
        {
            // Register log.
            LogInitializer.CreateLogInstance(CommerceServiceConfig.Instance.LogVerbosity,
                                             CommerceServiceConfig.Instance.ForceEventLog,
                                             General.CommerceLogSource,
                                             CommerceServiceConfig.Instance);

            // Register MVC constructs.
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Register security providers.
            IUsersDal usersDal = PartnerFactory.UsersDal(CommerceServiceConfig.Instance);
            if (CommerceServiceConfig.Instance.EnableDebugSecurityProvider == true)
            {
                Security.Add("user_debug", new UserDebugSecurityProvider(usersDal));
            }
            Security.Add("usertoken", new UserTokenSecurityProvider());

            // Register Analytics Service
            Analytics.Initialize(CommerceServiceConfig.Instance);
        }
    }
}