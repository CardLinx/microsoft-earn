//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Users.Dal.DataModel
{
    using Newtonsoft.Json;
    using System;
    using System.Text;

    /// <summary>
    /// Wraps the model that represents the user's location tracked for analytics purposes
    /// </summary>
    public class UserLocation
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public UserLocation()
        {
        }

        /// <summary>
        /// Creates an instance of UserLocation object.
        /// </summary>
        /// <param name="latitude">The latitude of the user location.</param>
        /// <param name="longitude">The longitude of the user location</param>
        public UserLocation(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Gets or sets the latitude of the user location. this field is mandatory.
        /// </summary>
        [JsonProperty(PropertyName = "latitude", Required = Required.Always)]
        public double Latitude
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the longitude of the user location. this field is mandatory.
        /// </summary>
        [JsonProperty(PropertyName = "longitude", Required = Required.Always)]
        public double Longitude
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the iso of the user location.
        /// </summary>
        [JsonProperty(PropertyName = "iso")]
        public string Iso
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zipcode of the user location.
        /// </summary>
        [JsonProperty(PropertyName = "zip")]
        public string Zipcode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dma of the user's location.
        /// </summary>
        [JsonProperty(PropertyName = "dma")]
        public string Dma
        {
            get;
            set;
        }

        /// <summary>
        /// Serializes the object to json string and then base 64 encodes it for safe transmission over the wire.
        /// </summary>
        /// <returns></returns>
        public string SerializeModel()
        {
            string jsonData = JsonConvert.SerializeObject(this);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonData));
        }

        /// <summary>
        /// Deserializes a base64 encoded json string received over the wire to UserLocation object.
        /// </summary>
        /// <param name="data">The base64 encoded json string received over the wire</param>
        /// <returns></returns>
        public static UserLocation DeSerializeModel(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentOutOfRangeException("data");
            }

            byte[] bytes = Convert.FromBase64String(data);
            string jsonData = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<UserLocation>(jsonData);
        }
    }
}