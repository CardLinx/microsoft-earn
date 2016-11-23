//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.Collections.Concurrent;

    /// <summary>
    /// Representation of the message we store in the Scheduler queue.
    /// </summary>
    public class ScheduledJobDetails
    {
        /// <summary>
        /// Gets or sets JobId
        /// </summary>
        [JsonProperty(PropertyName = "jobId")]
        public Guid JobId { get; set; }

        /// <summary>
        /// Gets or sets JobType
        /// </summary>
        [JsonProperty(PropertyName = "jobType")]
        public ScheduledJobType JobType { get; set; }

        /// <summary>
        /// Gets or sets JobDescription
        /// </summary>
        [JsonProperty(PropertyName = "jobDescription")]
        public string JobDescription { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether job execution will be orchestrated by a processor
        /// </summary>
        [JsonProperty(PropertyName = "orchestrated")]
        public bool Orchestrated { get; set; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public int Version { get; internal set; }

        /// <summary>
        /// Gets or sets the Recurrence
        /// </summary>
        [JsonProperty(PropertyName = "recurrence")]
        public Recurrence Recurrence { get; set; }

        /// <summary>
        /// Gets or sets the job start time
        /// </summary>
        [JsonProperty(PropertyName = "startTime")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the job state
        /// </summary>
        [JsonProperty(PropertyName = "jobState", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(ScheduledJobState.NotSpecified)]
        public ScheduledJobState JobState { get; set; }

        /// <summary>
        /// Gets or sets the job payload.
        /// </summary>
        /// <remarks>
        /// Objects can be stored in the payload if JSON-serialized.
        /// </remarks>
        [JsonProperty(PropertyName = "payload")]
        public IDictionary<string, string> Payload { get; set; }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns>
        /// Details of the jobs
        /// </returns>
        public override string ToString()
        {
            return string.Format(" Id : {0} \r\n" +
                                 " Type : {1} \r\n" +
                                 " Recurrence : {2} \r\n" +
                                 " Start time : {3}", JobId, JobType.ToString(), Recurrence, StartTime);
        }

        /// <summary>
        /// Validation logic on creating new job
        /// </summary>
        /// <param name="errorMessage">
        /// The message that should be returned to the callee
        /// </param>
        /// <returns>
        /// True/False indicating whether validation was successful or not
        /// </returns>
        internal bool Validate(out string errorMessage)
        {
            if (JobId == Guid.Empty)
            {
                errorMessage = "JobId must be non Empty Guid.";
                return false;
            }

            if (JobState != ScheduledJobState.NotSpecified)
            {
                errorMessage = "Job state cannot be " + JobState + " when scheduling a new job.";
                return false;
            }

            if (!Recurrence.Validate(out errorMessage))
            {
                return false;
            }
            
            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Validation logic on update job
        /// </summary>
        /// <param name="errorMessage">
        /// The message that should be returned to the callee
        /// </param>
        /// <returns>
        /// True/False indicating whether validation was successful or not
        /// </returns>
        internal bool ValidateUpdate(out string errorMessage)
        {
            if(JobState == ScheduledJobState.NotSpecified ||
                JobState  == ScheduledJobState.Completed)
            {
                errorMessage = "Job cannot be updated to " + JobState + " State.";
                return false;
            }

            if (Recurrence == null)
            {
                errorMessage = "Recurrence cannot be null";
                return false;
            }

            if (!Recurrence.Validate(out errorMessage))
            {
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}