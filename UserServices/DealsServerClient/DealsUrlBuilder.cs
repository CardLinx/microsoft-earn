//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The deals url builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DealsServerClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The deals url builder.
    /// </summary>
    internal class DealsUrlBuilder
    {
        #region Constants

        /// <summary>
        /// The active before key.
        /// </summary>
        private const string ActiveBeforeKey = "activeBefore";

        /// <summary>
        /// The business ids key.
        /// </summary>
        private const string BusinessIdsKey = "businessids";

        /// <summary>
        /// The client key.
        /// </summary>
        private const string ClientKey = "client";

        /// <summary>
        /// The coordinate key.
        /// </summary>
        private const string CoordKey = "coord";

        private const string RegionKey = "region";

        private const string AnidKey = "anid";

        /// <summary>
        /// The count key.
        /// </summary>
        private const string CountKey = "count";

        /// <summary>
        /// The format key.
        /// </summary>
        private const string FormatKey = "format";

        /// <summary>
        /// The deal ids key.
        /// </summary>
        private const string DealIdsKey = "dealids";

        /// <summary>
        /// The expires after key.
        /// </summary>
        private const string ExpiresAfterKey = "expiresAfter";

        /// <summary>
        /// The get deals by business method.
        /// </summary>
        private const string GetDealsByBusinessMethod = "GetDealsByBusiness";

        /// <summary>
        /// The get deals by id method.
        /// </summary>
        private const string GetDealsByIdMethod = "GetDealsById";

        /// <summary>
        /// The get nearby deals method.
        /// </summary>
        private const string GetNearbyDealsMethod = "GetNearbyDeals";

        /// <summary>
        /// The get deals by region method.
        /// </summary>
        private const string GetDealsByRegionMethod = "GetDealsByRegion";


        /// <summary>
        /// The get online deals method.
        /// </summary>
        private const string GetOnlineDealsMethod = "GetOnlineDeals";

        /// <summary>
        /// The max deals per business key.
        /// </summary>
        private const string MaxDealsPerBusinessKey = "maxDealsPerBusiness";

        /// <summary>
        /// The radius key.
        /// </summary>
        private const string RadiusKey = "radius";

        /// <summary>
        /// The refinements key.
        /// </summary>
        private const string RefinementsKey = "refinements";

        /// <summary>
        /// The sort key.
        /// </summary>
        private const string SortKey = "sort";

        #endregion

        #region Fields

        /// <summary>
        /// The base uri.
        /// </summary>
        private readonly Uri baseUri;

        /// <summary>
        /// The client query parameter.
        /// </summary>
        private readonly KeyValuePair<string, string> clientQueryParameter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DealsUrlBuilder"/> class.
        /// </summary>
        /// <param name="baseUri">
        /// The base uri.
        /// </param>
        /// <param name="client">
        /// client name
        /// </param>
        public DealsUrlBuilder(Uri baseUri, string client)
        {
            this.baseUri = baseUri;
            this.clientQueryParameter = new KeyValuePair<string, string>(ClientKey, client);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get deals by business uri.
        /// </summary>
        /// <param name="businessIds">
        /// The business ids.
        /// </param>
        /// <param name="maxDealsPerBusiness">
        /// The max deals per business.
        /// </param>
        /// <param name="sort">
        /// The sort.
        /// </param>
        /// <param name="expiresAfter">
        /// The expires after.
        /// </param>
        /// <param name="activeBefore">
        /// The active before.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        internal Uri GetDealsByBusinessUri(IList<string> businessIds, int? maxDealsPerBusiness = null, DealsSort? sort = null, DateTime? expiresAfter = null, DateTime? activeBefore = null)
        {
            var queryString = new List<KeyValuePair<string, string>> { this.clientQueryParameter };
            if (businessIds != null && businessIds.Any())
            {
                queryString.Add(new KeyValuePair<string, string>(BusinessIdsKey, string.Join(",", businessIds)));
            }

            if (maxDealsPerBusiness.HasValue)
            {
                queryString.Add(new KeyValuePair<string, string>(MaxDealsPerBusinessKey, maxDealsPerBusiness.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (sort.HasValue)
            {
                queryString.Add(new KeyValuePair<string, string>(SortKey, sort.Value.ToString()));
            }

            if (expiresAfter.HasValue)
            {
                // datetime ISO 8601 format
                // http://msdn.microsoft.com/en-us/library/az4se3k1.aspx#Sortable
                queryString.Add(new KeyValuePair<string, string>(ExpiresAfterKey, expiresAfter.Value.ToString("s")));
            }

            if (activeBefore.HasValue)
            {
                // datetime ISO 8601 format
                // http://msdn.microsoft.com/en-us/library/az4se3k1.aspx#Sortable
                queryString.Add(new KeyValuePair<string, string>(ActiveBeforeKey, activeBefore.Value.ToString("s")));
            }

            return this.BuildUrl(GetDealsByBusinessMethod, queryString);
        }

        /// <summary>
        /// The get deals by id.
        /// </summary>
        /// <param name="dealIds">
        /// The deal ids.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="refinements">The refinements. <seealso cref="Refinements"/> </param>
        /// <param name="format">The format of the response: "simple", "all",...</param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        internal Uri GetDealsById(IList<Guid> dealIds, int? count = null, Refinements refinements = null, string format = "default")
        {
            var queryString = new List<KeyValuePair<string, string>> { this.clientQueryParameter };
            if (dealIds != null && dealIds.Any())
            {
                queryString.Add(new KeyValuePair<string, string>(DealIdsKey, string.Join(",", dealIds)));
            }

            if (count.HasValue)
            {
                queryString.Add(new KeyValuePair<string, string>(CountKey, count.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (refinements != null)
            {
                queryString.Add(new KeyValuePair<string, string>(RefinementsKey, RefinementsQueryStringConstructor.ConstructString(refinements)));
            }

            if (string.Compare(format,"default",StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                queryString.Add(new KeyValuePair<string, string>(FormatKey, format));
            }

            return this.BuildUrl(GetDealsByIdMethod, queryString);
        }

        /// <summary>
        /// The get near by deals uri.
        /// </summary>
        /// <param name="coordinates">
        /// The coordinates.
        /// </param>
        /// <param name="radius">
        /// The radius.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="refinements">
        /// The refinements.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        internal Uri GetNearByDealsUri(Coordinates coordinates, double? radius = null, int? count = null, Refinements refinements = null)
        {
            var queryString = new List<KeyValuePair<string, string>> { this.clientQueryParameter };
            if (coordinates != null)
            {
                queryString.Add(new KeyValuePair<string, string>(CoordKey, string.Format("{0},{1}", coordinates.Latitude, coordinates.Longitude)));
            }

            if (radius.HasValue)
            {
                queryString.Add(new KeyValuePair<string, string>(RadiusKey, radius.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (count.HasValue)
            {
                queryString.Add(new KeyValuePair<string, string>(CountKey, count.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (refinements != null)
            {
                queryString.Add(new KeyValuePair<string, string>(RefinementsKey, RefinementsQueryStringConstructor.ConstructString(refinements)));
            }

            return this.BuildUrl(GetNearbyDealsMethod, queryString);
        }

        /// <summary>
        /// The get deals by region uri.
        /// </summary>
        /// <param name="regionCode">
        /// The region code.
        /// </param>
        /// <param name="coordinates">
        /// The coordinates.
        /// </param>
        /// <param name="radius">
        /// The radius.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="refinements">
        /// The refinements.
        /// </param>
        /// <param name="anid">
        /// Anid of the user
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        internal Uri GetDealsByRegionUri(string regionCode, Coordinates coordinates, double? radius, int? count, Refinements refinements, string anid = null)
        {
            var queryString = new List<KeyValuePair<string, string>> { this.clientQueryParameter };
            if (coordinates != null)
            {
                queryString.Add(new KeyValuePair<string, string>(CoordKey, string.Format("{0},{1}", coordinates.Latitude, coordinates.Longitude)));
            }

            if (!string.IsNullOrEmpty(regionCode))
            {
                queryString.Add(new KeyValuePair<string, string>(RegionKey, regionCode));
            }

            if (!string.IsNullOrEmpty(anid))
            {
                queryString.Add(new KeyValuePair<string, string>(AnidKey, anid));
            }

            if (radius.HasValue)
            {
                queryString.Add(new KeyValuePair<string, string>(RadiusKey, radius.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (count.HasValue)
            {
                queryString.Add(new KeyValuePair<string, string>(CountKey, count.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (refinements != null)
            {
                queryString.Add(new KeyValuePair<string, string>(RefinementsKey, RefinementsQueryStringConstructor.ConstructString(refinements)));
            }

            return this.BuildUrl(GetDealsByRegionMethod, queryString);
        }

        /// <summary>
        /// The get online deals uri.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="refinements">
        /// The refinements.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        internal Uri GetOnlineDealsUri(int? count = null, Refinements refinements = null)
        {
            var queryString = new List<KeyValuePair<string, string>> { this.clientQueryParameter };
            if (count.HasValue)
            {
                queryString.Add(new KeyValuePair<string, string>(CountKey, count.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (refinements != null)
            {
                queryString.Add(new KeyValuePair<string, string>(RefinementsKey, RefinementsQueryStringConstructor.ConstructString(refinements)));
            }

            return this.BuildUrl(GetOnlineDealsMethod, queryString);
        }

        /// <summary>
        /// The build url.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="queryString">
        /// The query string.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        private Uri BuildUrl(string methodName, List<KeyValuePair<string, string>> queryString)
        {
            string relativeUri = methodName;
            if (queryString.Any())
            {
                IEnumerable<string> flatQueryString = queryString.Select(elem => string.Format("{0}={1}", elem.Key, elem.Value));
                relativeUri = string.Format("{0}?{1}", methodName, string.Join("&", flatQueryString));
            }

            var uri = new Uri(this.baseUri, relativeUri);
            return uri;
        }

        #endregion

        
    }
}