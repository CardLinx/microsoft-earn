//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.MasterCardClient;
    using Lomo.Commerce.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// Represents operations on RedeemedDeal objects within the data store.
    /// </summary>
    public class RedeemedDealOperations : CommerceOperations, IRedeemedDealOperations
    {
        /// <summary>
        /// Adds the redeemed deal in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddRedeemedDeal()
        {
            int? offset = GetPartnerMerchantTimeZoneOffset();

            ResultCode result = ResultCode.Success;

            RedeemedDeal redeemedDeal = (RedeemedDeal)Context[Key.RedeemedDeal];
            Context[Key.RedeemedDealInfo] = new RedeemedDealInfo();

            // TEMPORARY: Determine if the allow list forces transaction rejection.
            if (PhysStoreFilter() == true)
            {
                PartnerMerchantIdType partnerMerchantIdType = PartnerMerchantIdType.Universal;
                if (Context.ContainsKey(Key.PartnerMerchantIdType) == true)
                {
                    partnerMerchantIdType = (PartnerMerchantIdType)Context[Key.PartnerMerchantIdType];
                }

//Context.Log.Critical("RedeemedDealId: {0}\r\nPartnerId: {1}\r\nPartnerDealId: {2}\r\nPartnerCardId: {3}\r\nPartnerClaimedDealId: {4}\r\nCardId: {5}\r\nDealId: {6}\r\nPartnerMerchantId: {7}\r\n" +
//                    "OutletPartnerMerchantId: {8}\r\nPartnerMerchantIdType: {9}\r\nCallbackEvent: {10}\r\nPurchaseDateTime: {11}\r\nAuthorizationAmount: {12}\r\nCurreny: {13}\r\n" +
//                    "PartnerRedeemedDealScopeId: {14}\r\nPartnerRedeemedDealId: {15}\r\nAnalyticsEventId: {16}\r\nCreditStatus: {17}\r\nPartnerReferenceNumber: {18}\r\nPartnerData: {19}\r\noffset: {20}", null,
//                     redeemedDeal.Id, (int)Context[Key.Partner], Context[Key.PartnerDealId], Context[Key.PartnerCardId], Context[Key.PartnerClaimedDealId], Context[Key.CardId], Context[Key.DealId], Context[Key.PartnerMerchantId],
//                     Context[Key.OutletPartnerMerchantId], (int)partnerMerchantIdType, (int)redeemedDeal.CallbackEvent, redeemedDeal.PurchaseDateTime, redeemedDeal.AuthorizationAmount,  redeemedDeal.Currency,
//                     redeemedDeal.PartnerRedeemedDealScopeId, redeemedDeal.PartnerRedeemedDealId, redeemedDeal.AnalyticsEventId, Context[Key.CreditStatus], Context[Key.PartnerReferenceNumber], Context[Key.PartnerData],
//                     offset);

                result = SqlProcedure("AddRedeemedDeal",
                                        new Dictionary<string, object>
                                        {
                                            { "@redeemedDealId", redeemedDeal.Id },
                                            { "@partnerId", (int)Context[Key.Partner] },
                                            { "@recommendedPartnerDealId", Context[Key.PartnerDealId] },
                                            { "@partnerCardId", Context[Key.PartnerCardId] },
                                            { "@recommendedPartnerClaimedDealId", Context[Key.PartnerClaimedDealId] },
                                            { "@recommendedCardId", Context[Key.CardId] },
                                            { "@recommendedDealId", Context[Key.DealId] },
                                            { "@partnerMerchantId", Context[Key.PartnerMerchantId] },
                                            { "@outletPartnerMerchantId", Context[Key.OutletPartnerMerchantId] },
                                            { "@partnerMerchantIdTypeId", (int)partnerMerchantIdType },
                                            { "@redemptionEventId", (int)redeemedDeal.CallbackEvent },
                                            { "@purchaseDateTime", redeemedDeal.PurchaseDateTime },
                                            { "@authAmount", redeemedDeal.AuthorizationAmount },
                                            { "@currency", redeemedDeal.Currency },
                                            { "@partnerRedeemedDealScopeId", redeemedDeal.PartnerRedeemedDealScopeId },
                                            { "@partnerRedeemedDealId", redeemedDeal.PartnerRedeemedDealId },
                                            { "@analyticsRedemptionEventId", redeemedDeal.AnalyticsEventId },
                                            { "@creditStatusId", Context[Key.CreditStatus] },
                                            { "@partnerReferenceNumber", Context[Key.PartnerReferenceNumber] },
                                            { "@partnerData", Context[Key.PartnerData] },
                                            { "@timeZoneOffset", offset }
                                        },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        RedeemedDealInfo redeemedDealInfo = new RedeemedDealInfo
                        {
                            GlobalId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalDealId")),
                            Currency = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Currency")),
                            Amount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Amount")),
                            Percent = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Percent")),
                            MinimumPurchase = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("MinimumPurchase")),
                            GlobalUserId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalUserId")),
                            PartnerDealId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerDealId")),
                            PartnerClaimedDealId = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("PartnerClaimedDealId")) ? null : sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerClaimedDealId")),
                            PartnerRedeemedDealId = redeemedDeal.PartnerRedeemedDealId,
                            LastFourDigits = sqlDataReader.GetString(sqlDataReader.GetOrdinal("LastFourDigits")),
                            DiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount")),
                            PartnerMerchantId = (string)Context[Key.PartnerMerchantId],
                            TransactionId = redeemedDeal.Id.ToString(),
                            TransactionDate = redeemedDeal.PurchaseDateTime,
                            PartnerId = (int)Context[Key.Partner],
                            ReimbursementTenderId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ReimbursementTenderId"))
                        };

                        // Function based partner claimed deal id is not stored (comes as null or empty string). Generate it.
                        if (string.IsNullOrEmpty(redeemedDealInfo.PartnerClaimedDealId))
                        {
                            int cardId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CardId"));
                            int dealId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DealId"));
                            redeemedDealInfo.PartnerClaimedDealId = General.TwoIntegersToHexString(cardId, dealId);
                        }

                        // Add nullable items.
                        int merchantNameColumnId = sqlDataReader.GetOrdinal("MerchantName");
                        if (sqlDataReader.IsDBNull(merchantNameColumnId) == false)
                        {
                            redeemedDealInfo.MerchantName = sqlDataReader.GetString(merchantNameColumnId);
                        }

                        int parentDealIdColumnId = sqlDataReader.GetOrdinal("ParentDealId");
                        if (sqlDataReader.IsDBNull(parentDealIdColumnId) == false)
                        {
                            redeemedDealInfo.ParentDealId = sqlDataReader.GetGuid(parentDealIdColumnId);
                        }

                        int partnerUserIdColumnId = sqlDataReader.GetOrdinal("PartnerUserId");
                        if (sqlDataReader.IsDBNull(partnerUserIdColumnId) == false)
                        {
                            redeemedDealInfo.PartnerUserId = sqlDataReader.GetString(partnerUserIdColumnId);
                        }

                        int discountSummaryColumnId = sqlDataReader.GetOrdinal("DiscountSummary");
                        if (sqlDataReader.IsDBNull(discountSummaryColumnId) == false)
                        {
                            redeemedDealInfo.DiscountSummary = sqlDataReader.GetString(discountSummaryColumnId);
                        }

                        // Populate the context.
                        Context[Key.RedeemedDealInfo] = redeemedDealInfo;
                    }
                });
            }
            // TEMPORARY: Log the disallowed transaction.
            else
            {
                result = ResultCode.NoApplicableDealFound;
                Context.Log.Warning("Transaction disallowed. Card / Allow list mismatch.");
            }

            if (result == ResultCode.Success)
            {
                result = ResultCode.Created;
            }

            return result;
        }

        /// <summary>
        /// Updates the data store to reflect the reversal of the redeemed deal described in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode ReverseRedeemedDeal()
        {
            ResultCode result;

            Context[Key.ReverseRedeemedDealInfo] = new ReverseRedeemedDealInfo();
            result = SqlProcedure("ReverseRedeemedDeal",
                                  new Dictionary<string, object>
                                  {
                                      { "@partnerId", (int)Context[Key.Partner] },
                                      { "@partnerRedeemedDealId", Context[Key.PartnerRedeemedDealId] },
                                      { "@redemptionEventId", Context[Key.RedemptionEvent] },
                                      { "@partnerDealId", Context[Key.PartnerDealId] },
                                      { "@partnerCardId", Context[Key.PartnerCardId] },
                                      { "@partnerMerchantId", Context[Key.PartnerMerchantId] }
                                  },
                                  BuildReverseRedeemedDealInfo);

            return result;
        }

        /// <summary>
        /// Updates the data store to reflect the reversal of the timed out redemption described in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode ProcessRedemptionTimeout()
        {
            ResultCode result;

            Context[Key.ReverseRedeemedDealInfo] = new ReverseRedeemedDealInfo();
            result = SqlProcedure("ProcessRedemptionTimeout",
                                  new Dictionary<string, object>
                                  {
                                      { "@partnerId", (int)Context[Key.Partner] },
                                      { "@authAmt", Context[Key.AuthorizationAmount] },
                                      { "@redemptionEventId", Context[Key.RedemptionEvent] },
                                      { "@recommendedPartnerClaimedDealId", Context[Key.PartnerClaimedDealId] },
                                      { "@recommendedPartnerDealId", Context[Key.PartnerDealId] },
                                      { "@partnerCardId", Context[Key.PartnerCardId] },
                                      { "@partnerMerchantId", Context[Key.PartnerMerchantId] },
                                      { "@partnerOfferMerchantId", Context[Key.PartnerOfferMerchantId] },
                                      { "@purchaseDateTime", Context[Key.PurchaseDateTime] }
                                  },
                                  BuildReverseRedeemedDealInfo);

            return result;
        }

        /// <summary>
        /// Processes the redeemed deal record received from a partner against the redeemed deals in the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode ProcessPartnerRedeemedDealRecord()
        {
            ResultCode result;

            // Get the settlement detail record and use it to determine the values to send to the stored procedure.
            SettlementDetail settlementDetail = (SettlementDetail)Context[Key.SettlementDetail];
            int settlementAmount = (int)(settlementDetail.TotalTransactionAmount * 100);
            int discountAmount = (int)(settlementDetail.RedemptionDiscountAmount * 100);
            RedemptionEvent redemptionEvent = RedemptionEvent.None;
            bool reversed = false;
            switch (settlementDetail.TransactionType)
            {
                case TransactionType.RealTimeRedemption:
                    redemptionEvent = RedemptionEvent.RealTime;
                    reversed = false;
                    break;
                case TransactionType.RealTimeTimeoutReversal:
                    redemptionEvent = RedemptionEvent.RealTime;
                    reversed = true;
                    break;
                case TransactionType.RealTimeNonTimeoutReversal:
                    redemptionEvent = RedemptionEvent.RealTime;
                    reversed = true;
                    break;
                case TransactionType.SettlementRedemption:
                    redemptionEvent = RedemptionEvent.Settlement;
                    reversed = false;
                    break;
                case TransactionType.SettlementTimeoutReversal:
                    redemptionEvent = RedemptionEvent.Settlement;
                    reversed = true;
                    break;
                case TransactionType.SettlementNonTimeoutReversal:
                    redemptionEvent = RedemptionEvent.Settlement;
                    reversed = true;
                    break;
            }

            Context[Key.PartnerMerchantId] = settlementDetail.LocationMid;
            int? offset = GetPartnerMerchantTimeZoneOffset();

            // Invoke the stored procedure.
            result = SqlProcedure("ProcessPartnerRedeemedDealRecord",
                                  new Dictionary<string, object>
                                  {
                                      { "@partnerId", (int)Context[Key.Partner] },
                                      { "@partnerRedeemedDealId", settlementDetail.TransactionId },
                                      { "@currency", settlementDetail.CurrencyCode },
                                      { "@purchaseDateTime", settlementDetail.TransactionDateTime },
                                      { "@redemptionEventId", (int)redemptionEvent },
                                      { "@settlementAmount", settlementAmount },
                                      { "@discountAmount", discountAmount },
                                      { "@reversed", reversed },
                                      { "@partnerDealId", settlementDetail.OfferId },
                                      { "@partnerUserId", settlementDetail.ConsumerId },
                                      { "@partnerCardSuffix", settlementDetail.CardSuffix },
                                      { "@partnerMerchantId", settlementDetail.LocationMid },
                                      { "@partnerReferenceNumber", settlementDetail.AcquirerReferenceNumber },
                                      { "@timeZoneOffset", offset }
                                  },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        Context[Key.PartnerCardId] = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerCardId"));
                        Context[Key.RedeemedDealId] = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("RedeemedDealId"));
                        Context[Key.GlobalUserId] = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalUserId"));
                    }
                });

            return result;
        }

        /// <summary>
        /// Retrieves the outstanding redeemed deal records for the partner in the conext.
        /// </summary>
        /// <returns>
        /// The collection of OutstandingRedeemedDealInfo Record objects built from the outstanding redeemed deal records.
        /// </returns>
        public Collection<OutstandingRedeemedDealInfo> RetrieveOutstandingPartnerRedeemedDealRecords()
        {
            Collection<OutstandingRedeemedDealInfo> result = new Collection<OutstandingRedeemedDealInfo>();

            SqlProcedure("GetOutstandingPartnerRedeemedDealRecords",
                         new Dictionary<string, object>
                         {
                             { "@partnerId", (int)Context[Key.Partner] }
                         },

                RetrieveOutstandingPartnerRedeemedDealRecordsLoader(result));

            return result;
        }
       

        /// <summary>
        /// Updates the data store to reflect the pending status of settlement redemptions for a set of redeemed deals.
        /// </summary>
        public void UpdatePendingPartnerRedeemedDeals()
        {
            UpdatePendingPartnerRedeemedDeals("UpdatePendingPartnerRedeemedDeals");
        }

        /// <summary>
        /// Updates the data store to reflect a set of redeemed deals as being settled as redeemed.
        /// </summary>
        public void MarkSettledAsRedeemed()
        {
            using (DataTable redeemedDealIdsTable = new DataTable("ListOfGuids"))
            {
                // Build the RedeemedDealIds table parameter.
                redeemedDealIdsTable.Locale = CultureInfo.InvariantCulture;
                redeemedDealIdsTable.Columns.Add("Id", typeof(Guid));
                Collection<OutstandingRedeemedDealInfo> outstandingRedeemedDeals = (Collection<OutstandingRedeemedDealInfo>)Context[Key.OutstandingRedeemedDeals];
                foreach (OutstandingRedeemedDealInfo outstandingRedeemedDeal in outstandingRedeemedDeals)
                {
                    redeemedDealIdsTable.Rows.Add(outstandingRedeemedDeal.RedeemedDealId);
                }

                // Update redeemed deal records.
                SqlProcedure("MarkSettledAsRedeemed",
                             new Dictionary<string, object>
                             {
                                 { "@redeemedDealIds", redeemedDealIdsTable }
                             });
            }
        }

        /// <summary>
        /// Updates the data store to reflect the status of settlement redemptions for a set of redeemed deals.
        /// </summary>
        public void UpdateRedeemedDeals()
        {
            using (DataTable transactionReferenceNumbersTable = new DataTable("TransactionReferenceNumbers"))
            {
                // Build the TransactionReferenceNumbers table parameter.
                Collection<int> referenceNumbers = (Collection<int>)Context[Key.ReferenceNumbers];
                transactionReferenceNumbersTable.Locale = CultureInfo.InvariantCulture;
                transactionReferenceNumbersTable.Columns.Add("TransactionReferenceNumber", typeof(string));
                foreach (int referenceNumber in referenceNumbers)
                {
                    transactionReferenceNumbersTable.Rows.Add(referenceNumber);
                }

                // Update redeemed deal records.
                SqlProcedure("UpdatePendingPartnerRedeemedDeals",
                             new Dictionary<string, object>
                             {
                                 { "@creditStatusId", (int)Context[Key.CreditStatus] },
                                 { "@transactionReferenceNumbers", transactionReferenceNumbersTable }
                             });
            }
        }

        
        /// <summary>
        /// Updates the data store to reflect the status of statement of credit for redeemed deals for Visa
        /// </summary>
        public void UpdateRedeemedDealsByVisa()
        {
            SqlProcedure("UpdateRedeemedDealsByVisa",
                            new Dictionary<string, object>
                            {
                                { "@creditStatusId", (int)Context[Key.CreditStatus] },
                                { "@partnerRedeemedDealScopeId", (string)Context[Key.Transaction]},
                                { "@dateCreditApproved", (DateTime)Context[Key.TransactionCreditApprovedDate] },
                            });
        }
        
        /// <summary>
        /// Updates in the data store the deal redemption record described by the RebateConfirmationData object in the context as rejected by MasterCard.
        /// </summary>
        /// <returns>
        /// The ResultCode that corresponds to the result of the operation.
        /// </returns>
        /// <remarks>
        /// MasterCard requires special treatment because, unlike other partners, MasterCard has no one ID that can be used to identify a transaction.
        /// </remarks>
        public ResultCode MarkRejectedByMasterCard()
        {
            ResultCode result = default(ResultCode);

            RebateConfirmationData rebateConfirmationData = (RebateConfirmationData)Context[Key.RebateConfirmationData];
            SqlProcedure("MarkRejectedByMasterCard",
                         new Dictionary<string, object>
                         {
                             { "@partnerCardId", rebateConfirmationData.BankCustomerNumber },
                             { "@discountAmount", (int)(rebateConfirmationData.RebateAmount * 100) },
                             { "@dateCreditApproved", rebateConfirmationData.RebateFileSendDate },
                             { "@partnerReferenceNumber", rebateConfirmationData.TransactionSequenceNumber }
                         },

                    (sqlDataReader) =>
                    {
                        if (sqlDataReader.Read() == true)
                        {
                            int selectValue = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("SelectValue"));
                            switch (selectValue)
                            {
                                case 0:
                                    result = ResultCode.MatchingRedeemedDealNotFound;
                                    break;
                                case 1:
                                    result = ResultCode.Success;
                                    break;
                                case 2:
                                    result = ResultCode.MultipleMatchingRedeemedDealsFound;
                                    break;
                                case -1:
                                    result = ResultCode.RedeemedDealFoundIsInexactMatch;
                                    break;
                            };
                        }
                    });

            return result;
        }

        /// <summary>
        /// Retrieves settlement analytics info for a batch of settled deal redemptions.
        /// </summary>
        public Collection<SettledDealInfo> RetrieveSettlementAnalyticsInfo()
        {
            Collection<SettledDealInfo> result = new Collection<SettledDealInfo>();

            using (DataTable transactionReferenceNumbersTable = new DataTable("TransactionReferenceNumbers"))
            {
                // Build the TransactionReferenceNumbers table parameter.
                Collection<int> referenceNumbers = (Collection<int>)Context[Key.ReferenceNumbers];
                transactionReferenceNumbersTable.Locale = CultureInfo.InvariantCulture;
                transactionReferenceNumbersTable.Columns.Add("TransactionReferenceNumber", typeof(string));
                foreach (int referenceNumber in referenceNumbers)
                {
                    transactionReferenceNumbersTable.Rows.Add(referenceNumber);
                }

                // Get settlement analytics for each settled redeemeed deal.
                SqlProcedure("GetSettlementAnalyticsInfo",
                             new Dictionary<string, object>
                             {
                                 { "@transactionReferenceNumbers", transactionReferenceNumbersTable }
                             },

                    (sqlDataReader) =>
                    {
                        while (sqlDataReader.Read() == true)
                        {
                            SettledDealInfo settledDealInfo = new SettledDealInfo
                            {
                                UserId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("UserId")),
                                DealId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("DealId")),
                                PartnerMerchantId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerMerchantId")),
                                Currency = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Currency")),
                                SettlementAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("SettlementAmount")),
                                DiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount")),
                                EventId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("EventId")),
                            };

                            // Add nullable items.
                            int merchantNameColumnId = sqlDataReader.GetOrdinal("MerchantName");
                            if (sqlDataReader.IsDBNull(merchantNameColumnId) == false)
                            {
                                settledDealInfo.MerchantName = sqlDataReader.GetString(merchantNameColumnId);
                            }

                            int parentDealIdColumnId = sqlDataReader.GetOrdinal("ParentDealId");
                            if (sqlDataReader.IsDBNull(parentDealIdColumnId) == false)
                            {
                                settledDealInfo.ParentDealId = sqlDataReader.GetGuid(parentDealIdColumnId);
                            }

                            int correlationIdColumnId = sqlDataReader.GetOrdinal("CorrelationId");
                            if (sqlDataReader.IsDBNull(correlationIdColumnId) == false)
                            {
                                settledDealInfo.CorrelationId = sqlDataReader.GetGuid(correlationIdColumnId);
                            }

                            result.Add(settledDealInfo);
                        }
                    });
            }

            return result;
        }

        /// <summary>
        /// Gets the report for the deals within the query in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode RetrieveDealReports()
        {
            ResultCode result = ResultCode.UnknownError;

            DealReportsQuery dealReportsQuery = (DealReportsQuery)Context[Key.DealReportsQuery];
            GetDealReportsResponse response = (GetDealReportsResponse)Context[Key.Response];
            List<DealReportDataContract> dealReports = new List<DealReportDataContract>();
            DataTable parentDealIds = new DataTable("ParentDealIds");
            try
            {
                if (parentDealIds != null)
                {
                    parentDealIds.Locale = CultureInfo.InvariantCulture;
                    parentDealIds.Columns.Add("ParentDealId", typeof(Guid));
                    parentDealIds.Columns.Add("UnconstrainedDiscounts", typeof(bool));

                    using (DataTable dealIds = new DataTable("DealIds"))
                    {
                        dealIds.Locale = CultureInfo.InvariantCulture;
                        dealIds.Columns.Add("DealId", typeof(Guid));

                        if (dealReportsQuery.DealReportQueries != null)
                        {
                            foreach (DealReportQuery dealReportQuery in dealReportsQuery.DealReportQueries)
                            {
                                bool unconstrainedDiscounts = dealReportsQuery.ReportDiscounts == true &&
                                    (dealReportQuery.DiscountIds == null || dealReportQuery.DiscountIds.Count() == 0);
                                parentDealIds.Rows.Add(dealReportQuery.DealId, unconstrainedDiscounts);

                                if (dealReportQuery.DiscountIds != null)
                                {
                                    foreach (Guid discountId in dealReportQuery.DiscountIds)
                                    {
                                        dealIds.Rows.Add(discountId);
                                    }
                                }
                            }
                        }

                        result = SqlProcedure("GetParentDealReports",
                                              new Dictionary<string, object>
                                              {
                                                  { "@parentDealIds", parentDealIds },
                                                  { "@dealIds", dealIds },
                                                  { "@startDate", dealReportsQuery.StartDay },
                                                  { "@endDate", dealReportsQuery.EndDay },
                                                  { "@reportDealRedemptions", dealReportsQuery.ReportDiscounts },
                                                  { "@redemptionDetails", dealReportsQuery.RedemptionDetails },
                                              },
                            (sqlDataReader) =>
                            {
                                Dictionary<Guid, List<DiscountReportDataContract>> discountReports =
                                                                        new Dictionary<Guid, List<DiscountReportDataContract>>();
                                while (sqlDataReader.Read() == true)
                                {
                                    List<DiscountReportDataContract> discountReportList = new List<DiscountReportDataContract>();
                                    DealReportDataContract dealReport = new DealReportDataContract
                                    {
                                        DealId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("ParentDealId")),
                                        DiscountReports = discountReportList,
                                        TotalRedemptions = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TotalRedemptions")),
                                        TotalSettlementAmount =
                                                       sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TotalSettlementAmount")),
                                        TotalDiscount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TotalDiscount"))
                                    };

                                    discountReports[dealReport.DealId] = discountReportList;
                                    dealReports.Add(dealReport);
                                }

                                if (dealReportsQuery.ReportDiscounts == true && sqlDataReader.NextResult() == true)
                                {
                                    Dictionary<Guid, List<DiscountRedemptionDataContract>> discountRedemptions =
                                                                    new Dictionary<Guid, List<DiscountRedemptionDataContract>>();
                                    while (sqlDataReader.Read() == true)
                                    {
                                        List<DiscountRedemptionDataContract> discountRedemptionList =
                                                                                      new List<DiscountRedemptionDataContract>();
                                        DiscountReportDataContract discountReport = new DiscountReportDataContract
                                        {
                                            DiscountId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("Id")),
                                            DiscountRedemptions = discountRedemptionList,
                                            Currency = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Currency")),
                                            TotalRedemptions =
                                                sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TotalRedemptions")),
                                            TotalSettlementAmount =
                                                       sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TotalSettlementAmount")),
                                            TotalDiscount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TotalDiscount"))
                                        };

                                        discountRedemptions[discountReport.DiscountId] = discountRedemptionList;
                                        discountReports[sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("ParentDealId"))]
                                            .Add(discountReport);
                                    }

                                    if (dealReportsQuery.RedemptionDetails == true && sqlDataReader.NextResult() == true)
                                    {
                                        while (sqlDataReader.Read() == true)
                                        {
                                            DiscountRedemptionDataContract discountRedemption =
                                                                                               new DiscountRedemptionDataContract
                                            {
                                                DateAndTime =
                                                         sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("PurchaseDateTime")),
                                                SettlementAmount =
                                                            sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("SettlementAmount")),
                                                Discount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount")),
                                                LastFourDigits =
                                                              sqlDataReader.GetString(sqlDataReader.GetOrdinal("LastFourDigits"))
                                            };

                                            discountRedemptions[sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("Id"))]
                                                .Add(discountRedemption);
                                        }
                                    }
                                }

                                response.DealReports = dealReports;
                            });
                    }
                }
            }
            finally
            {
                if (parentDealIds != null)
                {
                    parentDealIds.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the number of redemptions for a merchant's deals.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode RetrieveMerchantDealRedemptions()
        {
            ResultCode result;

            MerchantReportQuery merchantReportQuery = (MerchantReportQuery)Context[Key.MerchantReportQuery];
            GetMerchantReportResponse response = (GetMerchantReportResponse)Context[Key.Response];
            List<DealRedemptions> dealRedemptions = new List<DealRedemptions>();
            response.DealRedemptions = dealRedemptions;
            List<PartnerMerchantDealRedemptions> partnerMerchantDealRedemptions = new List<PartnerMerchantDealRedemptions>();
            response.PartnerMerchantDealRedemptions = partnerMerchantDealRedemptions;

            using (DataTable dealPartnerMerchantIdsTable = new DataTable("DealPartnerMerchantIds"))
            {
                // Build the DealPartnerMerchantIds table parameter.
                dealPartnerMerchantIdsTable.Locale = CultureInfo.InvariantCulture;
                dealPartnerMerchantIdsTable.Columns.Add("PartnerId", typeof(Int32));
                dealPartnerMerchantIdsTable.Columns.Add("PartnerMerchantId", typeof(string));
                dealPartnerMerchantIdsTable.Columns.Add("PartnerMerchantIdTypeId", typeof(Int32));
                dealPartnerMerchantIdsTable.Columns.Add("MerchantTimeZoneId", typeof(string));
                foreach (PartnerMerchantIds partnerMerchantIds in merchantReportQuery.PartnerMerchantIds)
                {
                    foreach (string partnerMerchantId in partnerMerchantIds.MerchantIds)
                    {
                        Partner partner = Partner.None;
                        if (Enum.TryParse<Partner>(partnerMerchantIds.Partner, out partner) == true)
                        {
                            dealPartnerMerchantIdsTable.Rows.Add((int)partner, partnerMerchantId, null);
                        }
                    }
                }

                result = SqlProcedure("GetMerchantDealRedemptions",
                                      new Dictionary<string, object>
                                  {
                                      { "@dealPartnerMerchantIds", dealPartnerMerchantIdsTable },
                                      { "@startDate", merchantReportQuery.StartDay },
                                      { "@endDate", merchantReportQuery.EndDay },
                                      { "@redemptionDetails", merchantReportQuery.RedemptionDetails }
                                  },

                    (sqlDataReader) =>
                    {
                        if (sqlDataReader.Read() == true)
                        {
                            response.NumberOfRedemptions = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("NumberOfRedemptions"));
                            int totalSettlementAmountColumnId = sqlDataReader.GetOrdinal("TotalSettlementAmount");
                            if (sqlDataReader.IsDBNull(totalSettlementAmountColumnId) == false)
                            {
                                response.TotalSettlementAmount = sqlDataReader.GetInt32(totalSettlementAmountColumnId);
                            }
                            int totalDiscountAmountColumnId = sqlDataReader.GetOrdinal("TotalDiscountAmount");
                            if (sqlDataReader.IsDBNull(totalDiscountAmountColumnId) == false)
                            {
                                response.TotalDiscountAmount = sqlDataReader.GetInt32(totalDiscountAmountColumnId);
                            }

                            if (merchantReportQuery.RedemptionDetails == true && sqlDataReader.NextResult() == true)
                            {
                                while (sqlDataReader.Read() == true)
                                {
                                    dealRedemptions.Add(new DealRedemptions
                                    {
                                        DealId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("DealId")),
                                        NumberOfRedemptions = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("NumberOfRedemptions")),
                                        TotalSettlementAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TotalSettlementAmount")),
                                        TotalDiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TotalDiscountAmount"))
                                    });
                                }

                                if (sqlDataReader.NextResult() == true)
                                {
                                    while (sqlDataReader.Read() == true)
                                    {
                                        partnerMerchantDealRedemptions.Add(new PartnerMerchantDealRedemptions
                                        {
                                            Partner = ((Partner)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("PartnerId"))).ToString(),
                                            PartnerMerchantId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerMerchantId")),
                                            NumberOfRedemptions = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("NumberOfRedemptions"))
                                        });
                                    }
                                }
                            }

                        }
                    });
            }

            return result;
        }

        /// <summary>
        /// Distributes the amount in the context from the user's Microsoft Store vouchers.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public ResultCode DistributeMssv()
        {
            ResultCode result = ResultCode.Success;

            // Convert distribution amount to cents-only and then to an int.
            decimal distAmount = (decimal)Context[Key.DistributionAmount] * 100;
            int distributionAmount = (int)distAmount;

            // Invoke the stored procedure.
            int remainingFunds = -100;
            result = SqlProcedure("DistributeMSSVoucher",
                                  new Dictionary<string, object>
                                  {
                                      { "@globalUserId", Context[Key.GlobalUserId] },
                                      { "@distributionAmount", distributionAmount },
                                      { "@voucherExpirationUtc", Context[Key.VoucherExpirationUtc] },
                                      { "@notes", Context[Key.Notes] }
                                  },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        remainingFunds = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("RemainingFunds"));
                    }
                });

            // Return remaining funds in dollars and cents.
            Context[Key.RemainingFunds] = (decimal)remainingFunds / 100;

            return result;
        }

        /// <summary>
        /// Distributes the amount in the context from the user's Microsoft Store vouchers.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public ResultCode MssVoucherDistributionHistory()
        {
            ResultCode result = ResultCode.Success;

            result = SqlProcedure("GetMSSVoucherDistributionHistory",
                                  new Dictionary<string, object>
                                  {
                                      { "@globalUserId", Context[Key.GlobalUserId] }
                                  },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        List<DistributionHistory> distributionHistoryItems = new List<DistributionHistory>();
                        List<TransactionHistory> transactionHistoryItems = new List<TransactionHistory>();
                        MssVoucherDistributionHistory history = new MssVoucherDistributionHistory
                        {
                            DistributionHistory = distributionHistoryItems,
                            TransactionHistory = transactionHistoryItems
                        };

                        // Get the amount remaining.
                        history.AmountRemaining = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("AmountRemaining"));

                        // Get the distribution history.
                        if (sqlDataReader.NextResult() == true)
                        {
                            while (sqlDataReader.Read() == true)
                            {
                                DistributionHistory distributionHistory = new DistributionHistory
                                {
                                    DistributionDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("DistributionDateUtc")),
                                    Amount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Amount")),
                                    Currency = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Currency")),
                                    ExpirationDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("VoucherExpirationUtc"))
                                };
                                DateTime.SpecifyKind(distributionHistory.DistributionDate, DateTimeKind.Utc);
                                DateTime.SpecifyKind(distributionHistory.ExpirationDate, DateTimeKind.Utc);
                                distributionHistoryItems.Add(distributionHistory);
                            }
                            // Get the transaction history.
                            if (sqlDataReader.NextResult() == true)
                            {
                                while (sqlDataReader.Read() == true)
                                {
                                    TransactionHistory transactionHistory = new TransactionHistory
                                    {
                                        Business = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Business")),
                                        CreditStatus = (CreditStatus)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CreditStatusId")),
                                        PurchaseDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("PurchaseDateTime")),
                                        DiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount"))
                                    };
                                    DateTime.SpecifyKind(transactionHistory.PurchaseDate, DateTimeKind.Utc);
                                    transactionHistoryItems.Add(transactionHistory);
                                }
                            }
                        }

                        Context[Key.MssVoucherDistributionHistory] = history;
                    }
                });

            return result;
        }

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
        public ResultCode GetRedeemedDealTransactionInfosByDate(
            DateTime startTimeInclusive,
            DateTime endTimeExclusive)
        {
            var transactionInfos = new List<RedeemedDealTransactionInfo>();

            var result = SqlProcedure(
                "GetRedeemedDealsByDate",
                new Dictionary<string, object>
                {
                    { "@startDateTimeInclusive", startTimeInclusive },
                    { "@endDateTimeExclusive", endTimeExclusive }
                },
                reader =>
                {
                    while (reader.Read())
                    {
                        transactionInfos.Add(new RedeemedDealTransactionInfo()
                                             {
                                                 MerchantName = reader.GetString(reader.GetOrdinal("MerchantName")),
                                                 RedemptionId = reader.GetGuid(reader.GetOrdinal("RedemptionId")),
                                                 TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                                                 TransactionAmount = reader.GetInt32(reader.GetOrdinal("TransactionAmount")),
                                                 CardLastFourDigits = reader.GetString(reader.GetOrdinal("CardLastFourDigits")),
                                                 CardType = (CardBrand)reader.GetInt32(reader.GetOrdinal("CardBrand")),
                                                 DealId = reader.GetGuid(reader.GetOrdinal("DealId")),
                                                 ProviderId = reader.GetInt32(reader.GetOrdinal("ProviderId"))

                        });
                    }
                });

            this.Context[Key.RedeemedDealInfo] = transactionInfos;

            return result;
        }

        /// <summary>
        /// Builds a ReverseRedeemedDealInfo object within the context from data within the specified SqlDataReader.
        /// </summary>
        /// <param name="sqlDataReader">
        /// The SqlDataReader whose data to use to build the ReverseRedeemedDealInfo.
        /// </param>
        private void BuildReverseRedeemedDealInfo(SqlDataReader sqlDataReader)
        {
            if (sqlDataReader.Read() == true)
            {
                Context[Key.ReverseRedeemedDealInfo] = new ReverseRedeemedDealInfo
                {
                    Amount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Amount")),
                    Percent = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Percent")),
                    AuthorizationAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("AuthorizationAmount")),
                    PartnerRedeemedDealId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerRedeemedDealId")),
                    DiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount"))
                };
            }
        }

        /// <summary>
        /// Gets merchant time zone offset
        /// </summary>
        /// <returns>time zone offest</returns>
        private int? GetPartnerMerchantTimeZoneOffset()
        {
            // get merchant time zone offset now 
            CommonOperations commonOps = new CommonOperations { Context = Context };
            string merchantTimeZoneId = commonOps.GetPartnerMerchantTimeZoneId();
            int? offset = General.GetTimeZoneOffset(DateTime.UtcNow, merchantTimeZoneId);
            return offset;
        }

        /// <summary>
        /// Creates a delegate to convert sqlDataReader stream for outstanding redeemed deals into collection of OutstandingRedeemedDealInfo
        /// </summary>
        /// <param name="result">return the delegate</param>
        /// <returns></returns>
        private static Action<SqlDataReader> RetrieveOutstandingPartnerRedeemedDealRecordsLoader(Collection<OutstandingRedeemedDealInfo> result)
        {
            return (sqlDataReader) =>
            {
                while (sqlDataReader.Read() == true)
                {
                    var dealInfo = new OutstandingRedeemedDealInfo()
                    {
                        RedeemedDealId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("RedeemedDealId")),
                        OfferId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerDealId")),
                        AcquirerReferenceNumber = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerReferenceNumber")),
                        Token = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerCardId")),
                        DiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount")),
                        TransactionDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("PurchaseDateTime")),
                        ReferenceNumber = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TransactionReferenceNumber")),
                        PartnerMerchantId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerMerchantId")),
                        MerchantName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("MerchantName")),
                        SettlementAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("SettlementAmount")),
                        DiscountId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalId")).ToString(),
                        ReimbursementTender = ((ReimbursementTender)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ReimbursementTenderId")))
                    };

                    // Get parent deal ID.
                    Guid parentDealId = Guid.Empty;
                    int parentDealIdColumnId = sqlDataReader.GetOrdinal("ParentDealId");
                    if (sqlDataReader.IsDBNull(parentDealIdColumnId) == false)
                    {
                        parentDealId = sqlDataReader.GetGuid(parentDealIdColumnId);
                    }
                    dealInfo.DealId = parentDealId.ToString();

                    // Add partner data if any exists.
                    int partnerDataColumnId = sqlDataReader.GetOrdinal("PartnerData");
                    if (sqlDataReader.IsDBNull(partnerDataColumnId) == false)
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(sqlDataReader.GetString(partnerDataColumnId));
                        dealInfo.PartnerData = xmlDocument;
                    }

                    // Get GlobalUserId
                    int globalUserIdColumnId = sqlDataReader.GetOrdinal("GlobalUserId");
                    if (sqlDataReader.IsDBNull(globalUserIdColumnId) == false)
                    {
                        dealInfo.GlobalUserId = sqlDataReader.GetGuid(globalUserIdColumnId);
                    }

                    // Get PartnerRedeemedDealScopeId
                    int partnerRedeemedDealScopeIdColumnId = sqlDataReader.GetOrdinal("PartnerRedeemedDealScopeId");
                    if (sqlDataReader.IsDBNull(partnerRedeemedDealScopeIdColumnId) == false)
                    {
                        dealInfo.PartnerRedeemedDealScopeId = sqlDataReader.GetString(partnerRedeemedDealScopeIdColumnId);
                    }
                    
                    result.Add(dealInfo);
                }
            };
        }

        /// <summary>
        /// Updates the data store to reflect the pending status of settlement redemptions for a set of redeemed deals. May also updates the processStatus depending upon stored procedure
        /// </summary>
        private void UpdatePendingPartnerRedeemedDeals(string storedProcedureName)
        {
            using (DataTable transactionReferenceNumbersTable = new DataTable("TransactionReferenceNumbers"))
            {
                // Build the TransactionReferenceNumbers table parameter.
                Collection<OutstandingRedeemedDealInfo> merchantRecords =
                    (Collection<OutstandingRedeemedDealInfo>)Context[Key.MerchantRecords];
                transactionReferenceNumbersTable.Locale = CultureInfo.InvariantCulture;
                transactionReferenceNumbersTable.Columns.Add("TransactionReferenceNumber", typeof(string));
                foreach (OutstandingRedeemedDealInfo merchantRecord in merchantRecords)
                {
                    transactionReferenceNumbersTable.Rows.Add(merchantRecord.ReferenceNumber);
                }

                // Update redeemed deal records.
                SqlProcedure(storedProcedureName,
                    new Dictionary<string, object>
                    {
                        {"@creditStatusId", (int) Context[Key.CreditStatus]},
                        {"@transactionReferenceNumbers", transactionReferenceNumbersTable}
                    });
            }
        }
    }
}