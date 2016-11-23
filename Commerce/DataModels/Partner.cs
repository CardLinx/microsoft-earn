//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents partners with which the Commerce platform interacts.
    /// </summary>
    public enum Partner
    {
        /// <summary>
        /// Indicates no partner has been specified.
        /// </summary>
        [XmlEnum("0")]
        None = 0,

        /// <summary>
        /// Represents First Data.
        /// </summary>
        [XmlEnum("1")]
        FirstData = 1,

        /// <summary>
        /// Represents Amex.
        /// </summary>
        [XmlEnum("2")]
        Amex = 2,

        /// <summary>
        /// Represents Visa.
        /// </summary>
        [XmlEnum("3")]
        Visa = 3,

        /// <summary>
        /// Represents MasterCard.
        /// </summary>
        [XmlEnum("4")]
        MasterCard = 4
    }
}