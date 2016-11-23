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
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// FirstData-specific API service implementation for LoMo Commerce Web service.
    /// </summary>
    public class FirstDataDealsService : providerInterface
    {
        /// <summary>
        /// Redeems the deal specified in the request.
        /// </summary>
        /// <param name="request">
        /// The deal redemption request.
        /// </param>
        /// <returns>
        /// A deal redemption response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter request cannot be null.
        /// </exception>
        public pubRedemptionResponse1 pubRedemption(pubRedemptionRequest1 request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "Parameter request cannot be null.");
            }

            Stopwatch callTimer = Stopwatch.StartNew();

            pubRedemptionResponse1 result = new pubRedemptionResponse1(new pubRedemptionResponse
                                                                       {
                                                                           reqID = request.pubRedemptionRequest.reqID
                                                                       });

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousContext("First Data Redeem deal",
                                                                              request.pubRedemptionRequest,
                                                                              result.pubRedemptionResponse);

            // Process the call.
            HttpStatusCode httpStatusCode = ProcessFirstDataCall(context, callTimer, () =>
                                 {
                                     FirstDataRedeemDealExecutor firstDataRedeemDealExecutor = new FirstDataRedeemDealExecutor(context);
                                     return firstDataRedeemDealExecutor.Execute();
                                 });

            // Throw exception if the operation did not succeed.
            if (httpStatusCode != HttpStatusCode.OK)
            {
                throw new WebFaultException<pubRedemptionResponse1>(result, httpStatusCode);
            }

            return result;
        }

        /// <summary>
        /// Reverses a previous deal redemption.
        /// </summary>
        /// <param name="request">
        /// The deal redemption reversal request.
        /// </param>
        /// <returns>
        /// A deal redemption reversal response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter request cannot be null.
        /// </exception>
        public pubReversalResponse1 pubReversal(pubReversalRequest1 request)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            if (request == null)
            {
                throw new ArgumentNullException("request", "Parameter request cannot be null.");
            }

            pubReversalResponse1 result = new pubReversalResponse1(new pubReversalResponse()
                                                                   {
                                                                       reqID = request.pubReversalRequest.reqID
                                                                   });

            // Split processing of timeout and non-timeout reversals.
            HttpStatusCode httpStatusCode;
            if (request.pubReversalRequest.revReason == FirstDataConstants.ReverseRedeemedDealIndicator)
            {
                // Build a context object to pass down the pipeline.
                CommerceContext context = CommerceContext.BuildSynchronousContext("First Data Reverse redeemed deal",
                                                                                  request.pubReversalRequest,
                                                                                  result.pubReversalResponse);

                // Process the call.
                httpStatusCode = ProcessFirstDataCall(context, callTimer, () =>
                       {
                           FirstDataReverseRedeemedDealExecutor executor = new FirstDataReverseRedeemedDealExecutor(context);
                           return executor.Execute();
                       });
            }
            else
            {
                // Build a context object to pass down the pipeline.
                CommerceContext context = CommerceContext.BuildSynchronousContext("First Data process redemption timeout",
                                                                                  request.pubReversalRequest,
                                                                                  result.pubReversalResponse);

                // Process the call.
                httpStatusCode = ProcessFirstDataCall(context, callTimer, () =>
                {
                    FirstDataProcessRedemptionTimeoutExecutor executor = new FirstDataProcessRedemptionTimeoutExecutor(context);
                    return executor.Execute();
                });
            }

            // Throw exception if the operation did not succeed.
            if (httpStatusCode != HttpStatusCode.OK)
            {
                throw new WebFaultException<pubReversalResponse1>(result, httpStatusCode);
            }

            return result;
        }

        /// <summary>
        /// Processes the call from First Data.
        /// </summary>
        /// <param name="context">
        /// The context of the call.
        /// </param>
        /// <param name="callTimer">
        /// The Stopwatch used to time the call.
        /// </param>
        /// <param name="executorInvoker">
        /// The callback that invokes the executor to execute call logic.
        /// </param>
        /// <returns>
        /// The HTTP status code to place within the response.
        /// </returns>
        private static HttpStatusCode ProcessFirstDataCall(CommerceContext context,
                                                           Stopwatch callTimer,
                                                           Func<ResultCode> executorInvoker)
        {
            HttpStatusCode result = HttpStatusCode.OK;

            context.Log.Information("Processing {0} call.", context.ApiCallDescription);

            CallCompletionStatus callCompletionStatus;
            try
            {
                HttpClientCertificate httpClientCertificate = HttpContext.Current.Request.ClientCertificate;
                context.Log.Verbose("Presented client certificate has serial number \"{0}\".",
                                    httpClientCertificate.SerialNumber);
                if (General.IsPresentedCertValid(httpClientCertificate,
                                          CommerceServiceConfig.Instance.FirstDataClientCertificateSerialNumbers) == true)
                {
                    ResultCode resultCode = executorInvoker();
                    if (resultCode == ResultCode.Success || resultCode == ResultCode.Created)
                    {
                        callCompletionStatus = CallCompletionStatus.Success;
                        context.Log.Information("{0} call processed successfully.", context.ApiCallDescription);
                    }
                    else
                    {
                        callCompletionStatus = CallCompletionStatus.SuccessWithWarnings;
#if !IntDebug && !IntRelease
                        context.Log.Warning("{0} call unsuccessfully processed.\r\n\r\nResultCode: {1}\r\n\r\nExplanation: {2}",
                                            (int)resultCode, context.ApiCallDescription, resultCode,
                                            ResultCodeExplanation.Get(resultCode));
#endif
                        if (resultCode == ResultCode.None || resultCode == ResultCode.UnknownError)
                        {
                            result = HttpStatusCode.InternalServerError;
                        }
                    }
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
                    result = HttpStatusCode.Unauthorized;
                }
            }
            catch (Exception ex)
            {
                callCompletionStatus = CallCompletionStatus.Error;
                context.Log.Critical("{0} call ended with an error.", ex, context.ApiCallDescription);
                result = HttpStatusCode.InternalServerError;
            }

            callTimer.Stop();
            context.PerformanceInformation.Add("Total", String.Format("{0} ms", callTimer.ElapsedMilliseconds));
            context.Log.CallCompletion(context.ApiCallDescription, callCompletionStatus, context.PerformanceInformation);

            return result;
        }
    }
}