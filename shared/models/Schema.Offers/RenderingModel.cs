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
    public class RenderingModel
    {
        [DataMember(EmitDefaultValue = false, Name = "offer")]
        [ProtoMember(1)]
        public Offer Offer { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "impression_url")]
        [ProtoMember(2)]
        public string ImpressionUrl{get;set;}

        [DataMember(EmitDefaultValue = false, Name = "transaction_url")]
        [ProtoMember(3)]
        public string TransactionUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "dismiss_url")]
        [ProtoMember(4)]
        public string DismissUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "template_model")]
        [ProtoMember(5)]
        public Dictionary<string, string> TemplateModel { get; set; }
    }
}