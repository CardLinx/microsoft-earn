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

    /// <summary>
    /// Referral type API controller for LoMo Commerce Web service.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class ReferralTypesController : ApiController
    {
        /// <summary>
        /// Adds the specified referral type to the system.
        /// </summary>
        /// <param name="rewardRecipient">
        /// The recipient of the reward.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing an AddReferralTypeResponse with detailed result information.
        /// </returns>
        [ApiAuth]
        public HttpResponseMessage Add(int rewardRecipient)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Add user referral type", Request,
                                                                                  new AddReferralTypeResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                context[Key.RewardRecipient] = rewardRecipient;

                // Add user ID to the context.
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);

                // Create an executor object to execute the API invocation.
                AddReferralTypeExecutor executor = new AddReferralTypeExecutor(context);
                executor.Execute();

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