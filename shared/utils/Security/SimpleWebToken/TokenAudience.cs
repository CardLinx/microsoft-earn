//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    /// <summary>
    /// Enumerates the audience entities for whom
    /// the SWT tokens are issued.
    /// </summary>
    public enum TokenAudience
    {
        /// <summary>
        /// The token audience is not known.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The token audience is the Rewards Service
        /// </summary>
        RewardsService = 1,

        /// <summary>
        /// The token audience is the Earn Service
        /// </summary>
        EarnService = 2,
    }
}