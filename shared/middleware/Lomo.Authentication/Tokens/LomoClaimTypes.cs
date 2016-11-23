//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication.Tokens
{
    /// <summary>
    /// Standard Lomo ClaimTypes
    /// </summary>
    public class LomoClaimTypes
    {
        /// <summary>
        /// Issuer name claim type.
        /// </summary>
        public const string IssuerClaimType = "issuer";

        /// <summary>
        /// Resource name claim type.
        /// </summary>
        public const string ResourceClaimType = "resource";

        /// <summary>
        /// action name claim type.
        /// </summary>
        public const string ActionClaimType = "action";

        /// <summary>
        /// expiration claim type.
        /// </summary>
        public const string ExpirationClaimType = "expiry_time";

        /// <summary>
        /// userId claim type.
        /// </summary>
        public const string UserIdClaimType = "user_id";

        /// <summary>
        /// user name claim type.
        /// </summary>
        public const string NameClaimType = "name";

        /// <summary>
        /// user name claim type.
        /// </summary>
        public const string EmailClaimType = "email";
    }
}