//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using ProtoBuf;

    [DataContract]
    [ProtoContract]
    public class SupportedApp
    {
        [DataMember(EmitDefaultValue = false, Name = "app")]
        [ProtoMember(1)]
        public ApplicationType App { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "dimensions")]
        [ProtoMember(2)]
        public ApplicationDimension Dimension{get;set;}

        [DataMember(EmitDefaultValue = false, Name = "template_id")]
        [ProtoMember(3)]
        public string TemplateId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "template_model")]
        [ProtoMember(4)]
        public Dictionary<string, string> TemplateModel { get; set; } 
    }
}