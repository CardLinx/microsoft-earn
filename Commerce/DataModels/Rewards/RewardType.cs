//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    /// <summary>
    /// Represents the different kinds of rewards that can be given.
    /// </summary>
    public enum RewardType
    {
        /// <summary>
        /// Indicates the reward being given is of an undefined (i.e. manual) type.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Indicates the reward being given is Bing Reward points.
        /// </summary>
        BingRewardPoints = 1,

        /// <summary>
        /// Indicates the reward being given is a statement credit.
        /// </summary>
        StatementCredit = 2,

        /// <summary>
        /// Indicates the reward being given is Earn / Burn rewards program credit.
        /// </summary>
        EarnCredit = 3
    }
}