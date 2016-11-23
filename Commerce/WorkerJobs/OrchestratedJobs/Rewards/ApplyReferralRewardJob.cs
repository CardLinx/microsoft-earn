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
    /// Job to apply a reward for a referral.
    /// </summary>
    public class ApplyReferralRewardJob : IOrchestratedJob
    {
        /// <summary>
        /// Initializes a new instance of the ApplyReferralRewardJob class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ApplyReferralRewardJob(CommerceLog log)
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
                return true;
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
                        Log.Verbose("Getting list of referrals to process for referred user {0}.", UserId);
                        CommerceContext context = new CommerceContext("Applying referral reward job", CommerceWorkerConfig.Instance);
                        context[Key.GlobalUserId] = UserId;
                        context[Key.ReferralEvent] = ReferralEvent;
                        IReferralOperations referralOperations = CommerceOperationsFactory.ReferralOperations(context);
                        Collection<RewardPayoutRecord> rewardPayoutRecords = referralOperations.RetrieveUserUnprocessedReferrals();

                        if (rewardPayoutRecords.Any() == true)
                        {
                            Log.Verbose("Creating tasks to perform reward payouts.");
                            tasks = new Collection<IOrchestratedTask>();
                            foreach (RewardPayoutRecord rewardPayoutRecord in rewardPayoutRecords)
                            {
                                ApplyRewardTask task = new ApplyRewardTask(rewardPayoutRecord, UserId, Log);
                                task.Initialize(JobDetails, Scheduler);
                                tasks.Add(task);
                            }
                        }
                        else
                        {
                            Log.Verbose("No referral rewards to apply were found.");
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

            // Extract the user ID.
            string userIdKey = Key.GlobalUserId.ToString();
            if (JobDetails.Payload.ContainsKey(userIdKey) == true)
            {
                UserId = JobDetails.Payload[userIdKey];
            }
            else
            {
                Log.Error("ApplyReferralRewardJob Payload does not contain the ID of the user who was referred.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the referral event.
            string referralEventKey = Key.ReferralEvent.ToString();
            if (JobDetails.Payload.ContainsKey(referralEventKey) == true)
            {
                ReferralEvent = (ReferralEvent)Enum.Parse(typeof(ReferralEvent), JobDetails.Payload[referralEventKey]);
            }
            else
            {
                Log.Error("ApplyReferralRewardJob Payload does not contain the referral event.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the ID of the user claiming the deal.
        /// </summary>
        private string UserId { get; set; }

        /// <summary>
        /// Gets or sets the referral event.
        /// </summary>
        private ReferralEvent ReferralEvent { get; set; }

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