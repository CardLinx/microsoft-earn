//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents Amex OAuth Response.
    /// </summary>
    [DataContract]
    public class AmexOAuthResponse
    {
        /// <summary>
        /// Gets or sets Access Token
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "access_token")]
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets Token Type
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "token_type")]
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets Expires In
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "expires_in")]
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets Refresh Token
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "refresh_token")]
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets Scope
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "scope")]
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets Mac Key
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "mac_key")]
        [JsonProperty(PropertyName = "mac_key")]
        public string MacKey { get; set; }

        /// <summary>
        /// Gets or sets Mac Algorithm
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "mac_algorithm")]
        [JsonProperty(PropertyName = "mac_algorithm")]
        public string MacAlgorithm { get; set; }
    }
}