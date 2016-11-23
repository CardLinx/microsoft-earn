//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;
    using Lomo.Commerce.AmexClient;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Worker.Actions;

    /// <summary>
    /// Class to create Statement Credit File
    /// </summary>
    public class StatementCreditFileBuilder
    {
        /// <summary>
        /// Initializes a new instance of the StatementCreditFileBuilder class.
        /// </summary>
        public StatementCreditFileBuilder()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
        }

        /// <summary>
        /// Build the string representation of the file.
        /// </summary>
        /// <returns>
        /// String representation of the file or null
        /// </returns>
        public async Task Build(Func<string, Task> onStatementCreditFileBuildFunc)
        {
            Collection<OutstandingRedeemedDealInfo> redeemedDealRecords = WorkerActions.RetrieveOutstandingPartnerRedeemedDealRecords(Partner.Amex,
                                                                        RedeemedDealOperations, Context);

            if (onStatementCreditFileBuildFunc != null)
            {
                // this will ftp the data and store it in blob
                StatementCreditFile file = BuildCreditFile(redeemedDealRecords);
                string result = ConvertFileToString(file);
                await onStatementCreditFileBuildFunc(result).ConfigureAwait(false);

                // Commenting the earlier implementation of marking the transactions as StatementCreditRequested
                //WorkerActions.UpdatePendingRedeemedDeals(redeemedDealRecords, CreditStatus.StatementCreditRequested, RedeemedDealOperations, Context);

                // follow the master card pattern to mark as settled unless we get an error from amex
                WorkerActions.MarkSettledAsRedeemed(redeemedDealRecords, RedeemedDealOperations, Context);
            }
        }

        internal static string ConvertFileToString(StatementCreditFile file)
        {
            if (file == null)
            {
                return null;
            }

            // convert file to string representation here
            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.Append(file.Header.BuildFileHeader());
            fileBuilder.Append("\n");
            foreach (StatementCreditDetail statementCreditRecord in file.StatementCreditRecords)
            {
                fileBuilder.Append(statementCreditRecord.BuildFileDetailRecord());
                fileBuilder.Append("\n");
            }
            fileBuilder.Append(file.Trailer.BuildFileTrailer());
            return fileBuilder.ToString();
        }

        /// <summary>
        /// Build the statement credit file
        /// </summary>
        /// <param name="result">
        /// Collection of records to build credit for
        /// </param>
        /// <returns>
        /// Instance of StatmentCreditFile <see cref="StatementCreditFile"/>
        /// </returns>
        internal StatementCreditFile BuildCreditFile(Collection<OutstandingRedeemedDealInfo> result)
        {
            StatementCreditFile file = null;
            if (result.Count > 0)
            {
                Context[Key.SequenceName] = "AmexStatementCreditSequence";
                SharedSequenceLogic sequenceLogic = new SharedSequenceLogic(Context, CommerceOperationsFactory.SequenceOperations(Context));
                int sequenceNumber = sequenceLogic.RetrieveNextValueInSequence();

                file = new StatementCreditFile()
                {
                    Header = new StatementCreditHeader()
                    {
                        Date = DateTime.UtcNow,
                        SequenceNumber = sequenceNumber
                    }
                };
                int totalAmount = 0;
                foreach (OutstandingRedeemedDealInfo outstandingRedeemedDealInfo in result)
                {
                    totalAmount += outstandingRedeemedDealInfo.DiscountAmount;
                    StatementCreditDetail detail = new StatementCreditDetail()
                    {
                        CampaignName = outstandingRedeemedDealInfo.MerchantName,
                        CardToken = outstandingRedeemedDealInfo.Token,
                        DiscountAmount = (decimal)outstandingRedeemedDealInfo.DiscountAmount / 100,
                        OfferId = outstandingRedeemedDealInfo.OfferId,
                        StatementDescriptor = StatmentDescriptor(outstandingRedeemedDealInfo.MerchantName),
                        TransactionId = outstandingRedeemedDealInfo.ReferenceNumber.ToString(CultureInfo.InvariantCulture)
                    };
                    file.StatementCreditRecords.Add(detail);
                }
                file.Trailer = new StatementCreditTrailer()
                {
                    TrailerAmount = (decimal)totalAmount / 100,
                    TrailerCount = file.StatementCreditRecords.Count
                };
            }


            return file;
        }

        /// <summary>
        /// Build 16 char long statement descriptor
        /// </summary>
        /// <param name="merchantName">
        /// Name of the merchant
        /// </param>
        /// <returns>
        /// String representation of the descriptor
        /// </returns>
        private static string StatmentDescriptor(string merchantName)
        {
            string prefix = "Bing-";
            string merchantPrefix = null;
            if (merchantName.Length < 11)
            {
                merchantPrefix = merchantName;
            }
            else
            {
                merchantPrefix = merchantName.Substring(0, 11);
            }
            return prefix + merchantPrefix;
        }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRedeemedDealOperations RedeemedDealOperations { get; set; }
    }
}