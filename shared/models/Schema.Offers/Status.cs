//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using System.Runtime.Serialization;

    [DataContract]
    public enum Status : short
    {
        // Only active and InActive states will be updated.
        [EnumMember]
        Active = 1,

        [EnumMember]
        Deleted = 2,

        [EnumMember]
        InActive = 3,

        [EnumMember]
        Invalid = 4,

        [EnumMember]
        Pending = 5
    }
}