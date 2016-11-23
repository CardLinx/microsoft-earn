//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Earn.Dashboard.Web.Service;
using Earn.DataContract.WebAnalytics;
using Earn.Dashboard.DAL.db.Analytics;

namespace Earn.Dashboard.Web.Controllers.Api
{
    public class WebAnalyticsController : ApiController
    {
        [HttpGet]
        public async Task<List<AggregatedVisitorsByDateRecord>> FetchVisitorsAnalyticsAsync([FromUri] RequestFilter filter)
        {
            var data = await WebAnalyticsService.FetchVisitorsByDateAnalyticsForPeriodAsync(filter);
            return data;
        }

        [HttpGet]
        public async Task<List<AggregatedByDeviceTypeRecord>> FetchByDeviceTypeAnalyticsAsync([FromUri] RequestFilter filter)
        {
            var data = await WebAnalyticsService.FetchByDeviceTypeAnalyticForPeriodAsync(filter);
            return data;
        }

        [HttpGet]
        public async Task<List<AggregatedByPageTitleRecord>> FetchByPageTitleAnalyticsAsync([FromUri] RequestFilter filter)
        {
            var data = await WebAnalyticsService.FetchByPageTitleAnalyticsForPeriodAsync(filter);
            return data;
        }

        [HttpGet]
        public async Task<List<AggregatedNewUsersByDateRecord>> FetchNewUsersAnalyticsAsync([FromUri] RequestFilter filter)
        {
            var data = await WebAnalyticsService.FetchNewUsersByDateAnalyticsForPeriodAsync(filter);
            return data;
        }

        [HttpGet]
        public async Task<List<analytic>> FetchAnalyticsAsync([FromUri] RequestFilter filter)
        {
            var data = await WebAnalyticsService.FetchAnalyticsForPeriodAsync(filter);
            return data;
        }
    }
}