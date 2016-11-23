//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.Worker.Actions;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Processes MasterCard filtering files.
    /// </summary>
    public class MasterCardFilteringProcessor : ISettlementFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the MasterCardFilteringProcessor class.
        /// </summary>
        public MasterCardFilteringProcessor()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            CardOperations = CommerceOperationsFactory.CardOperations(Context);
        }

        /// <summary>
        /// Processes the PTS file.
        /// </summary>
        public async Task Process()
        {
            // Build contents of the filtering file from the list of unfiltered MasterCard cards.
            IEnumerable<string> unfilteredMasterCards = WorkerActions.RetrieveUnfilteredMasterCards(CardOperations, Context);
            if (unfilteredMasterCards.Count() > 0)
            {
                Collection<FilteringRecord> filteringRecords = new Collection<FilteringRecord>();
                foreach(string unfilteredMasterCard in unfilteredMasterCards)
                {
                    filteringRecords.Add(new FilteringRecord
                    {
                        AuthorizationSetId = CommerceWorkerConfig.Instance.MasterCardAuthorizationSetId,
                        BankCustomerNumber = unfilteredMasterCard,
                        ClearingSetId = CommerceWorkerConfig.Instance.MasterCardClearingSetId,
                        EffectiveDate = DateTime.UtcNow.AddDays(-CommerceWorkerConfig.Instance.MasterCardExpectedFilteringDaysDelta),
                        Threshold = (decimal)CommerceWorkerConfig.Instance.MasterCardTransactionNotificationThreshold
                    });
                }

                // Upload the file to the blob store and to MasterCard, and then mark the cards with the filter added date.
                if (UploadFilteringFile != null)
                {
                    await UploadFilteringFile(FilteringBuilder.Build(filteringRecords, DateTime.UtcNow)).ConfigureAwait(false);
                    AddFilteredMasterCards(unfilteredMasterCards);
                }
            }
        }

        /// <summary>
        /// The ID of the task thread in which this object is operating.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Adds to the data store the filtering date for the MasterCards cards in the specified list.
        /// </summary>
        /// <param name="filteredMasterCards">
        /// The list of MasterCard cards whose filtering data to add to the data store.
        /// </param>
        public void AddFilteredMasterCards(IEnumerable<string> filteredMasterCards)
        {
            WorkerActions.AddFilteredMasterCards(filteredMasterCards, CardOperations, Context);
        }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Delegate to upload built file to its various destinations.
        /// </summary>
        public Func<string, Task> UploadFilteringFile { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for Card operations.
        /// </summary>
        private ICardOperations CardOperations { get; set; }
    }
}