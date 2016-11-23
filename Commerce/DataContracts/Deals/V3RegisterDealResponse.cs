//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a service Register Deal API invocation.
    /// </summary>
    [DataContract]
    public class V3RegisterDealResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the results for each registered discount.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
         Justification = "Property in data contract must be settable by callers or a constructor must be created. These are " +
                         " supposed to be pure data contracts, so constructors aren't allowed.")]
        [DataMember(EmitDefaultValue = false, Name = "discount_results")]
        public IDictionary<Guid, string> DiscountResults { get; set; }
    }
}