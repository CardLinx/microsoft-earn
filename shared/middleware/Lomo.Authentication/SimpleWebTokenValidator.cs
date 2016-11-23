//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Validates simple web tokens against specified expected values.
    /// </summary>
    public static class SimpleWebTokenValidator
    {
        /// <summary>
        /// Validates the specified token against the specified expected values.
        /// </summary>
        /// <param name="token">
        /// The token to validate.
        /// </param>
        /// <param name="resourceNamespace">
        /// The Azure Namespace of the resource being invoked.
        /// </param>
        /// <param name="resource">
        /// The URI of the resource being invoked (AKA the "trusted audience").
        /// </param>
        /// <param name="trustedSigningKey">
        /// The key used to sign trusted tokens.
        /// </param>
        /// <returns>
        /// * True if the token is valid.
        /// * Else returns false.
        /// </returns>
        public static bool Validate(
                                    string token,
                                    string resourceNamespace,
                                    string resource,
                                    string trustedSigningKey)
        {
            bool result = false;

            // Get the name/value pairs embedded in the token.
            Dictionary<string, string> tokenProperties = ExtractTokenProperties(token);

            if (IsHMACValid(token, Convert.FromBase64String(trustedSigningKey)) == true)
            {
                if (IsExpired(token, tokenProperties) == false)
                {
                    string trustedIssuer = string.Format(
                                                         "https://{0}.{1}/",
                                                         resourceNamespace.ToLowerInvariant(),
                                                         "accesscontrol.windows.net");
                    if (IsIssuerTrusted(token, trustedIssuer, tokenProperties) == true)
                    {
                        if (IsAudienceTrusted(token, resource, tokenProperties) == true)
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts name / value pair properties embedded in the token.
        /// </summary>
        /// <param name="token">
        /// The token from which to extract the name / value pairs.
        /// </param>
        /// <returns>
        /// The name / value pair properties.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter token cannot be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// * The token contains an incomplete name/value pair.
        /// -OR-
        /// * The token contains more than one name/value pair with the same name.
        /// </exception>
        public static Dictionary<string, string> ExtractTokenProperties(string token)
        {
            Dictionary<string, string> result;

            if (string.IsNullOrWhiteSpace(token) == true)
            {
                throw new ArgumentNullException("token", "Parameter token cannot be null.");
            }

            string[] tokenPairs = token.Split('&');
            result = tokenPairs.Aggregate(
                                          new Dictionary<string, string>(),
                (pairs, pair) =>
                {
                    if (string.IsNullOrWhiteSpace(pair) == false)
                    {
                        string[] splitPair = pair.Split('=');

                        if (splitPair.Length != 2)
                        {
                            throw new ArgumentException("The token contains an incomplete name/value pair.", "token");
                        }

                        if (pairs.ContainsKey(splitPair[0]) == true)
                        {
                            throw new ArgumentException(
                                                        "The token contains more than one name/value pair with the same name.",
                                                        "token");
                        }

                        pairs.Add(HttpUtility.UrlDecode(splitPair[0]), HttpUtility.UrlDecode(splitPair[1]));
                    }

                    return pairs;
                });

            return result;
        }

        /// <summary>
        /// Determines if the specified token's valid key-hash message authentication code is valid.
        /// </summary>
        /// <param name="token">
        /// The token whose HMAC to validate.
        /// </param>
        /// <param name="key">
        /// The key to use to validate the token's HMAC.
        /// </param>
        /// <returns>
        /// * True if the token's HMAC is valid.
        /// * Else returns false.
        /// </returns>
        private static bool IsHMACValid(
                                        string token,
                                        byte[] key)
        {
            bool result = false;

            string[] tokenWithSignature = token.Split(new string[] { "&HMACSHA256=" }, StringSplitOptions.None);

            if ((tokenWithSignature != null) && (tokenWithSignature.Length == 2))
            {
                HMACSHA256 hash = new HMACSHA256(key);
                byte[] signatureBytes = hash.ComputeHash(Encoding.ASCII.GetBytes(tokenWithSignature[0]));
                string signature = HttpUtility.UrlEncode(Convert.ToBase64String(signatureBytes));
                if (signature == tokenWithSignature[1])
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines if the specified token has expired.
        /// </summary>
        /// <param name="token">
        /// The token whose expiry to check.
        /// </param>
        /// <param name="tokenProperties">
        /// The name / value pair properties embedded in the token.
        /// </param>
        /// <returns>
        /// * True if the token has expired.
        /// * Else returns false.
        /// </returns>
        /// <exception cref="token">
        /// The token did not contain the ExpiresOn key.
        /// </exception>
        private static bool IsExpired(
                                      string token,
                                      Dictionary<string, string> tokenProperties)
        {
            bool result = true;

            try
            {
                // Get the expiry time.
                ulong expiresOn = Convert.ToUInt64(tokenProperties["ExpiresOn"]);

                // Get the current time.
                TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                ulong currentTime = Convert.ToUInt64(Convert.ToUInt64(timeSpan.TotalSeconds));

                if (currentTime <= expiresOn)
                {
                    result = false;
                }
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("The token did not contain the ExpiresOn key.", "token");
            }

            return result;
        }

        /// <summary>
        /// Determines whether the issuer of the token is trusted.
        /// </summary>
        /// <param name="token">
        /// The token whose issuer to validate.
        /// </param>
        /// <param name="trustedTokenIssuer">
        /// The trusted token issuer.
        /// </param>
        /// <param name="tokenProperties">
        /// The name / value pair properties embedded in the token.
        /// </param>
        /// <returns>
        /// * True if the issuer is trusted.
        /// * Else returns false.
        /// </returns>
        private static bool IsIssuerTrusted(
                                            string token,
                                            string trustedTokenIssuer,
                                            Dictionary<string, string> tokenProperties)
        {
            bool result = false;

            string issuerName;
            tokenProperties.TryGetValue("Issuer", out issuerName);
            if (string.IsNullOrWhiteSpace(issuerName) == false && issuerName.Equals(trustedTokenIssuer) == true)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Determines if the audience specified in the token is the trusted audience.
        /// </summary>
        /// <param name="token">
        /// The token whose audience to validate.
        /// </param>
        /// <param name="trustedAudience">
        /// The trusted audience.
        /// </param>
        /// <param name="tokenProperties">
        /// The name / value pair properties embedded in the token.
        /// </param>
        /// <returns>
        /// * True if the audience specified in the token is the trusted audience.
        /// * Else returns false.
        /// </returns>
        private static bool IsAudienceTrusted(
                                              string token,
                                              string trustedAudience,
                                              Dictionary<string, string> tokenProperties)
        {
            bool result = false;

            string audienceValue;
            tokenProperties.TryGetValue("Audience", out audienceValue);
            if (string.IsNullOrWhiteSpace(audienceValue) == false &&
                audienceValue.Equals(trustedAudience, StringComparison.OrdinalIgnoreCase) == true)
            {
                result = true;
            }

            return result;
        }
    }
}