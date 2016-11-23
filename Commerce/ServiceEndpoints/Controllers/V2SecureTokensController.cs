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
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// V2 Secure Tokens API controller for LoMo Commerce Web service.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class V2SecureTokensController : ApiController
    {
        /// <summary>
        /// Issues a secure token for the authenticated user for a CRUD operation on a Card object in the current environment.
        /// </summary>
        /// <param name="operation">
        /// The card CRUD operation for which the secure token is to be issued.
        /// </param>
        /// <param name="eventId">
        /// The ID of the event associated with the creation of a new user.
        /// </param>
        /// <param name="referralCode">
        /// The referral code associated with the creation of a new user.
        /// </param>
        /// <param name="referrer">
        /// The referrer to associate with the creation of a new user, when applicable.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a GetSecureCardOperationTokenResponse with detailed result information.
        /// </returns>
        [ApiAuth]
        [ActionName("Cards")]
        public HttpResponseMessage GetSecureCardOperationToken(string operation,
                                                               string eventId = "",
                                                               string referralCode = "",
                                                               string referrer = null)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get secure card operation token", Request,
                                                                                  new GetSecureCardOperationTokenResponse(),
                                                                                  callTimer);
            try
            {
                context[Key.RequestedCrudOperation] = operation;
                context[Key.ReferralTypeCode] = referralCode;
                context[Key.ReferralEvent] = ReferralEvent.Signup;
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);

                // Populate the context.
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);
                context[Key.ReferredUserId] = context[Key.GlobalUserId].ToString();
                Guid eventIdGuid;
                if (Guid.TryParse(eventId, out eventIdGuid) == true)
                {
                    context[Key.CorrelationId] = eventIdGuid;
                }
                context[Key.ReferrerId] = referrer;

                // Create an executor object to execute the API invocation.
                GetSecureCardOperationTokenExecutor executor = new GetSecureCardOperationTokenExecutor(context);
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