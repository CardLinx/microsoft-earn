//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using System.Runtime.Serialization;
    using ProtoBuf;

    [DataContract]
    public class Catalog
    {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [ProtoMember(1)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "blob")]
        [ProtoMember(2)]
        public string Blob { get; set; }
    }
}