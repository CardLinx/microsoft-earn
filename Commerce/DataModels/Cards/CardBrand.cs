//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the card brands supported by the Commerce service.
    /// </summary>
    public enum CardBrand
    {
        /// <summary>
        /// Indicates the card is of an unknown brand.
        /// </summary>
        [XmlEnum("0")]
        Unknown = 0,

        /// <summary>
        /// Indicates the card is an American Express card.
        /// </summary>
        [XmlEnum("3")]
        AmericanExpress = 3,

        /// <summary>
        /// Indicates the card is a Visa card.
        /// </summary>
        [XmlEnum("4")]
        Visa = 4,

        /// <summary>
        /// Indicates the card is a MasterCard.
        /// </summary>
        [XmlEnum("5")]
        MasterCard = 5
    }
}