//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The lo mo deals service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Lomo.DataModels.Offers.Query
{
    /// <summary>
    /// Encapsulates the lat/lon of a user's location.
    /// </summary>
    public class UserLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserLocation"/> class. 
        /// Creates a new instance of userlocation, at the given coordinate.
        /// </summary>
        /// <param name="lat">
        /// the Latitude.
        /// </param>
        /// <param name="lon">
        /// the Longitude
        /// </param>
        public UserLocation(double lat, double lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Returns a string representation of the location.
        /// </summary>
        /// <returns>A string representation of the location.</returns>
        public override string ToString()
        {
            return this.Latitude + "," + this.Longitude;
        }
    }
}