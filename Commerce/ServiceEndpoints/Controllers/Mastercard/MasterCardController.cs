//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web;
    using System.Web.Http;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Mastercard API controller for Mastercard Requests.
    /// </summary>
    public class MasterCardController : ApiController
    {
        /// <summary>
        /// Processes the MasterCard onAuthorization request.
        /// </summary>
        /// <param name="request">
        /// The MasterCard onAuthorization request.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage yielding HTTP status code 200.
        /// </returns>
        [HttpPost]
        [ActionName("Authorization")]
//        [MutualSslAuth("MasterCard")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "If HttpResponseMessage is disposed here, ObjectDisposedException will be thrown when .Net " +
                         "attempts to build an HTTP response from its contents.")]
        public HttpResponseMessage OnAuthorization(Transaction request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "Parameter request cannot be null.");
            }

            // Build a context object to pass down the pipeline.
            Stopwatch callTimer = Stopwatch.StartNew();
            Context = new CommerceContext("Incoming MasterCard Authorization Request");
            MasterCardAuthorizationResponse response = new MasterCardAuthorizationResponse();
            Context[Key.Transaction] = request;
            Context[Key.Response] = response;
            Context[Key.CallTimer] = callTimer;

            // Process the call and log its result.
            Context.Log.Information("Processing {0} call.", Context.ApiCallDescription);
//TODO: If we ever circle back and switch to client certificate authentication, uncomment this and remove ValidateIP.
            //CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
            //Context.Log.Verbose("Presented client certificate has subject \"{0}\" and thumbprint \"{1}\".",
            //                    clientIdentity.Name, clientIdentity.PresentedClientToken);
            ResultCode resultCode = ResultCode.Unauthorized;
            response.ResponseCode = ((int)resultCode).ToString();
            CallCompletionStatus callCompletionStatus = CallCompletionStatus.Error;
            if (ValidateIP() == true)
            {
                try
                {
                    MasterCardAuthorizationExecutor executor = new MasterCardAuthorizationExecutor(Context);
                    resultCode = executor.Execute();

                    // Determine call completion status from executor result and log it.
                    if (resultCode == ResultCode.Created || resultCode == ResultCode.DuplicateTransaction)
                    {
                        resultCode = ResultCode.Success;
                        callCompletionStatus = CallCompletionStatus.Success;
                        Context.Log.Information("{0} call processed successfully.", Context.ApiCallDescription);
                    }
                    else
                    {
                        callCompletionStatus = CallCompletionStatus.SuccessWithWarnings;
                        Context.Log.Warning("{0} call unsuccessfully processed.\r\n\r\nResultCode: {1}\r\n\r\nExplanation: {2}",
                                            (int)resultCode, Context.ApiCallDescription, resultCode,
                                            ResultCodeExplanation.Get(resultCode));
                    }
                }
                catch (Exception ex)
                {
                    Context.Log.Critical("{0} call ended with an error.", ex, Context.ApiCallDescription);
                    resultCode = ResultCode.UnknownError;
                    callCompletionStatus = CallCompletionStatus.Error;
                }
            }

            // Build the response.
            HttpResponseMessage message = new HttpResponseMessage(MapResultToStatusCode(resultCode));
            message.Content = new ObjectContent<MasterCardAuthorizationResponse>(response, new XmlMediaTypeFormatter());

            // Log the result of the call.
            callTimer.Stop();
            Context.PerformanceInformation.Add("Total", String.Format("{0} ms", callTimer.ElapsedMilliseconds));
            Context.Log.CallCompletion(Context.ApiCallDescription, callCompletionStatus, Context.PerformanceInformation);

            return message;
        }

        /// <summary>
        /// Validates caller's IP address against the list of valid IP addresses.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool ValidateIP()
        {
            bool result = false;

            string clientIP = HttpContext.Current.Request.UserHostAddress;
            Context.Log.Verbose("Caller has IP address {0}", clientIP);
            foreach(string masterCardAuthorizationIPAddress in CommerceServiceConfig.Instance.MasterCardAuthorizationIPAddresses)
            {
                if (clientIP == masterCardAuthorizationIPAddress)
                {
                    result = true;
                    break;
                }
            }

            return result;
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
        /// <remarks>
        /// MasterCard, for some reason, wants 200 back at all times for this API. 401 will still be returned if they fail authorization, however.
        /// </remarks>
        private static HttpStatusCode MapResultToStatusCode(ResultCode resultCode)
        {
            HttpStatusCode result;

            switch (resultCode)
            {
                case ResultCode.Success:
                    result = HttpStatusCode.OK;
                    break;
                case ResultCode.UnknownError:
                    result = HttpStatusCode.OK;
                    break;
                case ResultCode.Unauthorized:
                    result = HttpStatusCode.Unauthorized;
                    break;
                default:
                     result = HttpStatusCode.OK;
                     break;
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context for this MasterCard authorization call.
        /// </summary>
        private CommerceContext Context { get; set; }
    }
}