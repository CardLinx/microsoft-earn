//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Earn.Dashboard.DAL.db.Analytics;
using Earn.DataContract.WebAnalytics;

namespace Earn.Dashboard.DAL
{
    public class AnalyticsDal
    {
        protected static IQueryable<analytic> FetchAnalyticsQuery(RequestFilter filter, earnanalyticsEntities dbContext)
        {
            dbContext = new earnanalyticsEntities();
            IQueryable<analytic> query = from a in dbContext.analytics
                                         where (filter.StartDate <= a.ServerTimeStamp) && (a.ServerTimeStamp < filter.EndDate)
                                         select a;

            if (!string.IsNullOrWhiteSpace(filter.CampaignId))
            {
                query = query.Where(x => x.cmp_ref.Contains(filter.CampaignId));
            }

            if (filter.EventId != EventFilter.All)
            {
                string eventId = GetEventIdFilterValue(filter.EventId);
                query = query.Where(x => x.EventId == eventId);
            }

            return query;
        }

        public static async Task<List<analytic>> FetchAnalyticsForPeriodAsync(RequestFilter filter)
        {
            earnanalyticsEntities dbContext = new earnanalyticsEntities();
            var baseQuery = FetchAnalyticsQuery(filter, dbContext);
            return await baseQuery.ToListAsync();
        }

        public static async Task<List<AggregatedByPageTitleRecord>> FetchPageVisitsAnalyticsForPeriodAsync(RequestFilter filter)
        {
            earnanalyticsEntities dbContext = new earnanalyticsEntities();
            var baseQuery = FetchAnalyticsQuery(filter, dbContext);
            var query = from a in 
                           (from bq in baseQuery
                            where (filter.StartDate <= bq.ServerTimeStamp) && (bq.ServerTimeStamp < filter.EndDate)
                            select new { SessionId = bq.SessionId, PageTitle = bq.PageTitle }).Distinct()
                        group a by a.PageTitle into gr
                        orderby gr.Count() descending
                        select new AggregatedByPageTitleRecord
                        {
                            PageTitle = gr.Key,
                            Count = gr.Count()
                        };


            return await query.ToListAsync();
        }

        public static async Task<List<AggregatedByDeviceTypeRecord>> FetchByDeviceTypeAnalyticsForPeriodAsync(RequestFilter filter)
        {
            earnanalyticsEntities dbContext = new earnanalyticsEntities();
            var baseQuery = FetchAnalyticsQuery(filter, dbContext);
            var query = from a in
                           (from bq in baseQuery
                            where (filter.StartDate <= bq.ServerTimeStamp) && (bq.ServerTimeStamp < filter.EndDate)
                            select new { SessionId = bq.SessionId, DeviceType = bq.DeviceType }).Distinct()
                        group a by a.DeviceType into gr
                        select new AggregatedByDeviceTypeRecord
                        {
                            DeviceType = gr.Key,
                            Count = gr.Count()
                        };

            return await query.ToListAsync();
        }

        public static async Task<List<AggregatedVisitorsByDateRecord>> FetchVisitorsByDateAnalyticsForPeriodAsync(RequestFilter filter)
        {
            earnanalyticsEntities dbContext = new earnanalyticsEntities();
            var baseQuery = FetchAnalyticsQuery(filter, dbContext);
            var query = from a in
                           (from bq in baseQuery
                            where (filter.StartDate <= bq.ServerTimeStamp) && (bq.ServerTimeStamp < filter.EndDate)
                            select new { SessionId = bq.SessionId, Date = bq.ServerTimeStamp }).Distinct()
                        group a by DbFunctions.TruncateTime(a.Date).Value into gr
                        select new AggregatedVisitorsByDateRecord
                        {
                            Date = gr.Key,
                            TotalVisitors = gr.Count()
                        };

            return await query.ToListAsync();
        }

        public static async Task<List<AggregatedNewUsersByDateRecord>> FetchNewUsersByDateAnalyticsForPeriodAsync(RequestFilter filter)
        {
            earnanalyticsEntities dbContext = new earnanalyticsEntities();
            var baseQuery = FetchAnalyticsQuery(filter, dbContext);
            var query = from a in
                           (from bq in baseQuery
                            where (filter.StartDate <= bq.ServerTimeStamp) && (bq.ServerTimeStamp < filter.EndDate) && (bq.EventType == "registration") && (bq.EventId == "step3")
                            select new { SessionId = bq.SessionId, Date = bq.ServerTimeStamp }).Distinct()
                        group a by DbFunctions.TruncateTime(a.Date).Value into gr
                        select new AggregatedNewUsersByDateRecord
                        {
                            Date = gr.Key,
                            NewUsers = gr.Count()
                        };

            return await query.ToListAsync();
        }

        private static string GetEventIdFilterValue(EventFilter filter)
        {
            switch (filter)
            {
                case EventFilter.PageLoad:
                    {
                        return "page.load";
                    }
                case EventFilter.SessionNew:
                    {
                        return "session.new";
                    }
                case EventFilter.CardAdded:
                    {
                        return "card.added";
                    }
            }

            return string.Empty;
        }
    }
}