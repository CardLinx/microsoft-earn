//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.ApplicationInsights.Telemetry.Services;

namespace LoMo.EmailSubscription.Service
{
    using System.Diagnostics;

    using Lomo.Logging;

    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // Init Logger
            var listeners = new List<TraceListener>();
            listeners.Add(new DiagnosticMonitorTraceListener { Name = "AzureDiagnostics" });
            Log.Instance = new TraceLog(listeners);

            ServerAnalytics.Start("0a200247-f7ab-449f-ac7a-cdce9b3bdc32");
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender,
     EventArgs e)
        {
            ServerAnalytics.BeginRequest();
            ServerAnalytics.CurrentRequest.LogEvent(Request.Url.AbsolutePath);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}