//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The refinements query string constructor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DealsServerClient
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The refinements query string constructor.
    /// </summary>
    internal static class RefinementsQueryStringConstructor
    {
        #region Consts

        /// <summary>
        /// The expires after string.
        /// </summary>
        private const string ExpiresAfterString = "EXPIRESAFTER";

        /// <summary>
        /// The active before string.
        /// </summary>
        private const string ActiveBeforeString = "ACTIVEBEFORE";

        /// <summary>
        /// The results per biz string.
        /// </summary>
        private const string ResultsPerBizString = "RESULTSPERBIZ";

        /// <summary>
        /// The type string.
        /// </summary>
        private const string TypeString = "TYPE";

        /// <summary>
        /// The off set string.
        /// </summary>
        private const string OffSetString = "OFFSET";

        /// <summary>
        /// The timeout string.
        /// </summary>
        private const string TimeoutString = "TIMEOUT";

        /// <summary>
        /// The source string.
        /// </summary>
        private const string SourceString = "SOURCE";

        /// <summary>
        /// The categories string.
        /// </summary>
        private const string CategoriesString = "CATEGORIES";

        /// <summary>
        /// The query string.
        /// </summary>
        private const string QueryString = "QUERY";

        /// <summary>
        /// The sort string.
        /// </summary>
        private const string SortString = "SORT";

        /// <summary>
        /// The hot offer string.
        /// </summary>
        private const string HotOfferString = "HOTOFFER";

        /// <summary>
        /// The market string.
        /// </summary>
        private const string MarketString = "MARKET";

        /// <summary>
        /// Identifies the flight for this request to deals server.
        /// </summary>
        private const string FlightsString = "FLIGHTS";

        #endregion consts
        
        /// <summary>
        /// The construct refinements string.
        /// </summary>
        /// <param name="refinements">
        /// The refinements.
        /// </param>
        /// <returns>
        /// The refinements string
        /// </returns>
        internal static string ConstructString(Refinements refinements)
        {
            const string KeyvalueSeperator = ":";
            const string RefinmentsSeperator = ";";
            var propertyBag = BuildRefinementsPropertiesBag(refinements);
            var refinmentsList = propertyBag.Select(elem => string.Format("{0}{1}{2}", elem.Key, KeyvalueSeperator, elem.Value));
            var refinementsString = string.Join(RefinmentsSeperator, refinmentsList);
            return refinementsString;
        }

        /// <summary>
        /// Builds the  refinements properties bag.
        /// </summary>
        /// <param name="refinements">
        /// The refinements.
        /// </param>
        /// <returns>
        /// Mapping between refinement key and value
        /// </returns>
        private static Dictionary<string, string> BuildRefinementsPropertiesBag(Refinements refinements)
        {
            const string ListSplitChar = ",";

            var propertyBag = new Dictionary<string, string>();
            if (refinements.ExpiresAfter.HasValue)
            {
                // datetime ISO 8601 format
                // http://msdn.microsoft.com/en-us/library/az4se3k1.aspx#Sortable
                propertyBag.Add(ExpiresAfterString, refinements.ExpiresAfter.Value.ToString("s"));
            }

            if (refinements.ActiveBefore.HasValue)
            {
                // datetime ISO 8601 format
                // http://msdn.microsoft.com/en-us/library/az4se3k1.aspx#Sortable
                propertyBag.Add(ActiveBeforeString, refinements.ActiveBefore.Value.ToString("s"));
            }

            if (refinements.ResultsPerBusiness.HasValue)
            {
                propertyBag.Add(ResultsPerBizString, refinements.ResultsPerBusiness.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (refinements.HotOffer)
            {
                propertyBag.Add(TypeString, HotOfferString);
            }

            if (refinements.Offset.HasValue)
            {
                propertyBag.Add(OffSetString, refinements.Offset.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (refinements.Timeout.HasValue)
            {
                propertyBag.Add(TimeoutString, refinements.Timeout.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(refinements.Market))
            {
                propertyBag.Add(MarketString, refinements.Market);
            }

            if (refinements.Sources != null && refinements.Sources.Any())
            {
                propertyBag.Add(SourceString, string.Join(ListSplitChar, refinements.Sources));
            }

            if (refinements.Categories != null && refinements.Categories.Any())
            {
                propertyBag.Add(CategoriesString, string.Join(ListSplitChar, refinements.Categories));
            }

            if (refinements.QueryKeywords != null && refinements.QueryKeywords.Any())
            {
                propertyBag.Add(QueryString, string.Join(ListSplitChar, refinements.QueryKeywords));
            }

            if (!string.IsNullOrEmpty(refinements.Flights))
            {
                propertyBag.Add(FlightsString,refinements.Flights);
            }
            
            propertyBag.Add(SortString, refinements.Sort.ToString());

            return propertyBag;
        }
    }
}