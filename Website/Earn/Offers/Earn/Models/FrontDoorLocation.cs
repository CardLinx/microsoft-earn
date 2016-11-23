//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Earn.Offers.Earn.Models
{
    public class UserLocation
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the zip
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the dma
        /// </summary>
        public string Dma { get; set; }

        /// <summary>
        /// Gets or sets the Iso
        /// </summary>
        public string Iso { get; set; }

        /// <summary>
        /// The Location
        /// </summary>
        public string Location { get; set; }

        #endregion

    }
}