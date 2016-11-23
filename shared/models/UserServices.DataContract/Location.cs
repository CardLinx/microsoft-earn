//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The email subscription.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.UserServices.DataContract
{
    using Newtonsoft.Json;

    /// <summary>
    /// The email subscription.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Gets or sets the location id.
        /// </summary>
        [JsonProperty(PropertyName = "location_name")]
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or sets the location type.
        /// </summary>
        [JsonProperty(PropertyName = "location_type")]
        public LocationType LocationType { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [JsonProperty(PropertyName = "admin_district")]
        public string AdminDistrict { get; set; }
        
        /// <summary> The equals. </summary>
        /// <param name="other"> The other. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Location)other);
        }

        /// <summary> The get hash code. </summary>
        /// <returns> The <see cref="int"/>. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)this.LocationType;
                hashCode = (hashCode * 397) ^ (this.LocationName != null ? this.LocationName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.CountryCode != null ? this.CountryCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.AdminDistrict != null ? this.AdminDistrict.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary> The equals. </summary>
        /// <param name="other"> The other. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        protected bool Equals(Location other)
        {
            return this.LocationType == other.LocationType && string.Equals(this.LocationName, other.LocationName) && string.Equals(this.CountryCode, other.CountryCode) && string.Equals(this.AdminDistrict, other.AdminDistrict);
        }
    }
}