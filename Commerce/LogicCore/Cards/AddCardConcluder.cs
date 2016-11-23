//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;
    using Users.Dal;

    /// <summary>
    /// Contains logic necessary to conclude the execution of an add card request.
    /// </summary>
    public class AddCardConcluder
    {
        /// <summary>
        /// Initializes a new instance of the AddCardConcluder class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        public AddCardConcluder(CommerceContext context)
        {
            Context = context;
            UserOperations = CommerceOperationsFactory.UserOperations(Context);
            CardOperations = CommerceOperationsFactory.CardOperations(Context);
        }

        /// <summary>
        /// Concludes execution of the Add card call after previous work has been completed.
        /// </summary>
        /// <param name="resultCode">
        /// The ResultCode to set within the call response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public void Conclude(ResultCode resultCode)
        {
            try
            {
                Context.Log.Verbose("ResultCode when Conclude process begins: {0}.", resultCode);

                // If process succeeded, update internal data storage.
                AddCardResponse response = (AddCardResponse)Context[Key.Response];
                if (resultCode == ResultCode.Created)
                {
                    // Add the card.
                    resultCode = AddCard();
                    if (resultCode == ResultCode.Created || resultCode == ResultCode.Success)
                    {
                        response.NewCardId = General.GuidFromInteger(((Card)Context[Key.Card]).Id);

                        // If a new card was added, kick off confirmation process for unauthenticated users and add needed information to analytics.
                        // TODO: AddCard() above returns ResultCode.Success. So the code below will not execute. Is it ok?
                        if (resultCode == ResultCode.Created)
                        {
                            // Kick off confirmation process for unauthenticated users.
                            bool createUnauthenticatedAccount = false;
                            if (Context[Key.CreateUnauthenticatedAccount] != null)
                            {
                                createUnauthenticatedAccount = (bool)Context[Key.CreateUnauthenticatedAccount];
                            }

                            if (createUnauthenticatedAccount == true)
                            {
                                IUsersDal usersDal = PartnerFactory.UsersDal(Context.Config);
                                Task.Run(() => usersDal.CompleteUnauthenticatedUserSetUp((Guid)Context[Key.GlobalUserId]));
                            }

                            // Add analytics info.
                            Context.Log.Verbose("Adding new card to analytics.");
                            User user = (User)Context[Key.User];
                            Analytics.AddAddCardEvent(user.GlobalId, user.AnalyticsEventId, Guid.Empty, Context[Key.ReferrerId] as string);
                        }

                        // Queue deal claiming if set to do so.
                        bool queueDealClaiming = false;
                        if (Context[Key.QueueJob] != null)
                        {
                            queueDealClaiming = (bool)Context[Key.QueueJob];
                        }

                        // Linking is only for First Data, but by the time execution reaches this part of the code, the card may need to be linked to CLO offers or
                        //  Burn offers, or both, but definitely at least one of them. Therefore, a job has to be scheduled to cover the relevant combination of CLO
                        //  and Burn offers. That Earn offers are not registered with First Data doesn't change this-- the filtering will have to occur as part of the job.
                        if (queueDealClaiming == true)
                        {
                            QueueClaimingDeals(response);
                            resultCode = ResultCode.JobQueued;
                        }
                    }
                }

                response.ResultSummary.SetResultCode(resultCode);
                RestResponder.BuildAsynchronousResponse(Context);
            }
            catch (Exception ex)
            {
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Queues claiming already claimed deals for the new card.
        /// </summary>
        /// <param name="response">
        /// The AddCardResponse being built.
        /// </param>
        private void QueueClaimingDeals(AddCardResponse response)
        {
            Context.Log.Verbose("Queueing claiming user's existing claimed deals for the new card.");
            string userId = ((User)Context[Key.User]).GlobalId.ToString();
            ConcurrentDictionary<string, string> payload = new ConcurrentDictionary<string, string>();
            payload[Key.GlobalUserId.ToString()] = userId;
            payload[Key.CardId.ToString()] = General.IntegerFromGuid(response.NewCardId).ToString();
            ScheduledJobDetails scheduledJobDetails = new ScheduledJobDetails
            {
                JobId = Guid.NewGuid(),
                JobType = ScheduledJobType.ClaimDiscountsForNewCard,
                JobDescription = userId,
                Orchestrated = true,
                StartTime = DateTime.UtcNow,
                Payload = payload
            };
            IScheduler scheduler = PartnerFactory.Scheduler(CommerceServiceConfig.Instance.SchedulerQueueName,
                                                            CommerceServiceConfig.Instance.SchedulerTableName,
                                                            CommerceServiceConfig.Instance);
            scheduler.ScheduleJobAsync(scheduledJobDetails).Wait();
        }

        /// <summary>
        /// Gets or sets the data access object to use for User operations.
        /// </summary>
        internal IUserOperations UserOperations { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for Card operations.
        /// </summary>
        internal ICardOperations CardOperations { get; set; }

        /// <summary>
        /// Adds the specified Card to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode AddCard()
        {
            ResultCode result;

            Context.Log.Verbose("Attempting to add the card for the user to the data store.");
            result = CardOperations.AddCard();
            Context.Log.Verbose("ResultCode after adding the card to the data store: {0}", result);

            return result;
        }

        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        private CommerceContext Context { get; set; }
    }
}