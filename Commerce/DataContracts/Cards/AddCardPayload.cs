//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the payload for an add card event.
    /// </summary>
    [DataContract]
    public class AddCardPayload
    {
        /// <summary>
        /// Gets or sets the user's e-mail address.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the number of the card.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "number")]
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the referrer to associate with the unauthenticated signup.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referrer")]
        public string Referrer { get; set; }

        /// <summary>
        /// Gets or sets the user's location data.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "user_loc")]
        public string UserLocation { get; set; }
    }
}