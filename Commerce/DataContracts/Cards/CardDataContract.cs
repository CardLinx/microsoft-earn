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
    public class CardDataContract
    {
        /// <summary>
        /// Gets or sets the canonical ID for this card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name on the credit card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "name_on_card")]
        public string NameOnCard { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the card number.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "last_four_digits")]
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the expiration of the card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "expiration")]
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets or sets the brand of the card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "card_brand")]
        public string CardBrand { get; set; }
    }
}