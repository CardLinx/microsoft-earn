//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    [JsonConverter(typeof(StringEnumConverter))]
    [DataContract]
    public enum ApplicationDimension
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        All = 10,

        [EnumMember]
        D160X600 = 20,

        [EnumMember]
        D234X60 = 30,
        
        [EnumMember]
        D300X50 = 40,

        [EnumMember]
        D300X250 = 50,

        [EnumMember]
        D300X600 = 60,

        [EnumMember]
        D320X50 = 70
    }
}