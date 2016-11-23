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
    /// Represents a V3 deal.
    /// </summary>
    [DataContract]
    public class V3DealDataContract
    {
        /// <summary>
        /// Gets or sets the canonical ID for this deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the ProviderId of the merchant offering the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "provider_id")]
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the ProviderName of the merchant offering the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "provider_name")]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets whether the payload contains a deal for a national provider (true) or an aggregate provider (false).
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "is_national")]
        public bool IsNational { get; set; }

        /// <summary>
        /// Gets or sets the discounts belonging to this deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discounts")]
        public IEnumerable<V3DiscountDataContract> Discounts { get; set; }
    }
}