//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Xml.Serialization;
    using Lomo.Commerce.AmexClient;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;

    /// <summary>
    /// Amex API controller for Amex Requests.
    /// </summary>
    public class AmexController : ApiController
    {
        /// <summary>
        /// This will handle the Authorization POST requests from Mastercard and just log the content.
        /// uri : {base-uri}/api/commerce/amex/auth
        /// </summary>
        /// <returns>An HttpResponseMessage</returns>
        [HttpPost]
        [ActionName("Auth")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
        Justification = "If HttpResponseMessage is disposed here, ObjectDisposedException will be thrown when .Net attempts " +
                        "to build an HTTP response from its contents.")]
        public HttpResponseMessage OnAuthorization()
        {
            AmexAuthRequest request = ParseRequest();
            AmexAuthResponse response = new AmexAuthResponse();
            CommerceContext context = CommerceContext.BuildSynchronousContext("Amex payment authorization request", request, response);
            context.Log.Verbose("Amex payment authorization payload : \r\n\"{0}\".", Request.Content.ReadAsStringAsync().Result);

            if (IsAuthorized(context))
            {
                Stopwatch callTimer = Stopwatch.StartNew();
                HttpStatusCode httpStatusCode = ProcessAmexAuthCall(context, callTimer, () =>
                {
                    AmexAuthorizationExecutor amexAuthorizationExecutor = new AmexAuthorizationExecutor(context);
                    return amexAuthorizationExecutor.Execute();
                });

                if (httpStatusCode != HttpStatusCode.OK)
                {
                    response.ResponseCode = AmexAuthResponseCode.AmexAuthInternalError;
                    context.Log.Warning("Amex payment authorization processing error. Request : {0} ", request.ToString());
                }

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<AmexAuthResponse>(response, new XmlMediaTypeFormatter())
                };

                return message;
            }

            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Test whether Amex Incoming call is authorized. Check header value.
        /// </summary>
        /// <returns>
        /// True/False based on incoming request
        /// </returns>
        private bool IsAuthorized(CommerceContext context)
        {
            bool authResult = false;
            if (Request.Headers != null)
            {
                IEnumerable<string> values;
                Request.Headers.TryGetValues("AMEX_AUTH_SEC_TOKEN", out values);

                if (values != null)
                {
                    string incomingPartnerKey = values.FirstOrDefault();
                    if (incomingPartnerKey != null && incomingPartnerKey.Equals(AmexConstants.PartnerId, StringComparison.OrdinalIgnoreCase))
                    {
                        authResult = true;
                    }
                    else
                    {
                        context.Log.Warning("Amex payment authorization - Auth Header mismatch, incoming value: {0}", incomingPartnerKey);
                    }
                }
                else
                {
                    context.Log.Warning("Amex payment authorization - No auth header present");
                }
            }

            context.Log.Information("Amex payment authorization - IsAuthorized ? {0}", authResult);
            return authResult;
        }

        /// <summary>
        /// Helper Method to parse incoming payload from AMEX.
        /// </summary>
        /// <returns>
        /// Auth representation
        /// </returns>
        private AmexAuthRequest ParseRequest()
        {
            string payLoad = Request.Content.ReadAsStringAsync().Result;
            AmexAuthRequest resultingMessage = null;
            XmlSerializer serializer = new XmlSerializer(typeof(AmexAuthRequest));
            using (StringReader rdr = new StringReader(payLoad))
            {
                resultingMessage = (AmexAuthRequest)serializer.Deserialize(rdr);
            }

            return resultingMessage;
        }

        /// <summary>
        /// Process the Incoming Amex Auth Request
        /// </summary>
        /// <param name="context">
        /// Commerce Context
        /// </param>
        /// <param name="callTimer">
        /// Call Timer
        /// </param>
        /// <param name="executorInvoker">
        /// Executor handler
        /// </param>
        /// <returns>
        /// HttpStatus code
        /// </returns>
        private static HttpStatusCode ProcessAmexAuthCall(CommerceContext context,
                                                    Stopwatch callTimer,
                                                    Func<ResultCode> executorInvoker)
        {
            HttpStatusCode result = HttpStatusCode.OK;

            context.Log.Verbose("Processing {0}.", context.ApiCallDescription);
            CallCompletionStatus callCompletionStatus;
            try
            {
                ResultCode resultCode = executorInvoker();
                if (resultCode == ResultCode.Success || resultCode == ResultCode.Created || resultCode == ResultCode.DuplicateTransaction)
                {
                    callCompletionStatus = CallCompletionStatus.Success;
                    context.Log.Information("{0} processed successfully.", context.ApiCallDescription);
                }
                else
                {
                    callCompletionStatus = CallCompletionStatus.SuccessWithWarnings;
                    context.Log.Warning("{0} processed unsuccessfully.\r\nResultCode: {1}\r\nExplanation: {2}",
                                        (int)resultCode,
                                        context.ApiCallDescription,
                                        resultCode,
                                        ResultCodeExplanation.Get(resultCode));
                }
            }
            catch (Exception ex)
            {
                callCompletionStatus = CallCompletionStatus.Error;
                context.Log.Critical("{0} ended with an error.", ex, context.ApiCallDescription);
                result = HttpStatusCode.InternalServerError;
            }
            callTimer.Stop();
            context.PerformanceInformation.Add("Total", string.Format("{0} ms", callTimer.ElapsedMilliseconds));
            context.Log.CallCompletion(context.ApiCallDescription, callCompletionStatus, context.PerformanceInformation);

            return result;
        }
    }
}