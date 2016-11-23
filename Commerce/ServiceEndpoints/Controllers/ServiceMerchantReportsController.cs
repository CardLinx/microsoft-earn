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

    public class ServiceMerchantReportsController : ApiController
    {
        /// <summary>
        /// Retrieves the number of redemptions for a merchant's deals.
        /// </summary>
        /// <param name="merchantReportQuery">
        /// The query describing the information to include in the merchant report.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a GetMerchantReportResponse with detailed result information.
        /// </returns>
        [MutualSslAuth("Ingestion")]
        public HttpResponseMessage GenerateReport(MerchantReportQuery merchantReportQuery)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get merchant report", Request,
                                                                                  new GetMerchantReportResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                context.Log.Verbose("Presented client certificate has subject \"{0}\" and thumbprint \"{1}\".",
                                    clientIdentity.Name, clientIdentity.PresentedClientToken);

                // Add query to the context.
                context[Key.MerchantReportQuery] = merchantReportQuery;

                // Create an executor object to execute the API invocation.
                GetMerchantReportExecutor executor = new GetMerchantReportExecutor(context);
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