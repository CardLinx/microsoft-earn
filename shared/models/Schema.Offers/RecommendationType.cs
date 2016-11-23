//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using ProtoBuf;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class RecommendationType
    {
        [DataMember(EmitDefaultValue = false, Name = "target_devices")]
        [ProtoMember(1)]
        public List<RecommendationDevice> TargetDevices { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "content_type")]
        [ProtoMember(2)]
        public RecommendationContentType ContentType { get; set; }
    }
}