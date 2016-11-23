//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace PerformanceCounters
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// List of counter categories
    /// </summary>
    public enum CounterCategory
    {
        DealsServer,
        Commerce,
        AnalyticsWorker,
        OffersServer
    }

    /// <summary>
    ///     The counter.
    /// </summary>
    public static class Counter
    {

        #region Private Fields

        /// <summary>
        /// Category under which the counters has to be installed. 
        /// </summary>
        private static string category = "DealsServer";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The initialize method.
        /// </summary>
        /// <param name="appDefaultCateogry">
        /// The app default cateogry.
        /// </param>
        public static void Init(string appDefaultCateogry)
        {
            category = appDefaultCateogry;
        }

        /// <summary>
        ///     The get average time counter.
        /// </summary>
        /// <param name="counterName">
        ///     The counter name.
        /// </param>
        /// <param name="readOnly">
        ///     The read only.
        /// </param>
        /// <returns>
        ///     The <see cref="AverageTimeCounter" />.
        /// </returns>
        public static AverageTimeCounter GetAverageTimeCounter(string counterName, bool readOnly = true)
        {
            var averageTimeCounter = new AverageTimeCounter(counterName, readOnly);
            return averageTimeCounter;
        }

        /// <summary>
        ///     The get counter.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="readOnly">
        ///     The read only.
        /// </param>
        /// <returns>
        ///     The <see cref="PerformanceCounter" />.
        /// </returns>
        public static PerformanceCounter GetCounter(string name, bool readOnly = true)
        {
            PerformanceCounter counter;
            try
            {
                counter = new PerformanceCounter(category, name, readOnly);
            }
            catch
            {
                // this allows the code to work normally even when counters
                // were not created.
                // reported values will be incorrect for any rate type counter.
                counter = new PerformanceCounter
                              {
                                  CounterName = name,
                                  CategoryName = category,
                                  MachineName = "."
                              };
            }

            return counter;
        }

        /// <summary>
        ///     The increment.
        /// </summary>
        /// <param name="counterName">
        ///     The counter name.
        /// </param>
        public static void Increment(string counterName)
        {
            PerformanceCounter pc = GetCounter(counterName, false);
            if (pc != null && !pc.ReadOnly)
            {
                pc.Increment();
            }
        }

        /// <summary>
        /// The increment.
        /// </summary>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void IncrementBy(string counterName, int value)
        {
            PerformanceCounter pc = GetCounter(counterName, false);
            if (pc != null && !pc.ReadOnly)
            {
                pc.IncrementBy(value);
            }
        }


        /// <summary>
        ///     The decrement.
        /// </summary>
        /// <param name="counterName">
        ///     The counter name.
        /// </param>
        public static void Decrement(string counterName)
        {
            PerformanceCounter pc = GetCounter(counterName, false);
            if (pc != null && !pc.ReadOnly)
            {
                pc.Decrement();
            }
        }

        /// <summary>
        ///     The increment.
        /// </summary>
        /// <param name="counterName">The counter name.</param>
        /// <param name="stopWatch">The stop watch.</param>
        /// <param name="increment">The increment.</param>
        public static void Increment(string counterName, Stopwatch stopWatch, int increment = 1)
        {
            PerformanceCounter averageCounter = GetCounter(counterName, false);
            PerformanceCounter baseCounter = GetCounter(counterName + "base", false);
            if (averageCounter != null && !averageCounter.ReadOnly && baseCounter != null && !baseCounter.ReadOnly)
            {
                averageCounter.IncrementBy(stopWatch.ElapsedTicks);
                baseCounter.IncrementBy(increment);
            }
        }

        /// <summary>
        ///     The reset.
        /// </summary>
        /// <param name="counterName">
        ///     The counter name.
        /// </param>
        public static void Reset(string counterName)
        {
            PerformanceCounter pc = GetCounter(counterName, false);
            if (pc != null && !pc.ReadOnly)
            {
                pc.RawValue = 0;
            }
        }

        public static void SetValue(string counterName, int value)
        {
            PerformanceCounter pc = GetCounter(counterName, false);
            if (pc != null && !pc.ReadOnly)
            {
                pc.RawValue = value;
            }
        }

        public static void SetFractionValue(string counterName, long value, long baseValue)
        {
            PerformanceCounter pc = GetCounter(counterName, false);
            PerformanceCounter counterBase = GetCounter(counterName + "Base", false);
            if (pc != null && !pc.ReadOnly)
            {
                pc.RawValue = value;
            }

            if (counterBase != null && !counterBase.ReadOnly)
            {
                counterBase.RawValue = baseValue;
            }
        }

        /// <summary>
        /// Creates a list of counters for the given counter category
        /// </summary>
        /// <param name="counterCategory">counter category</param>
        public static void CreateCounters(CounterCategory counterCategory)
        {
            var counterCollection = new CounterCreationDataCollection();
            IEnumerable<CounterInfo> counters = GetCounters(counterCategory);

            if (counters != null)
            {
                string strCounterCategory = counterCategory.ToString();
                foreach (var counter in counters)
                {
                    switch (counter.Type)
                    {
                        case PerformanceCounterType.RateOfCountsPerSecond32:
                            counterCollection.AddRateOfCountsPerSecond32(counter.Name, counter.Description);
                            break;
                        case PerformanceCounterType.NumberOfItems32:
                            counterCollection.AddNumberOfItems32(counter.Name, counter.Description);
                            break;
                        case PerformanceCounterType.AverageTimer32:
                            counterCollection.AddAverageTimer32(counter.Name, counter.Description);
                            break;
                        case PerformanceCounterType.RawFraction:
                            counterCollection.AddRawFraction(counter.Name, counter.Description);
                            break;
                    }
                }

                // Register the counters
                PerformanceCounterCategory.Create(strCounterCategory, "Lomo Deal Preprocessor Counters", PerformanceCounterCategoryType.SingleInstance, counterCollection);
                category = strCounterCategory;
            }
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the list of counters for the specified category
        /// </summary>
        /// <param name="counterCategory">Counter category</param>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetCounters(CounterCategory counterCategory)
        {
            List<CounterInfo> counters = null;
            switch (counterCategory)
            {
                case CounterCategory.DealsServer:
                    counters = GetDealsServerCounters();
                    break;
                case CounterCategory.Commerce:
                    counters = GetAnalyticsCounters();
                    break;
                case CounterCategory.AnalyticsWorker:
                    counters = GetAnalyticsWorkerCounters();
                    break;

                case CounterCategory.OffersServer:
                    counters = GetOffersServerCounters();
                    break;
            }

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for Deals Server
        /// </summary>
        /// <returns>List of counters</returns>
        private static List<CounterInfo> GetDealsServerCounters()
        {
            var counters = GetDealsByIdsCounters().ToList();
            counters.AddRange(GetNearbyDealsCounters());
            counters.AddRange(GetInMemoryIndexCounters());
            counters.AddRange(GetGeocodingCounters());
            counters.AddRange(GetUserProfileCounters());
            counters.AddRange(GetDatasourceCounters());
            counters.AddRange(GetOnlineDealsCounters());
            counters.AddRange(GetImageResizeCounters());
            counters.AddRange(GetAnalyticsCounters());
            counters.AddRange(GetOutlookCounters());
            counters.AddRange(GetDealsByRegionCounters());
            counters.AddRange(GetTestCounters());

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for Offers Server
        /// </summary>
        /// <returns>List of counters</returns>
        private static List<CounterInfo> GetOffersServerCounters()
        {
            var counters = GetAllOffersServerCounters().ToList();
            return counters;
        }

        /// <summary>
        /// Returns the list of counters for testing
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<CounterInfo> GetTestCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test1AvgSec,
                                           Description = "Average time for Test 1.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test2AvgSec,
                                           Description = "Average time for Test 2.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test3AvgSec,
                                           Description = "Average time for Test 3.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test4AvgSec,
                                           Description = "Average time for Test 4.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test5AvgSec,
                                           Description = "Average time for Test 5.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test6AvgSec,
                                           Description = "Average time for Test 6.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test7AvgSec,
                                           Description = "Average time for Test 7.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test8AvgSec,
                                           Description = "Average time for Test 8.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.Test9AvgSec,
                                           Description = "Average time for Test 9.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for DealsById
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<CounterInfo> GetDealsByIdsCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ByIdRequestsPerSec,
                                           Description = "Number of requests per second to the GetDealsById cache lookup layer.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ByIdTotalRequests,
                                           Description = "Total number of requests to the GetDealsById cache lookup layer.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ByIdAvgSec,
                                           Description = "Average time per GetDealsById request to the cache lookup layer.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for DealsById
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<CounterInfo> GetAllOffersServerCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeoCodeServiceTotalGetCount,
                                           Description = "Total requests to GeoCodeService.Get method.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeoCodeServiceTotalRequestPerSecond,
                                           Description = "Number of requests per second to the GetCodeService.Get method",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeoCodeCacheHitCount,
                                           Description = "Total cache hits for GeoCode cache.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeoCodeCacheMissCount,
                                           Description = "Total cache missses for GeoCode cache.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeoCodeServiceGetTime,
                                           Description = "Average time for geocode service API call",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsByRegionApiTotalGetCount,
                                           Description = "Total number of requests to the GetDealsByRegion endpoint.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsByRegionApiGetFrequency,
                                           Description = "Number of requests per second to the GetDealsByRegion endpoint.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsByRegionApiGetTime,
                                           Description = "Average time for the GetDealsByRegion endpoint.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                    new CounterInfo
                                       {
                                           Name = CounterNames.GetNearbyDealsApiTotalGetCount,
                                           Description = "Total number of requests to the GetNearbyDeals endpoint.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetNearbyDealsApiGetFrequency,
                                           Description = "Number of requests per second to the GetNearbyDeals endpoint.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetNearbyDealsApiGetTime,
                                           Description = "Average time per GetNearbyDeals request end to end.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsByIdApiTotalGetCount,
                                           Description = "Total number of requests to the GetDealsById endpoint.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsByIdApiGetFrequency,
                                           Description = "Number of requests per second to the GetDealsById endpoint.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsByIdApiGetTime,
                                           Description = "Average time per GetDealsById request end to end.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsBySeoUrlApiTotalGetCount,
                                           Description = "Total number of requests to the GetDealsBySeoUrl endpoint.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsBySeoUrlApiGetFrequency,
                                           Description = "Number of requests per second to the GetDealsBySeoUrl endpoint.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetDealsBySeoUrlApiGetTime,
                                           Description = "Average time per GetDealsBySeoUrl request end to end.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                    new CounterInfo
                                       {
                                           Name = CounterNames.GetOnlineDealsApiTotalGetCount,
                                           Description = "Total number of requests to the GetOnlineDeals endpoint.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetOnlineDealsApiGetFrequency,
                                           Description = "Number of requests per second to the GetOnlineDeals endpoint.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GetOnlineDealsApiGetTime,
                                           Description = "Average time per GetOnlineDeals request end to end.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalOnlineDealsTotalQueryCount,
                                           Description = "Total number of requests to the MongoDB for online deals query.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalOnlineDealsQueryFrequency,
                                           Description = "Number of requests per second to the MongoDB for online deals query.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalOnlineDealsQueryTime,
                                           Description = "Average time taken by the MongoDB for online deals query.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalNearByDealsWhenRegionIsNullTotalQueryCount,
                                           Description = "Total number of requests to the MongoDB for NearByDeals query when region is null.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalNearByDealsWhenRegionIsNullTotalQueryFrequency,
                                           Description = "Number of requests per second to the MongoDB for NearByDeals query when region is null.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalNearByDealsWhenRegionIsNullQueryTime,
                                           Description = "Average time taken by the MongoDB for NearByDeals query when region is null.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalNearByDealsWhenRegionIsNotNullTotalQueryCount,
                                           Description = "Total number of requests to the MongoDB for NearByDeals query when region is not null.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalNearByDealsWhenRegionIsNotNullTotalQueryFrequency,
                                           Description = "Number of requests per second to the MongoDB for NearByDeals query when region is not null.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalNearByDealsWhenRegionIsNotNullQueryTime,
                                           Description = "Average time taken by the MongoDB for NearByDeals query when region is not null.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalDealsByIdTotalQueryCount,
                                           Description = "Total number of requests to the MongoDB for DealsById query.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalDealsByIdTotalQueryFrequency,
                                           Description = "Number of requests per second to the MongoDB for DealsById query",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalDealsByIdQueryTime,
                                           Description = "Average time by the MongoDB for DealsById query.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalDealsBySeoUrlTotalQueryCount,
                                           Description = "Total number of requests to the MongoDB for DealsBySeoUrl query.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalDealsBySeoUrlTotalQueryFrequency,
                                           Description = "Number of requests per second to the MongoDB for DealsBySeoUrl query.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalDealsBySeoUrlQueryTime,
                                           Description = "Average time by the MongoDB for DealsBySeoUrl query.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalNearByDealsSortingTime,
                                           Description = "Average time to sort deals in memory for GetNearbyDeals request.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalDealDeserializationTime,
                                           Description = "Average time to deserialize Deal from BsonDocument at Mongo Dal layer.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.MongoDalDealSlimDeserializationTime,
                                           Description = "Average time to deserialize DealSlim from BsonDocument at Mongo Dal layer.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                    new CounterInfo
                                       {
                                           Name = CounterNames.RedisCacheGetDealsByIdsTotalCount,
                                           Description = "Total requests to redis cache for DealsById.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                    new CounterInfo
                                       {
                                           Name = CounterNames.RedisCacheGetDealsByIdsFrequency,
                                           Description = "Total requests per second to redis cache for DealsById.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                    new CounterInfo
                                       {
                                           Name = CounterNames.RedisCacheGetDealsByIdsTime,
                                           Description = "Average time taken by redis for DealsById request.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },    
                                    new CounterInfo
                                       {
                                           Name = CounterNames.RedisCacheSaveDealsByIdsTotalCount,
                                           Description = "Total requests to redis cache for SaveDealsById.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                    new CounterInfo
                                       {
                                           Name = CounterNames.RedisCacheSaveDealsByIdsFrequency,
                                           Description = "Total requests per second to redis cache for SaveDealsById.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },  
                                    new CounterInfo
                                       {
                                           Name = CounterNames.RedisCacheSaveDealsByIdsTime,
                                           Description = "Average time taken by redis cache for SaveDealsById.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for GetDealsByRegion call
        /// </summary>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetDealsByRegionCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ByRegionRequestsPerSec,
                                           Description = "Number of requests per second to the GetDealsByRegion.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ByRegionTotalRequests,
                                           Description = "Total number of requests to the GetDealsByRegion.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ByRegionAvgSec,
                                           Description = "Average time per GetDealsByRegion request.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for NearbyDeals call
        /// </summary>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetNearbyDealsCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.NearbyRequestsPerSec,
                                           Description = "Number of requests per second to the GetNearbyDeals cache lookup layer.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.NearbyTotalRequests,
                                           Description = "Total number of requests to the GetNearbyDeals cache lookup layer.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.NearbyAvgSec,
                                           Description = "Average time per GetNearbyDeals request cache lookup layer.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.NearbyServiceAvgSec,
                                           Description = "Average time per GetNearbyDeals request service layer.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for InMemIndex
        /// </summary>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetInMemoryIndexCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.InMemIndexRequestsPerSec,
                                           Description = "Number of requests per second to in-mem index.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.InMemIndexTotalRequests,
                                           Description = "Total number of requests to in-mem index.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.InMemIndexAvgSec,
                                           Description = "Average time for in-mem index request.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for Geocoding call
        /// </summary>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetGeocodingCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeocodingRequestsPerSec,
                                           Description = "Number of requests per second to Geocode a location (call to GeocodeCache.Getgeocodepoint)",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeocodingTotalRequests,
                                           Description = "Total number of requests to Geocode a location (call to GeocodeCache.Getgeocodepoint)",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeocodingAvgSec,
                                           Description = "Average time to get geocodepoint from Geocode cache.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.GeocodingWithoutCacheAvgSec,
                                           Description = "Average time to get geocodepoint from external source.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for the user profile call to the EFS proxy service
        /// </summary>
        /// <returns>
        /// List of counters
        /// </returns>
        private static IEnumerable<CounterInfo> GetUserProfileCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.UserProfileRequestsPerSec,
                                           Description = "Number of requests per second to get an user profile",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.UserProfileTotalRequests,
                                           Description = "Total number of requests to get user profile",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.UserProfileFromCacheAvgSec,
                                           Description = "Average time to get user profile from profile data cache in deals server",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.UserProfileFromEfsAvgSec,
                                           Description = "Average time to get user profile from efs proxy via efs service",
                                           Type = PerformanceCounterType.AverageTimer32
                                       },
                                       new CounterInfo
                                       {
                                           Name = CounterNames.UserProfileCacheHits,
                                           Description = "Total number of cache hits for the user profile request",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                       new CounterInfo
                                       {
                                           Name = CounterNames.UserProfileCacheMiss,
                                           Description = "Total number of cache misses for the user profile request",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for DataSource
        /// </summary>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetDatasourceCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.DataSourceRequestsPerSec,
                                           Description = "Number of requests per second to backing data source.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.DataSourceTotalRequests,
                                           Description = "Total number of requests to backing data source.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.DataSourceAvgSec,
                                           Description = "Average time for backing data source request.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for OnlineDeals call
        /// </summary>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetOnlineDealsCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.OnlineRequestsPerSec,
                                           Description = "Number of requests per second to the GetOnlineDeals cache lookup layer.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.OnlineTotalRequests,
                                           Description = "Total number of requests to the GetOnlineDeals cache lookup layer.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.OnlineAvgSec,
                                           Description = "Average time per GetOnlineDeals request to the cache lookup layer.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for Image requests to CDN
        /// </summary>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetImageResizeCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ImageResizeParallelRequests,
                                           Description ="Number of parallel requests to image resizer (cdn controller).",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ImageResizeInputRequestsPerSec,
                                           Description ="Number of input requests per second to image resizer (cdn controller).",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ImageResizeCompletedRequestsPerSec,
                                           Description ="Number of completed requests per second to image resizer (cdn controller).",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ImageResizeCompletedTotalRequests,
                                           Description = "Total number of completed requests to image resize (cdn controller).",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.ImageResizeAvgSec,
                                           Description = "Average time for image resize request (cdn controller).",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for outlook controller
        /// </summary>
        /// <returns>List of counters</returns>
        private static IEnumerable<CounterInfo> GetOutlookCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.OutlookByIdParallelRequests,
                                           Description ="Number of parallel get deals by id requests to outlook controller.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.OutlookGetDealsParallelRequests,
                                           Description ="Number of parallel get deals requests to outlook controller.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       }
                                };

            return counters;
        }

        /// <summary>
        /// Returns the list of counters for Analytics
        /// </summary>
        /// <returns>List of counters</returns>
        private static List<CounterInfo> GetAnalyticsCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = CounterNames.AnalyticsLocalQueuePerSec,
                                           Description = "Number of blocks saved to analytics azure queue per second.",
                                           Type = PerformanceCounterType.RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.AnalyticsLocalQueueLength,
                                           Description = "Total number of analytics items in the local (memory) queue.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.AnalyticsApproximateAzureQueueLength,
                                           Description = "Total number of analytics items in the azure queue.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.AnalyticsAzureQueuePendingWrites,
                                           Description = "Total number of pending async writes waiting for the azure analytics queue.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name = CounterNames.AnalyticsAzureBatchSize,
                                           Description = "Enqueue Batch Size.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       }
                               };

            return counters;
        }

        /// <summary>
        /// The get analytics worker counters.
        /// </summary>
        /// <returns>
        /// The counters list </returns>
        private static List<CounterInfo> GetAnalyticsWorkerCounters()
        {
            var counters = new List<CounterInfo>
                               {
                                   new CounterInfo
                                       {
                                           Name = AnalyticsWorkerCounterNames.TotalTaskCount,
                                           Description =
                                               "Number of concurrent queue processing tasks.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name =
                                               AnalyticsWorkerCounterNames.TotalQueueCount,
                                           Description = "Number of analytics queues",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name =
                                               AnalyticsWorkerCounterNames
                                               .TotalMessageCount,
                                           Description =
                                               "Total number messages in analytics queues.",
                                           Type = PerformanceCounterType.NumberOfItems32
                                       },
                                   new CounterInfo
                                       {
                                           Name =
                                               AnalyticsWorkerCounterNames
                                               .ProcessingSuccessRate,
                                           Description = "Analytics Worker Processing Success Rate",
                                           Type = PerformanceCounterType.RawFraction
                                       },
                                       new CounterInfo
                                       {
                                           Name =
                                               AnalyticsWorkerCounterNames
                                               .FindParentEventSuccessRate,
                                           Description = "Analytics Worker Find Parent Event Success Rate",
                                           Type = PerformanceCounterType.RawFraction
                                       },
                                   new CounterInfo
                                       {
                                           Name =
                                               AnalyticsWorkerCounterNames
                                               .MessageProcessedPerSec,
                                           Description =
                                               "Analytics Worker Message Processed Per Sec",
                                           Type =
                                               PerformanceCounterType
                                               .RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name =
                                               AnalyticsWorkerCounterNames
                                               .EntityProcessedPerSec,
                                           Description =
                                               "Analytics Worker Entity Processed Per Sec",
                                           Type =
                                               PerformanceCounterType
                                               .RateOfCountsPerSecond32
                                       },
                                   new CounterInfo
                                       {
                                           Name = AnalyticsWorkerCounterNames.MessageProcessAvgTime,
                                           Description = "Average time per analytics message processing.",
                                           Type = PerformanceCounterType.AverageTimer32
                                       }
                               };

            return counters;
            
        } 

        #endregion
    }
}