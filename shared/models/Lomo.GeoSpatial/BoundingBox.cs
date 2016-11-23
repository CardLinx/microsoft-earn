//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Encapsulates the Bounding Box info for a location
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.GeoSpatial
{
    using System.Xml.Linq;

    /// <summary>
    /// the bounding box
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        /// Gets or sets the south latitude
        /// </summary>
        public double SouthLatitude { get; set; }

        /// <summary>
        /// Gets or sets the north latitude
        /// </summary>
        public double NorthLatitude { get; set; }

        /// <summary>
        /// Gets or sets the east longitude
        /// </summary>
        public double EastLongitude { get; set; }

        /// <summary>
        /// Gets or sets the west longitude
        /// </summary>
        public double WestLongitude { get; set; }

        /// <summary>
        /// Converts the Boundingbox data to an xml element
        /// </summary>
        /// <returns>XElement of bounding box</returns>
        public XElement ToXml()
        {
            var boundingBoxElement = new XElement(
                                        "BoundingBox",
                                        new XAttribute("SouthLatitude", SouthLatitude.ToString()),
                                        new XAttribute("NorthLatitude", SouthLatitude.ToString()),
                                        new XAttribute("WestLongitude", SouthLatitude.ToString()),
                                        new XAttribute("EastLongitude", SouthLatitude.ToString()));

            return boundingBoxElement;
        }
    }
}