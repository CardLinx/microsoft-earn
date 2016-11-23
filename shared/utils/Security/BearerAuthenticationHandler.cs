//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;

    using global::Common.Utils;

    /// <summary>
    /// Handler authentication using Bearer scheme.
    /// Currently on SWT tokens are supported.
    /// </summary>
    public class BearerAuthenticationHandler : AuthenticationHandlerBase
    {
        readonly IDictionary<TokenAudience, IDictionary<TokenIssuer, IList<string>>> audienceIssuerRolesMapping;

        /// <summary>
        /// The lookup dictionary for known token issuers and their token signing keys.
        /// </summary>
        private readonly ReadOnlyDictionary<TokenIssuer, Tuple<byte[], byte[]>> tokenSigningKeysForIssuer;

        public const string AuthScheme = "Bearer";

        /// <summary>
        /// Initializes a new instance of BearerAuthenticationHandler class.
        /// </summary>
        /// <param name="issuersAndKeys">The dictionary of token issuers and their token signing keys.</param>
        /// <param name="audienceIssuerRolesMapping"></param>
        public BearerAuthenticationHandler(
            IEnumerable<KeyValuePair<TokenIssuer, KeyPair>> issuersAndKeys, IDictionary<TokenAudience, IDictionary<TokenIssuer,IList<string>>> audienceIssuerRolesMapping)
            : base(AuthScheme)
        {
            if (issuersAndKeys == null)
            {
                throw new ArgumentNullException("issuersAndKeys");
            }

            if (audienceIssuerRolesMapping == null)
            {
                throw new ArgumentNullException("audienceIssuerRolesMapping");
            }

            var keyLookupDictionary = new Dictionary<TokenIssuer, Tuple<byte[], byte[]>>();

            foreach (var issuerAndKey in issuersAndKeys)
            {
                if (issuerAndKey.Value == null
                    || string.IsNullOrWhiteSpace(issuerAndKey.Value.PrimaryKey)
                    || string.IsNullOrWhiteSpace(issuerAndKey.Value.SecondaryKey))
                {
                    throw new ArgumentException(
                        "The issuer '{0}' has at least one invalid token signing key.".FormatInvariant(issuerAndKey.Key));
                }

                keyLookupDictionary.Add(
                    issuerAndKey.Key,
                    new Tuple<byte[], byte[]>(
                        issuerAndKey.Value.PrimaryKeyToByteArray(),
                        issuerAndKey.Value.SecondaryKeyToByteArray()));
            }

            this.tokenSigningKeysForIssuer =
                new ReadOnlyDictionary<TokenIssuer, Tuple<byte[], byte[]>>(keyLookupDictionary);

            this.audienceIssuerRolesMapping = audienceIssuerRolesMapping;
        }

        /// <summary>
        /// Authenticates the request using Bearer scheme.
        /// </summary>
        /// <param name="request">The request to be authenticated.</param>
        /// <returns>
        /// An instance of AuthenticationResult which specifies if the authentication was successful or not.
        /// If successful, it will contain the IPrincipal.
        /// If not successful, it will contain the error message.
        /// </returns>
        public override AuthenticationResult Authenticate(HttpRequestMessage request)
        {
            // Validate the request first.
            AuthenticationResult authenticationResult;
            if (!this.ValidateRequest(request, out authenticationResult))
            {
                return authenticationResult;
            }

            // Validate the token (note that the signature is verified only further down).
            SimpleWebToken token;

            try
            {
                string tokenString = request.Headers.Authorization.Parameter;
                token = SimpleWebToken.Parse(tokenString);
            }
            catch (FormatException)
            {
                return
                    AuthenticationResult.CreateFailedAuthenticationResult(
                        "The bearer token is not in a valid format.");
            }
            catch (SecurityException exception)
            {
                return AuthenticationResult.CreateFailedAuthenticationResult(exception.Message);
            }
            catch (Exception exception)
            {
                if (exception.IsFatal())
                {
                    throw;
                }

                return
                    AuthenticationResult.CreateFailedAuthenticationResult(
                        "An unexpected error occurred while parsing the token.");
            }
            
            // Validate issuer.
            TokenIssuer tokenIssuer;
            if ((!Enum.TryParse(token.Issuer, true, out tokenIssuer)) || (!Enum.IsDefined(typeof(TokenIssuer), token.Issuer)))
            {
                return AuthenticationResult.CreateFailedAuthenticationResult(
                    "Unrecognized token issuer: '{0}'",
                    token.Issuer);
            }

            // Validate audience
            TokenAudience tokenAudience;
            if ((!Enum.TryParse(token.Audience, true, out tokenAudience)) || (!Enum.IsDefined(typeof(TokenAudience), token.Audience)))
            {
                return AuthenticationResult.CreateFailedAuthenticationResult(
                    "Unrecognized token audience: '{0}'",
                    token.Audience);
            }

            // Now validate the signature using the specified issuer's keys
            bool isSignatureValid = this.IsSignatureValidForIssuer(token, tokenIssuer);

            if (!isSignatureValid)
            {
                return AuthenticationResult.CreateFailedAuthenticationResult("The token signature is not valid.");
            }

            // Make sure the token is not expired.
            if (token.IsExpiredNow)
            {
                // Token expired.
                return AuthenticationResult.CreateFailedAuthenticationResult(
                    "The token expired at {0}",
                    token.ExpiresOn.ToRoundtripFormatString());
            }

            // Identify the roles for this principal.
            IEnumerable<Claim> roleClaims = GetRoleClaims(tokenIssuer, tokenAudience);

            if (roleClaims == null)
            {
                return AuthenticationResult.CreateFailedAuthenticationResult(
                    "This combination of token issuer and audience is not allowed for this service.");
            }

            var claims = new List<Claim>();

            // We wont allow the caller to specify roles, we will assign the roles here.
            claims.AddRange(token.Claims.Where(c => c.Type != HolMonClaimTypes.Role));

            // Adding claims as we see fit.
            claims.AddRange(roleClaims);

            claims.Add(new Claim(HolMonClaimTypes.AuthenticationScheme, this.AuthenticationScheme));
            claims.Add(new Claim(HolMonClaimTypes.TokenAudience, token.Audience));
            claims.Add(new Claim(HolMonClaimTypes.TokenIssuer, token.Issuer));

            this.LogClaims(claims);

            var identity = new ClaimsIdentity(claims, request.Headers.Authorization.Scheme);
            var principal = new ClaimsPrincipal(identity);

            return AuthenticationResult.CreateSuccessfulAuthenticationResult(principal);
        }

        /// <summary>
        /// Gets the role claims for the current principal.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        private IEnumerable<Claim> GetRoleClaims(TokenIssuer issuer, TokenAudience audience)
        {
            IDictionary<TokenIssuer, IList<string>> issuerToRolesMapping;
            if (this.audienceIssuerRolesMapping.TryGetValue(audience, out issuerToRolesMapping))
            {
                IList<string> roles;
                if (issuerToRolesMapping.TryGetValue(issuer, out roles))
                {
                    IList<Claim> roleClaims = roles.Select(role => new Claim(HolMonClaimTypes.Role, role)).ToList();
                    return roleClaims;
                }
            }

            return new[] { new Claim(HolMonClaimTypes.Role, HolMonRoles.AnonymousAccessRole) };
        }

        /// <summary>
        /// Checks whether the token signature is valid using the specified token issuer's keys.
        /// </summary>
        /// <param name="token">the token whose signature should be validated.</param>
        /// <param name="tokenIssuer">The token issuer who keys are to be used for validation.</param>
        /// <returns>
        /// True if the signature could be validated with the specified issuer's keys.
        /// </returns>
        private bool IsSignatureValidForIssuer(SimpleWebToken token, TokenIssuer tokenIssuer)
        {
            if (!this.tokenSigningKeysForIssuer.ContainsKey(tokenIssuer))
            {
                return false;
            }

            var keys = this.tokenSigningKeysForIssuer[tokenIssuer];

            return this.ValidateTokenSignature(token, keys.Item1, keys.Item2);
        }

        /// <summary>
        /// Validates the token signature against the two keys.
        /// </summary>
        /// <param name="token">The token to be validated.</param>
        /// <param name="primaryKey">
        /// The first key to be used for validation.
        /// </param>
        /// <param name="secondaryKey">
        /// The second key to be used for validation if the second key fails.
        /// </param>
        /// <returns></returns>
        private bool ValidateTokenSignature(SimpleWebToken token, byte[] primaryKey, byte[] secondaryKey)
        {
            try
            {
                // try primary key first.
                bool isSignatureValid = token.VerifySignature(primaryKey);

                if (!isSignatureValid)
                {
                    // now try secondary key.
                    isSignatureValid = token.VerifySignature(secondaryKey);

                    if (!isSignatureValid)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }
    }
}