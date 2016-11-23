//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Scheduler.DataContracts
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    [DataContract]
    public class DealsContract : BaseContract
    {
        [JsonProperty(PropertyName = "location")]
        [DataMember(IsRequired = true, Name = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "deals")]
        [DataMember(IsRequired = true, Name = "deals")]
        public List<string> Deals { get; set; }
    }
}