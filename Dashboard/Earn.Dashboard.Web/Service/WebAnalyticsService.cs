//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Earn.Dashboard.DAL;
using Earn.DataContract.WebAnalytics;
using Earn.Dashboard.DAL.db.Analytics;

namespace Earn.Dashboard.Web.Service
{
    public class WebAnalyticsService
    {
        public static async Task<List<AggregatedByPageTitleRecord>> FetchByPageTitleAnalyticsForPeriodAsync(RequestFilter filter)
        {
            List<AggregatedByPageTitleRecord> data = await AnalyticsDal.FetchPageVisitsAnalyticsForPeriodAsync(filter);
            return data;
        }

        public static async Task<List<AggregatedByDeviceTypeRecord>> FetchByDeviceTypeAnalyticForPeriodAsync(RequestFilter filter)
        {
            List<AggregatedByDeviceTypeRecord> data = await AnalyticsDal.FetchByDeviceTypeAnalyticsForPeriodAsync(filter);
            return data;
        }

        public static async Task<List<AggregatedVisitorsByDateRecord>> FetchVisitorsByDateAnalyticsForPeriodAsync(RequestFilter filter)
        {
            List<AggregatedVisitorsByDateRecord> data = await AnalyticsDal.FetchVisitorsByDateAnalyticsForPeriodAsync(filter);
            return data;
        }

        public static async Task<List<AggregatedNewUsersByDateRecord>> FetchNewUsersByDateAnalyticsForPeriodAsync(RequestFilter filter)
        {
            List<AggregatedNewUsersByDateRecord> data = await AnalyticsDal.FetchNewUsersByDateAnalyticsForPeriodAsync(filter);
            return data;
        }

        public static async Task<List<analytic>> FetchAnalyticsForPeriodAsync(RequestFilter filter)
        {
            List<analytic> data = await AnalyticsDal.FetchAnalyticsForPeriodAsync(filter);
            return data;
        }
    }
}