//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.Context;

namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents operations on Authorization objects within the data store.
    /// </summary>
    public class AuthorizationOperations : CommerceOperations, IAuthorizationOperations
    {
        /// <summary>
        /// Adds the authorization in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddAuthorization()
        {
            int? offset = GetPartnerMerchantTimeZoneOffset();

            ResultCode result;
            Authorization authorization = (Authorization)Context[Key.Authorization];

            // TEMPORARY: Determine if the allow list forces transaction rejection.
            if (PhysStoreFilter() == true)
            {


//Context.Log.Critical("AuthorizationId: {0}\r\nPartnerId: {1}\r\nPartnerDealId: {2}\r\nPartnerCardId: {3}\r\nPartnerMerchantId: {4}\r\nPurchaseDateTime: {5}\r\nAuthorizationAmount: {6}\r\nTransactionScopeId: {7}" +
//                     "TransactionId: {8}\r\nCurrency: {9}\r\nPartnerData: {10}\r\noffset: {11}", null,
//                     authorization.Id, (int)Context[Key.Partner], Context[Key.PartnerDealId], Context[Key.PartnerCardId], Context[Key.PartnerMerchantId], authorization.PurchaseDateTime, authorization.AuthorizationAmount, authorization.TransactionScopeId,
//                     authorization.TransactionId, authorization.Currency, Context[Key.PartnerData], offset);


                result = SqlProcedure("AddAuthorization",
                                    new Dictionary<string, object>
                                    {
                                        { "@authorizationId", authorization.Id },
                                        { "@partnerId", (int)Context[Key.Partner] },
                                        { "@recommendedPartnerDealId", Context[Key.PartnerDealId] },
                                        { "@partnerCardId", Context[Key.PartnerCardId] },
                                        { "@partnerMerchantId", Context[Key.PartnerMerchantId] },
                                        { "@purchaseDateTime", authorization.PurchaseDateTime },
                                        { "@authAmount", authorization.AuthorizationAmount },
                                        { "@transactionScopeId", authorization.TransactionScopeId },
                                        { "@transactionId", authorization.TransactionId },
                                        { "@currency", authorization.Currency },
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
                            PartnerRedeemedDealId = authorization.PartnerRedeemedDealId,
                            DiscountSummary = sqlDataReader.GetString(sqlDataReader.GetOrdinal("DiscountSummary")),
                            LastFourDigits = sqlDataReader.GetString(sqlDataReader.GetOrdinal("LastFourDigits")),
                            DiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount")),
                            PartnerMerchantId = (string)Context[Key.PartnerMerchantId],
                            TransactionId = authorization.TransactionId,
                            TransactionDate = authorization.PurchaseDateTime,
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
    }
}