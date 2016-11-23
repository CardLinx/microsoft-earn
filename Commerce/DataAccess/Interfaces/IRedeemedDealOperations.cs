//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.ObjectModel;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Represents operations on RedeemedDeal objects within the data store.
    /// </summary>f
    public interface IRedeemedDealOperations
    {
        /// <summary>
        /// Adds the redeemed deal in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode AddRedeemedDeal();

        /// <summary>
        /// Updates the data store to reflect the reversal of the redeemed deal described in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode ReverseRedeemedDeal();

        /// <summary>
        /// Updates the data store to reflect the reversal of the timed out redemption described in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode ProcessRedemptionTimeout();

        /// <summary>
        /// Processes the redeemed deal record received from a partner against the redeemed deals in the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode ProcessPartnerRedeemedDealRecord();

        /// <summary>
        /// Retrieves the outstanding redeemed deal records for the partner in the conext.
        /// </summary>
        /// <returns>
        /// The collection of OutstandingRedeemedDealInfo objects built from the outstanding redeemed deal records.
        /// </returns>
        Collection<OutstandingRedeemedDealInfo> RetrieveOutstandingPartnerRedeemedDealRecords();

        /// <summary>
        /// Updates the data store to reflect the pending status of settlement redemptions for a set of redeemed deals.
        /// </summary>
        void UpdatePendingPartnerRedeemedDeals();

        /// <summary>
        /// Updates the data store to reflect a set of redeemed deals as being settled as redeemed.
        /// </summary>
        void MarkSettledAsRedeemed();

        /// <summary>
        /// Updates the data store to reflect the status of settlement redemptions for a set of redeemed deals.
        /// </summary>
        void UpdateRedeemedDeals();

        /// <summary>
        /// Updates in the data store the deal redemption record described by the RebateConfirmationData object in the context as rejected by MasterCard.
        /// </summary>
        /// <returns>
        /// The ResultCode that corresponds to the result of the operation.
        /// </returns>
        /// <remarks>
        /// MasterCard requires special treatment because, unlike other partners, MasterCard has no one ID that can be used to identify a transaction.
        /// </remarks>
        ResultCode MarkRejectedByMasterCard();

        /// <summary>
        /// Retrieves settlement analytics info for a batch of settled deal redemptions.
        /// </summary>
        Collection<SettledDealInfo> RetrieveSettlementAnalyticsInfo();

        /// <summary>
        /// Gets the report for the deals within the query in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode RetrieveDealReports();

        /// <summary>
        /// Gets the number of redemptions for a merchant's deals.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode RetrieveMerchantDealRedemptions();

        /// <summary>
        /// Distributes the amount in the context from the user's Microsoft Store vouchers.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        ResultCode DistributeMssv();

        /// <summary>
        /// Distributes the amount in the context from the user's Microsoft Store vouchers.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        ResultCode MssVoucherDistributionHistory();

        /// <summary>
        /// Retreives the redeemed deal transaction information within the specified dates.
        /// </summary>
        /// <param name="startTimeInclusive">
        /// The start date time (inclusive)
        /// </param>
        /// <param name="endTimeExclusive">
        /// The end datetime (exclusive)
        /// </param>
        /// <returns>
        /// Success if the deal transaction info could be retrieved.
        /// </returns>
        ResultCode GetRedeemedDealTransactionInfosByDate(DateTime startTimeInclusive, DateTime endTimeExclusive);
        
        /// <summary>
        /// Updates the data store to reflect the status of statement of credit for redeemed deals for Visa
        /// </summary>
        void UpdateRedeemedDealsByVisa();
        

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}