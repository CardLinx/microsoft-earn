//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Encapsulates the geographical info for a location
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.GeoSpatial
{
    /// <summary>
    /// Encapsulates the geographical info for a location
    /// </summary>
    public class GeocodePoint
    {
        /// <summary>
        /// Gets or sets the Name of the location
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or sets the Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the Radius
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the Location Info
        /// </summary>
        public GeoLocation Location { get; set; }

        /// <summary>
        /// Gets or sets the Bounding box of the location
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        public Point toPoint()
        {
            return new Point(this.Latitude, this.Longitude);
        }
    }
}