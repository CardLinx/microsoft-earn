//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Lomo.Logging;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.TraceListener;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class ApplicationInsightsBootstapper : IContextInitializer
    {
        public void Initialize(TelemetryContext context)
        {
            context.Properties["MachineName"] = Environment.MachineName;
            context.Properties["InstanceName"] = RoleEnvironment.CurrentRoleInstance.Role.Name;
        }

        public static void Initialize()
        {
            List<TraceListener> traceListeners;
            // configure trace listeners
            traceListeners = new List<TraceListener>
            {
                new ApplicationInsightsTraceListener
                {
                    Name = "ApplicationInsightsTraceListener",
                    Filter = new EventTypeFilter(SourceLevels.All),
                },
                new DiagnosticMonitorTraceListener()
                {
                    Name = "DiagnosticMonitorTraceListener",
                    Filter = new EventTypeFilter(SourceLevels.All),
                }
            };

            // configure application insights
            TelemetryConfiguration.Active.DisableTelemetry = false;
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = false;
            TelemetryConfiguration.Active.ContextInitializers.Add(new ApplicationInsightsBootstapper());
            Log.Instance = new TraceLog(traceListeners);
        }
    }
}