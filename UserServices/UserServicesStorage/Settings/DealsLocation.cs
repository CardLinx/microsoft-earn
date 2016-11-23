//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The location.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.Storage.Settings
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The location.
    /// </summary>
    [XmlRoot("DealsLocation")]
    public class DealsLocation
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the deal location id.
        /// </summary>
        [XmlElement("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        [XmlElement("Conditions")]
        public DealsSelectionConditions Conditions { get; set; }
        
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [XmlElement("Latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [XmlElement("Longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        [XmlElement("Radius")]
        public int Radius { get; set; }

        #endregion
    }
}