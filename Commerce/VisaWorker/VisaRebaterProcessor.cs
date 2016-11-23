//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.CardLink.Partners;
using Lomo.Commerce.Configuration;
using Lomo.Commerce.Context;
using Lomo.Commerce.DataAccess;
using Lomo.Commerce.DataContracts;
using Lomo.Commerce.DataContracts.Extensions;
using Lomo.Commerce.DataModels;
using Lomo.Commerce.Utilities;
using Lomo.Commerce.Worker.Actions;
using Lomo.Commerce.WorkerCommon;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Lomo.Commerce.VisaWorker
{
    /// <summary>
    /// Processes Visa clearing transactions.
    /// </summary>
    public class VisaRebaterProcessor : ISettlementProcessor
    {
        /// <summary>
        /// Initializes a new instance of the VisaPtsProcessor class.
        /// </summary>
        public VisaRebaterProcessor()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            UserOperations = CommerceOperationsFactory.UserOperations(Context);
        }

        /// <summary>
        /// Processes Visa clearing transactions.
        /// </summary>
        /// <returns></returns>
        public async Task Process()
        {
            var outstandingRedeemedDeals = WorkerActions.RetrieveOutstandingPartnerRedeemedDealRecords(Partner.Visa, RedeemedDealOperations, Context);

            if (outstandingRedeemedDeals != null && outstandingRedeemedDeals.Any())
            {
                foreach (var outstandingDeal in outstandingRedeemedDeals)
                {

                    try
                    {
                        
                        var partnerUserId = WorkerActions.RetrieveUserId(outstandingDeal.GlobalUserId, Partner.Visa, UserOperations, Context);
                        
                        Context[Key.CardToken] = outstandingDeal.Token;
                        Context[Key.DealDiscountAmount] = outstandingDeal.DiscountAmount;
                        Context[Key.Transaction] = outstandingDeal.PartnerRedeemedDealScopeId;
                        Context[Key.TransactionSettlementDate] = outstandingDeal.TransactionDate;
                        Context[Key.PartnerUserId] = partnerUserId;

                        UpdatePendingRedeemedDeals(outstandingDeal, CreditStatus.SendingStatementCreditRequest);

                        ResultCode resultCode;
                        try
                        {
                            IVisaClient client = new CardLink.Visa(Context);
                            resultCode = await client.SaveStatementCreditAsync();
                        }
                        catch (Exception ex)
                        {
                            Context.Log.Error("Error submitting statement of credit to Visa for outstanding redeemed dealId {0}.", ex, outstandingDeal.DealId);
                            UpdatePendingRedeemedDeals(outstandingDeal, CreditStatus.SendingStatementCreditRequestFailed);
                            continue;
                        }

                        if (resultCode == ResultCode.Created)
                        {
                            UpdatePendingRedeemedDeals(outstandingDeal, CreditStatus.StatementCreditRequested);
                        }
                        else
                        {
                            UpdatePendingRedeemedDeals(outstandingDeal, CreditStatus.RejectedByPartner);
                        }

                    }
                    catch (Exception ex)
                    {
                        Context.Log.Error("Error processing outstanding redeemed dealId {0}.", ex, outstandingDeal.DealId);
                        UpdatePendingRedeemedDeals(outstandingDeal, CreditStatus.GeneratingStatementCreditRequestFailed);
                    }
                }

                //outstandingRedeemedDeals = WorkerActions.RetrieveOutstandingPartnerRedeemedDealRecords(Partner.Visa, RedeemedDealOperations, Context);
            }
        }

        /// <summary>
        /// Updates the pending redeemed deals in the merchant record list to the credit status specified, if the credit status
        /// is valid.
        /// </summary>
        /// <param name="record">
        /// Merchant record whose redeemed deal to update.
        /// </param>
        /// <param name="creditStatus">
        /// The credit status to which to set the redeemed deals.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Parameter creditStatus contains an invalid CreditStatus for this operation.
        /// </exception>
        internal void UpdatePendingRedeemedDeals(OutstandingRedeemedDealInfo record, CreditStatus creditStatus)
        {
            // Update the credit status for the specified list of merchant records.
            WorkerActions.UpdatePendingRedeemedDeals(new Collection<OutstandingRedeemedDealInfo> {record}, creditStatus, RedeemedDealOperations, Context);
        }
        
        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRedeemedDealOperations RedeemedDealOperations { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for User Operations.
        /// </summary>
        private IUserOperations UserOperations { get; set; }
    }
}