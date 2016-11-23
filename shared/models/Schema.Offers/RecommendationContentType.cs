//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using System.Runtime.Serialization;

    [DataContract]
    public enum RecommendationContentType : short
    {
        [EnumMember]
        NoContentType = 0,

        [EnumMember]
        Games = 1,

        [EnumMember]
        AddOnPacks = 2,

        [EnumMember]
        Movie = 3,

        [EnumMember]
        TvSeries = 4,

        [EnumMember]
        AlbumAudio = 5,

        [EnumMember]
        SongAudio = 6,

        [EnumMember]
        SongVideo = 7,

        [EnumMember]
        MusicArtist = 8,

        [EnumMember]
        WindowsPhoneAppGame = 9,

        [EnumMember]
        WindowsAppGame = 10,

        [EnumMember]
        OnlineStoreProducts = 11
    }
}