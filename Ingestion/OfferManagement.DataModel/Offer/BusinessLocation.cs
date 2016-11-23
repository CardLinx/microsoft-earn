//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class BusinessLocation
    {
        /// <summary>
        ///     Gets or sets Street.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "address1")]
        public string Address1 { get; set; }

        /// <summary>
        ///     Gets or sets Street.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "address2")]
        public string Address2 { get; set; }

        /// <summary>
        ///   Gets or sets A branch office could have its own business id
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "business_id")]
        public string BusinessId { get; set; }

        /// <summary>
        ///   Gets or sets A branch office could have its own business id
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "cuisine")]
        public string Cuisine { get; set; }

        /// <summary>
        ///     Gets or sets City.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "city")]
        public string City { get; set; }

        /// <summary>
        ///     Gets or sets CountryOrRegion.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "country_region")]
        public string CountryOrRegion { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "full_address")]
        public string FullAddress { get; set; }

        /// <summary>
        ///     Gets or sets Latitude.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "latitude")]
        public double Latitude { get; set; }

        /// <summary>
        ///     Gets or sets Longitude.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "longitude")]
        public double Longitude { get; set; }

        /// <summary>
        ///   Gets or sets A branch office of a business could have its own name.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "name")]
        public string Name { get; set; }

        /// <summary>
        ///   Gets or sets the neighborhood
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "neighborhood")]
        public string Neighborhood { get; set; }

        /// <summary>
        ///     Gets or sets the Phone Number.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Gets or sets State.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "state")]
        public string State { get; set; }

        /// <summary>
        ///     Gets or sets Zip.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "zip")]
        public string Zip { get; set; }

        /// <summary>
        ///     Gets or sets ReservationURL.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "reservation_url")]
        public string ReservationUrl { get; set; }

        /// <summary>
        /// Creates shallow copy
        /// </summary>
        /// <returns>The location.</returns>
        public BusinessLocation ShallowCopy()
        {
            return (BusinessLocation)this.MemberwiseClone();
        }

    }
}