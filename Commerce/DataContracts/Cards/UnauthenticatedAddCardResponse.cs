//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to an unauthenticated Add Card API invocation.
    /// </summary>
    [DataContract]
    public class UnauthenticatedAddCardResponse : AddCardResponse
    {
        /// <summary>
        /// Gets or sets the token to be used when activating the new user.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "user_token")]
        public string ActivationToken { get; set; }

        /// <summary>
        /// Gets or sets the authentication vector in use for the account.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "authentication_vector")]
        public string AuthenticationVector { get; set; }
    }
}