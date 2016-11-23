//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;

    /// <summary>
    /// Job to apply a reward for a redemption.
    /// </summary>
    public class ApplyRedemptionRewardJob : IOrchestratedJob
    {
        /// <summary>
        /// Initializes a new instance of the ApplyRedemptionRewardJob class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ApplyRedemptionRewardJob(CommerceLog log)
        {
            Log = log;
        }

        /// <summary>
        /// Initializes the IOrchestratedJob instance
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the job being run.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler managing the jobs.
        /// </param>
        public void Initialize(ScheduledJobDetails jobDetails,
                               IScheduler scheduler)
        {
            JobDetails = jobDetails;
            Scheduler = scheduler;
        }

        /// <summary>
        /// Gets a value that indicates whether the tasks and child jobs within the orchestrated job will be run asynchronously
        /// </summary>
        public bool Asynchronous
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether tasks are executed before child jobs
        /// </summary>
        /// <remarks>
        /// This property is ignored when Asynchronous is True.
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
        /// Non-terminal error was encountered when gathering Tasks. Exception message will contain a structured error code.
        /// </exception>
        public Collection<IOrchestratedTask> Tasks
        {
            get
            {
                if (tasksGathered == false)
                {
                    if (ExtractPayload() == true)
                    {
                        Log.Verbose("Getting reward payout record and list of eligible users for reward payout record with ID " +
                                    "{0} and card with partner card ID {1}.", RewardPayoutId, PartnerCardId);
                        CommerceContext context = new CommerceContext("Getting reward payout record and list of eligible users.",
                                                                      CommerceWorkerConfig.Instance);
                        context[Key.RewardPayoutId] = RewardPayoutId;
                        context[Key.PartnerCardId] = PartnerCardId;
                        context[Key.PartnerRedeemedDealId] = PartnerRedeemedDealId;
                        context[Key.RewardId] = RewardId;
                        IRewardOperations rewardOperations = CommerceOperationsFactory.RewardOperations(context);
                        ResultCode resultCode = rewardOperations.RetrieveUnprocessedRedemptionReward();
                        if (resultCode == ResultCode.Success)
                        {
                            // Get the reward payout record.
                            RewardPayoutRecord rewardPayoutRecord = context[Key.RewardPayoutRecord] as RewardPayoutRecord;
                            if (rewardPayoutRecord != null)
                            {
                                // Add a job to process the reward. In order to ensure the payout status is updated, the task is
                                // created even when it's already known that the reward cannot be paid out.
                                Log.Verbose("Creating tasks to perform reward payouts.");
                                tasks = new Collection<IOrchestratedTask>();
                                ApplyRewardTask task = new ApplyRewardTask(rewardPayoutRecord, Guid.Empty.ToString(), Log);
                                task.Initialize(JobDetails, Scheduler);
                                tasks.Add(task);
                            }
                        }
                        else
                        {
                            Log.Warning("{0} call unsuccessfully processed.\r\n\r\nResultCode: {1}\r\n\r\nExplanation: {2}",
                                        (int)resultCode, context.ApiCallDescription, resultCode,
                                        ResultCodeExplanation.Get(resultCode));
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
        /// <exception cref="InvalidOperationException">
        /// Non-terminal error was encountered when gathering ChildJobs. Exception message will contain a structured error code.
        /// </exception>
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

            // Extract the reward payout ID.
            string rewardPayoutIdKey = Key.RewardPayoutId.ToString();
            if (JobDetails.Payload.ContainsKey(rewardPayoutIdKey) == true)
            {
                RewardPayoutId = new Guid(JobDetails.Payload[rewardPayoutIdKey]);
            }
            else
            {
                Log.Error("ApplyRedemptionRewardJob Payload does not contain a reward payout ID.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the partner card ID.
            string partnerCardIdKey = Key.PartnerCardId.ToString();
            if (JobDetails.Payload.ContainsKey(partnerCardIdKey) == true)
            {
                PartnerCardId = JobDetails.Payload[partnerCardIdKey];
            }
            else
            {
                Log.Error("ApplyRedemptionRewardJob Payload does not contain a partner card ID.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the partner redeemed deal ID.
            string partnerRedeemedDealIdKey = Key.PartnerRedeemedDealId.ToString();
            if (JobDetails.Payload.ContainsKey(partnerRedeemedDealIdKey) == true)
            {
                PartnerRedeemedDealId = JobDetails.Payload[partnerRedeemedDealIdKey];
            }
            else
            {
                Log.Error("ApplyRedemptionRewardJob Payload does not contain a partner redeemed deal ID.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the reward ID.
            string rewardIdKey = Key.RewardId.ToString();
            if (JobDetails.Payload.ContainsKey(rewardIdKey) == true)
            {
                RewardId = new Guid(JobDetails.Payload[rewardIdKey]);
            }
            else
            {
                Log.Error("ApplyRedemptionRewardJob Payload does not contain a reward ID.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the ID of the reward payout record.
        /// </summary>
        private Guid RewardPayoutId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID of the card used in the redemption.
        /// </summary>
        private string PartnerCardId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID of the redemption event.
        /// </summary>
        private string PartnerRedeemedDealId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the reward being processed.
        /// </summary>
        private Guid RewardId { get; set; }

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
    }
}