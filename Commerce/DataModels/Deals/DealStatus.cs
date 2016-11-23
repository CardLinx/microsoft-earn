//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents Deal Status
    /// </summary>
    public enum DealStatus
    {
        /// <summary>
        /// Indicates non persisted status.
        /// </summary>
        [XmlEnum("0")]
        None = 0,

        /// <summary>
        /// Indicates deal has been persisted, but registration with partners is not yet complete
        /// </summary>
        [XmlEnum("1")]
        PendingRegistration = 1,

        /// <summary>
        /// Deal is registered with partners, but autolinking job not yet done
        /// </summary>
        [XmlEnum("2")]
        PendingAutoLinking = 2,

        /// <summary>
        /// Deal has been auto linked with all user/card combinations
        /// </summary>
        [XmlEnum("3")]
        AutoLinkingComplete = 3,

        /// <summary>
        /// Deal server has been notified that the deal is active
        /// </summary>
        [XmlEnum("4")]
        Activated = 4
    }
}