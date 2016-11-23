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
    using System.Web.Http;
    using System.Net.Http.Formatting;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.VisaClient;
    using System.Web;
    using Lomo.Commerce.Configuration;

    enum VisaEPMRequestType
    {
        OnAuth, 
        OnClear,
        OnStatementCredit
    }

    /// <summary>
    ///     Visa API controller for Visa Requests.
    /// </summary>
    /// <remarks>
    ///     Get the call through first. Will work on security later.
    /// </remarks>
    public class VisaController : ApiController
    {
        /// <summary>
        ///     Initializes a new instance of the VisaController class.
        /// </summary>
        public VisaController()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the VisaController class.
        /// </summary>
        /// <param name="request">
        ///     The request to process within this instance.
        /// </param>
        public VisaController(HttpRequestMessage request)
        {
            Request = request;
        }

        /// <summary>
        ///     This will handle the OfferActivation POST requests from VISA and just log the content.
        ///     uri : {base-uri}/api/commerce/visa/offeractivation
        /// </summary>
        /// <returns>An HttpResponseMessage</returns>
        [HttpPost]
        [ActionName("OfferActivation")]
        public HttpResponseMessage OnOfferActivation()
        {
            Stopwatch callTimer = Stopwatch.StartNew();
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Incoming Offer Activation Request",
                                                                                  Request,
                                                                                  new VisaResponse(),
                                                                                  callTimer);
            HttpResponseMessage result;

            try
            {
                result = LogPayload(context);
            }
            catch (Exception ex)
            {
                result = RestResponder.BuildSynchronousResponse(context, ex);
            }

            return result;
        }

        /// <summary>
        ///     This will handle the Authorization POST requests from VISA and just log the content.
        ///     uri : {base-uri}/api/commerce/visa/authorization
        /// </summary>
        /// <returns>An HttpResponseMessage</returns>
        [HttpPost]
        [ActionName("Authorization")]
        public HttpResponseMessage OnAuthorization()
        {
            return ProcessVisaEndpointMessage(VisaEPMRequestType.OnAuth);
        }

        /// <summary>
        ///     This will handle the Clearing POST requests from VISA and just log the content.
        ///     uri : {base-uri}/api/commerce/visa/clearing
        /// </summary>
        /// <returns>An HttpResponseMessage</returns>
        [HttpPost]
        [ActionName("Clearing")]
        public HttpResponseMessage OnClearing()
        {
            return ProcessVisaEndpointMessage(VisaEPMRequestType.OnClear);
        }

        /// <summary>
        ///     This will handle the StatementCredit POST requests from VISA and just log the content.
        ///     uri : {base-uri}/api/commerce/visa/statementcredit
        /// </summary>
        /// <returns>An HttpResponseMessage</returns>
        [HttpPost]
        [ActionName("StatementCredit")]
        public HttpResponseMessage OnStatementCredit()
        {
            //Visa is sending "OnStatementCredit" message to "OnClearing" end point. This method is just a place holder if Visa send the message to new end point.
            return ProcessVisaEndpointMessage(VisaEPMRequestType.OnStatementCredit);
        }

        /// <summary>
        ///     Helper Method to log incoming payload from VISA.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>200 OK in all scenarios</returns>
        private HttpResponseMessage LogPayload(CommerceContext context)
        {
            string payLoad = Request.Content.ReadAsStringAsync().Result;
            context.Log.Information("Incoming POST payload from VISA : \r\n\"{0}\".", payLoad);

//TODO: Return Responses as per scenarios.
            ((ResultSummary) context[Key.ResultSummary]).SetResultCode(ResultCode.Success);
            return RestResponder.BuildSynchronousResponse(context);
        }

        /// <summary>
        ///     Helper Method to parse incoming payload from AMEX.
        /// </summary>
        /// <returns>
        ///     Auth representation
        /// </returns>
        private EndPointMessageRequest ParseRequest()
        {
            string payload = Request.Content.ReadAsStringAsync().Result;
            return General.DeserializeJson<EndPointMessageRequest>(payload);
        }

        /// <summary>
        ///     Test whether Visa Incoming call is authorized. To be developed.
        /// </summary>
        /// <returns>
        ///     True/False based on incoming request
        /// </returns>
        //private static bool IsAuthorized(CommerceContext context)
        //{
        //    //TODO: Lock down security once the interop is working. 
        //    context.Log.Error("Still not locked the endpoint yet {0}", null, "visa endpoint");
        //    return true;
        //}

        /// <summary>
        ///     Process the Incoming Visa EndpointMessage Request
        /// </summary>
        /// <param name="context">
        ///     Commerce Context
        /// </param>
        /// <param name="callTimer">
        ///     Call Timer
        /// </param>
        /// <param name="executorInvoker">
        ///     Executor handler
        /// </param>
        /// <returns>
        ///     HttpStatus code
        /// </returns>
        private HttpStatusCode ProcessVisaEndpointMessage(CommerceContext context,
                                                          Stopwatch callTimer,
                                                          Func<ResultCode> executorInvoker)
        {
            HttpStatusCode result = HttpStatusCode.Unauthorized;

            context.Log.Information("Processing {0} call.", context.ApiCallDescription);
            CallCompletionStatus callCompletionStatus = CallCompletionStatus.Error;
#if !IntDebug && !IntRelease
            if (ValidateIP(context) == true)
#endif
            {
                try
                {
                    ResultCode resultCode = executorInvoker();
                    if (resultCode == ResultCode.Success || resultCode == ResultCode.Created ||
                        resultCode == ResultCode.DuplicateTransaction)
                    {
                        callCompletionStatus = CallCompletionStatus.Success;
                        context.Log.Information("{0} call processed successfully.", context.ApiCallDescription);
                        result = HttpStatusCode.OK;
                    }
                    else
                    {
                        //if (resultCode == ResultCode.None)
                        //TODO: need to be more discrimitive
                        result = HttpStatusCode.InternalServerError;
                        callCompletionStatus = CallCompletionStatus.SuccessWithWarnings;
                        context.Log.Warning(
                            "{0} call unsuccessfully processed.\r\n\r\nResultCode: {1}\r\n\r\nExplanation: {2}",
                            (int) resultCode, context.ApiCallDescription, resultCode,
                            ResultCodeExplanation.Get(resultCode));
                    }
                }
                catch (Exception ex)
                {
                    context.Log.Critical("{0} call ended with an error.", ex, context.ApiCallDescription);
                    result = HttpStatusCode.InternalServerError;
                }
            }

            callTimer.Stop();
            context.PerformanceInformation.Add("Total", String.Format("{0} ms", callTimer.ElapsedMilliseconds));
            context.Log.CallCompletion(context.ApiCallDescription, callCompletionStatus, context.PerformanceInformation);

            return result;
        }

        /// <summary>
        ///     Process message 
        /// </summary>
        /// <param name="type"> type is onAuth or clearing</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification =
                "If HttpResponseMessage is disposed here, ObjectDisposedException will be thrown when .Net attempts " +
                "to build an HTTP response from its contents.")]
        private HttpResponseMessage ProcessVisaEndpointMessage(VisaEPMRequestType type)
        {
            VisaEpmResponse response = new VisaEpmResponse { StatusCode = "0", ErrorMessage = "" };
            CommerceContext context = null;
            string message = null;

            try
            {
                EndPointMessageRequest request = ParseRequest();

                context = CommerceContext.BuildSynchronousContext("Incoming Visa Request",
                                                                                  request,
                                                                                  response);

                message = Request.Content.ReadAsStringAsync().Result;
                context.Log.Verbose("Incoming POST payload from Visa : \r\n\"{0}\".", message);

                
                //If Visa do not send StatementCredit message on a new endpoint then we have to detect from the message if it is StatementCredit message
                if (request.MessageName != null && request.MessageName.IndexOf("StatementCreditEndPoint", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    type = VisaEPMRequestType.OnStatementCredit;
                }

                //if (IsAuthorized(context))
                {
                    Stopwatch callTimer = Stopwatch.StartNew();
                    var httpStatusCode = HttpStatusCode.OK;

                    if (type == VisaEPMRequestType.OnClear)
                    {
                        httpStatusCode = ProcessVisaEndpointMessage(context, callTimer, () =>
                            {
                                VisaRedeemDealExecutor visaEndpointMessageProcessor =
                                    new VisaRedeemDealExecutor(context);
                                return visaEndpointMessageProcessor.Execute();
                            });
                    }
                    else if (type == VisaEPMRequestType.OnAuth)
                    {
                        httpStatusCode = ProcessVisaEndpointMessage(context, callTimer, () =>
                        {
                            VisaAuthorizationExecutor visaEndpointMessageProcessor =
                                new VisaAuthorizationExecutor(context);
                            return visaEndpointMessageProcessor.Execute();
                        });
                    }
                    else if (type == VisaEPMRequestType.OnStatementCredit)
                    {
                        httpStatusCode = ProcessVisaEndpointMessage(context, callTimer, () =>
                        {
                            VisaStatementCreditExecutor visaEndpointMessageProcessor =
                                new VisaStatementCreditExecutor(context);
                            return visaEndpointMessageProcessor.Execute();
                        });
                    }
                    else
                    {
                        httpStatusCode = HttpStatusCode.BadRequest;
                        context.Log.Warning("Incoming request message is not of type auth, clearing or statementCredit. Request : {0} ", request.ToString());
                    }

                    if (httpStatusCode != HttpStatusCode.OK)
                    {
                        response.StatusCode = "100";
                        response.ErrorMessage = "General Input Error";
                        context.Log.Warning("Incoming {0} request processing error. Request : {1} ", type,
                                            request.ToString());
                    }
                    return ComposeResponseMessage(response, context, request.MessageId);
                }

                //return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                try
                {
                    if (context == null)
                    {
                        context = CommerceContext.BuildSynchronousContext("Incoming Visa Request", Request, response);
                    }

                    if (message == null)
                    {
                        try
                        {
                            message = Request.Content.ReadAsStringAsync().Result;
                        }
                        catch
                        {
                        }
                    }

                    context.Log.Error("{0} call ended with an error. Message:{1}", ex, context.ApiCallDescription, message);
                }
                catch
                {
                    
                }

                response.StatusCode = "100";
                response.ErrorMessage = "General Input Error";
                return ComposeResponseMessage(response);
            }
        }

        /// <summary>
        /// Compose a response message
        /// </summary>
        /// <param name="response">Visa EPM respons message</param>
        /// <param name="context">for logging</param>
        /// <param name="requestMessageId">for traceability</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static HttpResponseMessage ComposeResponseMessage(VisaEpmResponse response, CommerceContext context = null, string requestMessageId = null)
        {
            if(context != null)
                context.Log.Verbose("Outgoing response for Message {0} : {1}", requestMessageId, General.SerializeJson(response));
            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new ObjectContent<VisaEpmResponse>(response, new JsonMediaTypeFormatter());
            return message;
        }

        /// <summary>
        /// Validates caller's IP address against a range of valid IP addresses.
        /// </summary>
        /// <param name="context">
        /// The context object through which log entries can be made.
        /// </param>
        /// <returns>
        /// * True if the user is authorized.
        /// * Else returns false.
        /// </returns>
        private bool ValidateIP(CommerceContext context)
        {
            // TODO: MUST UPDATE THESE IP's to real values for IP whitelisting!
            bool result = false;
            string ourIP = "10.0";
            string VisaIP1 = "10.0.0.1";
            string VisaIP2 = "10.0.0.2";

            // TEMPORARY FIX: deliberately naive to save time.
            string clientIP = HttpContext.Current.Request.UserHostAddress;
            context.Log.Verbose("Caller has IP address {0}.", clientIP);

//TODO: Now that we know for sure this (IP whitelisting) is the approach Visa wants, move the actual IP addresses to config.
//TODO: Need to find out if UserHostAddress can ever contain port. For now, StartsWith will guard against that possibility.

            if (String.IsNullOrWhiteSpace(clientIP) == false &&
               (clientIP.StartsWith(VisaIP1) == true || clientIP.StartsWith(VisaIP2) == true) || clientIP.StartsWith(ourIP) == true)
            {
                result = true;
                context.Log.Verbose("Caller is authorized.");
            }
            else
            {
                context.Log.Error("Caller is NOT authorized.", null);
            }

            return result;
        }
    }
}