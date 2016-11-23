//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using global::Common.Utils;

    /// <summary>
    /// The helper class to validate an incoming SWT token string against set of signing keys from various issuers
    /// </summary>
    /// <remarks>
    /// This is a code extract from BEARER authentication implementation. 
    /// However, that is tightly coupled with AccountCache at the moment and 
    /// I didn't want to refactor that class (reduce high impact changes).
    /// </remarks>
    public class SimpleWebTokenValidator
    {
        /// <summary>
        /// The lookup dictionary for known token issuers and their token signing keys.
        /// </summary>
        private readonly ReadOnlyDictionary<TokenIssuer, Tuple<byte[], byte[]>> tokenSigningKeysForIssuer;

        public SimpleWebTokenValidator(
            IEnumerable<KeyValuePair<TokenIssuer, KeyPair>> issuersAndKeys)
        {

            if (issuersAndKeys == null)
            {
                throw new ArgumentNullException("issuersAndKeys");
            }

            var keyLookupDictionary = new Dictionary<TokenIssuer, Tuple<byte[], byte[]>>();

            foreach (var issuerAndKey in issuersAndKeys)
            {
                if (issuerAndKey.Value == null
                    || string.IsNullOrWhiteSpace(issuerAndKey.Value.PrimaryKey)
                    || string.IsNullOrWhiteSpace(issuerAndKey.Value.SecondaryKey))
                {
                    throw new ArgumentException(
                        string.Format("The issuer '{0}' has at least one invalid token signing key.", issuerAndKey.Key));
                }
                
                keyLookupDictionary.Add(
                    issuerAndKey.Key,
                    new Tuple<byte[], byte[]>(
                        issuerAndKey.Value.PrimaryKeyToByteArray(),
                        issuerAndKey.Value.SecondaryKeyToByteArray()));
            }

            this.tokenSigningKeysForIssuer =
                new ReadOnlyDictionary<TokenIssuer, Tuple<byte[], byte[]>>(keyLookupDictionary);
        }

        public bool Validate(string swtToken)
        {
            // Validate the token (note that the signature is verified only further down).
            SimpleWebToken token;

            try
            {
                token = SimpleWebToken.Parse(swtToken);
            }
            catch (SecurityException)
            {
                return false;
            }
            catch (Exception exception)
            {
                if (exception.IsFatal())
                {
                    throw;
                }

                return false;
            }

            // The token was parsed successfully.

            // Validate issuer.
            TokenIssuer tokenIssuer;
            if ((!Enum.TryParse(token.Issuer, true, out tokenIssuer)) || (!Enum.IsDefined(typeof(TokenIssuer), token.Issuer)))
            {
                return false;
            }

            // Validate audience
            TokenAudience tokenAudience;
            if ((!Enum.TryParse(token.Audience, true, out tokenAudience)) || (!Enum.IsDefined(typeof(TokenAudience), token.Audience)))
            {
                return false;
            }

            // Now validate the signature using the specified issuer's keys
            bool isSignatureValid = this.IsSignatureValidForIssuer(token, tokenIssuer);

            if (!isSignatureValid)
            {
                return false;
            }

            // Make sure the token is not expired.
            if (token.IsExpiredNow)
            {
                // Token expired.
                return false;
            }

            return true;
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