//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// History API controller for LoMo Commerce Web service.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class RedemptionHistoryController : ApiController
    {
        /// <summary>
        /// Retrieves the CLO redemption history for the authenticated user.
        /// </summary>
        /// <returns>
        /// An HttpResponseMessage containing a GetRedemptionHistoryResponse with detailed result information.
        /// </returns>
        [ApiAuth]
        public HttpResponseMessage Get()
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get redemption history", Request,
                                                                                  new GetRedemptionHistoryResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);

                // Add call parameters.
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);
                context[Key.RewardProgramType] = RewardPrograms.CardLinkOffers;

                // Create an executor object to execute the API invocation.
                GetRedemptionHistoryExecutor getRedemptionHistoryExecutor = new GetRedemptionHistoryExecutor(context);
                getRedemptionHistoryExecutor.Execute();

                // Build the response from the result of API invocation.
                result = RestResponder.BuildSynchronousResponse(context);
            }
            catch (Exception ex)
            {
                result = RestResponder.BuildSynchronousResponse(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Retrieves the MN redemption history for the authenticated user.
        /// </summary>
        /// <returns>
        /// An HttpResponseMessage containing a GetEarnBurnTransactionHistoryResponse with detailed result information.
        /// </returns>
        //[HttpGet]
        //[ActionName("earnhistory")]
        [ApiAuth]
        public HttpResponseMessage GetEarnHistory()
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get Earn/Burn history", Request,
                                                                                  new GetEarnBurnTransactionHistoryResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);

                // Add call parameters.
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);
                context[Key.RewardProgramType] = RewardPrograms.EarnBurn;

                // Create an executor object to execute the API invocation.
                GetRedemptionHistoryExecutor getRedemptionHistoryExecutor = new GetRedemptionHistoryExecutor(context);
                getRedemptionHistoryExecutor.Execute();

                // Build the response from the result of API invocation.
                result = RestResponder.BuildSynchronousResponse(context);
            }
            catch (Exception ex)
            {
                result = RestResponder.BuildSynchronousResponse(context, ex);
            }

            return result;
        }
    }
}
