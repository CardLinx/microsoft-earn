//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a Add Card API invocation.
    /// </summary>
    [DataContract]
    public class AddCardResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the canonical ID for the newly added card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "new_card_id")]
        public Guid NewCardId { get; set; }
    }
}