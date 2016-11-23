//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.ServiceModel.Web;
    using System.Web;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// FirstData-specific API service implementation for LoMo Commerce Web service.
    /// </summary>
    public class FirstDataPingService : providerInterfacePing
    {
        /// <summary>
        /// Pings the FirstDataService.
        /// </summary>
        /// <param name="request">
        /// The ping request.
        /// </param>
        /// <returns>
        /// A ping response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter request cannot be null.
        /// </exception>
        public pubPingResponse1 pubPing(pubPingRequest1 request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "Parameter request cannot be null.");
            }

            Stopwatch callTimer = Stopwatch.StartNew();

            pubPingResponse1 result = new pubPingResponse1(new pubPingResponse
                                                           {
                                                               reqID = request.pubPingRequest.reqID
                                                           });

            // Build a context object to pass down the pipeline.
            CommerceContext context = new CommerceContext("First Data ping");
            context[Key.Request] = request.pubPingRequest;
            context[Key.Response] = result.pubPingResponse;
            context.Log.Exhaustive("Processing {0} call.", context.ApiCallDescription);

            CallCompletionStatus callCompletionStatus = CallCompletionStatus.Error;
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;
            try
            {
                HttpClientCertificate httpClientCertificate = HttpContext.Current.Request.ClientCertificate;
                context.Log.Exhaustive("Presented client certificate has serial number \"{0}\".",
                                       httpClientCertificate.SerialNumber);
                if (General.IsPresentedCertValid(httpClientCertificate,
                                          CommerceServiceConfig.Instance.FirstDataClientCertificateSerialNumbers) == true)
                {

                    FirstDataPingExecutor firstDataPingExecutor = new FirstDataPingExecutor(context);
                    firstDataPingExecutor.Execute();
                    context.Log.Exhaustive("{0} call processed successfully.", context.ApiCallDescription);
                    callCompletionStatus = CallCompletionStatus.Success;
                }
                else
                {
                    callCompletionStatus = CallCompletionStatus.SuccessWithWarnings;
#if !IntDebug && !IntRelease
                    context.Log.Warning("{0} call unsuccessfully processed.\r\n\r\nResultCode: {1}\r\n\r\nExplanation: {2}",
                                        (int)ResultCode.InvalidClientCertificate, context.ApiCallDescription,
                                        ResultCode.InvalidClientCertificate,
                                        ResultCodeExplanation.Get(ResultCode.InvalidClientCertificate));
#endif
                    httpStatusCode = HttpStatusCode.Unauthorized;
                }
            }
            catch (Exception ex)
            {
                context.Log.Critical("{0} call ended with an error.", ex, context.ApiCallDescription);
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            callTimer.Stop();
            context.PerformanceInformation.Add("Total", String.Format("{0} ms", callTimer.ElapsedMilliseconds));
            context.Log.ExhaustiveCallCompletion(context.ApiCallDescription, callCompletionStatus, context.PerformanceInformation);

            // Throw exception if the operation did not succeed.
            if (httpStatusCode != HttpStatusCode.OK)
            {
                throw new WebFaultException<pubPingResponse1>(result, httpStatusCode);
            }

            return result;
        }
    }
}