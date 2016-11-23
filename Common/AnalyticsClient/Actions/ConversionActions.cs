//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The conversions actions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Actions
{
    /// <summary>
    /// The conversions actions.
    /// </summary>
    public class ConversionActions
    {
        #region Constants

        /// <summary>
        /// The conversion actions prefix.
        /// </summary>
        public const string Prefix = "conversion.";

        /// <summary>
        /// The canceled.
        /// </summary>
        public const string Canceled = Prefix + "canceled";

        /// <summary>
        /// The declined.
        /// </summary>
        public const string Declined = Prefix + "declined";

        /// <summary>
        /// The paid.
        /// </summary>
        public const string Paid = Prefix + "paid";

        /// <summary>
        /// The paid revenue.
        /// </summary>
        public const string PaidRevenue = Prefix + "revenue.paid";

        /// <summary>
        /// The bounty revenue.
        /// </summary>
        public const string BountyRevenue = Prefix + "revenue.bounty";

        /// <summary>
        /// The bounty.
        /// </summary>
        public const string Bounty = Prefix + "bounty";


        /// <summary>
        /// The refunded.
        /// </summary>
        public const string Refunded = Prefix + "refunded";

        /// <summary>
        /// The unknown.
        /// </summary>
        public const string Unknown = Prefix + "unknown";

        #endregion
    }
}