//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Cache Statistics
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMoCache
{
    using System.Diagnostics;
    using PerformanceCounters;

    /// <summary>
    /// Cache statistics.
    /// </summary>
    public class CacheStats
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheStats"/> class. 
        /// Initializes a new cache stats instance.
        /// </summary>
        public CacheStats()
        {
            TotalCacheHits = 0;
            TotalCacheMisses = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the total cache hits. This will evolve into window-based computation of hit rate over time
        /// </summary>
        public long TotalCacheHits { get; set; }

        /// <summary>
        /// Gets or sets the total cache misses.
        /// </summary>
        public long TotalCacheMisses { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the cache statistics for a call to the GetDealsById() method.
        /// </summary>
        /// <param name="cacheHits">Total number of cache hits made in method.</param>
        /// <param name="cacheMisses">Total number of cache misses made in method.</param>
        public void UpdateDealIdRequestStats(int cacheHits, int cacheMisses)
        {
            // perf counters are per method call, not cache call
            TotalCacheHits += cacheHits;
            TotalCacheMisses += cacheMisses;
        }

        /// <summary>
        /// Updates the cache statistics for a call to the GetOnlineDeals() method.
        /// </summary>
        /// <param name="stopwatch">the stop watch.</param>
        /// <param name="cacheHits">Total number of cache hits made in method.</param>
        /// <param name="cacheMisses">Total number of cache misses made in method.</param>
        public void UpdateOnlineDealsRequestStats(Stopwatch stopwatch, int cacheHits, int cacheMisses)
        {
            Counter.Increment(CounterNames.OnlineTotalRequests);
            Counter.Increment(CounterNames.OnlineRequestsPerSec);
            Counter.Increment(CounterNames.OnlineAvgSec, stopwatch);

            TotalCacheHits += cacheHits;
            TotalCacheMisses += cacheMisses;
        }

        #endregion
    }
}