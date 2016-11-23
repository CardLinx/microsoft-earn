//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace OfferManagement.DataModel
{
    //[JsonConverter(typeof(StringEnumConverter))]
    [DataContract]
    public enum PaymentProcessor
    {
        [EnumMember]
        Amex = 2,

        [EnumMember]
        Visa = 3,

        [EnumMember]
        MasterCard = 4
    }
}