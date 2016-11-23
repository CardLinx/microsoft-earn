//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the vectors through which referrals can be received.
    /// </summary>
    public enum ReferralVector
    {
        /// <summary>
        /// Indicates the referral was received from an unknown vector.
        /// </summary>
        [XmlEnum("0")]
        Unknown = 0,

        /// <summary>
        /// Indicates the referral was received from Facebook.
        /// </summary>
        [XmlEnum("1")]
        Facebook = 1,

        /// <summary>
        /// Indicates the referral was received from Twitter.
        /// </summary>
        [XmlEnum("2")]
        Twitter = 2
    }
}