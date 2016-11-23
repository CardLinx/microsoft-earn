//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the type of events that can trigger a partner callback.
    /// </summary>
    public enum RedemptionEvent
    {
        /// <summary>
        /// Indicates that no callback has occurred.
        /// </summary>
        [XmlEnum("0")]
        None = 0,

        /// <summary>
        /// Indicates that the callback occurred at the time of a purchase.
        /// </summary>
        [XmlEnum("1")]
        RealTime = 1,

        /// <summary>
        /// Indicates that the callback occurred at the settlement of a purchase.
        /// </summary>
        [XmlEnum("2")]
        Settlement = 2
    }
}