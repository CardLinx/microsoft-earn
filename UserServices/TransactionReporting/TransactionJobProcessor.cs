//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The transaction job processor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionReporting
{
    using System.Threading;
    using System;
    using LoMo.UserServices.DealsMailing;
    using Lomo.Logging;
    using TransactionReporting.Deem;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DealsServerClient;
    using DotM.DataContracts;
    using Microsoft.Azure;
    /// <summary>
    /// The transaction job processor.
    /// </summary>
    public class TransactionJobProcessor
    {
        /// <summary>
        /// The sleep time when the jobs queue empty.
        /// </summary>
        static readonly TimeSpan SleepTimeWhenQueueEmpty = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The sleep time between errors.
        /// </summary>
        readonly TimeSpan _sleepTimeAfterErrors = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The deals server address setting.
        /// </summary>
        private const string DealsServerAddressSetting = "LoMo.DealsServer.Address";

        /// <summary>
        /// The client name.
        /// </summary>
        private const string ClientName = "BO_EMAIL";

        /// <summary>
        /// The agent id.
        /// </summary>
        private readonly string _agentId;

        /// <summary>
        /// The partner transaction jobs queue.
        /// </summary>
        private readonly PartnerTransactionsQueue _jobsQueue;

        /// <summary>
        /// Deals client
        /// </summary>
        private readonly DealsClient _dealsClient;

        private Dictionary<Partners, ITransactionReporter<PartnerTransactionReportingCargo>> _partnerToReporter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionJobProcessor"/> class.
        /// </summary>
        /// <param name="agentId">
        /// The agent id.
        /// </param>
        /// <param name="jobsQueue">
        /// The jobs queue.
        /// </param>
        public TransactionJobProcessor(string agentId, PartnerTransactionsQueue jobsQueue)
        {
            this._agentId = agentId;
            this._jobsQueue = jobsQueue;
            Uri dealsServerBaseAddress = new Uri(CloudConfigurationManager.GetSetting(DealsServerAddressSetting));
            _dealsClient = new DealsClient(dealsServerBaseAddress, ClientName);
            InitializePartnerToReporter();
        }

        /// <summary>
        /// Transaction reporting processor worker method
        /// </summary>
        /// <param name="ct">
        /// The cancellation token. If ct is null the work will continue for ever otherwise it will continue until a cancel request
        /// </param>
        public void DoWork(CancellationToken? ct)
        {
            Log.Info(EventCode.EmailAgentStarted, "Starting Email Agent. Agent Id: {0}", this._agentId);
            while (!ct.HasValue || !ct.Value.IsCancellationRequested)
            {
                this.ProcessNextRequest();
            }

            Log.Info(EventCode.EmailAgentStopped, "Stop Email Agent. Agent Id: {0}", this._agentId);
        }

        /// <summary>
        /// Dequeues an item from the specified queue and processes the message
        /// </summary>
        private void ProcessNextRequest()
        {
            PartnerTransactionReportingCargo partnerTransactionReportingCargo = null;
            try
            {
                if (this._jobsQueue.TryDequeue(out partnerTransactionReportingCargo))
                {
                    Log.Info("Dequeued transaction job from partner-transactions queue. Cargo : {0}", partnerTransactionReportingCargo.ToString());
                    //Get the deal id from the cargo and query the deals server for the deal info
                    List<Guid> dealsGuid = new List<Guid> { new Guid(partnerTransactionReportingCargo.DealId) };
                    Log.Info("Querying the deal server for deal id : {0}", partnerTransactionReportingCargo.DealId);
                    Task<IEnumerable<Deal>> dealsByGuidTask = _dealsClient.GetDealsById(dealsGuid, format: "all");
                    IEnumerable<Deal> deals = dealsByGuidTask.Result.ToList();
                    if (deals.Any())
                    {
                        Deal deal = deals.First();
                        InvokeTransactionReporter(deal, partnerTransactionReportingCargo);
                    }
                    else
                    {
                        Log.Warn(string.Format("Deal {0} not found in deals server", partnerTransactionReportingCargo.DealId));
                    }
                }
                else
                {
                    // No jobs in the queue.
                    Log.Verbose("No jobs in the partner-transaction jobs queue. Agent Id: {0} is going to sleep for {1} seconds", this._agentId, SleepTimeWhenQueueEmpty.TotalSeconds);
                    Thread.Sleep(SleepTimeWhenQueueEmpty);
                }
            }
            catch (Exception exp)
            {
                this.HandleError(EventCode.EmailAgentUnexpectedError, exp, "Unexpected Error", this._agentId, partnerTransactionReportingCargo);
            }
        }

        /// <summary>
        /// Initializes the PartnerToReporter Dictionary and populates with the partners and corresponding reporter instances
        /// </summary>
        private void InitializePartnerToReporter()
        {
            _partnerToReporter = new Dictionary<Partners, ITransactionReporter<PartnerTransactionReportingCargo>>();
            ITransactionReporter<PartnerTransactionReportingCargo> deemReporter = new DeemTransactionReporter<PartnerTransactionReportingCargo>();
            deemReporter.Initialize();
            _partnerToReporter.Add(Partners.Deem, deemReporter);
        }

        /// <summary>
        /// Invokes the appropriate partner transaction reporter based on the provider deal id
        /// </summary>
        /// <param name="deal">Deal being reported</param>
        /// <param name="partnerTransactionReportingCargo">Transaction reporting cargo</param>
        private void InvokeTransactionReporter(Deal deal, PartnerTransactionReportingCargo partnerTransactionReportingCargo)
        {
            string errorMessage;
            if (!string.IsNullOrEmpty(deal.ProviderDealId) && deal.ProviderDealId.IndexOf(':') != -1)
            {
                string dealProvider = deal.ProviderDealId.Substring(0, deal.ProviderDealId.IndexOf(':'));
                Partners partners;
                if (Enum.TryParse(dealProvider.Trim(), true, out partners))
                {
                    switch (partners)
                    {
                        case Partners.Deem:
                            InvokeDeemReporter(deal, partnerTransactionReportingCargo);
                            break;
                    }
                }
                else
                {
                    errorMessage = string.Format(
                            "Cannot report the transaction for the deal {0}. Transaction JobProcessor does not have a reporter registered for provider {1}",
                            partnerTransactionReportingCargo.DealId, dealProvider);
                    Log.Info(errorMessage);
                }
            }
            else
            {
                errorMessage = string.Format(
                            "Cannot report the transaction for the deal {0}. ProviderDealId is not in expected format {1}",
                            partnerTransactionReportingCargo.DealId, deal.ProviderDealId);
                Log.Error(errorMessage);
            }
        }

        /// <summary>
        /// Invokes the Deem transaction reporter
        /// </summary>
        /// <param name="deal">Deal being reported</param>
        /// <param name="partnerTransactionReportingCargo">Transaction reporting cargo</param>
        private void InvokeDeemReporter(Deal deal, PartnerTransactionReportingCargo partnerTransactionReportingCargo)
        {
            //replace the deal id in the cargo with the deem provider deal id
            partnerTransactionReportingCargo.DealId = deal.ProviderDealId.Substring(deal.ProviderDealId.IndexOf(':') + 1);
            //Keep our deal id as the transaction reference
            partnerTransactionReportingCargo.TransactionReference = deal.Id;
            _partnerToReporter[Partners.Deem].Report(partnerTransactionReportingCargo);
        }


        /// <summary>
        /// Handle Agent Job Processing/ Fetching Error
        /// </summary>
        /// <param name="eventCode"> The event code. </param>
        /// <param name="exception">
        /// The exception. </param>
        /// <param name="errorMessagePrefix"> The error message prefix. </param>
        /// <param name="agentId"></param>
        /// <param name="emailCargo"> The email job. </param>
        private void HandleError(int eventCode, Exception exception, string errorMessagePrefix, string agentId, object emailCargo)
        {
            Log.Error(eventCode, exception, this.GetErrorMessage(errorMessagePrefix, agentId, emailCargo));
            Log.Verbose("Agent Id: {0} sleeping after error for {1} seconds", agentId, this._sleepTimeAfterErrors.TotalSeconds);
            Thread.Sleep(this._sleepTimeAfterErrors);
        }

        /// <summary>
        /// The get error message.
        /// </summary>
        /// <param name="prefix"> The message prefix.
        /// </param>
        /// <param name="agentId"></param>
        /// <param name="emailCargo"> The email job. </param>
        /// <returns>The error message. </returns>
        private string GetErrorMessage(string prefix, string agentId, object emailCargo)
        {
            return string.Format(
                "{0}. Agent Id: {1}; Job Details=[{2}]",
                prefix,
                agentId,
                emailCargo);
        }
    }

    /// <summary>
    /// List of partners to report transactions 
    /// </summary>
    public enum Partners
    {
        /// <summary>
        /// Deem partner
        /// </summary>
        Deem
    }
}