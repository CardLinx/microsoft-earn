//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Claimed deals service API controller for LoMo Commerce Web service.
    /// </summary>
    public class ServiceClaimedDealsController : ApiController
    {
        /// <summary>
        /// Allows the user to claim a deal for later redemption.
        /// </summary>
        /// <param name="claimDealPayload">
        /// Payload for a claim deal call.
        /// </param>
        /// <returns>
        /// A Task that, via an HttpResponseMessage, will yield a ClaimDealResponse with detailed result information.
        /// </returns>
        [SimpleWebTokenAuth("claimeddeals")]
        public Task<HttpResponseMessage> Claim(ClaimDealPayload claimDealPayload)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            Task<HttpResponseMessage> result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildAsynchronousRestContext("Claim deal in partners and data store",
                                                                                   Request, new ClaimDealResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                context.Log.Verbose("Presented credentials are for role \"{0}\" and include token \"{1}\".",
                                    clientIdentity.Name, clientIdentity.PresentedClientToken);
                context.Log.Verbose("{0} request:\r\n{1}", context.ApiCallDescription, General.SerializeJson(claimDealPayload));

                // Add payload information to the context.
                context[Key.ClaimDealPayload] = claimDealPayload;

                // Create an executor object to execute the API invocation.
                ClaimDealExecutor claimDealExecutor = new ClaimDealExecutor(context);
                Task.Factory.StartNew(async () => await claimDealExecutor.Execute());

                result = ((TaskCompletionSource<HttpResponseMessage>)context[Key.TaskCompletionSource]).Task;
            }
            catch (Exception ex)
            {
                result = RestResponder.CreateExceptionTask(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Retrieves the current claimed deals registered for the specified user.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user whose claimed deals to retrieve.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a GetClaimedDealsResponse with detailed result information.
        /// </returns>
        [SimpleWebTokenAuth("claimeddeals")]
        public HttpResponseMessage Get(Guid userId)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get claimed deals", Request,
                                                                                  new GetClaimedDealsResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                context.Log.Verbose("Presented credentials are for role \"{0}\" and include token \"{1}\".",
                                    clientIdentity.Name, clientIdentity.PresentedClientToken);

                // Add ID for the user making this call to the context.
                context[Key.GlobalUserId] = userId;

                // Create an executor object to execute the API invocation.
                GetClaimedDealsExecutor getClaimedDealsExecutor = new GetClaimedDealsExecutor(context);
                getClaimedDealsExecutor.Execute();

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