//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents the reward program or the client calling the commerce service
    /// </summary>
    [Flags]
    public enum RewardPrograms
    {
        None = 0,

        /// <summary>
        /// Indicates that the client is CLO/Bing offers program
        /// </summary>
        CardLinkOffers = 1,

        /// <summary>
        /// Indicates that the client is Microsoft Earn/Burn program
        /// </summary>
        EarnBurn = 2,

        /// <summary>
        /// Indicates that all award programs are being addressed.
        /// </summary>
        All = 0x7FFFFFFF
    }
}