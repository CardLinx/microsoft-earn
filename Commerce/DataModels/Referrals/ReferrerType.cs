//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the types of entities which can make referrals.
    /// </summary>
    public enum ReferrerType
    {
        /// <summary>
        /// Indicates the referrer is a User.
        /// </summary>
        [XmlEnum("0")]
        User = 0,

        /// <summary>
        /// Indicates the referrer was a Merchant.
        /// </summary>
        [XmlEnum("1")]
        Merchant = 1
    }
}