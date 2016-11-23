//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a partner and a list of partner merchant IDs.
    /// </summary>
    [DataContract]
    public class PartnerMerchantIds
    {
        /// <summary>
        /// Gets or sets the partner whose partner merchant Ids are specified within this object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "partner")]
        public string Partner { get; set; }

        /// <summary>
        /// Gets or sets the partner merchant IDs.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "merchant_ids")]
        public IEnumerable<string> MerchantIds { get; set; }
    }
}