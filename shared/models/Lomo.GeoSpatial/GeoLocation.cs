//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Encapsulates the Location Info
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.GeoSpatial
{
    public class GeoLocation
    {
        /// <summary>
        /// Gets or sets the admin district.
        /// </summary>
        /// <value>
        /// The admin district.
        /// </value>
        public string AdminDistrict { get; set; }

        /// <summary>
        /// Gets or sets the Country/Region.
        /// </summary>
        /// <value>
        /// Country/Region
        /// </value>
        public string CountryRegion { get; set; }

        /// <summary>
        /// Gets or sets the Locality.
        /// </summary>
        /// <value>
        /// Locality
        /// </value>
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the PostalCode.
        /// </summary>
        /// <value>
        /// Postal Code
        /// </value>
        public string PostalCode { get; set; }
    }
}