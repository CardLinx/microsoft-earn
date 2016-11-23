//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Interface for deal to be ranked.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Lomo.DataModels.Offers.Query
{
    using System;

    /// <summary>
    /// Ranker's name enumeration
    /// </summary>
    public enum RankerName
    {
        /// <summary>
        /// DistanceRanker name
        /// </summary>
        DistanceRanker = 0,

        /// <summary>
        /// EndTimeRanker name
        /// </summary>
        EndTimeRanker = 1,

        /// <summary>
        /// StartTimeRanker name
        /// </summary>
        StartTimeRanker = 2,

        /// <summary>
        /// StartTimeDescRanker name 
        /// </summary>
        StartTimeDescRanker = 3,

        /// <summary>
        /// OnlineDealsRanker name
        /// </summary>
        OnlineDealsRanker = 4,

        /// <summary>
        /// DefaultRanker name
        /// </summary>
        DefaultRanker = 5,
    }
    
    /// <summary>
    /// Interface for deal rankers.
    /// </summary>
    public interface IRankable
    {
        /// <summary>
        /// Returns deal Id.
        /// </summary>
        /// <returns>deal Id.</returns>
        Guid GetId();

        /// <summary>
        /// Returns deal end time.
        /// </summary>
        /// <returns>deal end time.</returns>
        DateTime? GetEndTime();

        /// <summary>
        /// Returns deal start time.
        /// </summary>
        /// <returns>deal start time.</returns>
        DateTime? GetStartTime();

        /// <summary>
        /// Gets deal rank
        /// </summary>
        /// <param name="queryContext">the query context</param>
        /// <returns>the rank</returns>
        byte GetRank(QueryContext queryContext);

        /// <summary>
        /// Gets deal BusinessId
        /// </summary>
        /// <returns>the business id</returns>
        string GetBusinessId();
    }
}