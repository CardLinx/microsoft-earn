//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Hyak.Common;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Scheduler;
using Microsoft.WindowsAzure.Scheduler.Models;

namespace Azure.Utils
{
    public class AzureScheduler
    {
        private readonly CertificateCloudCredentials cloudCredentials;

        private readonly string cloudServiceName;

        private readonly string jobCollectionName;

        public AzureScheduler(X509Certificate2 subscriptionCertificate, string subscriptionId,
            string cloudServiceName, string jobCollectionName)
        {
            cloudCredentials = new CertificateCloudCredentials(subscriptionId, subscriptionCertificate);
            this.cloudServiceName = cloudServiceName;
            this.jobCollectionName = jobCollectionName;
        }

        public async Task<HttpStatusCode> ScheduleQueueTypeJobAsync(string accountName, string queueName, string sasToken, 
            string payload, string jobId)
        {
            SchedulerClient schedulerClient = new SchedulerClient(cloudServiceName, jobCollectionName, cloudCredentials);
            var jobAction = new JobAction()
            {
                Type = JobActionType.StorageQueue,
                QueueMessage = new JobQueueMessage
                {
                    StorageAccountName = accountName,
                    QueueName = queueName,
                    SasToken = sasToken,
                    Message = payload
                }
            };
            var jobCreateOrUpdateParameters = new JobCreateOrUpdateParameters(jobAction);
            JobCreateOrUpdateResponse jobCreateResponse =
                await schedulerClient.Jobs.CreateOrUpdateAsync(jobId, jobCreateOrUpdateParameters).ConfigureAwait(false);
            
            return jobCreateResponse.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteJob(string jobId)
        {
            SchedulerClient schedulerClient = new SchedulerClient(cloudServiceName, jobCollectionName, cloudCredentials);
            AzureOperationResponse jobResponse = await schedulerClient.Jobs.DeleteAsync(jobId);

            return jobResponse.StatusCode;
        }

        public async Task<Job> GetJob(string jobId)
        {
            SchedulerClient schedulerClient = new SchedulerClient(cloudServiceName, jobCollectionName, cloudCredentials);
            Job job = null;
            try
            {
                JobGetResponse jobResponse = await schedulerClient.Jobs.GetAsync(jobId);
                job = jobResponse.Job;
            }
            catch (Exception exception)
            {
                CloudException cloudException = exception as CloudException;
                if (cloudException != null && cloudException.Response.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return job;
        }

        public async Task<IList<Job>> GetJobs()
        {
            SchedulerClient schedulerClient = new SchedulerClient(cloudServiceName, jobCollectionName, cloudCredentials);
            JobListResponse jobResponse = await schedulerClient.Jobs.ListAsync(new JobListParameters());

            return jobResponse.Jobs;
        }
    }
}