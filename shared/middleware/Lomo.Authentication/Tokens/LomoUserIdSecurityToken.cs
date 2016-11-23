//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication.Tokens
{
    using System;

    /// <summary>
    /// Initializes a new instance of the <see cref="LomoUserIdSecurityToken"/> class.
    /// </summary>
    public class LomoUserIdSecurityToken : LomoSecurityTokenBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LomoUserIdSecurityToken"/> class.
        /// </summary>
        /// <param name="base64EncodedString">The base64 encoded token string.</param>
        /// <param name="signingKey">The signing key. Minimum length should be 8 characters.</param>
        /// <param name="decryptionPassword">The decryption password to dervice the decryption key from. Minimum length should be 8 characters.</param>
        /// <param name="decryptionSalt">The decryption salt to derive the decryption IV from. Minimum length should be 8 characters.</param>
        /// <param name="maxClockSkew"> The maxClockSkew interval in seconds to use for token's validity determination.</param>
        public LomoUserIdSecurityToken(string base64EncodedString, string signingKey, string decryptionPassword, string decryptionSalt, ulong maxClockSkew)
            : base(base64EncodedString, signingKey, decryptionPassword, decryptionSalt, maxClockSkew)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LomoUserIdSecurityToken"/> class.
        /// </summary>
        /// <param name="userId">The current user's Id.</param>
        /// <param name="issuer">The issuer of the token.</param>
        /// <param name="resource">The resource the token is meant for.</param>
        /// <param name="action">The action of the token.</param>
        /// <param name="tokenLifeTimeInSeconds">The lifetime of the token in seconds.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <param name="encryptionPassword">The encryption password to dervice the encryption key from. Minimum length should be 8 characters.</param>
        /// <param name="encryptionSalt">The encryption salt to derive the encryption IV from. Minimum length should be 8 characters.</param>
        public LomoUserIdSecurityToken(string userId, string issuer, string resource, string action, long tokenLifeTimeInSeconds, string signingKey, string encryptionPassword, string encryptionSalt)
            : base(issuer, resource, action, tokenLifeTimeInSeconds, signingKey, encryptionPassword, encryptionSalt)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            this.AddClaim(LomoClaimTypes.UserIdClaimType, userId);
        }

        /// <summary>
        /// Gets the ID of the current Lomo User.
        /// </summary>
        public string UserId
        {
            get
            {
                return base[LomoClaimTypes.UserIdClaimType];
            }
        }

        /// <summary>
        /// Validates the claims added in the subclass.
        /// </summary>
        /// <returns>Returns True if the claims in the token are valid else returns false.</returns>
        protected override bool ValidateClaims()
        {
            if (string.IsNullOrEmpty(this.UserId))
            {
                return false;
            }

            return true;
        }
    }
}