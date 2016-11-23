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
    public enum ApplicationType
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        MsnWeb = 1,

        [EnumMember]
        WinLock = 2,

        [EnumMember]
        Serp = 3,

        [EnumMember]
        MsStore = 4,

        [EnumMember]
        Outlook = 5,

        [EnumMember]
        Win8 = 6,

        [EnumMember]
        Xbox = 7,

        [EnumMember]
        Email = 8,

        [EnumMember]
        LandingPage = 9,

        [EnumMember]
        Skype = 10,

        [EnumMember]
        SkypeMobile = 11,

        [EnumMember]
        SkypeSmall = 12,

        [EnumMember]
        OneDrive = 13,

        [EnumMember]
        Default = 14
    }
}