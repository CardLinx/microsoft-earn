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
    using System.Net;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Notifications;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Orchestrated job to claim all the active discounts in the system.
    /// </summary>
    public class ClaimDiscountsForNewCardJob : IOrchestratedJob
    {
        /// <summary>
        /// Initializes a new instance of the ClaimDiscountsForNewCardJob class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ClaimDiscountsForNewCardJob(CommerceLog log)
        {
            Log = log;
        }

        /// <summary>
        /// Initializes the ClaimDiscountsForNewCardJob instance.
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
        /// ClaimDeal will claim each already claimed deal for the new card. This can be a large number of operations, but they
        /// can be run in parallel.
        /// </remarks>
        public bool Asynchronous
        {
            get
            {
                return true;
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
                        Log.Verbose("Gathering list of active discounts to determine which discounts still need to be linked to the card");

                        Uri uri = new Uri(String.Concat((string)CommerceWorkerConfig.Instance.MbiServiceAuthority,
                                                        (string)CommerceWorkerConfig.Instance.ServiceDiscountsControllerEndpoint));
                        HttpStatusCode httpStatusCode;
//TODO: Sending ReimbursementTender.MicrosoftBurn is a hack. Since ReimbursementTender was not intended for bitmasking, it has neither a None nor an All.
//       Until this can be changed, sending Burn will be treated the same as DealCurrency | MicrosoftBurn would be were it an option.
//       This can be cleaned up when First Data has been deprecated and can be removed.
                        string response = RestServiceClient.CallRestService(uri, RestServiceVerbs.Get, null, null,
                                                                      RestServiceClient.ObtainAuthorizationToken("discounts"),
                                                                      out httpStatusCode,
                                                                      String.Format("?reimbursementTender={0}&partner={1}",
                                                                                    ReimbursementTender.MicrosoftBurn, Partner.FirstData));

                        if (httpStatusCode == HttpStatusCode.OK)
                        {
                            GetActiveDiscountIdsResponse getActiveDiscountIds;
                            getActiveDiscountIds = General.DeserializeJson<GetActiveDiscountIdsResponse>(response);
                            
                            // Determine which deals still need to be claimed.
                            IEnumerable<Guid> pendingClaimedDeals;
                            pendingClaimedDeals = getActiveDiscountIds.DiscountIds;
                            if (AlreadyClaimedList.Any() == true)
                            {
                                pendingClaimedDeals = Enumerable.Except(getActiveDiscountIds.DiscountIds, AlreadyClaimedList);
                            }

                            // Build a list of tasks to claim the remaining deals, if any.
                            if (pendingClaimedDeals.Any())
                            {
                                Log.Verbose("Creating tasks to perform pending claim deal calls.");
                                tasks = new Collection<IOrchestratedTask>();

                                foreach (Guid dealId in pendingClaimedDeals)
                                {
                                    // Build a ClaimDealPayload and place it into a new task.
                                    ClaimDealPayload claimDealPayload = new ClaimDealPayload
                                    {
                                        ClaimDealInfo = new ClaimDealInfo
                                        {
                                            CardId = CardId,
                                            DealId = dealId
                                        },
                                        UserId = UserId
                                    };
                                    ClaimDealTask claimDealTask = new ClaimDealTask(claimDealPayload, Log);
                                    claimDealTask.Initialize(JobDetails, Scheduler);
                                    tasks.Add(claimDealTask);
                                }
                            }
                            else
                            {
                                Log.Verbose("No pending claimed deals for the new card were found.");
                            }

                            tasksGathered = true;
                        }
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
        /// <param name="jobResult">
        /// The result of job.
        /// </param>
        /// <returns>
        /// The result of the execution of the tear down tasks.
        /// </returns>
        public OrchestratedExecutionResult TearDown(OrchestratedExecutionResult executionResult)
        {
            if (executionResult == OrchestratedExecutionResult.Success)
            {
                CommerceContext context = new CommerceContext("Claim Discount For Existing Cards Job Context", CommerceWorkerConfig.Instance);

                context[Key.UserId] = UserId;
                context[Key.CardId] = CardId;

                Log.Information("Have to send notification here, userid :{0}, cardid :{1}", UserId,CardId);
                // send add card notification here
                Notify notify = new NotifyAddCard(context);
                Task.Run(new Action(notify.SendNotification));
            }

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

            // Extract the UserID.
            string userIdKey = Key.GlobalUserId.ToString();
            if (JobDetails.Payload.ContainsKey(userIdKey) == true)
            {
                UserId = new Guid(JobDetails.Payload[userIdKey]);
            }
            else
            {
                Log.Error("ClaimDiscountsForNewCardJob Payload does not contain the ID of the user adding the card.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            // Extract the CardID.
            string cardIdKey = Key.CardId.ToString();
            if (JobDetails.Payload.ContainsKey(cardIdKey) == true)
            {
                CardId = int.Parse(JobDetails.Payload[cardIdKey]);
            }
            else
            {
                Log.Error("ClaimDiscountsForNewCardJob Payload does not contain the ID of the card being added.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Gets the list of deals that have already been claimed for the new card.
        /// </summary>
        private IEnumerable<Guid> AlreadyClaimedList
        {
            get
            {
                if (alreadyClaimedList == null)
                {
                    Collection<Guid> ids = new Collection<Guid>();

                    Guid id;
                    foreach (string key in JobDetails.Payload.Keys)
                    {
                        if (Guid.TryParse(key, out id) == true)
                        {
                            if (JobDetails.Payload[key] == ClaimDealTask.DealMarker)
                            {
                                ids.Add(id);
                            }
                        }
                    }

                    alreadyClaimedList = ids;
                }

                return alreadyClaimedList;
            }
        }
        private IEnumerable<Guid> alreadyClaimedList = null;

        /// <summary>
        /// Gets or sets the ID of the user claiming the deal.
        /// </summary>
        private Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the card being claimed.
        /// </summary>
        private int CardId { get; set; }

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