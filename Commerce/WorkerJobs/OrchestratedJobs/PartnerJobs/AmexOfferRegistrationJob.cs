//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Lomo.Commerce.AmexWorker;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;

    /// <summary>
    /// Orchestrated job to upload offer registration detail to blob store
    /// Important: This job just uploads the details to the blob and those contents will be picked up and 
    /// combined to form a file to be sent to Amex later by another job.
    /// </summary>
    public class AmexOfferRegistrationJob : IOrchestratedJob
    {
        /// <summary>
        /// Initializes a new instance of the AmexOfferRegistrationJob class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public AmexOfferRegistrationJob(CommerceLog log)
        {
            Log = log;
        }

        /// <summary>
        /// Initializes the AmexOfferRegistrationJob instance.
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the job being run.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler managing the jobs.
        /// </param>
        public void Initialize(ScheduledJobDetails jobDetails, IScheduler scheduler)
        {
            JobDetails = jobDetails;
            Scheduler = scheduler;
        }


        /// <summary>
        /// Gets or sets a value that indicates whether the work items within the orchestrated job will be run asynchronously.
        /// </summary>
        /// <remarks>
        /// Only 1 task has to be done for this job, so no async
        /// </remarks>
        public bool Asynchronous
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether tasks are executed before child jobs.
        /// </summary>
        /// <remarks>
        /// This job contains no child jobs, so tasks will be run first.
        /// </remarks>
        public bool TasksFirst
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the collection of tasks belonging to this job
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Non-terminal error was encountered when gathering Tasks. Exception message will contain a ResultCode.
        /// </exception>
        public Collection<IOrchestratedTask> Tasks
        {
            get
            {
                if (tasksGathered == false)
                {
                    if (ExtractPayload() == true)
                    {
                        // Call the MBI endpoint to get the active discounts
                        Log.Verbose("Starting task to upload offer data to blob store");
                        tasks = new Collection<IOrchestratedTask>();
                        string[] merchantIds = AmexMerchantIds.Split(',');
                        foreach (string merchantId in merchantIds)
                        {
                            string slimMerchantId = merchantId.Trim();
                            string offerRecord = null;
                            if (!UploadedMerchantIds.Contains(slimMerchantId))
                            {
                                if (Action == "Add")
                                {
                                    offerRecord = OfferRegistrationRecordBuilder.BuildAddOffer(DiscountSummary, DiscountStartDate, DiscountEndDate, slimMerchantId, AmexMerchantName);
                                }
                                else
                                {
                                    offerRecord = OfferRegistrationRecordBuilder.BuildUpdateOffer(DiscountSummary, DiscountStartDate, DiscountEndDate, slimMerchantId, AmexMerchantName);
                                }
                                AmexOfferBlobUploadTask task = new AmexOfferBlobUploadTask(offerRecord, slimMerchantId, Log);
                                task.Initialize(JobDetails, Scheduler);
                                tasks.Add(task);
                            }
                        }
                        
                        tasksGathered = true;
                       
                    }
                    else
                    {
                        tasksGathered = true;
                    }
                }

                return tasks;
            }
        }
        private Collection<IOrchestratedTask> tasks = null;
        private bool tasksGathered = false;

        /// <summary>
        /// Gets the collection of child jobs belonging to this job
        /// </summary>
        /// <remarks>
        /// There are no child jobs for this job.
        /// </remarks>
        public Collection<IOrchestratedJob> ChildJobs
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Executes job startup tasks.
        /// </summary>
        /// <returns>
        /// The result of the execution of the startup tasks.
        /// </returns>
        public OrchestratedExecutionResult StartUp()
        {
            return OrchestratedExecutionResult.Success;
        }
        /// <summary>
        /// Executes job tear down tasks.
        /// </summary>
        /// <param name="executionResult">
        /// The result of job startup tasks.
        /// </param>
        /// <returns>
        /// The result of the execution of the tear down tasks.
        /// </returns>
        public OrchestratedExecutionResult TearDown(OrchestratedExecutionResult executionResult)
        {
            return OrchestratedExecutionResult.Success;
        }

        /// <summary>
        /// Extracts needed information from the payload and logs any errors encountered while doing so.
        /// </summary>
        /// <returns>
        /// * True if the needed information was extracted successfully.
        /// * Else returns false.
        /// </returns>
        private bool ExtractPayload()
        {
            bool result = true;

            // Extract the amex merchant id (s) : comma seperated if multiple
            string amexMerchantId = Key.PartnerMerchantId.ToString();
            if (JobDetails.Payload.ContainsKey(amexMerchantId))
            {
                AmexMerchantIds = JobDetails.Payload[amexMerchantId];
            }
            else
            {
                Log.Error("AmexOfferRegistrationJob Payload does not contain AMEX Merchant SE ID", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the amex merchant name.
            string amexMerchantName = Key.MerchantName.ToString();
            if (JobDetails.Payload.ContainsKey(amexMerchantName))
            {
                AmexMerchantName = JobDetails.Payload[amexMerchantName];
            }
            else
            {
                Log.Error("AmexOfferRegistrationJob Payload does not contain AMEX Merchant Name", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the discount start date.
            string discountStartDateKey = Key.DiscountStartDate.ToString();
            if (JobDetails.Payload.ContainsKey(discountStartDateKey))
            {
                // DiscountStartDate = DateTime.Parse(JobDetails.Payload[discountStartDateKey]);
                DiscountStartDate = DateTime.Now; // If it is in past, Amex creeps out. So this hack is there
            }
            else
            {
                Log.Error("AmexOfferRegistrationJob Payload does not contain the start date of discount being added.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the discount end date.
            string discountEndDateKey = Key.DiscountEndDate.ToString();
            if (JobDetails.Payload.ContainsKey(discountEndDateKey))
            {
                DiscountEndDate = DateTime.Parse(JobDetails.Payload[discountEndDateKey]);
            }
            else
            {
                Log.Error("AmexOfferRegistrationJob Payload does not contain the end date of discount being added.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the discount description
            string discountSummaryKey = Key.DealDiscountSummary.ToString();
            if (JobDetails.Payload.ContainsKey(discountSummaryKey))
            {
                DiscountSummary = JobDetails.Payload[discountSummaryKey];
            }
            else
            {
                Log.Error("AmexOfferRegistrationJob Payload does not contain the summary of discount being added.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract action
            string actionKey = Key.DealAction.ToString();
            if (JobDetails.Payload.ContainsKey(actionKey))
            {
                Action = JobDetails.Payload[actionKey];
            }
            else
            {
                Log.Error("AmexOfferRegistrationJob Payload does not contain the action being done for the deal.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Gets the merchant Ids for which the offer has already been uploaded to blobstore
        /// </summary>
        private IEnumerable<string> UploadedMerchantIds
        {
            get
            {
                if (uploadedMerchantIds == null)
                {
                    Collection<string> ids = new Collection<string>();
                    foreach (string key in JobDetails.Payload.Keys)
                    {
                        if (JobDetails.Payload[key] == AmexOfferBlobUploadTask.AmexMerchantIdMarker)
                        {
                            ids.Add(key);
                        }
                    }
                    uploadedMerchantIds = ids;
                }
                return uploadedMerchantIds;
            }
        }
        private IEnumerable<string> uploadedMerchantIds = null;

        /// <summary>
        /// Gets or sets the AMEX specific merchant Ids
        /// </summary>
        private string AmexMerchantIds { get; set; }

        /// <summary>
        /// Gets or sets the AMEX specific merchant name
        /// </summary>
        private string AmexMerchantName { get; set; }

        /// <summary>
        /// Gets or sets the discount start date
        /// </summary>
        private DateTime DiscountStartDate { get; set; }

        /// <summary>
        /// Gets or sets the discount end date
        /// </summary>
        private DateTime DiscountEndDate { get; set; }

        /// <summary>
        /// Gets or sets the  discount summary
        /// </summary>
        private string DiscountSummary { get; set; }
        
        /// <summary>
        /// Gets or sets the details of the job being run.
        /// </summary>
        private ScheduledJobDetails JobDetails { get; set; }

        /// <summary>
        /// Gets or sets the scheduler managing the job.
        /// </summary>
        private IScheduler Scheduler { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// Gets or sets Action to be performed
        /// </summary>
        private string Action { get; set; }
    }
}