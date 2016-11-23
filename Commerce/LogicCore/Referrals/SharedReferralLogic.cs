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
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;

    /// <summary>
    /// Contains helper methods to perform shared business logic for Referral objects.
    /// </summary>
    public class SharedReferralLogic
    {
        /// <summary>
        /// Initializes a new instance of the SharedReferralLogic class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        /// <param name="referralOperations">
        /// The object to use to perform operations on referrals.
        /// </param>
        public SharedReferralLogic(CommerceContext context,
                                   IReferralOperations referralOperations)
        {
            Context = context;
            ReferralOperations = referralOperations;
        }

        /// <summary>
        /// Adds referral information for the sign up of a new user, if a referral code is specified.
        /// </summary>
        /// <param name="userId">
        /// The ID of the newly signed up user.
        /// </param>
        public ResultCode AddReferral(string userId)
        {
            ResultCode result = ResultCode.SubmissionRejected;

            string referralCode = Context[Key.ReferralTypeCode] as String;
            if (String.IsNullOrWhiteSpace(referralCode) == false)
            {
                Context.Log.Verbose("Adding referral with code {0} for new user signup.", referralCode);
                result = AddReferralToDataStore();
            }

            return result;
        }

        /// <summary>
        /// Adds the referral in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode AddReferralToDataStore()
        {
            ResultCode result = ResultCode.Created;

            Context.Log.Verbose("Adding referral to the data store.");
            Context[Key.ReferredUserFirstEarnRewardAmount] = CommerceServiceConfig.Instance.ReferredUserFirstEarnRewardAmount;
            Context[Key.ReferredUserFirstEarnRewardExplanation] = CommerceServiceConfig.Instance.ReferredUserFirstEarnRewardExplanation;
            result = ReferralOperations.AddReferral();
            if (result == ResultCode.Created)
            {
                bool referralAdded = (bool)Context[Key.ReferralAdded];
                if (referralAdded == true)
                {
                    Context.Log.Verbose("Referral added to the data store.");
                }
                else
                {
                    result = ResultCode.Success;
                    Context.Log.Verbose("User has reached the per user limit for this referral. " +
                                        "No referral was added to the data store.");
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the object to use to perform operations on referrals.
        /// </summary>
        private IReferralOperations ReferralOperations { get; set; }
    }
}