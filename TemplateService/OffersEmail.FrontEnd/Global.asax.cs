//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.FrontEnd
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Lomo.Logging;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Application Global CallBacks
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application Start
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var localResourcePath = RoleEnvironment.IsAvailable ? RoleEnvironment.GetLocalResource("Screenshot").RootPath : Server.MapPath("\\Screenshot");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OffersEmail.FrontEnd.External.phantomjs.exe"))
            {
                if (stream == null)
                {
                    throw new FileLoadException("An error occurred while loading the file", "phantomjs.exe");
                }

                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                File.WriteAllBytes(Path.Combine(localResourcePath, "phantomjs.exe"), bytes);
            }

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OffersEmail.FrontEnd.External.screenshot.js"))
            {
                if (stream == null)
                {
                    throw new FileLoadException("An error occurred while loading the file", "screenshot.js");
                }

                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                File.WriteAllBytes(Path.Combine(localResourcePath, "screenshot.js"), bytes);
            }

            if (RoleEnvironment.IsAvailable)
            {
                var traceListners = new List<TraceListener> { new DiagnosticMonitorTraceListener { Name = "AzureDiagnostics", Filter = new EventTypeFilter(SourceLevels.All) } };
                Log.Instance = new TraceLog(traceListners);
            }
        }
    }
}