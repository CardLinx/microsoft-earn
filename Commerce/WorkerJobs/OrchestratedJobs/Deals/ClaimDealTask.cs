//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Net;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;
    using Newtonsoft.Json;

    /// <summary>
    /// Orchestrated task to claim a deal for a user's card.
    /// </summary>
    public class ClaimDealTask : IOrchestratedTask
    {
        /// <summary>
        /// Initializes a new instance of the ClaimDealTask class.
        /// </summary>
        /// <param name="claimDealPayload">
        /// The claimDealPayload to send to the ClaimDeal API.
        /// </param>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public ClaimDealTask(ClaimDealPayload claimDealPayload,
                             CommerceLog log)
        {
            ClaimDealPayload = claimDealPayload;
            Log = log;
        }

        /// <summary>
        /// Initializes the ClaimDealTask instance.
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the job being run.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler managing the jobs.
        /// </param>
        public void Initialize(ScheduledJobDetails jobDetails,
                               IScheduler scheduler)
        {
            JobDetails = jobDetails;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>
        /// The result of the execution of the task.
        /// </returns>
        public OrchestratedExecutionResult Execute()
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.NonTerminalError;

            // Call the MBI endpoint to claim the deal.
            Uri uri = new Uri(String.Concat((string) CommerceWorkerConfig.Instance.MbiServiceAuthority,
                                            (string) CommerceWorkerConfig.Instance.ServiceClaimedDealsControllerEndpoint));
            HttpStatusCode httpStatusCode = HttpStatusCode.Forbidden;
            string response = String.Empty;
            
            try
            {
                response = RestServiceClient.CallRestService(uri, RestServiceVerbs.Post, RestServiceContentTypes.Json,
                                                             General.SerializeJson(ClaimDealPayload),
                                                             RestServiceClient.ObtainAuthorizationToken("claimeddeals"),
                                                             out httpStatusCode);
            }
            catch(WebException ex)
            {
                Log.Warning("An unknown exception was thrown during the service call. Job will be retried. " +
                            "Exception:\r\n{0}", ex);
            }

            // If a response was received, try to deserialize the response body into a ClaimDealResponse.
            if (String.IsNullOrWhiteSpace(response) == false)
            {
                ClaimDealResponse claimDealResponse = null;
                try
                {
                    claimDealResponse = General.DeserializeJson<ClaimDealResponse>(response);
                }
                catch (JsonReaderException)
                {
                    claimDealResponse = null;
                }

                // If we have a deserialized response, process it.
                if (claimDealResponse != null)
                {
                    result = ProcessResponse(claimDealResponse, httpStatusCode);
                }
                // Otherwise, log the call response and try again.
                else
                {
                    Log.Warning("Unexpected service response encountered during ClaimDealTask execution. Job will be retried. " +
                                "Response:\r\n{0}", response);
                    result = OrchestratedExecutionResult.NonTerminalError;
                }
            }

            return result;
        }

        /// <summary>
        /// Process the response of Claim Deal API call
        /// </summary>
        /// <param name="claimDealResponse">
        /// Response of the call
        /// </param>
        /// <returns>
        /// OrchestratedExecutionResult
        /// </returns>
        internal OrchestratedExecutionResult ProcessResponse(ClaimDealResponse claimDealResponse, HttpStatusCode httpStatusCode)
        {
            OrchestratedExecutionResult result;
            ResultCode resultCode;

            switch (httpStatusCode)
            {
                case HttpStatusCode.Created:
                case HttpStatusCode.OK:
                    JobDetails.Payload[ClaimDealPayload.ClaimDealInfo.DealId.ToString()] = DealMarker;
                    JobDetails.Payload[ClaimDealPayload.ClaimDealInfo.CardId.ToString()] = CardMarker;
                    result = OrchestratedExecutionResult.Success;
                    break;
                case HttpStatusCode.Forbidden:
                default:
                    resultCode = (ResultCode)Enum.Parse(typeof(ResultCode), claimDealResponse.ResultSummary.ResultCode);
                    if (resultCode == ResultCode.UnregisteredCard)
                    {
                        Log.Verbose("Card is not registered to this partner. Aborting attempt to link deals to this card.", (int)resultCode);
                        result = OrchestratedExecutionResult.Success;
                    }
                    else if (resultCode == ResultCode.UnregisteredDeal)
                    {
                        // This is actually not unregistered deal with deal not registered with a particular partner.
                        // For example, trying to claim a deal with an amex card with a partner that deal is not registered with.
                        Log.Warning("Deal is not registered to the partner we are trying to claim it with", (int)resultCode);
                        JobDetails.Payload[ClaimDealPayload.ClaimDealInfo.DealId.ToString()] = DealMarker;
                        JobDetails.Payload[ClaimDealPayload.ClaimDealInfo.CardId.ToString()] = CardMarker;
                        result = OrchestratedExecutionResult.Success;
                    }
                    else
                    {
                        Log.Warning("Unexpected result encountered during ClaimDealTask execution. ResultCode: {0}. Job " +
                                    "will be retried.", (int)resultCode, resultCode);
                        result = OrchestratedExecutionResult.NonTerminalError;
                    }
                    break;
            }

            return result;

        }

        /// <summary>
        /// String that marks an ID as belonging to a deal.
        /// </summary>
        public const string DealMarker = "Deal";

        /// <summary>
        /// String that marks an ID as belonging to a card.
        /// </summary>
        public const string CardMarker = "Card";

        /// <summary>
        /// Gets or sets the claimDealPayload to send to the ClaimDeal API.
        /// </summary>
        private ClaimDealPayload ClaimDealPayload { get; set; }

        /// <summary>
        /// Gets or sets the details of the job being run.
        /// </summary>
        private ScheduledJobDetails JobDetails { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}