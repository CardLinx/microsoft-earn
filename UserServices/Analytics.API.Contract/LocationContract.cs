//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The location model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Analytics.API.Contract
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The location contract.
    /// </summary>
    [DataContract(Name = "location")]
    public class LocationContract
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the address 1.
        /// </summary>
        [DataMember(Name = "address1")]
        public string Address1 { get; set; }

        /// <summary>
        ///     Gets or sets the address 2.
        /// </summary>
        [DataMember(Name = "address2")]
        public string Address2 { get; set; }

        /// <summary>
        ///     Gets or sets the city.
        /// </summary>
        [DataMember(Name = "city")]
        public string City { get; set; }

        /// <summary>
        ///     Gets or sets the country or region.
        /// </summary>
        [DataMember(Name = "country_or_region")]
        public string CountryOrRegion { get; set; }

        /// <summary>
        ///     Gets or sets the latitude.
        /// </summary>
        [DataMember(Name = "latitude")]
        public decimal Latitude { get; set; }

        /// <summary>
        ///     Gets or sets the longitude.
        /// </summary>
        [DataMember(Name = "longitude")]
        public decimal Longitude { get; set; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        [DataMember(Name = "state")]
        public string State { get; set; }

        /// <summary>
        ///     Gets or sets the postal.
        /// </summary>
        [DataMember(Name = "postal")]
        public string Postal { get; set; }

        #endregion
    }
}