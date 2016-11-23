//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace OfferManagement.DataModel
{
    [DataContract]
    public enum MerchantFileType
    {
        [EnumMember]
        MasterCardAuth = 0,

        [EnumMember]
        MasterCardClearing = 1,

        [EnumMember]
        Visa = 2,

        [EnumMember]
        Amex = 3,

        [EnumMember]
        MasterCardProvisioning = 4,
    }
}