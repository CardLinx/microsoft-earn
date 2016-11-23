//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Scheduler.DataContracts
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class BaseContract
    {
        [JsonProperty(PropertyName = "subject")]
        [DataMember(IsRequired = true, Name = "subject")]
        public string Subject { get; set; }
    }
}