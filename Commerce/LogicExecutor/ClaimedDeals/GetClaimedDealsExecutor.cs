//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to execute a get claimed deals request.
    /// </summary>
    public class GetClaimedDealsExecutor
    {
        /// <summary>
        /// Initializes a new instance of the GetClaimedDealsExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        public GetClaimedDealsExecutor(CommerceContext context)
        {
            Context = context;
            ClaimedDealOperations = CommerceOperationsFactory.ClaimedDealOperations(Context);
        }

        /// <summary>
        /// Executes the get claimed deals invocation.
        /// </summary>
        public void Execute()
        {
            SharedUserLogic sharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));

            User user = sharedUserLogic.RetrieveUser();
            Context[Key.User] = user;
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];
            if (user != null)
            {
                resultSummary.SetResultCode(GetClaimedDeals());
            }
            else
            {
                resultSummary.SetResultCode(ResultCode.UnregisteredUser);
            }
        }

        /// <summary>
        /// Retrieves the list of deals claimed by the user in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode GetClaimedDeals()
        {
            IEnumerable<Guid> claimedDeals = new List<Guid>();

            // Retrieve the list of claimed deals.
            Context.Log.Verbose("Retrieving list of claimed deals belonging to user {0}", ((User)Context[Key.User]).GlobalId);
            claimedDeals = ClaimedDealOperations.RetrieveClaimedDeals();
            Context.Log.Verbose("{0} claimed deals were retrieved from the data store.", claimedDeals.Count());

            ((GetClaimedDealsResponse)Context[Key.Response]).ClaimedDeals = claimedDeals;

            return ResultCode.Success;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for ClaimedDeal operations.
        /// </summary>
        internal IClaimedDealOperations ClaimedDealOperations { get; set; }
    }
}