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
using Earn.DataContract.Commerce;

namespace Earn.Dashboard.DAL
{
    public class CommerceDal
    {
        public const int EarnTransactionType = 0;
        public const int BurnTransactionType = 1;

        public const int UnprocessedRewardPayoutStatusId = 0;
        public const int PaidRewardPayoutStatusId = 2;

        public const string AllMerchants = "All";

        private static CommerceEntities BuildDBContext()
        {
            CommerceEntities dbContext = new CommerceEntities();
            dbContext.Database.CommandTimeout = 0;

            return dbContext;
        }

        public static async Task<List<Transaction>> FetchTransactionsByFilter(TransactionsFilter filter)
        {
            CommerceEntities dbContext = BuildDBContext();

            var query = from auth in dbContext.Authorizations
                        join tr in dbContext.TransactionLinks on auth.ClaimedDealId equals tr.Id
                        join merchants in dbContext.Merchants on auth.MerchantNameId equals merchants.Id
                        join cards in dbContext.Cards on tr.CardId equals cards.Id
                        join cardBrands in dbContext.CardBrands on cards.CardBrand equals cardBrands.Id
                        join offers in dbContext.Offers on tr.DealId equals offers.Id
                        join users in dbContext.Users on tr.UserId equals users.Id
                        orderby tr.DateAdded descending
                        select new Transaction
                        {
                            TransactionLinkId = tr.Id,
                            DateAdded = auth.DateAdded,
                            MerchantId = merchants.Id,
                            MerchantName = merchants.Name,
                            TransactionTypeId = offers.OfferType,
                            TransactionType = offers.OfferType == EarnTransactionType ? "Earn" : "Burn",
                            Percent = offers.PercentBack,
                            TransactionAmount = Math.Round((double)auth.TransactionAmount / 100, 2),
                            DiscountAmount = Math.Round((double)auth.DiscountAmount / 100, 2),
                            LastFourDigits = cards.LastFourDigits,
                            CardBrandId = cards.CardBrand,
                            CardBrandName = cardBrands.Name,
                            UserGlobalId = users.GlobalId
                        };

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                query = query.Where(x => filter.StartDate <= x.DateAdded && x.DateAdded < filter.EndDate);
            }

            if (filter.TransactionTypes != null && filter.TransactionTypes.Any())
            {
                query = query.Where(x => filter.TransactionTypes.Contains(x.TransactionTypeId));
            }

            if (filter.CardBrandIds != null && filter.CardBrandIds.Any())
            {
                query = query.Where(x => filter.CardBrandIds.Contains(x.CardBrandId));
            }

            if (!string.IsNullOrWhiteSpace(filter.Last4Digits))
            {
                query = query.Where(x => x.LastFourDigits == filter.Last4Digits);
            }

            if (!string.IsNullOrWhiteSpace(filter.MerchantName) && filter.MerchantName != AllMerchants)
            {
                query = query.Where(x => x.MerchantName == filter.MerchantName);
            }

            if (filter.UserId.HasValue)
            {
                query = query.Where(x => x.UserGlobalId == filter.UserId);
            }

            query = query.OrderBy(x => x.DateAdded);

            return await query.ToListAsync();
        }

        public static async Task<List<Settlement>> FetchSettlementsByFilter(TransactionsFilter filter)
        {
            CommerceEntities dbContext = BuildDBContext();
            var query = from rr in dbContext.RedemptionReports
                        join offers in dbContext.Offers on rr.DealId equals offers.GlobalID
                        join rd in dbContext.RedeemedDeals on rr.TransactionId equals rd.Id
                        join m in dbContext.Merchants on rd.MerchantNameId equals m.Id
                        join cardBrands in dbContext.CardBrands on rr.CardBrand equals cardBrands.Name
                        select new Settlement
                        {
                            PartnerName = rr.PartnerName,
                            CardBrandId = cardBrands.Id,
                            CardBrandName = rr.CardBrand,
                            LastFourDigits = rr.LastFourDigits,
                            PartnerMerchantId = m.Id,
                            MerchantName = m.Name,
                            AuthorizationDateTimeLocal = rr.AuthorizationDateTimeLocal,
                            AuthorizationDateTimeUtc = rr.AuthorizationDateTimeUtc,
                            UtcReachedTerminalState = rr.UtcReachedTerminalState,
                            TransactionTypeId = offers.OfferType,
                            TransactionType = offers.OfferType == EarnTransactionType ? "Earn" : "Burn",
                            AuthorizationAmount = Math.Round((double)rr.AuthorizationAmount / 100, 2),
                            SettlementAmount = Math.Round((double)rr.SettlementAmount / 100, 2),
                            DiscountAmount = Math.Round((double)rr.DiscountAmount / 100, 2),
                            CreditStatus = rr.CreditStatus,
                            CurrentState = rr.CurrentState,
                            GlobalUserId = rr.GlobalUserID
                        };

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                query = query.Where(x => filter.StartDate <= x.UtcReachedTerminalState && x.UtcReachedTerminalState < filter.EndDate);
            }

            if (filter.TransactionTypes != null && filter.TransactionTypes.Any())
            {
                query = query.Where(x => filter.TransactionTypes.Contains(x.TransactionTypeId));
            }

