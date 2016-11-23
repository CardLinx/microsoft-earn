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
    /// V2 Cards API controller for LoMo Commerce Web service.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class V2CardsController : ApiController
    {
        /// <summary>
        /// Retrieves the cards registered to the authenticated user.
        /// </summary>
        /// <returns>
        /// An HttpResponseMessage containing a V2GetCardsResponse with detailed result information.
        /// </returns>
        [ApiAuth]
        public HttpResponseMessage Get()
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get cards", Request, new V2GetCardsResponse(),
                                                                                  callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);

                // Add ID for the user making this call to the context.
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);
                context[Key.RewardProgramType] = ControllerHelper.GetRewardProgramAssociatedWithRequest(this.Request);

                // Create an executor object to execute the API invocation.
                V2GetCardsExecutor getCardsExecutor = new V2GetCardsExecutor(context);
                getCardsExecutor.Execute();

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