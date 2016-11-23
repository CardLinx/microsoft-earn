//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class Location
    {
        [DataMember(EmitDefaultValue = false, Name = "address")]
        [JsonProperty("address")]
        public string Address { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "city")]
        [JsonProperty("city")]
        public string City { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "state")]
        [JsonProperty("state")]
        public string State { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "zip")]
        [JsonProperty("zip")]
        public string Zip { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"{Address},{City},{State},{Zip}";
        }
    }
}