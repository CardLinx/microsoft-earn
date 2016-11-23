//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a Get UserToken For Card Operation API invocation.
    /// </summary>
    [DataContract]
    public class GetSecureCardOperationTokenResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the token pairing the user with the intended operation and environment.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "token")]
        public string Token { get; set; }
    }
}