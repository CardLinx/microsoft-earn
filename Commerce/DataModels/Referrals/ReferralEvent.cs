//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the types of referrals which can be made.
    /// </summary>
    public enum ReferralEvent
    {
        /// <summary>
        /// Indicates the referral was received when a link was clicked.
        /// </summary>
        [XmlEnum("0")]
        LinkClicked = 0,

        /// <summary>
        /// Indicates the referral was received when a user was signed up.
        /// </summary>
        [XmlEnum("1")]
        Signup = 1,

        /// <summary>
        /// Indicates the referral was received when a user redeemed.
        /// </summary>
        [XmlEnum("2")]
        Redemption = 2
    }
}