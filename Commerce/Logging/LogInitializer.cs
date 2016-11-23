//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Utilities;
    using Lomo.Logging;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    /// <summary>
    /// Includes startup methods used across roles.
    /// </summary>
    public static class LogInitializer
    {
        /// <summary>
        /// Creates the appropriate Log instance for the current environment.
        /// </summary>
        /// <param name="logVerbosity">
        /// Specified the verbosity level for entries to commit to the log.
        /// </param>
        /// <param name="forceEventLog">
        /// Specifies whether to force use of the event log instead of other logging mechanisms.
        /// </param>
        /// <param name="source">
        /// The source under which to log events.
        /// </param>
        /// <param name="configuration">
        /// The configuration to use to get settings.
        /// </param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
         Justification = "To fully fix this, TraceLog class would have to dispose of the listeners sent to it as well. " +
                         "Since the application will fail to start if this fails, this can't represent an important leak.")]
        public static void CreateLogInstance(SourceLevels logVerbosity,
                                             bool forceEventLog,
                                             string source,
                                             CommerceConfig configuration)
        {
            // Setup the log.
            string deploymentId = null;
            string instanceId = null;
            TraceListener traceListener = null;
            if (General.RunningInAzure == true)
            {
                deploymentId = RoleEnvironment.DeploymentId;
                instanceId = RoleEnvironment.CurrentRoleInstance.Id;
                traceListener = new DiagnosticMonitorTraceListener { Name = "AzureDiagnostics" };
            }

            CreateLogInstance(deploymentId, instanceId, traceListener, logVerbosity, forceEventLog, source, configuration);
        }        

        /// <summary>
        /// Creates the appropriate Log instance for the current environment.
        /// </summary>
        /// <param name="deploymentId">
        /// * If running in Azure, the ID of the deployment that created the VM hosting this application
        /// * Else null.
        /// </param>
        /// <param name="instanceId">
        /// * If running in Azure, the ID of the role instance currently hosting this application
        /// * Else null.
        /// </param>
        /// <param name="traceListener">
        /// * If running in Azure, the Azure diagnostic monitor trace listener to use for the log.
        /// * Else null.
        /// </param>
        /// <param name="logVerbosity">
        /// Specified the verbosity level for entries to commit to the log.
        /// </param>
        /// <param name="forceEventLog">
        /// Specifies whether to force use of the event log instead of other logging mechanisms.
        /// </param>
        /// <param name="source">
        /// The source under which to log events.
        /// </param>
        /// <param name="configuration">
        /// The configuration to use to get settings.
        /// </param>
        internal static void CreateLogInstance(string deploymentId,
                                               string instanceId,
                                               TraceListener traceListener,
                                               SourceLevels logVerbosity,
                                               bool forceEventLog,
                                               string source,
                                               CommerceConfig configuration)
        {
            lock (CreateLogInstanceLock)
            {
                string dashboardConnectionKey;

                if (General.RunningInAzure == true)
                {
                    // If the event log is not set to be used regardless of environment, create a TraceLog and a listener to
                    // funnel log entries into central storage.
                    if (forceEventLog == false)
                    {
                        Log.Instance = new TraceLog(new List<TraceListener> { traceListener }, logVerbosity)
                        {
                            Source = source
                        };
                    }
                    else
                    {
                        Log.Instance = new EventLogLog(logVerbosity)
                        {
                            Source = source
                        };
                    }

                    // Set the server ID to a value useful in Azure.
                    RequestInformationExtensions.ServerId = String.Format("{0}_{1}", deploymentId, 
                                      instanceId.Substring(instanceId.LastIndexOf("_", StringComparison.OrdinalIgnoreCase) + 1));

                    dashboardConnectionKey = CloudConfigurationManager.GetSetting(DashboardConnectionString);
                }
                else
                {
                    Log.Instance = new EventLogLog(logVerbosity)
                    {
                        Source = source
                    };

                    dashboardConnectionKey = ConfigurationManager.AppSettings[DashboardConnectionString];
                }

                // Initialize the dashboard table into which a subset of warnings will be logged.
                if (String.IsNullOrEmpty(dashboardConnectionKey) == false)
                {
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(dashboardConnectionKey);
                    PartnerFactory.AzureTable(configuration).Initialize(cloudStorageAccount, DashboardTableName,
                                                           new ExponentialRetry(TimeSpan.FromSeconds(5), 1));
                }

                // Flag log instance as set so CommerceLog won't attempt to create one.
                CommerceLog.LogInstanceSet = true;
                CommerceLog.Config = configuration;
            }
        }

        /// <summary>
        /// The lock to use when creating the log instance.
        /// </summary>
        private static readonly object CreateLogInstanceLock = new object();

        /// <summary>
        /// The app settings key under which the dashboard storage connection string can be found.
        /// </summary>
        private const string DashboardConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

        /// <summary>
        /// The name of the Azure table in which to log dashboard-bound entries.
        /// </summary>
        private const string DashboardTableName = "Warnings";
    }
}