//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Earn.Dashboard.DAL;
using Earn.Dashboard.LomoUsersDAL;
using Earn.DataContract.Commerce;
using Earn.DataContract.LomoUsers;

namespace Earn.Dashboard.Web.Service
{
    public class CustomerService
    {
        public static async Task<List<Transaction>> FetchTransactionsByFilter(TransactionsFilter filter)
        {
            var data = await CommerceDal.FetchTransactionsByFilter(filter);
            return data;
        }

        public static async Task<List<Settlement>> FetchSettlementsByFilter(TransactionsFilter filter)
        {
            var data = await CommerceDal.FetchSettlementsByFilter(filter);
            return data;
        }

        public static async Task<List<string>> FetchMerchantsAsync()
        {
            var data = await CommerceDal.FetchMerchantsAsync();
            List<string> uniqueMerchants = data.Select(x => x.Name).Distinct().ToList();
            return uniqueMerchants;
        }

        public static async Task<List<CardBrand>> FetchCardBrandsAsync()
        {
               var data = await CommerceDal.FetchCardBrandsAsync();
            return data;
        }

        public static List<TransactionType> FetchTransactionTypes()
        {
            var data = CommerceDal.FetchTransactionTypes();
            return data;
        }

        public static async Task<List<Customer>> FindCustomers(string filter)
        {
            List<Customer> users = new List<Customer>();
            Guid tmp;

            if (filter.Length == 4)
            {
                List<CardInfo> userCards = await CommerceDal.FetchCardInfo(filter);
                List<Guid> userIds = userCards.Select(x => x.GlobalUserId)
                                                .ToList();

                users = await LomoUserDal.FetchUsersByIds(userIds);
            }
            else if (filter.Contains("@"))
            {
                users = await LomoUserDal.FetchUsersByFilter(new CustomerFilter { Email = filter });
            }
            else if (Guid.TryParse(filter, out tmp))
            {
                users = await LomoUserDal.FetchUsersByFilter(new CustomerFilter { UserId = tmp });
            }
            else
            {
                users = await LomoUserDal.FetchUsersByFilter(new CustomerFilter { MSIDorPUID = filter });
            }

            foreach (var user in users)
            {
                user.Id = await CommerceDal.FetchUserIdByGlobalId(user.GlobalId);
                var userCards = await CommerceDal.FetchCardInfo(user.GlobalId);
                if (userCards != null && userCards.Any())
                {
                    user.Cards = userCards;
                }
                else
                {
                    user.Cards = new List<CardInfo>();
                }
            }

            return users;
        }

        public static async Task<List<UserReferral>> FetchReferrals(Guid userId)
        {
            var data = await CommerceDal.FetchReferrals(userId);
            return data;
        }

        public static async Task<List<EarnBurnLineItem>> FetchUserEarnBurnLineItems(Guid userId)
        {
            var data = await CustomerServiceDAL.GetEarnBurnLineItems(userId);
            return data;
        }

        public static async Task<List<EarnBurnHistoryRecord>> FetchEarnBurnHistory(Guid guid)
        {
            var data = await CustomerServiceDAL.GetEarnBurnHistory(guid);
            return data;
        }

        public static async Task<IssueCreditsResponse> IssueCredits(IssueCreditsRequest request)
        {
            int amount = (int)(request.Amount * 100);
            string issuer = request.Issuer + " via Earn Dashboard Customer Service Tool";
            var result = await CustomerServiceDAL.IssueCredits(request.UserId, amount, request.Explanation, issuer);

            return result;
        }
    }
}