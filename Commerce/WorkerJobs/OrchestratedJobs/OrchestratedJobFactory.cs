//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;

    /// <summary>
    /// Creates appropriate instances of the IOrchestratedJob class.
    /// </summary>
    public static class OrchestratedJobFactory
    {
        /// <summary>
        /// Creates and initializes an instance of IOrchestratedJob of the specified type.
        /// </summary>
        /// <param name="jobDetails">
        /// Details describing the IOrchestratedJob type to create.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler managing the job.
        /// </param>
        /// <param name="log">
        /// The object through which log entries can be made.
        /// </param>
        /// <returns>
        /// An instance of IOrchestratedJob of the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// * Parameter jobDetails cannot be null.
        /// -OR-
        /// * Parameter log cannot be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Parameter JobDetails does not specify a valid IOrchestratedJob type.
        /// </exception>
        public static IOrchestratedJob Create(ScheduledJobDetails jobDetails,
                                              IScheduler scheduler,
                                              CommerceLog log)
        {
            if (jobDetails == null)
            {
                throw new ArgumentNullException("jobDetails", "Parameter jobDetails cannot be null.");
            }

            if (log == null)
            {
                throw new ArgumentNullException("log", "Parameter log cannot be null.");
            }

            IOrchestratedJob result = null;

            if (jobDetails.Payload != null)
            {
                switch (jobDetails.JobType)
                {
                    case ScheduledJobType.ApplyReferralReward:
                        result = new ApplyReferralRewardJob(log);
                        break;
                    case ScheduledJobType.ApplyRedemptionReward:
                        result = new ApplyRedemptionRewardJob(log);
                        break;
                    case ScheduledJobType.ClaimDiscountsForNewCard:
                        result = new ClaimDiscountsForNewCardJob(log);
                        break;
                    case ScheduledJobType.ClaimDiscountForExistingCards:
                        result = new ClaimDiscountForExistingCardsJob(log);
                        break;
                    case ScheduledJobType.AmexOfferRegistration:
                        result = new AmexOfferRegistrationJob(log);
                        break;
                    default:
                        throw new ArgumentException("Parameter JobDetails does not specify a valid IOrchestratedJob type.",
                                                    "jobDetails");
                }

                result.Initialize(jobDetails, scheduler);
            }
            else
            {
                log.Error("{0} orchestrated job contains no Payload.", null, jobDetails.JobType,
                          ResultCode.JobContainsNoPayload);
            }
            
            return result;
        }
    }
}