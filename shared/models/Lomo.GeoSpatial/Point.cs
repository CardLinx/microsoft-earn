//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Point on Earth
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.GeoSpatial
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Point on Earth
    /// </summary>
    [DataContract]
    public class Point
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point" /> class.
        /// </summary>
        /// <param name="lat">the latitude</param>
        /// <param name="lon">the longitude</param>
        public Point(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Point" /> class.
        /// </summary>
        public Point()
        {
        }

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        [DataMember(EmitDefaultValue = true, Name = "lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        [DataMember(EmitDefaultValue = true, Name = "lon")]
        public double Longitude { get; set; }
    }
}