//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Serialization;

namespace Earn.DataContract.WebAnalytics
{
    [DataContract]
    public class AggregatedNewUsersByDateRecord
    {
        [DataMember(EmitDefaultValue = false, Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "new_users")]
        public int NewUsers { get; set; }
    }
}