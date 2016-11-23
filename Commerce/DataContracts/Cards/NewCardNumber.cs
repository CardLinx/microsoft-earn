//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a new card number in an object that can be passed as the body of the AddCard call.
    /// </summary>
    [DataContract]
    public class NewCardNumber
    {
        /// <summary>
        /// Gets or sets the number of the card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "number")]
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets flight id.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "flight_id")]
        public string FlightId { get; set; }

        /// <summary>
        /// Gets or sets the referrer to associate with the authenticated add card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referrer")]
        public string Referrer { get; set; }
    }
}