//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    /// <summary>
    /// The interface for classes that obtain information about a user for analytics.
    /// </summary>
    public interface IAnalyticsUserInfo
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
        string GetAnidFromPuid(string puid);
    }
}