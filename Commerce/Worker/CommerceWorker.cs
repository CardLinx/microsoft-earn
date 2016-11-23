//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Lomo.Commerce.Worker
{
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Commerce.WorkerJobs;
    using Lomo.Scheduler;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The commerce worker role, used for redemption batch processing.
    /// </summary>
    public class CommerceWorker : RoleEntryPoint
    {
        /// <summary>
        /// Initializes a new instance of the CommerceWorker class.
        /// </summary>
        public CommerceWorker()
        {
            ExemptConfigurationItems = new[] { ProcessJobsPropertyKey };
            Scheduler = PartnerFactory.Scheduler(CommerceWorkerConfig.Instance.SchedulerQueueName,
                                                 CommerceWorkerConfig.Instance.SchedulerTableName,
                                                 CommerceWorkerConfig.Instance);

            // Register Analytics Service
            Analytics.Initialize(CommerceWorkerConfig.Instance);

#if IntDebug || IntRelease
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
#endif
        }

        /// <summary>
        /// Performs worker role tasks.
        /// </summary>
        public override void Run()
        {
            RunAsync().Wait();
        }

        /// <summary>
        /// Async wrapper for Commerce Worker Role
        /// </summary>
        /// <returns>
        /// A Task to be waited on
        /// </returns>
        private async Task RunAsync()
        {
            Log.Verbose("Running Commerce Worker role.");

            Log.Verbose("Checking if we can start processing jobs ...");
            while (!ProcessJobs)
            {
                // is it fine ? too fast or slow? 
                // we will finetune this after few tries on prod.
                Thread.Sleep(CommerceWorkerConfig.Instance.ProcessingLoopPollingInterval); 
            }
           
            Log.Verbose("Entering processing loop.");

            do
            {
                try
                {
                    Thread.Sleep(CommerceWorkerConfig.Instance.ProcessingLoopPollingInterval);
                    ScheduledJobDetails jobDetails = await Scheduler.GetJobToProcessAsync();
                    if (jobDetails != null)
                    {
                        IJobRunner runner = JobRunnerFactory.Runner(
                                           jobDetails, Scheduler, CommerceWorkerConfig.Instance, Log);
                        Log.Information("Running {0} job.", jobDetails.JobType);
                        
                        Tuple<IScheduler, ScheduledJobDetails> timerState = new Tuple<IScheduler, ScheduledJobDetails>(Scheduler, jobDetails);
                        using (Timer tmr = new Timer(ExtendTimeout, timerState, WhenToExtendTimeout, WhenToExtendTimeout))
                        {
                            await runner.RunJobAsync(jobDetails);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Critical("An unknown error occurred during processing.", ex);
                }
            }
            while (ProcessJobs);

            Log.Information("Processing loop has exited.");

            // We are no longer processing jobs new now...just waiting to be killed when processing ends.
            while (!ExitRole)
            {
#if !IntDebug && !IntRelease
                Log.Information("Wating for role to exit ...");
#endif
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            
            Log.Information("Role is shutting down ...");
        }

        /// <summary>
        /// When the worker role starts, we do the following:
        /// 1. Initialize Logger to be used
        /// 2. Schedule Extract Processing job on startup if configured for.
        /// 3. Schedule Ping Job
        /// </summary>
        /// <remarks>
        /// If we schedule Extract Processing job at the same time everyday, we might get
        /// duplicate jobs to execute at the same time (which might be a problem when we have
        /// more than one instance of the role). We can revisit this later, but for now we will
        /// schedule it at the start.
        /// </remarks>
        /// <returns>
        /// boolean status about startup
        /// </returns>
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 100 * Environment.ProcessorCount;

            // Use only for debugging
            // TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("APPINSIGHTS_INSTRUMENTATIONKEY");

            // Create a CommerceLog instance to funnel log entries to the log.
            LogInitializer.CreateLogInstance(CommerceWorkerConfig.Instance.LogVerbosity,
                                             CommerceWorkerConfig.Instance.ForceEventLog, CommerceLogSource,
                                             CommerceWorkerConfig.Instance);
            Log = new CommerceLog(Guid.NewGuid(), CommerceWorkerConfig.Instance.LogVerbosity, CommerceLogSource);
            ConfigChangeHandler = new ConfigChangeHandler(Log, ExemptConfigurationItems);

            // turn off processing jobs at start
            ProcessJobs = Convert.ToBoolean(CloudConfigurationManager.GetSetting(ProcessJobsPropertyKey));
            // role should not exit even after processing jobs stop
            ExitRole = false;

            // event handlers
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            RoleEnvironment.Changed += RoleEnvironmentChanged;

            if (!string.IsNullOrEmpty(ConcurrencyMonitorConnectionString))
            {
                Log.Verbose("Initializing Jobs.");
                using (ConcurrencyMonitor monitor = new ConcurrencyMonitor(ConcurrencyMonitorConnectionString))
                {
                    monitor.InvokeWithLease(() => PartnerJobInitializer.InitializeJobs(Scheduler));
                }
                Log.Verbose("Initialized Jobs.");
            }

            return base.OnStart();
        }

        /// <summary>
        /// Delegate to extend timeout for a job under process
        /// </summary>
        /// <param name="timerState">
        /// State - tuple of scheduler and jobdetails
        /// </param>
        internal static void ExtendTimeout(object timerState)
        {
            // this runs on pooled thread
            Tuple<IScheduler, ScheduledJobDetails> state = timerState as Tuple<IScheduler, ScheduledJobDetails>;
            state.Item1.IncreaseVisibilityTimeout(state.Item2, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Changing event handler
        /// </summary>
        /// <param name="sender">
        /// Sender object
        /// </param>
        /// <param name="changingEventArgs">
        /// Chaning Event details
        /// </param>
        internal void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs changingEventArgs)
        {
            ConfigChangeHandler.RoleEnvironmentChanging(sender, changingEventArgs);
        } 

        /// <summary>
        /// Changed Event Handler
        /// </summary>
        /// <param name="sender">
        /// Sender of the event.
        /// </param>
        /// <param name="e">
        /// Chaning Event details
        /// </param>
        internal void RoleEnvironmentChanged(object sender, RoleEnvironmentChangedEventArgs e)
        {
            IDictionary<string, string> newValues = ConfigChangeHandler.RetrieveNewValues(e);

            // right now we  only care about process jobs
            // if we have more properties to care about, we should iterate the dictionary
            if (newValues.ContainsKey(ProcessJobsPropertyKey))
            {
                ProcessJobs = Convert.ToBoolean(newValues[ProcessJobsPropertyKey]);
                Log.Information("Configuration change successfully applied");
            }
        }

#if IntDebug || IntRelease
        internal static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            var result = true;
            
            if (error != SslPolicyErrors.None)
            {
                result = false;
                var host = ((HttpWebRequest)sender).Address.Host;
                if (host == "maiclqaservices.visa.com")
                {
                    var certificate2 = (X509Certificate2)certificate;
                    if (string.Equals(certificate2.SerialNumber, "0EE7905FC4FDFC8AA5B7CE6A6B7022F5", StringComparison.OrdinalIgnoreCase))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }
#endif
        /// <summary>
        /// Gets or sets the Scheduler
        /// </summary>
        internal IScheduler Scheduler { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        internal CommerceLog Log { get; set; }

        /// <summary>
        /// The name of the Commerce log source.
        /// </summary>
        internal const string CommerceLogSource = "LomoCommerceWorker";

        /// <summary>
        /// Process jobs flag.
        /// </summary>
        internal bool ProcessJobs { get; set; }

        /// <summary>
        /// Flag to exit the role from running state
        /// </summary>
        internal bool ExitRole { get; set; }

        /// <summary>
        /// Concurrency Monitor Connection String
        /// </summary>
        private string ConcurrencyMonitorConnectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.ConcurrencyLease.ConnectionString");

        /// <summary>
        /// Process Jobs property key
        /// </summary>
        private string ProcessJobsPropertyKey = "Lomo.Commerce.Worker.ProcessJobs";

        /// <summary>
        /// Exempt config on which we handle restart
        /// </summary>
        private readonly string[] ExemptConfigurationItems;

        /// <summary>
        /// Change handler
        /// </summary>
        private IConfigChangeHandler ConfigChangeHandler;

        /// <summary>
        /// At what time should we extend the timeout for a job - its taking more time to process
        /// </summary>
        private TimeSpan WhenToExtendTimeout = TimeSpan.FromSeconds(55);

    }
}