//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The initialise service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace LoMo.EmailSubscription.Service
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using Lomo.Logging;

    using Microsoft.WindowsAzure.Diagnostics;

    /// <summary>
    /// The initialization service.
    /// </summary>
    public class InitializeService
    {
        /// <summary>
        /// Application initialization method
        /// </summary>
        public static void AppInitialize()
        {
            // Init Logger
            var listeners = new List<TraceListener>();
            listeners.Add(new DiagnosticMonitorTraceListener { Name = "AzureDiagnostics" });
            Log.Instance = new TraceLog(listeners);
        }
    }
}