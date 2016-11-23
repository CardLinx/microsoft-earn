//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Configuration;

    /// <summary>
    /// Referral API controller for LoMo Commerce Web service.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class ReferralsController : ApiController
    {
        /// <summary>
        /// Adds the specified referral to the system.
        /// </summary>
        /// <param name="referralDataContract">
        /// The referral to add to the system.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing an AddReferralResponse with detailed result information.
        /// </returns>
        public HttpResponseMessage Add(ReferralDataContract referralDataContract)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Add user referral", Request,
                                                                                  new AddReferralResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                context[Key.ReferralDataContract] = referralDataContract;
                context.Log.Verbose("{0} request:\r\n{1}", context.ApiCallDescription,
                                    General.SerializeJson(referralDataContract));

                // Create an executor object to execute the API invocation.
                context[Key.ReferredUserFirstEarnRewardAmount] = CommerceServiceConfig.Instance.ReferredUserFirstEarnRewardAmount;
                context[Key.ReferredUserFirstEarnRewardExplanation] = CommerceServiceConfig.Instance.ReferredUserFirstEarnRewardExplanation;
                AddReferralExecutor executor = new AddReferralExecutor(context);
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

        /// <summary>
        /// Gets the referrals that resulted from a user's sharing and the events associated with those referrals.
        /// </summary>
        /// <returns>
        /// An HttpResponseMessage containing a GetUsersReferralsResponse with detailed result information.
        /// </returns>
        [ApiAuth]
        public HttpResponseMessage Get()
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get users referrals", Request,
                                                                                  new GetUsersReferralsResponse(),
                                                                                  callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);

                // Add ID for the user making this call to the context.
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);

                // Create an executor object to execute the API invocation.
                GetUsersReferralsExecutor executor = new GetUsersReferralsExecutor(context);
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