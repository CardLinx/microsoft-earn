//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the types of discounts that can appear within a deal.
    /// </summary>
    public enum DiscountType
    {
        /// <summary>
        /// Indicates the discount is for a credit card statement credit for a static amount.
        /// </summary>
        [XmlEnum("0")]
        StaticStatementCredit = 0,

        /// <summary>
        /// Indicates the discount is for a credit card statement credit for a percentage of the settlement amount.
        /// </summary>
        [XmlEnum("1")]
        PercentageStatementCredit = 1
    }
}