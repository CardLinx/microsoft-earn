//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using Earn.Dashboard.Web.Utils;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Earn.Dashboard.Web
{
    public class ApplicationInsightsBootstapper : IContextInitializer
    {
        public void Initialize(TelemetryContext context)
        {
            context.Properties["MachineName"] = Environment.MachineName;
        }

        public static void Initialize()
        {
            TelemetryConfiguration.Active.DisableTelemetry = !Config.IsProduction;
            TelemetryConfiguration.Active.ContextInitializers.Add(new ApplicationInsightsBootstapper());
        }
    }
}