//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace AcsToken
{
    [DataContract]
    public class AmexOAuth2TokenResponse
    {
        [DataMember(Name = "access_token")]
        public string Token { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }

        [DataMember(Name = "mac_key")]
        public string MacKey { get; set; }

        [DataMember(Name = "mac_algorithm")]
        public string MacAlgorithm { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }        
    }
}