//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Earn.DataContract.Commerce;

namespace Earn.DataContract.LomoUsers
{
    [DataContract]
    public class Customer
    {
        [DataMember(EmitDefaultValue = false, Name = "global_id")]
        public Guid GlobalId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id")]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "id_hex")]
        public string IdHex { get { return Id.ToString("X"); } }

        [DataMember(EmitDefaultValue = false, Name = "name")]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "email")]
        public string Email { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "msid")]
        public string MSID { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "phone_number")]
        public string PhoneNumber { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "state")]
        public string State { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "city")]
        public string City { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "zip_code")]
        public string ZipCode { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "cards")]
        public List<CardInfo> Cards { get; set; }
    }
}