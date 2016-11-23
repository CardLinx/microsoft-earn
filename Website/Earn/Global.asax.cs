//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Earn.App_Start;
using Microsoft.Passport.RPS;

namespace Earn
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            InitRps();
            HandleSslCertValidation();
            ServicePointManager.DefaultConnectionLimit = 48;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
        }

        private void HandleSslCertValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }

        void InitRps()
        {
            RPS globalRPS = new RPS();
            Application["globalRPS"] = globalRPS;
            globalRPS.Initialize(null);
        }

        void Application_End(object sender, EventArgs e)
        {
            if (Application["globalRPS"] is RPS)
            {
                RPS globalRPS = (RPS)Application["globalRPS"];
                globalRPS.Shutdown();
                Application["globalRPS"] = null;
            }
        }
    }
}