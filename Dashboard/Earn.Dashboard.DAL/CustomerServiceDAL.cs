//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Earn.Dashboard.DAL.db.Commerce;
using Earn.Dashboard.DAL.db.CustomerService;
using Earn.DataContract.Commerce;
using System.Threading;

namespace Earn.Dashboard.DAL
{
    public class CustomerServiceDAL
    {
        public static async Task<IssueCreditsResponse> IssueCredits(Guid customerId, int amount, string explanation, string issuer)
        {
            CustomerServiceEntities customerServiceDbContext = new CustomerServiceEntities();

            IssueCreditsResponse result = await Task.Run(() =>
            {
                try
                {
                    customerServiceDbContext.CustomerServiceIssueEarnCredit(customerId, amount, explanation, issuer);
                }
                catch (Exception e)
                {
                    string message = e.InnerException != null ? e.InnerException.Message : e.Message;

                    return new IssueCreditsResponse
                    {
                        Message = message,
                        Successful = false
                    };
                }

                return new IssueCreditsResponse
                {
                    Message = "Issued credits successfully",
                    Successful = true
                };
            });

            return result;
        }

        public static async Task<List<EarnBurnHistoryRecord>> GetEarnBurnHistory(Guid userId)
        {
            CommerceEntities commerceDbContext = new CommerceEntities();

            // Tenders
            Dictionary<int, string> tenders = await (from trs in commerceDbContext.ReimbursementTenders
                                                        select trs).ToDictionaryAsync(x => x.Id, x => x.Name);

            // Cards
            Dictionary<int, string> cardBrands = await (from cb in commerceDbContext.CardBrands
                                                        select cb).ToDictionaryAsync(x => x.Id, x => x.Name);

            // Credit Status
            Dictionary<int, string> creditStatus = await (from trs in commerceDbContext.CreditStatus
                                                            select trs).ToDictionaryAsync(x => x.Id, x => x.Name);

            CustomerServiceEntities customerServiceDbContext = new CustomerServiceEntities();

            var query = (from h in customerServiceDbContext.GetEarnBurnHistory(userId)
                        orderby h.PurchaseDateTime descending
                        select new EarnBurnHistoryRecord
                        {
                            ReimbursementTenderId = h.ReimbursementTenderId,
                            MerchantName = h.MerchantName,
                            DiscountSummary = h.DiscountSummary,
                            Percent = h.Percent,
                            PurchaseDateTime = h.PurchaseDateTime,
                            AuthorizationAmount = Math.Round((double)h.AuthorizationAmount / 100, 2),
                            Reversed = h.Reversed,
                            CreditStatusId = h.CreditStatusId,
                            DiscountAmount = Math.Round((double)h.DiscountAmount / 100, 2),
                            LastFourDigits = h.LastFourDigits,
                            CardBrandId = h.CardBrandId,
                        }).ToList();

            foreach (var record in query)
            {
                record.ReimbursementTender = record.ReimbursementTenderId.HasValue && tenders.ContainsKey(record.ReimbursementTenderId.Value)
                                            ? tenders[record.ReimbursementTenderId.Value]
                                            : string.Empty;

                record.CreditStatus = record.CreditStatusId.HasValue && creditStatus.ContainsKey(record.CreditStatusId.Value)
                                        ? creditStatus[record.CreditStatusId.Value]
                                        : string.Empty;

                record.CardBrand = record.CardBrandId.HasValue && cardBrands.ContainsKey(record.CardBrandId.Value)
                                    ? cardBrands[record.CardBrandId.Value]
                                    : string.Empty;
            }

            return query;
        }

        public static async Task<List<EarnBurnLineItem>> GetEarnBurnLineItems(Guid userId)
        {
            CommerceEntities commerceDbContext = new CommerceEntities();
            // Cards
            Dictionary<int, string> cardBrands = await (from cb in commerceDbContext.CardBrands
                                                  select cb).ToDictionaryAsync(x => x.Id, x => x.Name);

            // TransactionReviewStatus
            Dictionary <int, string> trReviewStatus = await (from trs in commerceDbContext.TransactionReviewStatus
                                                        select trs).ToDictionaryAsync(x => x.Id, x => x.Name);

            // Credit Status
            Dictionary<int, string> creditStatus = await (from trs in commerceDbContext.CreditStatus
                                                            select trs).ToDictionaryAsync(x => x.Id, x => x.Name);


            CustomerServiceEntities customerServiceDbContext = new CustomerServiceEntities();
            customerServiceDbContext.Database.CommandTimeout = 0;

            var query = from li in customerServiceDbContext.QueryEarnBurnLineItems()
                        where li.GlobalId == userId
                        orderby li.TransactionDate descending
                         select new EarnBurnLineItem
                         {
                             TransactionId = li.TransactionId,
                             UserId = li.GlobalId,
                             TransactionDate = li.TransactionDate,
                             EarnCredit = Math.Round((double) li.EarnCredit / 100, 2),
                             BurnDebit = Math.Round((double) li.BurnDebit / 100, 2),
                             HasRedeemedDealRecord = li.HasRedeemedDealRecord,
                             TransactionTypeId = li.TransactionType,
                             DealSummary = li.DealSummary,
                             DealPercent = li.DealPercent,
                             MerchantName = li.MerchantName,
                             TransactionAmount = Math.Round((double) li.TransactionAmount / 100, 2),
                             Reversed = li.Reversed,
                             TransactionStatusId = li.TransactionStatusId,
                             Last4Digits = li.LastFourDigits,
                             CardBrandId = li.CardBrand,
                             PermaPending = li.PermaPending,
                             ReviewStatusId = li.ReviewStatusId,
                             RedeemDealId = li.RedeemedDealId
                         };

            List<EarnBurnLineItem> result = await query.ToListAsync();

            foreach (var row in result)
            {
                row.TransactionType = row.TransactionTypeId == CommerceDal.BurnTransactionType
                                ? "Burn"
                                : "Earn";

                row.TransactionStatus = row.TransactionStatusId.HasValue && creditStatus.ContainsKey(row.TransactionStatusId.Value)
                                ? creditStatus[row.TransactionStatusId.Value]
                                : string.Empty;

                row.CardBrand = row.CardBrandId.HasValue && cardBrands.ContainsKey(row.CardBrandId.Value)
                                ? cardBrands[row.CardBrandId.Value]
                                : string.Empty;

                row.ReviewStatus = row.ReviewStatusId.HasValue && trReviewStatus.ContainsKey(row.ReviewStatusId.Value)
                                ? trReviewStatus[row.ReviewStatusId.Value]
                                : string.Empty;
            }

            return result;
        }
    }
}