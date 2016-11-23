//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace OfferManagement.DataModel.Enums
{
    [DataContract]
    public enum  MerchantSource
    {
        [EnumMember]
        MasterCard,

        [EnumMember]
        Visa,

        [EnumMember]
        RewardNetworks
    }
}