//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class Image
    {
        [DataMember(EmitDefaultValue = false, Name = "url")]
        [JsonProperty("url")]
        public string Url { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "width")]
        [JsonProperty("width")]
        public ushort Width { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "height")]
        [JsonProperty("height")]
        public ushort Height { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "status")]
        [JsonProperty("status")]
        public ImageStatusType Status { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "rank")]
        [JsonProperty("rank")]
        public float Rank { get; set; }
    }
}