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
    /// Represents the response to a Get Claimed Deals API invocation.
    /// </summary>
    [DataContract]
    public class GetClaimedDealsResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the ClaimedDeals object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "claimedDeals")]
        public IEnumerable<Guid> ClaimedDeals { get; set; }
    }
}