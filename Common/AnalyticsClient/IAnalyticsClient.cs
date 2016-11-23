//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // AnalyticsClient interface to be implemented
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient
{
    public interface IAnalyticsClient
    {
        /// <summary>
        /// Initializes the Analytics Client
        /// </summary>
        /// <param name="queueNameSuffix">
        /// The queue Name Suffix.
        /// </param>
        void Initialize(string queueNameSuffix = null);

        /// <summary>
        /// Adds a new analytics item 
        /// </summary>
        /// <param name="analyticsItem">
        /// The analytics item to add
        /// </param>
        void Add(AnalyticsItem analyticsItem);
    }
}