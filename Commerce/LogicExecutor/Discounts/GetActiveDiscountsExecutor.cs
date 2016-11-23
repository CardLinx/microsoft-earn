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
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Executor for getting all active deals
    /// </summary>
    public class GetActiveDiscountsExecutor
    {
        /// <summary>
        /// Ctor for getting discounts
        /// </summary>
        /// <param name="context">
        /// Commerce Context
        /// </param>
        public GetActiveDiscountsExecutor(CommerceContext context)
        {
            Context = context;
            DealOperations = CommerceOperationsFactory.DealOperations(context);
        }

        /// <summary>
        /// Executes the get active discounts invocation.
        /// </summary>
        public void Execute()
        {
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];
            resultSummary.SetResultCode(GetActiveDiscountIds());
        }

        /// <summary>
        /// Retrieves the list of all active discounts
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode GetActiveDiscountIds()
        {
            IEnumerable<Guid> activeDiscounts = new List<Guid>();

            // Retrieve the list of discounts.
            Context.Log.Verbose("Retrieving list of all active discounts");
            activeDiscounts = DealOperations.RetrieveActiveDiscountIds();
            Context.Log.Verbose("{0} active discounts were retrieved from the data store.", activeDiscounts.Count());

            ((GetActiveDiscountIdsResponse)Context[Key.Response]).DiscountIds = activeDiscounts;

            return ResultCode.Success;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for ClaimedDeal operations.
        /// </summary>
        internal IDealOperations DealOperations { get; set; }
    }
}