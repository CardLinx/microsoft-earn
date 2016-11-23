//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the entities that can receive a reward from a referral.
    /// </summary>
    public enum RewardRecipient
    {
        /// <summary>
        /// Indicates the recipient of the reward will be the referrer.
        /// </summary>
        [XmlEnum("0")]
        Referrer = 0
    }
}