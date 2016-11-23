//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System.Collections.Generic;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents operations on user deal redemption history.
    /// </summary>
    public class RedemptionHistoryOperations : CommerceOperations, IRedemptionHistoryOperations
    {
        /// <summary>
        /// Retrieves the list of redemption history items for the user in the context.
        /// </summary>
        /// <returns>
        /// The list of redemption history items for the user in the context.
        /// </returns>
        public IEnumerable<RedemptionHistoryItem> RetrieveRedemptionHistory()
        {
            List<RedemptionHistoryItem> result = new List<RedemptionHistoryItem>();

            SqlProcedure("GetRedemptionHistory",
                         new Dictionary<string, object>
                         {
                             { "@userId", ((User)Context[Key.User]).GlobalId }
                         },

                (sqlDataReader) =>
                {
                    while (sqlDataReader.Read() == true)
                    {
                        // Add non-nullable items.
                        RedemptionHistoryItem redemptionHistoryItem = new RedemptionHistoryItem
                        {
                            DealAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DealAmount")),
                            DealPercent = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Percent")),
                            DealCurrency = sqlDataReader.GetString(sqlDataReader.GetOrdinal("DealCurrency")),
                            MinimumPurchase = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("MinimumPurchase")),
                            MaximumDiscount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("MaximumDiscount")),
                            NameOnCard = sqlDataReader.GetString(sqlDataReader.GetOrdinal("NameOnCard")),
                            LastFourDigits = sqlDataReader.GetString(sqlDataReader.GetOrdinal("LastFourDigits")),
                            CardExpiration = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("Expiration")),
                            CardBrand = (CardBrand)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CardBrandId")),
                            CreditStatus = (CreditStatus)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CreditStatusId")),
                            DiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount")),
                            Event = (RedemptionEvent)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("RedemptionEventId")),
                            EventDateTime = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("PurchaseDateTime")),
                            EventAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("EventAmount"))
                        };

                        // Add nullable items.
                        int merchantNameColumnId = sqlDataReader.GetOrdinal("MerchantName");
                        if (sqlDataReader.IsDBNull(merchantNameColumnId) == false)
                        {
                            redemptionHistoryItem.MerchantName = sqlDataReader.GetString(merchantNameColumnId);
                        }

                        int discountSummaryColumnId = sqlDataReader.GetOrdinal("DiscountSummary");
                        if (sqlDataReader.IsDBNull(discountSummaryColumnId) == false)
                        {
                            redemptionHistoryItem.DiscountSummary = sqlDataReader.GetString(discountSummaryColumnId);
                        }

                        int eventCurrencyColumnId = sqlDataReader.GetOrdinal("EventCurrency");
                        if (sqlDataReader.IsDBNull(eventCurrencyColumnId) == false)
                        {
                            redemptionHistoryItem.EventCurrency = sqlDataReader.GetString(eventCurrencyColumnId);
                        }

                        int reversedColumnId = sqlDataReader.GetOrdinal("Reversed");
                        if (sqlDataReader.IsDBNull(reversedColumnId) == false)
                        {
                            redemptionHistoryItem.Reversed = sqlDataReader.GetBoolean(reversedColumnId);
                        }
                        
                        result.Add(redemptionHistoryItem);
                    }
                });

            Context.Log.Verbose("GetRedemptionHistory retrieved {0} history items for the specified User.", result.Count);

            return result;
        }

        /// <summary>
        /// Returns the list of Earn/Burn transactions for the user in context
        /// </summary>
        /// <returns>
        /// List of Earn/Burn transactions for the user in context
        /// </returns>
        public IEnumerable<RedemptionHistoryItem> RetrieveMicrosoftEarnRedemptionHistory()
        {
            List<RedemptionHistoryItem> result = new List<RedemptionHistoryItem>();
            SqlProcedure("GetEarnBurnHistory",
                      new Dictionary<string, object>
                         {
                             { "@userId", ((User)Context[Key.User]).GlobalId }
                         },

             (sqlDataReader) =>
             {
                 while (sqlDataReader.Read())
                 {
                     RedemptionHistoryItem redemptionHistoryItem = new RedemptionHistoryItem
                     {
                         ReimbursementTender = (ReimbursementTender)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ReimbursementTenderId")),
                         DealPercent = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("Percent")),
                         LastFourDigits = sqlDataReader.GetString(sqlDataReader.GetOrdinal("LastFourDigits")),
                         CardBrand = (CardBrand)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CardBrandId")),
                         CreditStatus = (CreditStatus)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CreditStatusId")),
                         DiscountAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DiscountAmount")),
                         EventDateTime = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("PurchaseDateTime")),
                         EventAmount = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("AuthorizationAmount"))
                     };
                     
                     int merchantNameColumnId = sqlDataReader.GetOrdinal("MerchantName");
                     if (sqlDataReader.IsDBNull(merchantNameColumnId) == false)
                     {
                         redemptionHistoryItem.MerchantName = sqlDataReader.GetString(merchantNameColumnId);
                     }

                     int discountSummaryColumnId = sqlDataReader.GetOrdinal("DiscountSummary");
                     if (sqlDataReader.IsDBNull(discountSummaryColumnId) == false)
                     {
                         redemptionHistoryItem.DiscountSummary = sqlDataReader.GetString(discountSummaryColumnId);
                     }

                     int reversedColumnId = sqlDataReader.GetOrdinal("Reversed");
                     if (sqlDataReader.IsDBNull(reversedColumnId) == false)
                     {
                         redemptionHistoryItem.Reversed = sqlDataReader.GetBoolean(reversedColumnId);
                     }

                     result.Add(redemptionHistoryItem);
                 }
             });

            Context.Log.Verbose("GetEarnBurnHistory retrieved {0} history items for the specified User.", result.Count);

            return result;
        }
    }
}