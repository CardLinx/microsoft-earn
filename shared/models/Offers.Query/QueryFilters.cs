//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The query filters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DotM.DataContracts;

    /// <summary>
    /// Encapsulates service query filters
    /// </summary>
    public class QueryFilters
    {
        #region Constants

        /// <summary>
        /// Default value for the offset parameter.
        /// </summary>
        private const int DefaultOffset = 0;

        /// <summary>
        /// Default value for the count parameter.
        /// </summary>
        private const int DefaultCount = 50;

        /// <summary>
        /// Default value for the maxdealsperbusiness parameter.
        /// </summary>
        private readonly int? defaultMaxDealsPerBusiness = null;

        /// <summary>
        /// Default value for the activebefore parameter.
        /// </summary>
        private readonly DateTime defaultActiveBefore = DateTime.UtcNow;

        /// <summary>
        /// Default value for the expiresafter parameter.
        /// </summary>
        private readonly DateTime defaultExpiresAfter = DateTime.UtcNow;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFilters"/> class. 
        /// Creates a new query filter object and populates it with default values.
        /// </summary>
        /// <param name="count">Count to be returned by query.</param>
        /// <param name="radius">Radius associated with the query.</param>
        /// <param name="filterLocationsByRadius">Value indicating whether business locations are trimmed based on closeness to user location</param>
        public QueryFilters(int count = DefaultCount, double? radius = null, bool filterLocationsByRadius = true)
        {
            MaxDealsPerBusiness = defaultMaxDealsPerBusiness;
            QueryKeywords = Enumerable.Empty<string>();
            Categories = Enumerable.Empty<Category>();
            Sources = Enumerable.Empty<string>();
            ActiveBefore = defaultActiveBefore;
            ExpiresAfter = defaultExpiresAfter;
            Offset = DefaultOffset;
            Count = count;
            Radius = radius;
            FilterLocationsByRadius = filterLocationsByRadius;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the max numberof deals to return associated with a single business.
        /// </summary>
        public int? MaxDealsPerBusiness { get; set; }

        /// <summary>
        /// Gets or sets the query keywords.
        /// </summary>
        public IEnumerable<string> QueryKeywords { get; set; }

        /// <summary>
        /// Gets or sets the query categories.
        /// </summary>
        public IEnumerable<Category> Categories { get; set; }

        /// <summary>
        /// Gets or sets the query sources.
        /// </summary>
        public IEnumerable<string> Sources { get; set; }

        /// <summary>
        /// Gets or sets the timestamp that restricts the result set.
        /// Deals are included only if they are active before the given time.
        /// </summary>
        public DateTime ActiveBefore { get; set; }

        /// <summary>
        /// Gets or sets the timestamp that restricts the result set.
        /// Deals are included only if they expire after the given time.
        /// </summary>
        public DateTime ExpiresAfter { get; set; }

        /// <summary>
        /// Gets or sets the query paging offset.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the query count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the query radius (if applicable).
        /// </summary>
        public double? Radius { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to trim list of business locations. This is used to avoid business locations trimming based on user location.
        /// </summary>
        public bool FilterLocationsByRadius { get; set; }

        /// <summary>
        /// Gets or sets the query culture.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to match query string to business name only 
        /// </summary>
        public bool? IsBizNameOnly { get; set; }

        #endregion
    }
}