//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   Analytics actions for deal server
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Actions
{
    /// <summary>
    /// Analytics actions for deal server
    /// </summary>
    public static partial class Actions
    {
        /// <summary>
        /// The deal click.
        /// </summary>
        public const string DealClick = "click";

        /// <summary>
        /// The start deal link.
        /// </summary>
        public const string StartDealLink = "claimdeal.start";

        /// <summary>
        /// The deals query prefix.
        /// </summary>
        public const string DealsQueryPrefix = "api.";

        /// <summary>
        /// The deals business query.
        /// </summary>
        public const string DealsByBusiness = DealsQueryPrefix + "getdeals.bybusiness";

        /// <summary>
        /// The deals by id query.
        /// </summary>
        public const string DealsById = DealsQueryPrefix + "getdeals.byid";

        /// <summary>
        /// The deals by seoUrl query.
        /// </summary>
        public const string DealsBySeoUrl = DealsQueryPrefix + "getdeals.byseourl";

        /// <summary>
        /// The related deals query.
        /// </summary>
        public const string DealsRelated = DealsQueryPrefix + "getdeals.related";

        /// <summary>
        /// The near by deals query.
        /// </summary>
        public const string DealsByLocation = DealsQueryPrefix + "getdeals.nearby";

        /// <summary>
        /// The near by deals with fall-back to online query.
        /// </summary>
        public const string DealsByLocationOnline = DealsQueryPrefix + "getdeals.nearby.online";

        /// <summary>
        /// The deals by region query.
        /// </summary>
        public const string DealsByRegion = DealsQueryPrefix + "getdeals.byregion";

        /// <summary>
        /// The deals by region with fall-back to online query.
        /// </summary>
        public const string DealsByRegionOnline = DealsQueryPrefix + "getdeals.byregion.online";

        /// <summary>
        /// The online deals query.
        /// </summary>
        public const string DealsOnline = DealsQueryPrefix + "getdeals.online";
    }
}