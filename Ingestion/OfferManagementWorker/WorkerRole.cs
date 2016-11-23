//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Azure.Utils;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Lomo.Logging;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using OfferManagement.JobProcessor;

namespace OfferManagementWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private AzureQueue azureQueue;

        private const string EarnStorageConnectionString = "EarnStorageConnectionString";
        private const string EarnJobsQueueName = "EarnJobsQueueName";
        private const string MaxThreads = "MaxThreads";

        public override void Run()
        {
            Log.Verbose("OfferManagementWorker is running");

            try
            {
                Log.Verbose("Instantiating the job queue");
                string queueConnectionString = CloudConfigurationManager.GetSetting(EarnStorageConnectionString);
                string queueName = CloudConfigurationManager.GetSetting(EarnJobsQueueName);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(queueConnectionString);
                azureQueue = new AzureQueue(storageAccount, queueName);
                JobManager.Start();
                
                int maxThreads = 5;
                if (!int.TryParse(CloudConfigurationManager.GetSetting(MaxThreads), out maxThreads))
                {
                    Log.Warn($"MaxThreads has an invalid value {CloudConfigurationManager.GetSetting(MaxThreads)} in the config. Default value of 5 will be used");
                }
                
                Log.Info($"MaxThreads is set to {maxThreads}");
                Parallel.For(0, 5, (i) => { this.RunAsync(this.cancellationTokenSource.Token).Wait(); });
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error in running earn jobs role");
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            Log.Instance = new TraceLog(new List<TraceListener> { new DiagnosticMonitorTraceListener { Name = "AzureDiagnostics", Filter = new EventTypeFilter(SourceLevels.All) } });

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Log.Verbose("OfferManagementWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Log.Verbose("OfferManagementWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Log.Verbose("OfferManagementWorker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Log.Verbose("Checking for scheduled jobs in the earn jobs queue...");
                try
                {
                    CloudQueueMessage message = await azureQueue.DequeueMessageAsync().ConfigureAwait(false);
                    if (message != null)
                    {
                        string messageInfo = message.AsString;
                        await azureQueue.DeleteMessageAsync(message.Id, message.PopReceipt).ConfigureAwait(false);
                        XDocument xDoc = XDocument.Parse(messageInfo);
                        var messageEement = xDoc.Descendants().FirstOrDefault(d => d.Name == "Message");
                        if (messageEement != null)
                        {
                            ScheduledJobInfo scheduledJobInfo =
                                JsonConvert.DeserializeObject<ScheduledJobInfo>(messageEement.Value);
                            Log.Info("JobType {0} found in the queue", scheduledJobInfo.JobType.ToString());
                            IScheduledJob scheduledJob = JobManager.GetJobByType(scheduledJobInfo.JobType);
                            JobManager.DeleteJob(scheduledJobInfo.JobId);
                            Log.Info($"Deleted the azure scheduler job {scheduledJobInfo.JobId}");
                            var executeAsync = scheduledJob?.ExecuteAsync(scheduledJobInfo);
                            if (executeAsync != null)
                                await executeAsync;

                            Log.Info("Finished executing the earn job : {0}", scheduledJobInfo.ToString());
                        }
                        else
                        {
                            Log.Error("Invalid Payload in the job message : {0}", messageInfo);
                        }
                    }
                    else
                    {
                        Log.Info("No job found in the earn jobs queue...");
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Error in processing the earn job");
                }
                
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }
}