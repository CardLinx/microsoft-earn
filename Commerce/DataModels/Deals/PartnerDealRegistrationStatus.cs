//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents partners deal registration status
    /// </summary>
    public enum PartnerDealRegistrationStatus
    {
        /// <summary>
        /// Indicates non persisted status.
        /// </summary>
        [XmlEnum("0")]
        None = 0,

        /// <summary>
        /// Indicates deal has not yet been registered with the partner.
        /// </summary>
        [XmlEnum("1")]
        Pending = 1,

        /// <summary>
        /// Deal registration with partner is complete.
        /// </summary>
        [XmlEnum("2")]
        Complete = 2,

        /// <summary>
        /// Deal Registration with partner encountered errors.
        /// </summary>
        [XmlEnum("3")]
        Error = 3
    }
}