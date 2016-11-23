//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to process the timeout reversal of a FirstData deal redemption.
    /// </summary>
    public class FirstDataProcessRedemptionTimeoutExecutor
    {
        /// <summary>
        /// Initializes a new instance of the FirstDataProcessRedemptionTimeoutExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context for the API being invoked.
        /// </param>
        public FirstDataProcessRedemptionTimeoutExecutor(CommerceContext context)
        {
            Context = context;
            Context[Key.Partner] = Partner.FirstData;
        }

        /// <summary>
        /// Executes processing of the deal redemption timeout reversal request.
        /// </summary>
        public ResultCode Execute()
        {
            ResultCode result;

            // Marshal the First Data reverse deal request into a the context.
            FirstData firstData = new FirstData(Context);
            firstData.MarshalRedemptionTimeout();

            // Update the data store to reflect the reversal of the redeemed deal.
            result = ProcessRedemptionTimeout();

            if (result == ResultCode.Success)
            {                
                Analytics.Add(AnalyticsAction.ReversedDealNonTimeout, Context.Log.ActivityId);
            }

            // Build the response to send back to First Data based on the result of reversing the deal.
            Context[Key.ResultCode] = result;
            firstData.BuildRedemptionTimeoutResponse();

            return result;
        }

        /// <summary>
        /// Updates the data store to reflect the reversal of the redeemed deal.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode ProcessRedemptionTimeout()
        {
            ResultCode result;

            Context.Log.Verbose("Attempting to update the data store to reflect the reversal of the timed out redemption.");
            IRedeemedDealOperations redeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            result = redeemedDealOperations.ProcessRedemptionTimeout();
            Context.Log.Verbose("ResultCode after reversing the timed out redemption in the data store: {0}", result);

            return result;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}