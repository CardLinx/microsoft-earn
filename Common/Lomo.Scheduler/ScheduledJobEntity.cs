//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;

    /// <summary>
    /// Wrapper for Scheduled Job details stored in Table Storage
    /// </summary>
    public class ScheduledJobEntity : TableEntity
    {
        /// <summary>
        /// Contruct a new Instance of the Entity
        /// </summary>
        /// <param name="jobDetails">
        /// Details of the job
        /// </param>
        public ScheduledJobEntity(ScheduledJobDetails jobDetails)
        {
            RowKey = GetRowKey(jobDetails.JobId);
            PartitionKey = GetPartitionKey(jobDetails.JobType);
            JobDescription = jobDetails.JobDescription;
            Timestamp = DateTime.UtcNow;
            StartTime = jobDetails.StartTime;
            Recurrence = JsonConvert.SerializeObject(jobDetails.Recurrence);
            Version = jobDetails.Version;
            if (jobDetails.Payload != null)
            {
                Payload = JsonConvert.SerializeObject(jobDetails.Payload);
            }
        }

        /// <summary>
        /// Do not call. 
        /// Intended exclusively for use by deserialization.
        /// </summary>
        public ScheduledJobEntity()
        {
        }

        /// <summary>
        /// Get partition key for the Entity
        /// </summary>
        /// <param name="scheduledJobType">
        /// Type of the job
        /// </param>
        /// <returns>
        /// partition key
        /// </returns>
        public static string GetPartitionKey(ScheduledJobType scheduledJobType)
        {
            return Enum.GetName(typeof (ScheduledJobType), scheduledJobType);
        }

        /// <summary>
        /// Get row key for the Entity
        /// </summary>
        /// <param name="jobId">
        /// Id of the job
        /// </param>
        /// <returns>
        /// row key
        /// </returns>
        public static string GetRowKey(Guid jobId)
        {
            return jobId.ToString();
        }

        /// <summary>
        /// Gets or sets the JobDescription of the Job
        /// </summary>
        public string JobDescription { get; set; }

        /// <summary>
        /// Gets or sets the State of the Job
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the Version of the Job
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the Start Time of the job
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the Last Run Time of the job
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// Gets or sets the Count; number of times the job has run
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the Recurrence Schedule
        /// </summary>
        public string Recurrence { get; set; }

        /// <summary>
        /// Gets or sets the job payload.
        /// </summary>
        public string Payload { get; set; }
    }
}