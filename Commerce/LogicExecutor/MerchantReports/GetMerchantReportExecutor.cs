//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Linq;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to execute a get merchant report request.
    /// </summary>
    public class GetMerchantReportExecutor
    {
        /// <summary>
        /// Initializes a new instance of the GetMerchantReportExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        public GetMerchantReportExecutor(CommerceContext context)
        {
            Context = context;
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
        }

        /// <summary>
        /// Executes the get merchant report invocation.
        /// </summary>
        public void Execute()
        {
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];
            ResultCode resultCode = ExtractParameters();
            if (resultCode == ResultCode.Success)
            {
                resultSummary.SetResultCode(GetMerchantDealRedemptions());
            }
            else
            {
                resultSummary.SetResultCode(resultCode);
            }
        }

        /// <summary>
        /// Extracts parameters from the context and validates their state.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode ExtractParameters()
        {
            ResultCode result = ResultCode.Success;

            if (Context.ContainsKey(Key.MerchantReportQuery) == true)
            {
                MerchantReportQuery merchantReportQuery = (MerchantReportQuery)Context[Key.MerchantReportQuery];
                if (merchantReportQuery.EndDay == DateTime.MinValue)
                {
                    merchantReportQuery.EndDay = DateTime.MaxValue;
                }
                if (merchantReportQuery.StartDay > merchantReportQuery.EndDay)
                {
                    result = ResultCode.InvalidParameter;
                }
            }
            else
            {
                result = ResultCode.ParameterCannotBeNull;
            }

            return result;
        }

        /// <summary>
        /// Retrieves the the redemption history for a merchant's deals.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode GetMerchantDealRedemptions()
        {
            ResultCode result = ResultCode.Success;

            // Retrieve the redemption history for the merchant's deals.
            Context.Log.Verbose("Retrieving redemption history for deals offered by merchants with partner merchant IDs listed within " +
                                "the query.");
            result = RedeemedDealOperations.RetrieveMerchantDealRedemptions();

            return result;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        internal IRedeemedDealOperations RedeemedDealOperations { get; set; }
    }
}