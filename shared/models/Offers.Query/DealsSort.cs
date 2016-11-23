//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Deals sort options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Lomo.DataModels.Offers.Query
{
    /// <summary>
    /// The options for deals sort.
    /// </summary>
    public enum DealsSort
    {
        /// <summary>
        ///     Rank by relevance, as determined by the service.
        ///     This triggers our own internal rankers.
        ///     This is the default "sort" option.
        /// </summary>
        Relevance = 0,

        /// <summary>
        ///     Rank by distance, nearest to farthest.
        ///     Useful for highlighting deals closest to the user.
        /// </summary>
        Distance = 1,

        /// <summary>
        ///     Rank by deal expiration date, soonest to latest.
        ///     Useful for highlighting deals about to expire.
        /// </summary>
        EndTime = 2,

        /// <summary>
        ///     Rank by deal start time, earliest start times first.
        /// </summary>
        StartTime = 3,

        /// <summary>
        ///     Rank by deal start time, newest start times first.
        /// </summary>
        StartTimeDesc = 4,
    }
}