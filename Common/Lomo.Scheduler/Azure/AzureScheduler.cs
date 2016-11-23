//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Lomo.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;

    /// <summary>
    /// Scheduler based on Azure Storage to schedule jobs and to process them.
    /// </summary>
    public class AzureScheduler : IScheduler
    {
        /// <summary>
        /// Creates an instance of the scheduler.
        /// Azure scheduler needs 2 things:
        /// 1. Azure Queue
        /// 2. Azure Table
        /// 
        /// Queues are used to send messages which arrive at scheduled time.
        /// Tables are used to manage state and versions.
        /// </summary>
        /// <param name="connectionString">
        /// Azure Storage Connection String
        /// </param>
        /// <param name="queueName">
        /// Name of the Queue
        /// </param>
        /// <param name="tableName">
        /// Name of the Table
        /// </param>
        internal AzureScheduler(string connectionString, string queueName, string tableName)
        {
            azureQueueProvider = new AzureQueueProvider(connectionString, queueName);
            azureTableProvider = new AzureTableProvider(connectionString, tableName);
        }

        /// <summary>
        /// Schedules a job of a given type 
        /// </summary>
        /// <param name="jobDetails">
        /// Details of the job to schedule <see cref="ScheduledJobDetails"/>
        /// </param>
        /// <returns>
        /// Async Task handler
        /// </returns>
        public async Task ScheduleJobAsync(ScheduledJobDetails jobDetails)
        {
            ValidateAndPopulateNewJobDetails(jobDetails);
            
            TimeSpan whenToRun = WhenToSchedule(jobDetails);
            jobDetails.JobState = ScheduledJobState.Running;
            string serializedJob = JsonConvert.SerializeObject(jobDetails);
            
            Log.Info("Incoming request to scheduler a new job \r\n " +
                     "details : {0}", serializedJob);

            // put the job in table
            ScheduledJobEntity jobEntity = new ScheduledJobEntity(jobDetails);
            jobEntity.State = Enum.GetName(typeof(ScheduledJobState), ScheduledJobState.Running);
            Log.Verbose("About to insert entity for Job {0}", jobDetails.JobId);
            await azureTableProvider.InsertAsync(jobEntity).ConfigureAwait(false);
            Log.Verbose("Insert Done for job {0}", jobDetails.JobId);

            // send message to queue
            Log.Verbose("About to enqueue new job {0}", jobDetails.JobId);
            await azureQueueProvider.EnqueueAsync(new CloudQueueMessage(serializedJob), whenToRun).ConfigureAwait(false);
            Log.Verbose("Enqueue done for job {0}", jobDetails.JobId);
            
            Log.Info("Successfully scheduled job with id {0}", jobDetails.JobId);
        }

        
        /// <summary>
        /// Get a job off the queue to process if available
        /// </summary>
        /// <returns>
        /// Details of the job to run, or NULL if none present
        /// </returns>
        public async Task<ScheduledJobDetails> GetJobToProcessAsync()
        {
            // Get message from queue - timeout of 1 min
            CloudQueueMessage message = await azureQueueProvider.DequeueAsync(TimeSpan.FromMinutes(1)).ConfigureAwait(false);
            ScheduledJobDetails details = null;

//TODO: Add logic to log these every _n_ times instead of once per polling interval (currently 50ms).            
//            Log.Verbose("Incoming request to get a job to be processed");
            if (message != null)
            {
                // Get entity from table
                details = AzureScheduledJobDetails.FromCloudQueueMessage(message);
                TableResult result = await azureTableProvider.RetrieveAsync(
                                        ScheduledJobEntity.GetPartitionKey(details.JobType),
                                        ScheduledJobEntity.GetRowKey(details.JobId)).ConfigureAwait(false);

                ScheduledJobEntity entity = (ScheduledJobEntity)result.Result;
                
                if (entity == null)
                {
                    // something bad happened in scheduling
                    Log.Critical(500, "Deleting Job from queue as entity does not exist , jobId {0} ", details.JobId);
                    await DeleteJobFromQueueAsync(details).ConfigureAwait(false);
                    details = null;
                }
                else
                {
                    int currentVersion = entity.Version;

                    // if version mismatch, that means job has been updated 
                    // We should delete the current message from the Queue
                    if (currentVersion != details.Version)
                    {
                        await DeleteJobFromQueueAsync(details).ConfigureAwait(false);
                        details = null;
                    }
                }
            }

            if (details != null)
            {
                Log.Info("Job to be processed retrieved \r\n" +
                         "details : {0}", details);
            }
               
            return details;
        }

        /// <summary>
        /// Mark the job iteration as complete. 
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the job which we want to mark as complete
        /// </param>
        /// <returns>
        /// Task wrapper for async operation
        /// </returns>
        public async Task CompleteJobIterationAsync(ScheduledJobDetails jobDetails)
        {
            Log.Info("Incoming request to complete a job iteration \r\n" +
                     "details: {0}", jobDetails);

            await DeleteJobFromQueueAsync(jobDetails).ConfigureAwait(false);
            Log.Verbose("Job {0} deleted from the queue", jobDetails.JobId);

            TableResult result = await azureTableProvider.RetrieveAsync(
                                    ScheduledJobEntity.GetPartitionKey(jobDetails.JobType),
                                    ScheduledJobEntity.GetRowKey(jobDetails.JobId));

            ScheduledJobEntity entity = (ScheduledJobEntity)result.Result;
            if (entity != null)
            {
                entity.LastRunTime = DateTime.UtcNow;
                entity.Count = entity.Count + 1;
                if (jobDetails.Payload != null)
                {
                    entity.Payload = JsonConvert.SerializeObject(jobDetails.Payload);
                }
                else
                {
                    entity.Payload = null;
                }

                Recurrence scheduledRecurrence = JsonConvert.DeserializeObject<Recurrence>(entity.Recurrence);
                if (entity.Count == scheduledRecurrence.Count)
                {
                    entity.State = Enum.GetName(typeof(ScheduledJobState), ScheduledJobState.Completed);
                    Log.Verbose("Job {0} completed all scheduled runs. Will be marked complete ", jobDetails.JobId);

                    // if we are marking job as complete, delete payload. It unnecessary inflates the job size 
                    // and we don't need it.
                    // Max size for each property is 64K !
                    entity.Payload = null;
                }
                else
                {
                    // schedule next recurrence
                    AzureScheduledJobDetails details = jobDetails as AzureScheduledJobDetails;
                    if (details != null)
                    {
                        details.QueueMessage = null;
// TODO : Include StartTime in calculation ... else drift will increase
                        await azureQueueProvider.EnqueueAsync(new CloudQueueMessage(JsonConvert.SerializeObject(details)),
                            scheduledRecurrence.ToTimeSpan()).ConfigureAwait(false);
                        Log.Verbose("Job {0} completed {1} runs, scheduling the next due occurrence", jobDetails.JobId, entity.Count);
                    }
                }

                await azureTableProvider.UpdateAsync(entity).ConfigureAwait(false);
            }

            Log.Info("Successfully marked iteration of job {0} as complete", jobDetails.JobId);
        }

        /// <summary>
        /// Update the job. This will requeue the message depending on the Recurrence.
        /// Use this to:
        /// 1. Update State :
        ///     a. Set to Pause to pause a job
        ///     b. Set to Running to resume a job
        ///     c. Set to Canceled to cancel a job
        /// 2. Update payload.
        /// 3. Update Recurrence Schedule
        /// 4. Update description
        /// </summary>
        /// <param name="jobDetails">
        /// Details to be updated
        /// </param>
        /// <returns>
        /// Task wrapper for async operation
        /// </returns>
        public async Task UpdateJobAsync(ScheduledJobDetails jobDetails)
        {
            if (jobDetails == null)
            {
                throw new SchedulerException("JobDetails cannot be null");
            }

            string errorMessage;
            if (!jobDetails.ValidateUpdate(out errorMessage))
            {
                throw new SchedulerException(errorMessage);
            }

            Log.Info("Incoming request to update a job \r\n" +
                     "details: {0}", jobDetails);

            TableResult result = await azureTableProvider.RetrieveAsync(
                                    ScheduledJobEntity.GetPartitionKey(jobDetails.JobType),
                                    ScheduledJobEntity.GetRowKey(jobDetails.JobId)).ConfigureAwait(false);

            ScheduledJobEntity entity = (ScheduledJobEntity)result.Result;

            if (entity != null)
            {   
                // start time cannot be changed.
                // If we need to change start time, cancel this job and create a new one.
                jobDetails.StartTime = entity.StartTime;

                // always update state
                entity.State = Enum.GetName(typeof(ScheduledJobState), jobDetails.JobState);
                
                // always increment version number
                entity.Version = entity.Version + 1;
                
                // update the payload if needed
                if (jobDetails.Payload != null)
                {
                    entity.Payload = JsonConvert.SerializeObject(jobDetails.Payload);
                }
                else
                {
                    entity.Payload = null;
                }

                // update recurrence if needed
                if (jobDetails.Recurrence != null)
                {
                    entity.Recurrence = JsonConvert.SerializeObject(jobDetails.Recurrence);
                }

                // udate description
                entity.JobDescription = jobDetails.JobDescription;

                //update table
                await azureTableProvider.UpdateAsync(entity).ConfigureAwait(false);

                //queue a new message with updated version if the status is supposed to be Running
                if (jobDetails.JobState == ScheduledJobState.Running)
                {
                    // when should we run the job
                    TimeSpan whenToRun = jobDetails.Recurrence.ToTimeSpan();
                    jobDetails.Version = entity.Version;

                    await azureQueueProvider.EnqueueAsync(
                        new CloudQueueMessage(JsonConvert.SerializeObject(jobDetails)),
                        whenToRun).ConfigureAwait(false);
                }
            }

            Log.Info("Successfully updated job {0} ", jobDetails.JobId);
        }

        /// <summary>
        /// Get all active (Running or Paused) jobs
        /// </summary>
        /// <param name="type">
        /// Type of jobs we want to query
        /// </param>
        /// <returns>
        /// Enumeration of all entities that match the filter
        /// </returns>
        public IEnumerable<ScheduledJobEntity> GetAllActiveJobsByType(ScheduledJobType type)
        {
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                                                                           ScheduledJobEntity.GetPartitionKey(type));

            string activeJobFilter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("State", QueryComparisons.Equal, "Running"),
                TableOperators.Or,
                TableQuery.GenerateFilterCondition("State", QueryComparisons.Equal, "Paused"));

            TableQuery<ScheduledJobEntity> query = new TableQuery<ScheduledJobEntity>().Where(
                                                        TableQuery.CombineFilters(
                                                            partitionKeyFilter,
                                                            TableOperators.And,
                                                            activeJobFilter));
            return azureTableProvider.Table.ExecuteQuery(query);
        }

        /// <summary>
        /// Get all jobs of matching on type and description in specified states
        /// </summary>
        /// <param name="type">
        /// Type of jobs we want to query
        /// </param>
        /// <param name="description">
        /// Description belonging to jobs we want to query
        /// </param>
        /// <param name="states">
        /// States job must be in to be returned by query
        /// </param>
        /// <returns>
        /// Enumeration of matching jobs
        /// </returns>
        public IEnumerable<ScheduledJobDetails> GetJobsByTypeAndDescription(ScheduledJobType type,
                                                                            string description,
                                                                            ScheduledJobState states)
        {
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                                                                           ScheduledJobEntity.GetPartitionKey(type));
            string descriptionFilter = TableQuery.GenerateFilterCondition("JobDescription", QueryComparisons.Equal, description);

            string partitionAndDescriptionFilter = TableQuery.CombineFilters(
                                                    partitionKeyFilter,
                                                    TableOperators.And,
                                                    descriptionFilter);
            IEnumerable<string> statesToQueryOn = FindAllStates(states);
            string stateFilter = null;
            foreach (string state in statesToQueryOn)
            {
                if (stateFilter == null)
                {
                    stateFilter = TableQuery.GenerateFilterCondition("State", QueryComparisons.Equal, state);
                }
                else
                {
                    stateFilter = TableQuery.CombineFilters(stateFilter,
                                                            TableOperators.Or,
                                                            TableQuery.GenerateFilterCondition("State",
                                                                                               QueryComparisons.Equal,
                                                                                               state));
                }
            }

            TableQuery<ScheduledJobEntity> query = new TableQuery<ScheduledJobEntity>().Where(
                                                        TableQuery.CombineFilters(
                                                            partitionAndDescriptionFilter,
                                                            TableOperators.And,
                                                            stateFilter));
            IEnumerable<ScheduledJobEntity> entities = azureTableProvider.Table.ExecuteQuery(query);
            Collection<ScheduledJobDetails> jobs = new Collection<ScheduledJobDetails>();
            foreach (ScheduledJobEntity scheduledJobEntity in entities)
            {
                ScheduledJobDetails details = new ScheduledJobDetails()
                {
                    JobId = new Guid(scheduledJobEntity.RowKey),
                    JobState =
                        (ScheduledJobState)Enum.Parse(typeof(ScheduledJobState), scheduledJobEntity.State),
                    JobType =
                        (ScheduledJobType)Enum.Parse(typeof(ScheduledJobType), scheduledJobEntity.PartitionKey),
                    Recurrence = JsonConvert.DeserializeObject<Recurrence>(scheduledJobEntity.Recurrence),
                    StartTime = scheduledJobEntity.StartTime,
                    Version = scheduledJobEntity.Version,
                    JobDescription = scheduledJobEntity.JobDescription
                };
                if (scheduledJobEntity.Payload != null)
                {
                    details.Payload =
                        JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(scheduledJobEntity.Payload);
                }
                jobs.Add(details);
            }
            return jobs;
        }

        /// <summary>
        /// Get Job Details by Job Id
        /// </summary>
        /// <param name="jobId">
        /// JobId of the job
        /// </param>
        /// <returns>
        /// Job Details if job found else null
        /// </returns>
        public ScheduledJobDetails GetJobById(Guid jobId)
        {
            string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal,
                                                                           ScheduledJobEntity.GetRowKey(jobId));
            TableQuery<ScheduledJobEntity> query = new TableQuery<ScheduledJobEntity>().Where(rowKeyFilter);
            IEnumerable<ScheduledJobEntity> entities = azureTableProvider.Table.ExecuteQuery(query);
            foreach (ScheduledJobEntity scheduledJobEntity in entities)
            {
                ScheduledJobDetails details = new ScheduledJobDetails()
                    {
                        JobId = new Guid(scheduledJobEntity.RowKey),
                        JobState = 
                            (ScheduledJobState) Enum.Parse(typeof (ScheduledJobState), scheduledJobEntity.State),
                        JobType =
                            (ScheduledJobType) Enum.Parse(typeof (ScheduledJobType), scheduledJobEntity.PartitionKey),
                        Recurrence = JsonConvert.DeserializeObject<Recurrence>(scheduledJobEntity.Recurrence),
                        StartTime = scheduledJobEntity.StartTime,
                        Version = scheduledJobEntity.Version
                    };
                if (scheduledJobEntity.Payload != null)
                {
                    details.Payload =
                        JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(scheduledJobEntity.Payload);
                }
                return details;
            }

            return null;
        }

        /// <summary>
        /// Update the job payload to the new payload specified
        /// </summary>
        /// <param name="jobDetails">
        /// Details of the job to be updated
        /// </param>
        /// <returns>
        /// Task wrapper for async operation
        /// </returns>
        /// <remarks>
        /// This update is done in place, hence queue order is not changed.
        /// Please be aware that this call only updates the payload.
        /// Imp : NOT THREADSAFE. So multiple calls can arrive out of order
        /// to make payload be off sync. Be careful if you have to use it.
        /// </remarks>
        public async Task UpdateJobPayload(ScheduledJobDetails jobDetails)
        {
            if (jobDetails == null)
            {
                throw new SchedulerException("JobDetails cannot be null");    
            }

            string errorMessage;
            if (!jobDetails.ValidateUpdate(out errorMessage))
            {
                throw new SchedulerException(errorMessage);
            }

            Log.Info("Incoming request to update payload of a job \r\n" +
                     "details: {0}", jobDetails);

            TableResult result = await azureTableProvider.RetrieveAsync(
                                    ScheduledJobEntity.GetPartitionKey(jobDetails.JobType),
                                    ScheduledJobEntity.GetRowKey(jobDetails.JobId)).ConfigureAwait(false);

            ScheduledJobEntity entity = (ScheduledJobEntity)result.Result;

            if (entity != null)
            {
                entity.Payload = JsonConvert.SerializeObject(jobDetails.Payload);
                await azureTableProvider.UpdateAsync(entity).ConfigureAwait(false);

                AzureScheduledJobDetails azureJobDetails = jobDetails as AzureScheduledJobDetails;
                CloudQueueMessage message = AzureScheduledJobDetails.ToCloudQueueMessage(azureJobDetails);
                // azureJobDetails.QueueMessage = null;
                message.SetMessageContent(JsonConvert.SerializeObject(jobDetails));
                await azureQueueProvider.UpdateAsync(message).ConfigureAwait(false);
            }

            Log.Info("Successfully updated payload of job {0} ", jobDetails.JobId);
        }

        /// <summary>
        /// Use this to increase visibility timeout of a job which might take longer to process
        /// </summary>
        /// <param name="jobDetails">
        /// Job Details
        /// </param>
        /// <param name="newTimeout">
        /// What should be the new timeout
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async Task IncreaseVisibilityTimeout(ScheduledJobDetails jobDetails, TimeSpan newTimeout)
        {
            if (jobDetails == null)
            {
                throw new SchedulerException("JobDetails cannot be null");
            }

            Log.Info("Updating timeout of job {0} to {1} ", jobDetails.JobId, newTimeout.ToString());
            AzureScheduledJobDetails azureJobDetails = jobDetails as AzureScheduledJobDetails;
            CloudQueueMessage message = AzureScheduledJobDetails.ToCloudQueueMessage(azureJobDetails);
            await azureQueueProvider.IncreaseTimeout(message, newTimeout).ConfigureAwait(false);
            Log.Info("Successfully updated timeout of job {0} ", jobDetails.JobId);
        }

        /// <summary>
        /// Calculates when to schedule the job to run
        /// </summary>
        /// <param name="jobDetails">
        /// Details of the job
        /// </param>
        /// <returns>
        /// Time when to schedule
        /// </returns>
        internal static TimeSpan WhenToSchedule(ScheduledJobDetails jobDetails)
        {
            TimeSpan whenToRun = jobDetails.Recurrence.ToTimeSpan();
            if (jobDetails.StartTime != null)
            {
                TimeSpan? diff = jobDetails.StartTime - DateTime.UtcNow;
                if (diff.Value.TotalSeconds > 0)
                {
                    whenToRun = whenToRun.Add(diff.Value);
                }
            }
            else
            {
                jobDetails.StartTime = DateTime.UtcNow;
            }
            return whenToRun;
        }


        /// <summary>
        /// Testing hook for Cleanup
        /// </summary>
        internal static void DeleteQueueAndTable(string connectionString, string queueName, string tableName)
        {
            AzureTableProvider.DeleteTable(connectionString, tableName);
            AzureQueueProvider.DeleteQueue(connectionString, queueName);
        }

        /// <summary>
        /// If Recurrence is not specified, we create a Run Once job
        /// </summary>
        /// <param name="details">
        /// Scheduled job details
        /// </param>
        private void CreateRecurrenceIfNotExist(ScheduledJobDetails details)
        {
            if (details.Recurrence == null)
            {
                details.Recurrence = new Recurrence()
                    {
                        Count = 1,
                        Frequency = RecurrenceFrequency.None
                    };
            }
        }

        /// <summary>
        /// Validates that job details are valid 
        /// and sets recurrence properly if needed
        /// </summary>
        /// <param name="jobDetails"></param>
        private void ValidateAndPopulateNewJobDetails(ScheduledJobDetails jobDetails)
        {
            if (jobDetails == null)
            {
                throw new SchedulerException("Parameter jobDetails cannot be null");
            }

            CreateRecurrenceIfNotExist(jobDetails);

            if (jobDetails.Recurrence.Count == 0)
            {
                jobDetails.Recurrence.Count = int.MaxValue;
            }

            string errorMessage;
            if (!jobDetails.Validate(out errorMessage))
            {
                throw new SchedulerException(errorMessage);
            }
        }

        /// <summary>
        /// Delete job from the queue
        /// </summary>
        /// <param name="jobDetails">
        /// Job Details
        /// </param>
        /// <returns>
        /// Task wrapper for async operations
        /// </returns>
        private async Task DeleteJobFromQueueAsync(ScheduledJobDetails jobDetails)
        {
            await azureQueueProvider.DeleteAsync(
                AzureScheduledJobDetails.ToCloudQueueMessage((AzureScheduledJobDetails)jobDetails)).ConfigureAwait(false);
        }

        /// <summary>
        /// Find all states that are set in flag enum to build a filter
        /// </summary>
        /// <param name="states">
        /// All set states
        /// </param>
        /// <returns>
        /// String enumeration of all states
        /// </returns>
        private IEnumerable<string> FindAllStates(ScheduledJobState states)
        {
            if ((states & ScheduledJobState.Running) == ScheduledJobState.Running)
                yield return "Running";

            if ((states & ScheduledJobState.Paused) == ScheduledJobState.Paused)
                yield return "Paused";

            if ((states & ScheduledJobState.Completed) == ScheduledJobState.Completed)
                yield return "Completed";

            if ((states & ScheduledJobState.Canceled) == ScheduledJobState.Canceled)
                yield return "Canceled";
        }

        /// <summary>
        /// Handle to queue provider
        /// </summary>
        internal readonly AzureQueueProvider azureQueueProvider;

        /// <summary>
        /// Handle to table provider
        /// </summary>
        internal readonly AzureTableProvider azureTableProvider;
    }
}