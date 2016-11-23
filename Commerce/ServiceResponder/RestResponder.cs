//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Helper methods to build REST API responses.
    /// </summary>
    public static class RestResponder
    {
        /// <summary>
        /// Builds the response for a synchronous API call.
        /// </summary>
        /// <param name="context">
        /// The context for the current API call.
        /// </param>
        /// <param name="ex">
        /// The exception to log for the API call, if any (optional).
        /// </param>
        /// <returns>
        /// The HttpResponseMessage to return for the synchronous API call.
        /// </returns>
        public static HttpResponseMessage BuildSynchronousResponse(CommerceContext context,
                                                                   Exception ex = null)
        {
            HttpResponseMessage result = null;

            BuildResponse(context, ex,
                (httpResponseMessage, httpStatusCode, callCompletionStatus) =>
                {
                    result = httpResponseMessage;
                    context.Log.CallCompletion(context.ApiCallDescription, callCompletionStatus, context.PerformanceInformation);
                }
            );

            return result;
        }

        /// <summary>
        /// Builds the response for an asynchronous API call.
        /// </summary>
        /// <param name="context">
        /// The context for the current API call.
        /// </param>
        /// <param name="ex">
        /// The exception to log for the API call, if any (optional).
        /// </param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
         Justification = "If HttpResponseMessage is disposed here, it will not be available when the Task resumes, and "
                         + "ObjectDisposedException will be thrown.")]
        public static void BuildAsynchronousResponse(CommerceContext context,
                                                     Exception ex = null)
        {
            BuildResponse(context, ex,
                (httpResponseMessage, httpStatusCode, callCompletionStatus) =>
                 {
                    if (httpResponseMessage == null)
                    {
                       httpResponseMessage = new HttpResponseMessage();
                    }

                    // Signal completion of the task.
                    TaskCompletionSource<HttpResponseMessage> taskCompletionSource =
                                                    (TaskCompletionSource<HttpResponseMessage>)context[Key.TaskCompletionSource];
                    if (httpStatusCode == HttpStatusCode.OK || httpStatusCode == HttpStatusCode.Created || httpStatusCode == HttpStatusCode.Accepted)
                    {
                        taskCompletionSource.SetResult(httpResponseMessage);
                    }
                    else
                    {
                        HttpResponseException httpResponseException = new HttpResponseException(httpResponseMessage);
                        taskCompletionSource.SetException(httpResponseException);
                    }
                 
                    // Log completion of the call.
                    context.Log.CallCompletion(context.ApiCallDescription, callCompletionStatus, context.PerformanceInformation);
                }
            );
        }

        /// <summary>
        /// Creates a task to process unhandled exceptions for asynchronous APIs.
        /// </summary>
        /// <param name="context">
        /// The context for the current API call.
        /// </param>
        /// <param name="ex">
        /// The exception to log for the API call.
        /// </param>
        /// <returns>
        /// A task to process the unhandled exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public static Task<HttpResponseMessage> CreateExceptionTask(CommerceContext context,
                                                                    Exception ex)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            ((ResultSummary)context[Key.ResultSummary]).SetResultCode(ResultCode.UnknownError);
            BuildAsynchronousResponse(context, ex);

            return ((TaskCompletionSource<HttpResponseMessage>)context[Key.TaskCompletionSource]).Task;
        }

        /// <summary>
        /// Maps the specified ResultCode to the appropriate HttpStatusCode.
        /// </summary>
        /// <param name="resultCode">
        /// The ResultCode to map to an HttpStatusCode.
        /// </param>
        /// <returns>
        /// The mapped HttpStatusCode.
        /// </returns>
        internal static HttpStatusCode MapResultToStatusCode(ResultCode resultCode)
        {
            HttpStatusCode result = HttpStatusCode.Forbidden;

            switch (resultCode)
            {
                case ResultCode.Success:
                case ResultCode.UnregisteredUser:
                case ResultCode.AlreadyClaimed:
                case ResultCode.PartnerDealExpired:
                case ResultCode.CardStateUnchanged:
                    result = HttpStatusCode.OK;
                    break;
                case ResultCode.Created:
                    result = HttpStatusCode.Created;
                    break;
                case ResultCode.JobQueued:
                    result = HttpStatusCode.Accepted;
                    break;
                case ResultCode.ParameterCannotBeNull:
                case ResultCode.InvalidParameter:
                    result = HttpStatusCode.BadRequest;
                    break;
                case ResultCode.UnknownError:
                case ResultCode.None:
                    result = HttpStatusCode.InternalServerError;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Adds the performance information if the request included the directive to do so.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        /// <param name="queryString">
        /// The query string for the URL that invoked the current API request.
        /// </param>
        /// <param name="responseHeaders">
        /// The response headers for the API call being processed.
        /// </param>
        internal static void AddPerformanceInformation(CommerceContext context,
                                                string queryString,
                                                HttpResponseHeaders responseHeaders)
        {
            Stopwatch callTimer = (Stopwatch)context[Key.CallTimer];
            callTimer.Stop();
            context.PerformanceInformation.Add("Total", String.Format("{0} ms", callTimer.ElapsedMilliseconds));

            NameValueCollection queryStringItems = HttpUtility.ParseQueryString(queryString);
            if (queryStringItems["analytics"] == "1")
            {
                responseHeaders.Add("x-analytics", context.PerformanceInformation.Collate());
            }
        }

        /// <summary>
        /// Builds the response for an API call.
        /// </summary>
        /// <param name="context">
        /// The context for the current API call.
        /// </param>
        /// <param name="ex">
        /// The exception to log for the API call, if any.
        /// </param>
        /// <param name="completeResponse">
        /// The method to invoke to complete the response.
        /// </param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
         Justification = "If HttpResponseMessage is disposed here, ObjectDisposedException will be thrown when .Net attempts " +
                         "to build an HTTP response from its contents.")]
        private static void BuildResponse(CommerceContext context,
                                          Exception ex,
                                          Action<HttpResponseMessage,
                                                 HttpStatusCode,
                                                 CallCompletionStatus> completeResponse)
        {
            ResultSummary resultSummary = (ResultSummary)context[Key.ResultSummary];

            // If an exception is specified, set the result code accordingly.
            if (ex != null)
            {
                resultSummary.SetResultCode(ResultCode.UnknownError);
            }

            // Log the response.
            object response = context[Key.Response];
            context.Log.Verbose("{0} response:\r\n{1}", context.ApiCallDescription, General.SerializeJson(response));

            // Get response status code.
            ResultCode resultCode = resultSummary.GetResultCode();
            HttpStatusCode httpStatusCode = MapResultToStatusCode(resultCode);

            // Log the result and get call completion status.
            CallCompletionStatus callCompletionStatus = CallCompletionStatus.Success;
            if (httpStatusCode == HttpStatusCode.OK ||
                httpStatusCode == HttpStatusCode.Created ||
                httpStatusCode == HttpStatusCode.Accepted)
            {
                context.Log.Information("{0} call processed successfully.", context.ApiCallDescription);
            }
            else
            {
                if (ex != null)
                {
                    callCompletionStatus = CallCompletionStatus.Error;
                    context.Log.Critical("{0} call ended with an error.", ex, context.ApiCallDescription);
                }
                else
                {
                    callCompletionStatus = CallCompletionStatus.SuccessWithWarnings;
                    context.Log.Warning("{0} call unsuccessfully processed.\r\n\r\nResultCode: {1}\r\n\r\nExplanation: {2}",
                                        (int)resultCode, context.ApiCallDescription, resultSummary.ResultCode,
                                        resultSummary.Explanation);
                }
            }

            // Next, build the HttpResponseMessage for the call.
            HttpResponseMessage httpResponseMessage = null;
            if (context[Key.Request] != null)
            {
                HttpRequestMessage request = (HttpRequestMessage)context[Key.Request];
                httpResponseMessage = request.CreateResponse(httpStatusCode, response);
                AddPerformanceInformation(context, request.RequestUri.Query, httpResponseMessage.Headers);
            }

            // Finally, complete the response.
            completeResponse(httpResponseMessage, httpStatusCode, callCompletionStatus);
        }
    }
}