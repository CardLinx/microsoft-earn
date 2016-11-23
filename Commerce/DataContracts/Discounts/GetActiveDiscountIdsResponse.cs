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
    /// Response contract for active discount ids
    /// </summary>
    [DataContract]
    public class GetActiveDiscountIdsResponse : CommerceResponse
    {
        /// <summary>
        /// Discount Ids
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_ids")]
        public IEnumerable<Guid> DiscountIds { get; set; }
    }
}