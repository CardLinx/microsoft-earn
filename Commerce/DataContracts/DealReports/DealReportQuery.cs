//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a query for a deal report.
    /// </summary>
    [DataContract]
    public class DealReportQuery
    {
        /// <summary>
        /// Gets or sets the ID of the deal specified within this query.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_id")]
        public Guid DealId { get; set; }

        /// <summary>
        /// Gets or sets the IDs of discounts for which to return information as part of this query, if any.
        /// </summary>
        /// <remarks>
        /// * When this parameter is not populated, either no discount information will be returned or information on all
        ///   discounts will be returned, depending on parameters in the parent query object.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "discount_ids")]
        public IEnumerable<Guid> DiscountIds { get; set; }
    }
}