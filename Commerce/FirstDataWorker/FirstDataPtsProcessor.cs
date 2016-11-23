//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Worker.Actions;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Processes First Data PTS files.
    /// </summary>
    public class FirstDataPtsProcessor : ISettlementFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the FirstDataPtsProcessor class.
        /// </summary>
        public FirstDataPtsProcessor()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
        }

        /// <summary>
        /// Processes the PTS file.
        /// </summary>
        public async Task Process()
        {
            Collection<OutstandingRedeemedDealInfo> result = WorkerActions.RetrieveOutstandingPartnerRedeemedDealRecords(Partner.FirstData,
                                                                        RedeemedDealOperations, Context);
            if (OnPtsBuild != null)
            {
                // FDC requires EST.
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime estNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, easternZone);

                // this will ftp the data and store it in blob
                // Always construct file as sequence number of 1, because FDC PTS job runs first each day.
                string ptsFileContents = PtsBuilder.Build(result, estNow, 1);
                await OnPtsBuild(ptsFileContents).ConfigureAwait(false);
                // now update db
                UpdatePendingRedeemedDeals(result, CreditStatus.StatementCreditRequested);

                if (TransactionPublisher != null)
                {
                    foreach (OutstandingRedeemedDealInfo outstandingRedeemedDealInfo in result)
                    {
                        TransactionDetail transactionDetail = new TransactionDetail()
                        {
                            TransactionDate = outstandingRedeemedDealInfo.TransactionDate.ToString("yyMMdd-HHmmss"),
                            DealId = outstandingRedeemedDealInfo.DealId,
                            DiscountId = outstandingRedeemedDealInfo.DiscountId,
                            DiscountAmount = outstandingRedeemedDealInfo.DiscountAmount,
                            SettlementAmount = outstandingRedeemedDealInfo.SettlementAmount
                        };

                        await TransactionPublisher.PublishTransactionAsync(transactionDetail).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// The ID of the task thread in which this object is operating.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Updates the pending redeemed deals in the merchant record list to the credit status specified, if the credit status
        /// is valid.
        /// </summary>
        /// <param name="records">
        /// The list of merchant records whose redeemed deals to update.
        /// </param>
        /// <param name="creditStatus">
        /// The credit status to which to set the redeemed deals.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Parameter creditStatus contains an invalid CreditStatus for this operation.
        /// </exception>
//TODO: Make internal once Worker role is up and running.
        public void UpdatePendingRedeemedDeals(Collection<OutstandingRedeemedDealInfo> records,
                                                 CreditStatus creditStatus)
        {
            // Ensure specified credit status is valid for this operation.
            if (creditStatus != CreditStatus.StatementCreditRequested && creditStatus != CreditStatus.CreditGranted)
            {
                throw new InvalidOperationException("Parameter creditStatus contains an invalid CreditStatus for this operation.");
            }

            // Update the credit status for the specified list of merchant records.
            WorkerActions.UpdatePendingRedeemedDeals(records, creditStatus, RedeemedDealOperations, Context);
        }

        /// <summary>
        /// Gets or sets the transaction publisher.
        /// If transactions need to be published to downstream systems, set relevant publisher here.
        /// </summary>
        public ITransactionPublisher TransactionPublisher { get; set; }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Delegate to do post processing after pts contents are built
        /// </summary>
        public Func<string, Task> OnPtsBuild { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRedeemedDealOperations RedeemedDealOperations { get; set; }
    }
}