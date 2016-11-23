//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The data contract for the Service Claim Deal API.
    /// </summary>
    [DataContract]
    public class ClaimDealPayload
    {
        /// <summary>
        /// Gets or sets the ID of the user attempting to claim the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the ClaimDealInfo for this payload.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "claim_deal_info")]
        public ClaimDealInfo ClaimDealInfo { get; set; }
    }
}