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
    /// Contains logic necessary to execute a get deal reports request.
    /// </summary>
    public class GetDealReportsExecutor
    {
        /// <summary>
        /// Initializes a new instance of the GetDealReportsExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        public GetDealReportsExecutor(CommerceContext context)
        {
            Context = context;
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
        }

        /// <summary>
        /// Executes the get deal report invocation.
        /// </summary>
        public void Execute()
        {
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];
            ResultCode resultCode = ValidateParameters();
            if (resultCode == ResultCode.Success)
            {
                resultSummary.SetResultCode(RetrieveDealReports());
            }
            else
            {
                resultSummary.SetResultCode(resultCode);
            }
        }

        /// <summary>
        /// Validates parameters from the context and updates default values if needed.
        /// </summary>
        private ResultCode ValidateParameters()
        {
            ResultCode result = ResultCode.Success;

            // Get the deal reports query and update EndDay if necessary.
            DealReportsQuery = (DealReportsQuery)Context[Key.DealReportsQuery];
            if (DealReportsQuery.StartDay <= DealReportsQuery.EndDay)
            {
                if (DealReportsQuery.EndDay == DateTime.MinValue)
                {
                    DealReportsQuery.EndDay = DateTime.MaxValue;
                }
            }
            else
            {
                result = ResultCode.InvalidParameter;
            }

            return result;
        }

        /// <summary>
        /// Retrieves the the requested deal reports.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode RetrieveDealReports()
        {
            ResultCode result = ResultCode.Success;

            GetDealReportsResponse response = (GetDealReportsResponse)Context[Key.Response];
            Context.Log.Verbose("Retrieving requested deal reports.");
            result = RedeemedDealOperations.RetrieveDealReports();
            Context.Log.Verbose("Successfully retrieved information for {0} of the {1} requested deals.",
                                response.DealReports.Count(), DealReportsQuery.DealReportQueries.Count());

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

        /// <summary>
        /// Gets or sets the query for the requests deal reports.
        /// </summary>
        private DealReportsQuery DealReportsQuery { get; set; }
    }
}