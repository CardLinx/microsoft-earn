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

    public class ServiceDealReportsController : ApiController
    {
        /// <summary>
        /// Retrieves reports for the specified deals and their discounts, according to query parameters.
        /// </summary>
        /// <param name="dealReportsQuery">
        /// The query describing the deals and their discounts whose reports to generate.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a GetDealReportsResponse with detailed result information.
        /// </returns>
        [MutualSslAuth("Ingestion")]
        [HttpPost]
        public HttpResponseMessage Get(DealReportsQuery dealReportsQuery)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get deal report", Request,
                                                                                  new GetDealReportsResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                context.Log.Verbose("Presented client certificate has subject \"{0}\" and thumbprint \"{1}\".",
                                    clientIdentity.Name, clientIdentity.PresentedClientToken);

                // Populate the context with call parameters.
                context[Key.DealReportsQuery] = dealReportsQuery;

                // Create an executor object to execute the API invocation.
                GetDealReportsExecutor executor = new GetDealReportsExecutor(context);
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