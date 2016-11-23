//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Earn.Dashboard.Web.Service;
using Earn.DataContract.Commerce;
using Earn.Dashboard.Web.Attributes;

namespace Earn.Dashboard.Web.Controllers.Api
{
    [WebApiAuthorizeSG]
    public class CommerceController : ApiController
    {
        [HttpGet]
        public async Task<List<Transaction>> FetchTransactions([FromUri] TransactionsFilter filter)
        {
            var data = await CustomerService.FetchTransactionsByFilter(filter);
            return data;
        }

        [HttpGet]
        public async Task<List<Settlement>> FetchSettlements([FromUri] TransactionsFilter filter)
        {
            var data = await CustomerService.FetchSettlementsByFilter(filter);
            return data;
        }

        [HttpGet]
        public async Task<List<UserReferral>> FetchReferrals([FromUri] Guid userId)
        {
            var data = await CustomerService.FetchReferrals(userId);
            return data;
        }

        [HttpGet]
        public async Task<List<EarnBurnLineItem>> FetchEarnBurnLineItems([FromUri] Guid userId)
        {
            var data = await CustomerService.FetchUserEarnBurnLineItems(userId);
            return data;
        }

        [HttpGet]
        public async Task<List<EarnBurnHistoryRecord>> FetchEarnBurnHistory([FromUri] Guid userId)
        {
            var data = await CustomerService.FetchEarnBurnHistory(userId);
            return data;
        }

        [HttpPost]
        public async Task<IssueCreditsResponse> IssueCredits([FromBody] IssueCreditsRequest request)
        {
            var result = await CustomerService.IssueCredits(request);
            return result;
        }
    }
}