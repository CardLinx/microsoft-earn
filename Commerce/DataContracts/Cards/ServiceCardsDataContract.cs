//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a card.
    /// </summary>
    [DataContract]
    public class ServiceCardDataContract
    {
        /// <summary>
        /// Gets or sets the canonical ID for this card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the User ID for this card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "user_id")]
        public Guid UserId { get; set; }

    }
}