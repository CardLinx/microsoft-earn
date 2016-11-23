//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a Get Deal Reports API invocation.
    /// </summary>
    [DataContract]
    public class GetDealReportsResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the DealReports object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_reports")]
        public IEnumerable<DealReportDataContract> DealReports { get; set; }
    }
}