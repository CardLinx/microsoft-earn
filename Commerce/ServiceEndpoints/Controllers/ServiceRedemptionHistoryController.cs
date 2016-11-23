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
    using System.Web.Http;
    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// History API controller for LoMo Commerce Service.
    /// </summary>
    public class ServiceRedemptionHistoryController : ApiController
    {
        /// <summary>
        /// Retrieves the redemption history for the specified user.
        /// </summary>
        /// <param name="userId"/>
        /// The Id of user whose redemption history is retrived.
        /// <returns>
        /// An HttpResponseMessage containing a GetRedemptionHistoryResponse with detailed result information.
        /// </returns>
        [MutualSslAuth("Ingestion")]
        public HttpResponseMessage Get(Guid userId)
        {
            HttpResponseMessage result;

            Stopwatch callTimer = Stopwatch.StartNew();

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get redemption history", Request,
                                                                                  new GetRedemptionHistoryResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                context.Log.Verbose("Presented client certificate has subject \"{0}\" and thumbprint \"{1}\".",
                                    clientIdentity.Name, clientIdentity.PresentedClientToken);

                // Add ID for the user making this call to the context.
                context[Key.GlobalUserId] = userId;
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

    }
}