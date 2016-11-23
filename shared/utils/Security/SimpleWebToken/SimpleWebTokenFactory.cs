//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    using global::Common.Utils;

    /// <summary>
    /// Helper class for creating SWT tokens.
    /// </summary>
    public static class SimpleWebTokenFactory
    {
        /// <summary>
        /// Factory method for creating SWT tokens.
        /// </summary>
        /// <param name="issuer">The entity issuing the token.</param>
        /// <param name="audience">The entity receiving the token.</param>
        /// <param name="expiresOnUtc">The expiry time for the token in UTC.</param>
        /// <param name="signingKey">The token signing key.</param>
        /// <returns>
        /// An instance of SimpleWebToken class.
        /// </returns>
        public static SimpleWebToken CreateToken(
            TokenIssuer issuer,
            TokenAudience audience,
            DateTime expiresOnUtc,
            byte[] signingKey)
        {
            return CreateToken(issuer, audience, expiresOnUtc, null, signingKey);
        }

        /// <summary>
        /// Factory method for creating SWT tokens. Use this overload when there is a requirement
        /// to encode additional claims into the token.
        /// </summary>
        /// <param name="issuer">The entity issuing the token.</param>
        /// <param name="audience">The entity receiving the token.</param>
        /// <param name="expiresOnUtc">The expiry time for the token in UTC.</param>
        /// <param name="additionalClaims">
        /// Any additional claims to be included as part of the token.
        /// </param>
        /// <param name="signingKey">The token signing key.</param>
        /// <returns>
        /// An instance of SimpleWebToken class.
        /// </returns>
        public static SimpleWebToken CreateToken(
            TokenIssuer issuer,
            TokenAudience audience,
            DateTime expiresOnUtc,
            IEnumerable<Claim> additionalClaims,
            byte[] signingKey)
        {
            if (issuer == TokenIssuer.Unknown)
            {
                throw new ArgumentException(
                    "{0} is not not allowed as token issuer.".FormatInvariant(issuer.ToString()));
            }

            if (audience == TokenAudience.Unknown)
            {
                throw new ArgumentException(
                    "{0} is not allowed as token audience.".FormatInvariant(audience.ToString()));
            }

            if (signingKey == null || signingKey.Length == 0)
            {
                throw new ArgumentException("The signing key is not valid.");
            }

            var claims = new List<Claim>();

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            var token = SimpleWebToken.Create(issuer.ToString(), audience.ToString(), expiresOnUtc, claims);
            token.SignToken(signingKey);
            return token;
        }
    }
}