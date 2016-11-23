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
    /// V3 Deals API controller for LoMo Commerce Web service.
    /// </summary>
    public class V3ServiceDealsController : ApiController
    {
        /// <summary>
        /// Registers the specified deal with the system.
        /// </summary>
        /// <param name="dealDataContract">
        /// The V3DealDataContract to register with the system.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a V3RegisterDealResponse with detailed result information.
        /// </returns>
        [MutualSslAuth("Ingestion")]
        public HttpResponseMessage Register(V3DealDataContract dealDataContract)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Register offer", Request,
                                                                                   new V3RegisterDealResponse(), callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                context.Log.Verbose("Presented client certificate has subject \"{0}\" and thumbprint \"{1}\".",
                                    clientIdentity.Name, clientIdentity.PresentedClientToken);
                context[Key.DealDataContract] = dealDataContract;
                context.Log.Verbose("{0} request:\r\n{1}", context.ApiCallDescription, General.SerializeJson(dealDataContract));

                // Create an executor object to execute the API invocation.
                V3ServiceRegisterDealExecutor registerDealExecutor = new V3ServiceRegisterDealExecutor(context);
                registerDealExecutor.Execute();

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