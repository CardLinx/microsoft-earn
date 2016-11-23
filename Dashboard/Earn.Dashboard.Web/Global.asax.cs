//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Earn.Dashboard.Web.Controllers;
using Earn.Dashboard.Web.Utils;
using Microsoft.ApplicationInsights;

namespace Earn.Dashboard.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ApplicationInsightsBootstapper.Initialize();
        }

        protected void Application_BeginRequest()
        {
            Guid requestId = Guid.NewGuid();
            System.Diagnostics.Trace.CorrelationManager.ActivityId = requestId;
            System.Diagnostics.Tracing.EventSource.SetCurrentThreadActivityId(requestId);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();

            var controller = new ErrorController();
            var routeData = new RouteData();

            var httpCode = string.Empty;
            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;
                httpCode = string.Concat(" (", httpEx.GetHttpCode(), ")");
            }

            Log.Error(ex, "Application_Error");
            TelemetryClient telemetry = new TelemetryClient();
            telemetry.TrackException(ex);
            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;
            httpContext.Response.ContentType = "text/html";
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";

            controller.ViewData.Model = new HandleErrorInfo(ex, string.Concat(currentController, httpCode), currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}