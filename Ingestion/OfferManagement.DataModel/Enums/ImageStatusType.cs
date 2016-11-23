//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OfferManagement.DataModel
{
    //[JsonConverter(typeof(StringEnumConverter))]
    [DataContract]
    public enum ImageStatusType
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// The good image.
        /// </summary>
        [EnumMember]
        GoodImage = 1,

        /// <summary>
        /// The default image being used.
        /// </summary>
        [EnumMember]
        DefaultImageBeingUsed = 2,

        /// <summary>
        /// The invalid image.
        /// </summary>
        [EnumMember]
        InvalidImage = 3
    }
}