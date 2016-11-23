//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Queue scheduled job details
    /// It stores handle to the actual queue messsage
    /// </summary>
    public class AzureScheduledJobDetails : ScheduledJobDetails
    {
        /// <summary>
        /// Get Job Details from Cloud Queue Message
        /// </summary>
        /// <param name="queueMessage">
        /// Cloud Queue Message
        /// </param>
        /// <returns>
        /// Job Details
        /// </returns>
        public static ScheduledJobDetails FromCloudQueueMessage(CloudQueueMessage queueMessage)
        {
            AzureScheduledJobDetails jobDetails = JsonConvert.DeserializeObject<AzureScheduledJobDetails>(queueMessage.AsString);
            jobDetails.QueueMessage = queueMessage;

            return jobDetails;
        }

        /// <summary>
        /// Convert Job Details to Cloud Queue Message
        /// </summary>
        /// <param name="azureScheduledJobDetails">
        /// Job Details
        /// </param>
        /// <returns>
        /// Cloud Queue Message
        /// </returns>
        public static CloudQueueMessage ToCloudQueueMessage(AzureScheduledJobDetails azureScheduledJobDetails)
        {
            return azureScheduledJobDetails.QueueMessage ;
        }

        /// <summary>
        /// Gets or sets the queue message
        /// </summary>
        public CloudQueueMessage QueueMessage { get; set; }
    }
}