//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Worker.Actions;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Processes MasterCard rebate files.
    /// </summary>
    public class MasterCardRebateProcessor : ISettlementFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the MasterCardRebateProcessor class.
        /// </summary>
        public MasterCardRebateProcessor()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
        }

        /// <summary>
        /// Processes the PTS file.
        /// </summary>
        public async Task Process()
        {
            // Get the list of MasterCard redemptions marked ReadyForSettlement.
            Collection<OutstandingRedeemedDealInfo> redeemedDealRecords = WorkerActions.RetrieveOutstandingPartnerRedeemedDealRecords(Partner.MasterCard,
                                                                                                                                      RedeemedDealOperations, Context);

            // Build contents of the rebate file from the list of ReadyForSettlement redemptions.
            string rebateFileContents = RebateBuilder.Build(MasterCard.MarshalOutstandingRedeemedDeals(redeemedDealRecords), DateTime.UtcNow);

            // Upload the file to the blob store and to MasterCard, and then mark the redemptions as SettledAsRedeemed.
            // NOTE: Status is immediately marked SettledAsRedeemed, because, unlike other partners, MasterCard only tells us which rebates it rejects.
            if (UploadRebateFile != null)
            {
                await UploadRebateFile(rebateFileContents).ConfigureAwait(false);
                WorkerActions.MarkSettledAsRedeemed(redeemedDealRecords, RedeemedDealOperations, Context);
            }
        }

        /// <summary>
        /// The ID of the task thread in which this object is operating.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Delegate to upload built file to its various destinations.
        /// </summary>
        public Func<string, Task> UploadRebateFile { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRedeemedDealOperations RedeemedDealOperations { get; set; }
    }
}