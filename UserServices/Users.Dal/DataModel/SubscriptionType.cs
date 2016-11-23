//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Subscriptiontype enumeration
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal.DataModel
{
    /// <summary>
    /// Identifies the type of Bing Offers Email subscription
    /// </summary>
    public enum SubscriptionType
    {
        /// <summary>
        /// Weekly deals Email
        /// </summary>
        WeeklyDeals,

        /// <summary>
        /// Bing Offers Promotional Email
        /// </summary>
        Promotional,

        /// <summary>
        /// Transaction report for Bing Offers Merchant users
        /// </summary>
        TransactionReport
    }
}