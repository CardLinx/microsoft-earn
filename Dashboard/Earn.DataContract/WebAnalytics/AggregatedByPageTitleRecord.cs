//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace Earn.DataContract.WebAnalytics
{
    [DataContract]
    public class AggregatedByPageTitleRecord
    {
        [DataMember(EmitDefaultValue = false, Name = "page_title")]
        public string PageTitle { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "count")]
        public int Count { get; set; }
    }
}