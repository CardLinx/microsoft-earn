//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a new card.
    /// </summary>
    [DataContract]
//    [Obsolete] Not sure if it's safe to remove this yet. UI is still receiving placeholder info. Shiv is on vacation... need to be sure we won't break UI
//  if we remove the deprecated Cards columns/fields, etc..
    public class NewCardInfo
    {
        /// <summary>
        /// Gets or sets the name on the credit card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "name_on_card")]
        public string NameOnCard { get; set; }

        /// <summary>
        /// Gets or sets the number of the card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "number")]
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the expiration of the card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "expiration")]
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets or sets the type of the card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "card_brand")]
        public string CardBrand { get; set; }

        /// <summary>
        /// Gets or sets flight id.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "flight_id")]
        public string FlightId { get; set; }
    }
}