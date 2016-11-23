//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OfferManagement.DataModel
{
    public class FeedInformation
    {
        [DataMember(EmitDefaultValue = false, Name = "url")]
        [JsonProperty("url")]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "user_name")]
        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "password")]
        [JsonProperty("extended_attributes")]
        public string Password { get; set; }
    }
}