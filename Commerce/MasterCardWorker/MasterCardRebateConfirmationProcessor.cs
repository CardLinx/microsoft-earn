//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Worker.Actions;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Scheduler;

    /// <summary>
    /// Processes MasterCard rebate confirmation files.
    /// </summary>
    public class MasterCardRebateConfirmationProcessor : ISettlementFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the MasterCardRebateConfirmationProcessor class.
        /// </summary>
        public MasterCardRebateConfirmationProcessor()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
        }

        /// <summary>
        /// Processes the rebate confirmation file.
        /// </summary>
        public async Task Process()
        {
            await Task.Factory.StartNew(ProcessRebateConfirmation).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes the rebate confirmation file.
        /// </summary>
        private void ProcessRebateConfirmation()
        {
            // Deserialize rebate confirmation file into a RebateConfirmation object.
            RebateConfirmationParser rebateConfirmationParser = new RebateConfirmationParser(Context.Log);
            RebateConfirmation rebateConfirmation = rebateConfirmationParser.Parse(FileName, Stream);

            if (rebateConfirmation != null)
            {
                for (int count = 0; count < rebateConfirmation.DataRecords.Count; count++)
                {
                    RebateConfirmationData rebateConfirmationData = rebateConfirmation.DataRecords[count];
                    if (rebateConfirmationData != null)
                    {
                        // Mark the redemption as RejectedByPartner.
                        Context[Key.RebateConfirmationData] = rebateConfirmationData;
                        ResultCode result = RedeemedDealOperations.MarkRejectedByMasterCard();

                        // Log warning if needed.
                        switch(result)
                        {
                            case ResultCode.MatchingRedeemedDealNotFound:
                                Context.Log.Warning("RebateConfirmationData record #{0} could not be marked RejectedByPartner, because " +
                                                    "no matching redemption record could be found.", (int)result, count + 1);
                                break;
                            case ResultCode.MultipleMatchingRedeemedDealsFound:
                                Context.Log.Warning("More than one matching redemption record matched RebateConfirmationData record " +
                                                    "#{0}. One of these was marked RejectedByPartner, but since the data is " +
                                                    "ambiguous, it may not correspond to the correct redemption.", (int)result, count + 1);
                                break;
                            case ResultCode.RedeemedDealFoundIsInexactMatch:
                                Context.Log.Warning("A matching redemption record for RebateConfirmationData record #{0} was marked " +
                                                    "RejectedByPartner, but the record was not an exact match.", (int)result, count + 1);
                                break;
                        };
                    }
                }
            }
        }

        /// <summary>
        /// The ID of the task thread in which this object is operating.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the stream containing rebate confirmation file contents.
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// Gets or sets the name of the rebate confirmation file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRedeemedDealOperations RedeemedDealOperations { get; set; }
    }
}