            if (filter.CardBrandIds != null && filter.CardBrandIds.Any())
            {
                query = query.Where(x => filter.CardBrandIds.Contains(x.CardBrandId));
            }

            if (!string.IsNullOrWhiteSpace(filter.Last4Digits))
            {
                query = query.Where(x => x.LastFourDigits == filter.Last4Digits);
            }

            if (!string.IsNullOrWhiteSpace(filter.MerchantName) && filter.MerchantName != AllMerchants)
            {
                query = query.Where(x => x.MerchantName == filter.MerchantName);
            }

            query = query.OrderByDescending(x => x.UtcReachedTerminalState);

            return await query.ToListAsync();
        }


        public static async Task<List<DataContract.Commerce.Merchant>> FetchMerchantsAsync()
        {
            CommerceEntities dbContext = BuildDBContext();
            var query = from m in dbContext.Merchants
                        orderby m.Name
                        select new DataContract.Commerce.Merchant
                        {
                            Id = m.Id,
                            Name = m.Name
                        };
            return await query.ToListAsync();
        }

        public static async Task<List<DataContract.Commerce.CardBrand>> FetchCardBrandsAsync()
        {
            CommerceEntities dbContext = BuildDBContext();
            var query = from cb in dbContext.CardBrands
                        select new DataContract.Commerce.CardBrand
                        {
                            Id = cb.Id,
                            Name = cb.Name
                        };

            return await query.ToListAsync();
        }

        public static List<TransactionType> FetchTransactionTypes()
        {
            return new List<TransactionType> {
                new TransactionType { Id = EarnTransactionType, Name = "Earn" },
                new TransactionType { Id = BurnTransactionType, Name = "Burn" }
            };
        }

        public static async Task<List<CardInfo>> FetchCardInfo(string last4Digits)
        {
            CommerceEntities dbContext = BuildDBContext();

            var query = from c in dbContext.Cards
                        join cb in dbContext.CardBrands on c.CardBrand equals cb.Id
                        join u in dbContext.Users on c.UserId equals u.Id
                        where c.LastFourDigits == last4Digits
                        orderby c.Active descending
                        select new CardInfo
                        {
                            Id = c.Id,
                            UserId = c.UserId,
                            GlobalUserId = u.GlobalId,
                            Last4Digits = c.LastFourDigits,
                            CardBrandId = c.CardBrand,
                            CardBrand = cb.Name,
                            DateAddedUTC = c.UtcAdded,
                            Active = c.Active,
                            Token = c.PartnerToken
                        };

            return await query.ToListAsync();
        }

        public static async Task<List<CardInfo>> FetchCardInfo(Guid globalUserId)
        {
            CommerceEntities dbContext = BuildDBContext();

            var query = from c in dbContext.Cards
                        join cb in dbContext.CardBrands on c.CardBrand equals cb.Id
                        join u in dbContext.Users on c.UserId equals u.Id
                        where u.GlobalId == globalUserId
                        orderby c.Active descending
                        select new CardInfo
                        {
                            Id = c.Id,
                            UserId = c.UserId,
                            GlobalUserId = u.GlobalId,
                            Last4Digits = c.LastFourDigits,
                            CardBrandId = c.CardBrand,
                            CardBrand = cb.Name,
                            DateAddedUTC = c.UtcAdded,
                            Active = c.Active,
                            Token = c.PartnerToken
                        };

            return await query.ToListAsync();
        }

        public static async Task<int> FetchUserIdByGlobalId(Guid globalUserId)
        {
            CommerceEntities dbContext = BuildDBContext();

            var query = from u in dbContext.Users
                        where u.GlobalId == globalUserId
                        select u.Id;

            return await query.FirstAsync();
        }

        public static async Task<List<UserReferral>> FetchReferrals(Guid payeeId)
        {
            CommerceEntities dbContext = BuildDBContext();

            var query = from rp in dbContext.RewardPayouts
                        join rr in dbContext.RewardReasons on rp.RewardReasonId equals rr.Id
                        join rps in dbContext.RewardPayoutStatus on rp.RewardPayoutStatusId equals rps.Id
                        where rp.PayeeId == payeeId &&
                              rp.RewardReasonId == 0 &&
                              (rp.RewardPayoutStatusId == UnprocessedRewardPayoutStatusId || rp.RewardPayoutStatusId == PaidRewardPayoutStatusId)
                        orderby rp.PayoutScheduledDateUtc descending
                        select new UserReferral
                        {
                            PayeeId = payeeId,
                            AgentId = rp.AgentId,
                            RewardReasonId = rp.RewardReasonId,
                            RewardReason = rr.Name,
                            Explanation = rp.Explanation,
                            RewardPayoutStatusId = rp.RewardPayoutStatusId,
                            RewardPayoutStatus = rps.Name,
                            PayoutFinalizedDateUtc = rp.PayoutFinalizedDateUtc,
                            PayoutScheduledDateUtc = rp.PayoutScheduledDateUtc,
                            Amount = Math.Round((double)rp.Amount / 100, 2),
                        };

            return await query.ToListAsync();
        }
    }
}