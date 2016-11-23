//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The http context helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Payloads
{
    using Newtonsoft.Json;

    public class FdLocation
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        [JsonProperty(PropertyName = "latitude", NullValueHandling = NullValueHandling.Ignore)]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        [JsonProperty(PropertyName = "longitude", NullValueHandling = NullValueHandling.Ignore)]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the zip
        /// </summary>
        [JsonProperty(PropertyName = "postal", NullValueHandling = NullValueHandling.Ignore)]
        public string Postal { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        [JsonProperty(PropertyName = "city", NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        [JsonProperty(PropertyName = "country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the dma
        /// </summary>
        [JsonProperty(PropertyName = "dma", NullValueHandling = NullValueHandling.Ignore)]
        public string Dma { get; set; }

        /// <summary>
        /// Gets or sets the dma
        /// </summary>
        [JsonProperty(PropertyName = "iso", NullValueHandling = NullValueHandling.Ignore)]
        public string Iso { get; set; }

        #endregion
    }
}