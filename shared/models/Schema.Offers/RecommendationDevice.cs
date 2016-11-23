//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using System.Runtime.Serialization;

    [DataContract]
    public enum RecommendationDevice : short
    {
        [EnumMember]
        NoDevice = 0,

        [EnumMember]
        XboxOne = 1,

        [EnumMember]
        Xbox360 = 2,

        [EnumMember]
        Xbox = 3,

        [EnumMember]
        WindowsPhone = 4,

        [EnumMember]
        Windows = 5,

        [EnumMember]
        AllDevices = 6
    }
}