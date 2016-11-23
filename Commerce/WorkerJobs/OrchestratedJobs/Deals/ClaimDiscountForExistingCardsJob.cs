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
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;

    /// <summary>
    /// Orchestrated job to claim a Discount for all active cards.
    /// </summary>
    public class ClaimDiscountForExistingCardsJob : IOrchestratedJob
    {
        /// <summary>
        /// Initializes a new instance of the ClaimDiscountForExistingCardsJob class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ClaimDiscountForExistingCardsJob(CommerceLog log)
        {
            Log = log;
        }

        /// <summary>
        /// Initializes the ClaimDiscountForExistingCardsJob instance.
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
        /// Gets or sets a value that indicates whether the work items within the orchestrated job will be run asynchronously.
        /// </summary>
        /// <remarks>
        /// ClaimDeal will claim the new deal for each already added card. This can be a large number of operations, but they
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
        public Collection<IOrchestratedTask> Tasks
        {
            get
            {
                if (tasksGathered == false)
                {
                    if (ExtractPayload() == true)
                    {

                        Log.Verbose("Gathering list of cards for which the deal still needs to be claimed.");
                        Uri uri = new Uri(String.Concat((string) CommerceWorkerConfig.Instance.MbiServiceAuthority,
                                            (string) CommerceWorkerConfig.Instance.ServiceCardsControllerEndpoint));

                        // Get all active cards that have been registered with First Data for all users regardless of which reward program(s) the card is in.
                        HttpStatusCode httpStatusCode;
                        string response = RestServiceClient.CallRestService(uri, RestServiceVerbs.Get, null, null,
                                                RestServiceClient.ObtainAuthorizationToken("cards"),
                                                out httpStatusCode,
                                                String.Format("?userId=00000000-0000-0000-0000-000000000000&rewardPrograms={0}&partner={1}",
                                                              RewardPrograms.All, Partner.FirstData));
                        if (httpStatusCode == HttpStatusCode.OK)
                        {
                            ServiceGetCardsResponse getCardsResponse;
                            getCardsResponse = General.DeserializeJson<ServiceGetCardsResponse>(response);
                            // Determine the cards for which the deal still needs to be claimed.
                            IEnumerable<int> pendingCards;
                            pendingCards = getCardsResponse.Cards.Select(_ => General.IntegerFromGuid(_.Id));
                            if (AlreadyClaimedList.Any())
                            {
                                pendingCards = pendingCards.Except(AlreadyClaimedList);
                            }

                            // Build a list of tasks to claim the deal for the remaining cards, if any.
                            if (pendingCards.Any())
                            {
                                Log.Verbose("Creating tasks to perform pending claim deal calls.");
                                tasks = new Collection<IOrchestratedTask>();

                                foreach (ServiceCardDataContract cardDataContract in getCardsResponse.Cards)
                                {
                                    // Build a ClaimDealPayload and place it into a new task.
                                    ClaimDealPayload claimDealPayload = new ClaimDealPayload
                                    {
                                        ClaimDealInfo = new ClaimDealInfo
                                        {
                                            CardId = General.IntegerFromGuid(cardDataContract.Id),
                                            DealId = DealId
                                        },
                                        UserId = cardDataContract.UserId
                                    };
                                    ClaimDealTask claimDealTask = new ClaimDealTask(claimDealPayload, Log);
                                    claimDealTask.Initialize(JobDetails, Scheduler);
                                    tasks.Add(claimDealTask);
                                }
                            }
                            else
                            {
                                Log.Verbose("No pending cards needing to claim the deal were found.");
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
        /// For the current job. once all autolinking taks are successfully done, we do the following:
        /// 1. Retrieve the deal by id
        /// 2. Update the deal stauts to : AutoLinkingComplete
        /// 3. Updat the deal
        /// 4. Schedule the job to which will tell Deal Server that deal is active
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
            Log.Information("AutoLinking Job Complete for Deal :{0}", DealId);
            Log.Information("Job Result :{0}", executionResult);
            if (executionResult == OrchestratedExecutionResult.Success)
            {
                CommerceContext context = new CommerceContext("Claim Discount For Existing Cards Job Context", CommerceWorkerConfig.Instance);

                // get the deal
                context[Key.GlobalDealId] = DealId;
                SharedDealLogic sharedDealLogic = new SharedDealLogic(context, CommerceOperationsFactory.DealOperations(context));
                Deal deal = sharedDealLogic.RetrieveDeal();

                //now upate deal status to mark autolinking complete
                context[Key.InitialDeal] = new Deal(deal);
                context[Key.Deal] = deal;
                deal.DealStatusId = DealStatus.AutoLinkingComplete;
                context[Key.PreviouslyRegistered] = true;
                sharedDealLogic.RegisterDeal();

//TODO:schedule the job to tell DS that deal should be activated, but now do it right here
                context[Key.GlobalDealId] = DealId;
                deal = sharedDealLogic.RetrieveDeal();
                context[Key.InitialDeal] = new Deal(deal);
                context[Key.Deal] = deal;
                deal.DealStatusId = DealStatus.Activated;
                context[Key.PreviouslyRegistered] = true;
                sharedDealLogic.RegisterDeal();
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

            // Extract the DealID.
            string dealIdKey = Key.GlobalDealId.ToString();
            if (JobDetails.Payload.ContainsKey(dealIdKey) == true)
            {
                DealId = new Guid(JobDetails.Payload[dealIdKey]);
            }
            else
            {
                Log.Error("ClaimDiscountForExistingCardsJob Payload does not contain the ID of the discount being claimed.", null,
                          ResultCode.JobPayloadMissingData);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Gets the list of cards for which the deal has already been claimed.
        /// </summary>
        private IEnumerable<int> AlreadyClaimedList
        {
            get
            {
                if (alreadyClaimedList == null)
                {
                    Collection<int> ids = new Collection<int>();

                    int id;
                    foreach (string key in JobDetails.Payload.Keys)
                    {
                        if (int.TryParse(key, out id) == true)
                        {
                            if (JobDetails.Payload[key] == ClaimDealTask.CardMarker)
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
        private IEnumerable<int> alreadyClaimedList = null;

        /// <summary>
        /// Gets or sets the ID of the deal being claimed.
        /// </summary>
        private Guid DealId { get; set; }

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