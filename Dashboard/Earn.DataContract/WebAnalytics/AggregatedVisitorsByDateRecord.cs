//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Serialization;

namespace Earn.DataContract.WebAnalytics
{
    [DataContract]
    public class AggregatedVisitorsByDateRecord
    {
        [DataMember(EmitDefaultValue = false, Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "total_visitors")]
        public int TotalVisitors { get; set; }
    }
}