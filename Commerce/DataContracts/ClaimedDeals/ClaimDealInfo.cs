//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The data contract for the Claim Deal API.
    /// </summary>
    [DataContract]
    public class ClaimDealInfo
    {
        /// <summary>
        /// Gets or sets the ID of the deal the user is attempting to claim.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_id")]
        public Guid DealId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the card to which the claimed deal will be linked.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "card_id")]
        public int CardId { get; set; }
    }
}