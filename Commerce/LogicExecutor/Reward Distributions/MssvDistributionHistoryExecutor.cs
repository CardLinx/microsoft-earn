//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to obtain information on a user's Microsoft Store voucher distribution history.
    /// </summary>
    public class MssvDistributionHistoryExecutor
    {
        /// <summary>
        /// Initializes a new instance of the MSSVDistributionHistoryExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context of the request.
        /// </param>
        public MssvDistributionHistoryExecutor(CommerceContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Executes the distribute MSSV API invocation.
        /// </summary>
        public void Execute()
        {
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];

            // Make sure the user exists.
            SharedUserLogic sharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));
            if (sharedUserLogic.RetrieveUser() != null)
            {
                ResultCode result = CommerceOperationsFactory.RedeemedDealOperations(Context).MssVoucherDistributionHistory();
                MssVoucherDistributionHistory history = (MssVoucherDistributionHistory)Context[Key.MssVoucherDistributionHistory];

                GetMssVoucherDistributionHistoryResponse response = (GetMssVoucherDistributionHistoryResponse)Context[Key.Response];
                response.AmountRemaining = (decimal)history.AmountRemaining / 100;

                List<DistributionHistoryDataContract> distributionHistoryList = new List<DistributionHistoryDataContract>();
                response.DistributionHistory = distributionHistoryList;
                foreach (DistributionHistory distributionHistoryItem in history.DistributionHistory)
                {
                    DistributionHistoryDataContract distributionHistoryItemDataContract = new DistributionHistoryDataContract
                    {
                        DistributionDate = distributionHistoryItem.DistributionDate,
                        Amount = (decimal)distributionHistoryItem.Amount / 100,
                        Currency = distributionHistoryItem.Currency,
                        ExpirationDate = distributionHistoryItem.ExpirationDate
                    };
                    distributionHistoryList.Add(distributionHistoryItemDataContract);
                }

                List<TransactionHistoryDataContract> transactionHistoryList = new List<TransactionHistoryDataContract>();
                response.TransactionHistory = transactionHistoryList;
                foreach (TransactionHistory transactionHistoryItem in history.TransactionHistory)
                {
                    TransactionHistoryDataContract transactionHistoryItemDataContract = new TransactionHistoryDataContract
                    {
                        Business = transactionHistoryItem.Business,
                        CreditStatus = transactionHistoryItem.CreditStatus.ToString(),
                        PurchaseDate = transactionHistoryItem.PurchaseDate,
                        DiscountAmount = (decimal)transactionHistoryItem.DiscountAmount / 100
                    };
                    transactionHistoryList.Add(transactionHistoryItemDataContract);
                }

                resultSummary.SetResultCode(result);
            }
            else
            {
                resultSummary.SetResultCode(ResultCode.UnexpectedUnregisteredUser);
            }
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// The maximum acceptable length for user-submitted notes. Submission is truncated at 500 characters.
        /// </summary>
        private const int MaxNotesLength = 500;
    }
}