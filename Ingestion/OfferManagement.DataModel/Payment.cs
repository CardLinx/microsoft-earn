//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class Payment
    {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "processor")]
        [JsonProperty("processor")]
        public PaymentProcessor Processor { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "payments_mids")]
        [JsonProperty("payment_mids")]
        public Dictionary<string, string> PaymentMids { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "last_update")]
        [JsonProperty("last_update")]
        public DateTime LastUpdate { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "synced_with_commerce")]
        [JsonProperty("synced_with_commerce")]
        public bool SyncedWithCommerce { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "is_active")]
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }
    }
}