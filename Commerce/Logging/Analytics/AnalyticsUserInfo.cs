//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    using AnalyticsClient;

    /// <summary>
    /// Obtains information about a user for analytics.
    /// </summary>
    public class AnalyticsUserInfo : IAnalyticsUserInfo
    {
        /// <summary>
        /// Obtains the ANID for the specified PUID.
        /// </summary>
        /// <param name="puid">
        /// The PUID whose ANID to acquire.
        /// </param>
        /// <returns>
        /// The ANID for the specified PUID.
        /// </returns>
        public string GetAnidFromPuid(string puid)
        {
            return AnalyticsUserId.GetAnidUserIdFromMsId(puid);
        }
    }
}