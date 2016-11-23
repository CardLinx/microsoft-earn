//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to execute a get user's referral request.
    /// </summary>
    public class GetUsersReferralsExecutor
    {
        /// <summary>
        /// Initializes a new instance of the GetUsersReferralsExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context describing the referral type to register.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public GetUsersReferralsExecutor(CommerceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            Context = context;
            ReferralOperations = CommerceOperationsFactory.ReferralOperations(Context);
        }

        /// <summary>
        /// Executes the get user's referrals API invocation.
        /// </summary>
        public void Execute()
        {
            SharedUserLogic sharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];

            if (sharedUserLogic.RetrieveUser() != null)
            {
                Context[Key.ReferrerId] = Context[Key.GlobalUserId];
                Context[Key.ReferrerType] = ReferrerType.User;
                resultSummary.SetResultCode(GetUsersReferrals());
            }
            else
            {
                resultSummary.SetResultCode(ResultCode.UnexpectedUnregisteredUser);
            }
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets the users referrals by event type and count.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode GetUsersReferrals()
        {
            GetUsersReferralsResponse response = (GetUsersReferralsResponse)Context[Key.Response];
            Context.Log.Verbose("Retrieving user's referrals by event type and count.");
            response.ReferralCodeReports = ReferralOperations.RetrieveReferralCounts();
            Context.Log.Verbose("User's referral events and counts retrieved.");

            return ResultCode.Success;
        }

        /// <summary>
        /// Gets or sets the object to use to perform operations on referrals.
        /// </summary>
        private IReferralOperations ReferralOperations { get; set; }
    }
}