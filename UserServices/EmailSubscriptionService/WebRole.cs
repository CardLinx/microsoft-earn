//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The web role.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EmailSubscriptionService
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using Lomo.Logging;

    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// The web role.
    /// </summary>
    public class WebRole : RoleEntryPoint
    {
        /// <summary>
        /// The on start.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool OnStart()
        {
            // Init Logger
            Log.Instance = new TraceLog(new List<TraceListener> { new DiagnosticMonitorTraceListener { Name = "AzureDiagnostics", Filter = new EventTypeFilter(SourceLevels.All) } });
            return base.OnStart();
        }
    }
}