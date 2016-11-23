//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The worker role.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.Worker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Lomo.Logging;
    using LoMo.UserServices.DealsMailing;
    using TransactionReporting;

    /// <summary>
    /// The worker role.
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        /// <summary>
        /// The run.
        /// </summary>
        public override void Run()
        {
            Log.Info(EventCode.DealsEmailingEntryPoint, "User Services worker role entry point");

            try
            {
                MailingManager mailingManager = new MailingManager();
                var jobProcessors = mailingManager.Bootstrap(RoleEnvironment.CurrentRoleInstance.Id);
                foreach (EmailJobProcessor jobProcessor in jobProcessors)
                {
                    EmailJobProcessor emailJobProcessor = jobProcessor;
                    Task.Factory.StartNew(() => emailJobProcessor.DoWork(null))
                        .ContinueWith(task => Log.Critical(EventCode.DealsEmailingUnexpectedError, task.Exception, "Unexpected Error From Agent"), TaskContinuationOptions.OnlyOnFaulted);
                }

                //Start the Email job fetcher
                JobFetcher.Start();

                ReportingManager reportingManager = new ReportingManager();
                var transactionJobProcessors = reportingManager.Bootstrap(RoleEnvironment.CurrentRoleInstance.Id);
                foreach (TransactionJobProcessor jobProcessor in transactionJobProcessors)
                {
                    TransactionJobProcessor transactionJobProcessor = jobProcessor;
                    Task.Factory.StartNew(() => transactionJobProcessor.DoWork(null))
                        .ContinueWith(task => Log.Critical(EventCode.DealsEmailingUnexpectedError, task.Exception, "Unexpected Error From Agent"), TaskContinuationOptions.OnlyOnFaulted);
                }
            }
            catch (Exception exception)
            {
                Log.Critical(EventCode.DealsEmailingInitializeError, exception, "Couldn't initialize user services worker role");
                
                // Sleep in order to make sure that the exception is being written to the logs
                Thread.Sleep(TimeSpan.FromSeconds(60));
                throw;
            }

            // Initalize complete - keep the role alive
            while (true)
            {
                Thread.Sleep(10000);
                Log.Verbose("User Services worker role running");
            }
        }

        /// <summary>
        /// The on start.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool OnStart()
        {
            Log.Instance = new TraceLog(new List<TraceListener> { new DiagnosticMonitorTraceListener { Name = "AzureDiagnostics", Filter = new EventTypeFilter(SourceLevels.All) } });
            
            Log.Info(EventCode.DealsEmailingStarted, "Deals Emailing Role Started");

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            return base.OnStart();
        }

        /// <summary>
        /// The on stop.
        /// </summary>
        public override void OnStop()
        {
            JobFetcher.Stop();
            Log.Info(EventCode.DealsEmailingStopped, "Deals Emailing Role Stopped");
            base.OnStop();
        }
    }
}