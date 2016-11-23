//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace Earn.DataContract.WebAnalytics
{
    [DataContract]
    public class AggregatedByDeviceTypeRecord
    {
        [DataMember(EmitDefaultValue = false, Name = "device_type")]
        public string DeviceType { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "count")]
        public int Count { get; set; }
    }
}