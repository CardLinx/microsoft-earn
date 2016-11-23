//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace PerformanceCounters
{
    /// <summary>
    /// The counter names.
    /// </summary>
    public class CounterNames
    {
        #region Fields

        /// <summary>
        /// The total deals by id requests with cache.
        /// </summary>
        public const string ByIdTotalRequests = "ById Total Requests";

        /// <summary>
        /// The deals by id requests per second with cache.
        /// </summary>
        public const string ByIdRequestsPerSec = "ById Requests/sec";

        /// <summary>
        /// The deals by id request average time with cache.
        /// </summary>
        public const string ByIdAvgSec = "ById Avg sec";

        // GetNearbyDeals Requests
        public const string NearbyRequestsPerSec = "Nearby Requests/sec";
        public const string NearbyTotalRequests = "Nearby Total Requests";
        public const string NearbyAvgSec = "Nearby Avg sec";
        public const string NearbyServiceAvgSec = "Nearby Service Avg sec";

        // GetDealsByRegion Requests
        public const string ByRegionRequestsPerSec = "ByRegion Requests/sec";
        public const string ByRegionTotalRequests = "ByRegion Total Requests";
        public const string ByRegionAvgSec = "ByRegion Avg sec";

        // InMemIndex counters
        public const string InMemIndexRequestsPerSec = "InMemIndex Requests/sec";
        public const string InMemIndexTotalRequests = "InMemIndex Total Requests";
        public const string InMemIndexAvgSec = "InMemIndex Avg sec";

        // Geocode call counters
        public const string GeocodingRequestsPerSec = "Geocoding Requests/sec";
        public const string GeocodingTotalRequests = "Geocoding Total Requests";
        public const string GeocodingAvgSec = "Geocoding Avg sec";
        public const string GeocodingWithoutCacheAvgSec = "Geocoding (w/o cache) Avg sec";

        public const string UserProfileRequestsPerSec = "UserProfile Requests/sec";
        public const string UserProfileTotalRequests = "UserProfile Total Requests";
        public const string UserProfileFromCacheAvgSec = "UserProfile from Cache Avg sec";
        public const string UserProfileFromEfsAvgSec = "UserProfile from efs Avg sec";
        public const string UserProfileCacheHits = "UserProfile Cache Hits";
        public const string UserProfileCacheMiss = "UserProfile Cache Misses";

        // Data source counters
        public const string DataSourceRequestsPerSec = "DataSource Requests/sec";
        public const string DataSourceTotalRequests = "DataSource Total Requests";
        public const string DataSourceAvgSec = "DataSource Avg sec";

        // GetOnlineDeals Requests
        /// <summary>
        /// The total online deals requests with cache.
        /// </summary>
        public const string OnlineTotalRequests = "Online Total Requests";

        /// <summary>
        /// The online deals requests per second with cache.
        /// </summary>
        public const string OnlineRequestsPerSec = "Online Requests/sec";

        /// <summary>
        /// The online deals request average time with cache.
        /// </summary>
        public const string OnlineAvgSec = "Online Avg sec";

        /// <summary>
        /// analytics: blocks sent to azure queue per second (1 block could contains several items)
        /// </summary>
        public const string AnalyticsLocalQueuePerSec = "AnalyticsLocalQueuePerSecond";

        /// <summary>
        /// analytics: # items in the local (memory queue)
        /// </summary>
        public const string AnalyticsLocalQueueLength = "AnalyticsLocalQueueLength";

        /// <summary>
        /// analytics: # items in the azure queue
        /// </summary>
        public const string AnalyticsApproximateAzureQueueLength = "AnalyticsApproximateAzureQueueLength";

        /// <summary>
        /// analytics: # pending async writers to the azure queue
        /// </summary>
        public const string AnalyticsAzureQueuePendingWrites = "AnalyticsAzureQueuePendingWrites";

        /// <summary>
        /// Analytics : batch size
        /// </summary>
        public const string AnalyticsAzureBatchSize = "AnalyticsAzureBatchSize";


        // Image Resizer counters (CDN Controller)
        public const string ImageResizeParallelRequests = "ImageResize Parallel Requests";
        public const string ImageResizeInputRequestsPerSec = "ImageResize Input Requests/sec";
        public const string ImageResizeCompletedRequestsPerSec = "ImageResize Completed Requests/sec";
        public const string ImageResizeCompletedTotalRequests = "ImageResize Total Completed Requests";
        public const string ImageResizeAvgSec = "ImageResize Avg sec";

        // outlook counters
        public const string OutlookByIdParallelRequests = "Outlook ById Parallel Requests";
        public const string OutlookGetDealsParallelRequests = "Outlook GetDeals Parallel Requests";

        public const string Test1AvgSec = "Test 1 Avg sec";
        public const string Test2AvgSec = "Test 2 Avg sec";
        public const string Test3AvgSec = "Test 3 Avg sec";
        public const string Test4AvgSec = "Test 4 Avg sec";
        public const string Test5AvgSec = "Test 5 Avg sec";
        public const string Test6AvgSec = "Test 6 Avg sec";
        public const string Test7AvgSec = "Test 7 Avg sec";
        public const string Test8AvgSec = "Test 8 Avg sec";
        public const string Test9AvgSec = "Test 9 Avg sec";

        //Offers server counters

        public const string GeoCodeServiceTotalGetCount = "GeoCodeServiceTotalGetCount";
        public const string GeoCodeServiceTotalRequestPerSecond = "GeoCodeServiceTotalRequestPerSecond";
        public const string GeoCodeServiceGetTime = "GeoCodeServiceGetTime";
        public const string GeoCodeCacheHitCount = "GeoCodeCacheHitCount";
        public const string GeoCodeCacheMissCount = "GeoCodeCacheMissCount";
        
        public const string GetDealsByRegionApiTotalGetCount = "GetDealsByRegionApiTotalGetCount";
        public const string GetDealsByRegionApiGetFrequency = "GetDealsByRegionApiGetFrequency";
        public const string GetDealsByRegionApiGetTime = "GetDealsByRegionApiTotalTime";

        public const string GetNearbyDealsApiTotalGetCount = "GetNearbyDealsApiTotalGetCount";
        public const string GetNearbyDealsApiGetFrequency = "GetNearbyDealsApiGetFrequency";
        public const string GetNearbyDealsApiGetTime = "GetNearbyDealsApiGetTime";

        public const string GetDealsByIdApiTotalGetCount = "GetDealsByIdApiTotalGetCount";
        public const string GetDealsByIdApiGetFrequency = "GetDealsByIdApiGetFrequency";
        public const string GetDealsByIdApiGetTime = "GetDealsByIdApiGetTime";

        public const string GetDealsBySeoUrlApiTotalGetCount = "GetDealsBySeoUrlApiTotalGetCount";
        public const string GetDealsBySeoUrlApiGetFrequency = "GetDealsBySeoUrlGetFrequency";
        public const string GetDealsBySeoUrlApiGetTime = "GetDealsBySeoUrlApiGetTime";

        public const string GetOnlineDealsApiTotalGetCount = "GetOnlineDealsApiTotalGetCount";
        public const string GetOnlineDealsApiGetFrequency = "GetOnlineDealsApiGetFrequency";
        public const string GetOnlineDealsApiGetTime = "GetOnlineDealsApiGetTime";

        public const string MongoDalOnlineDealsTotalQueryCount = "MongoDalOnlineDealsTotalQueryCount";
        public const string MongoDalOnlineDealsQueryFrequency = "MongoDalOnlineDealsQueryFrequency";
        public const string MongoDalOnlineDealsQueryTime = "MongoDalOnlineDealsQueryTime";

        public const string MongoDalNearByDealsWhenRegionIsNullTotalQueryCount = "MongoDalNearByDealsWhenRegionIsNullTotalQueryCount";
        public const string MongoDalNearByDealsWhenRegionIsNullTotalQueryFrequency = "MongoDalNearByDealsWhenRegionIsNullTotalQueryFrequency";
        public const string MongoDalNearByDealsWhenRegionIsNullQueryTime = "MongoDalNearByDealsWhenRegionIsNullQueryTime";

        public const string MongoDalNearByDealsWhenRegionIsNotNullTotalQueryCount = "MongoDalNearByDealsWhenRegionIsNotNullTotalQueryCount";
        public const string MongoDalNearByDealsWhenRegionIsNotNullTotalQueryFrequency = "MongoDalNearByDealsWhenRegionIsNotNullTotalQueryFrequency";
        public const string MongoDalNearByDealsWhenRegionIsNotNullQueryTime = "MongoDalNearByDealsWhenRegionIsNotNullQueryTime";

        public const string MongoDalDealsByIdTotalQueryCount = "MongoDalDealsByIdTotalQueryCount";
        public const string MongoDalDealsByIdTotalQueryFrequency = "MongoDalDealsByIdTotalQueryFrequency";
        public const string MongoDalDealsByIdQueryTime = "MongoDalDealsByIdQueryTime";

        public const string MongoDalDealsBySeoUrlTotalQueryCount = "MongoDalDealsBySeoUrlTotalQueryCount";
        public const string MongoDalDealsBySeoUrlTotalQueryFrequency = "MongoDalDealsBySeoUrlTotalQueryFrequency";
        public const string MongoDalDealsBySeoUrlQueryTime = "MongoDalDealsBySeoUrlQueryTime";

        public const string MongoDalNearByDealsSortingTime = "MongoDalNearByDealsSortingTime";
        public const string MongoDalDealDeserializationTime = "MongoDalDealDeserializationTime";
        public const string MongoDalDealSlimDeserializationTime = "MongoDalDealSlimDeserializationTime";

        public const string RedisCacheGetDealsByIdsTotalCount = "RedisCacheGetDealsByIdsTotalCount";
        public const string RedisCacheGetDealsByIdsFrequency = "RedisCacheGetDealsByIdsFrequency";
        public const string RedisCacheGetDealsByIdsTime = "RedisCacheGetDealsByIdsTime";

        public const string RedisCacheSaveDealsByIdsTotalCount = "RedisCacheSaveDealsByIdsTotalCount";
        public const string RedisCacheSaveDealsByIdsFrequency = "RedisCacheSaveDealsByIdsFrequency";
        public const string RedisCacheSaveDealsByIdsTime = "RedisCacheSaveDealsByIdsTime";

        #endregion Fields
    }
}