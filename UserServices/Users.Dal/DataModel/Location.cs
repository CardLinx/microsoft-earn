//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The location type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal.DataModel
{
    using System;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The location.
    /// </summary>
    [DataContract]
    public class Location
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <param name="adminDistrict">
        /// The admin district.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="locationType">
        /// The location type.
        /// </param>
        public Location(string countryCode, string adminDistrict, string value, LocationType locationType)
        {
            this.CountryCode = countryCode != null ? countryCode.ToLowerInvariant().Trim() : string.Empty;
            this.AdminDistrict = adminDistrict != null ? adminDistrict.ToLowerInvariant().Trim() : string.Empty;
            this.Value = value != null ? value.ToLowerInvariant().Trim() : string.Empty;
            this.Type = locationType;
        }

        /// <summary>
        /// Gets the country.
        /// </summary>
        [JsonProperty(PropertyName = "country_code")]
        [DataMember(IsRequired = false, Name = "country_code")]
        public string CountryCode { get; private set; }

        /// <summary>
        /// Gets the admin district
        /// </summary>
        [JsonProperty(PropertyName = "admin_district")]
        [DataMember(IsRequired = false, Name = "admin_district")]
        public string AdminDistrict { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        [DataMember(IsRequired = false, Name = "value")]
        public string Value { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [JsonProperty(PropertyName = "type", ItemConverterType = typeof(StringEnumConverter))]
        [DataMember(IsRequired = false, Name = "type")]
        public LocationType Type { get; private set; }

        /// <summary> The parse location. </summary>
        /// <param name="input"> The location string. </param>
        /// <returns> The <see cref="Location"/>.The location </returns>
        /// <exception cref="FormatException"> can't get location from input </exception>
        public static Location Parse(string input)
        {
            string[] locationIdParts = input.Split(new[] { ':' }, 4);
            if (locationIdParts.Length != 3 && locationIdParts.Length != 4)
            {
                throw new FormatException(string.Format("Location Str wrong format. Location Str={0}", input));
            }

            int index = 0;
            string country = locationIdParts[index];
            index++;
            string adminDistrict = string.Empty;
            
            if (locationIdParts.Length == 4)
            {
                adminDistrict = locationIdParts[index];
                index++;
            }

            LocationType locationType;
            if (!Enum.TryParse(locationIdParts[index], true, out locationType) || (locationType == LocationType.None))
            {
                throw new FormatException(string.Format("Location Id wrong format. Problem with location type. Location Str={0}", input));
            }

            index++;
            string value = locationIdParts[index];

            return new Location(country, adminDistrict, value, locationType);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            string adminDistrictStr = !string.IsNullOrEmpty(this.AdminDistrict) ? ":" + this.AdminDistrict : string.Empty;

            return string.Format(
                "{0}{1}:{2}:{3}", 
                this.CountryCode == null ? string.Empty : this.CountryCode.ToLowerInvariant(), 
                adminDistrictStr,
                this.Type.ToString().ToLowerInvariant(), 
                this.Value ?? string.Empty);
        }

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
                int hashCode = (int)this.Type;
                hashCode = (hashCode * 397) ^ (this.Value != null ? this.Value.GetHashCode() : 0);
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
            return this.Type == other.Type && string.Equals(this.Value, other.Value) && string.Equals(this.CountryCode, other.CountryCode) && string.Equals(this.AdminDistrict, other.AdminDistrict);
        }
    }
}