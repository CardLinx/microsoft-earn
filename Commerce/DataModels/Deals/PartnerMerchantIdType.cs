//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents different types of partner merchant IDs.
    /// </summary>
    public enum PartnerMerchantIdType
    {
        /// <summary>
        /// Indicates the partner merchant ID is applicable for all uses.
        /// </summary>
        [XmlEnum("0")]
        Universal = 0,

        /// <summary>
        /// Indicates the partner merchant ID is used only during authorization events.
        /// </summary>
        [XmlEnum("1")]
        AuthorizationOnly = 1,

        /// <summary>
        /// Indicates the partner merchant ID is used only during settlement events.
        /// </summary>
        [XmlEnum("2")]
        SettlementOnly = 2
    }
}