//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.AmexClient;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;

    /// <summary>
    /// Implementation for processing Amex Registration Response File
    /// </summary>
    public class RegistrationResponseFileProcessor
    {
        /// <summary>
        /// Process the response file
        /// </summary>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public virtual async Task<bool> ProcessAsync()
        {
            OfferRegistrationResponseFileParser parser = new OfferRegistrationResponseFileParser(Context.Log);
            OfferRegistrationResponseFile responseFile = parser.Parse(ResponseFileName, ResponseFileStream);
            bool submissionValid = true;
            if (responseFile != null)
            {
                if (responseFile.Header.ResponseCode == "A")
                {
                    foreach (OfferRegistrationResponseDetail record in responseFile.ResponseRecords)
                    {
                        Context[Key.PartnerDealId] = record.OfferId;
                        Context[Key.Partner] = Partner.Amex;
                        IDealOperations dealOperations = CommerceOperationsFactory.DealOperations(Context);
                        Guid? discountId = dealOperations.RetrieveDiscountIdFromPartnerDealId();
                        Context[Key.GlobalDealId] = discountId.Value;
                        SharedDealLogic dealLogic = new SharedDealLogic(Context, CommerceOperationsFactory.DealOperations(Context));
                        Deal deal = dealLogic.RetrieveDeal();

                        // for each record - check the status and process accordingly
                        if (record.ResponseCode == "A")
                        {
                            if (record.ActionCode == OfferRegistrationActionCodeType.Add)
                            {
                                // Possible Race condition in this part of the code
                                // By time time we check whether all partners are registered, things could change in DB
                                // this is not a concern right now but we need to figure it out.
                                bool allOtherPartnersRegistered = true;
                                foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
                                {
                                    if (partnerDealInfo.PartnerId != Partner.Amex)
                                    {
                                        if (partnerDealInfo.PartnerDealRegistrationStatusId !=
                                            PartnerDealRegistrationStatus.Complete)
                                        {
                                            allOtherPartnersRegistered = false;
                                            break;
                                        }
                                    }
                                }

                                // now update deal status
                                deal.DealStatusId = DealStatus.PendingAutoLinking;
                                foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
                                {
                                    if (partnerDealInfo.PartnerId == Partner.Amex)
                                    {
                                        partnerDealInfo.PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Complete;
                                    }
                                }
                                Context[Key.Deal] = deal;
                                dealOperations.RegisterDeal();

                                if (allOtherPartnersRegistered)
                                {
                                    // schedule autolinking    
                                    ConcurrentDictionary<string, string> payload = new ConcurrentDictionary<string, string>();
                                    payload[Key.GlobalDealId.ToString()] = deal.GlobalId.ToString();
                                    ScheduledJobDetails scheduledJobDetails = new ScheduledJobDetails
                                    {
                                        JobId = Guid.NewGuid(),
                                        JobType = ScheduledJobType.ClaimDiscountForExistingCards,
                                        Orchestrated = true,
                                        StartTime = DateTime.UtcNow,
                                        Payload = payload
                                    };
                                    await Scheduler.ScheduleJobAsync(scheduledJobDetails).ConfigureAwait(false);
                                }
                            }
                            else if (record.ActionCode == OfferRegistrationActionCodeType.Update)
                            {
                                // previously registered, and update was successful.
                                foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
                                {
                                    if (partnerDealInfo.PartnerId == Partner.Amex)
                                    {
                                        partnerDealInfo.PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Complete;
                                    }
                                }
                                Context[Key.Deal] = deal;
                                dealOperations.RegisterDeal();
                                // TODO:Tell Deal Server we are done.
//                                ConcurrentDictionary<string, string> payload = new ConcurrentDictionary<string, string>();
//                                payload[Key.DealId.ToString()] = deal.Id.ToString();
//                                ScheduledJobDetails scheduledJobDetails = new ScheduledJobDetails
//                                {
//                                    JobId = Guid.NewGuid(),
//                                    JobType = ScheduledJobType.DiscountActivationJob,
//                                    Orchestrated = false,
//                                    StartTime = DateTime.UtcNow,
//                                    Payload = payload
//                                };
//                                await Scheduler.ScheduleJobAsync(scheduledJobDetails).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            Context.Log.Warning("Attempt to register a deal with Amex failed\r\nOffer Id {0}\r\n Reason {1}", (int)ResultCode.SubmissionRejected, record.OfferId, record.ResponseCodeMessage);
                            // update the deal to reflect error
                            foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
                            {
                                if (partnerDealInfo.PartnerId == Partner.Amex)
                                {
                                    partnerDealInfo.PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Error;
                                }
                            }
                            Context[Key.Deal] = deal;
                            dealOperations.RegisterDeal();
                        }
                    }
                 }
                else
                {
                    // file submission was rejected.
                    submissionValid = false;
                }
            }
            return submissionValid;
        }

        /// <summary>
        /// Gets or sets data Stream for response file contents
        /// </summary>
        public Stream ResponseFileStream { get; set; }

        /// <summary>
        /// Gets or sets response file name
        /// </summary>
        public string ResponseFileName { get; set; }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        public CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the scheduler
        /// </summary>
        public IScheduler Scheduler { get; set; }
    }
}