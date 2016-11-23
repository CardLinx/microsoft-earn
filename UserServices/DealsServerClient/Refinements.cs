//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The lomo deals refinements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DealsServerClient
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The options for in-memory deals sort, to be applied on the set of deals retrieved from the database.
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
        Expiration = 2, 

        /// <summary>
        ///     Rank by deal start time, earliest start times first.
        /// </summary>
        StartTimeAsc = 3, 

        /// <summary>
        ///     Rank by deal end time, latest start times first.
        ///     Useful for highlighting newly-active deals.
        /// </summary>
        StartTimeDesc = 4, 
    }

    /// <summary>
    ///     The deals refinements.
    /// </summary>
    public class Refinements
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the categories that's used to filter the result set to results from one or more sources.
        /// </summary>
        public IEnumerable<string> Categories { get; set; }

        /// <summary>
        ///     Gets or sets the active before. date and time that's used to restrict the results to only those deals that are active before the given time.
        /// </summary>
        public DateTime? ActiveBefore { get; set; }

        /// <summary>
        ///     Gets or sets the expires after date and time. Used to restrict the results to only those deals that expire after the given time.
        /// </summary>
        public DateTime? ExpiresAfter { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether hot offer.
        /// </summary>
        public bool HotOffer { get; set; }

        /// <summary>
        ///     Gets or sets  the results per business
        /// </summary>
        public int? ResultsPerBusiness { get; set; }

        /// <summary>
        ///     Gets or sets the offset.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        ///     Gets or sets the query keywords that's used to filter results to only deals that include the given query keyword/phrase to use in filtering.
        /// </summary>
        public IEnumerable<string> QueryKeywords { get; set; }

        /// <summary>
        /// Gets or sets the sort order requested by the query.
        /// </summary>
        public DealsSort Sort { get; set; }

        /// <summary>
        ///     Gets or sets the sources that's used to filter the result set to results from one or more sources.
        /// </summary>
        public IEnumerable<string> Sources { get; set; }

        /// <summary>
        ///     Gets or sets the timeout. (in milliseconds)
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the Market
        /// </summary>
        public string Market { get; set; }

        /// <summary>
        /// Gets or sets the flight
        /// </summary>
        public string Flights { get; set; }

        #endregion

    }
}