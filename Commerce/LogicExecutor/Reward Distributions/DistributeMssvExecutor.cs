//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to distribute Microsoft Store Voucher funds.
    /// </summary>
    public class DistributeMssvExecutor
    {
        /// <summary>
        /// Initializes a new instance of the DistributeMssvExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context describing the deal to register.
        /// </param>
        public DistributeMssvExecutor(CommerceContext context)
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
                // Validate the distribution amount.
                decimal distributionAmount = (decimal)Context[Key.DistributionAmount];
                if (distributionAmount >= 0 && distributionAmount % 10 == 0)
                {
                    // Validate the voucher expiration date.
                    DateTime voucherExpiration = (DateTime)Context[Key.VoucherExpirationUtc];
                    DateTime utcNow = DateTime.UtcNow;
                    if (voucherExpiration > utcNow && voucherExpiration <= utcNow + TimeSpan.FromDays(366))
                    {
                        // HTML encode user-submitted text to prevent script injection and then truncate to maximum length. (SQL injection is prevented through use of a
                        //  paramterized stored procedure.)
                        string notes = Context[Key.Notes] as string;
                        if (String.IsNullOrWhiteSpace(notes) == false)
                        {
                            notes = HttpUtility.HtmlEncode(notes);
                            if (notes.Length > MaxNotesLength)
                            {
                                notes = notes.Substring(0, MaxNotesLength);
                            }
                        }

                        // Submit the distribution request to the database.
                        ResultCode result = CommerceOperationsFactory.RedeemedDealOperations(Context).DistributeMssv();
                        DistributeMssvResponse response = (DistributeMssvResponse)Context[Key.Response];
                        response.RemainingFunds = (decimal)Context[Key.RemainingFunds];
                        resultSummary.SetResultCode(result);
                    }
                    else
                    {
                        resultSummary.SetResultCode(ResultCode.InvalidExpirationDate);
                    }
                }
                else
                {
                    resultSummary.SetResultCode(ResultCode.InvalidDistributionAmount);
                }
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