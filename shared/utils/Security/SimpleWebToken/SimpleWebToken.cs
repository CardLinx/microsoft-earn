//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    using global::Common.Utils;

    /// <summary>
    /// Contains methods to parse, create, sign and verify SWT tokens.
    /// </summary>
    public class SimpleWebToken
    {
        /// <summary>
        /// The audience for the token.
        /// </summary>
        private string tokenAudience;

        /// <summary>
        /// The claims made in the token.
        /// </summary>
        private List<Claim> tokenClaims;

        /// <summary>
        /// The issuer of the token.
        /// </summary>
        private string tokenIssuer;

        /// <summary>
        /// The expiry time of the token.
        /// </summary>
        private DateTime tokenExpiryTime;

        /// <summary>
        /// The signature of the token.
        /// </summary>
        private string tokenSignature;

        /// <summary>
        /// The token string without the signature.
        /// </summary>
        private string tokenUnsignedString;

        /// <summary>
        /// Creates a SimpleWebToken by parsing the form-encoded
        /// string representation of the token.
        /// </summary>
        /// <param name="encodedToken">The encoded token string.</param>
        /// <returns>
        /// An instance of SimpleWebToken.
        /// </returns>
        /// <exception cref="SecurityException">
        /// Thrown if errors are encountered while processing the token string.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the issuer secret or encoded token is null or invalid.
        /// </exception>
        public static SimpleWebToken Parse(string encodedToken)
        {
            if (string.IsNullOrWhiteSpace(encodedToken))
            {
                throw new SecurityException("The encoded token is not valid.");
            }

            var token = new SimpleWebToken();
            token.Decode(encodedToken);
            return token;
        }

        /// <summary>
        /// Factory method for creating a new SimpleWebToken.
        /// </summary>
        /// <param name="issuer">The token issuer.</param>
        /// <param name="audience">The token audience.</param>
        /// <returns>
        /// A new SimpleWebToken instance with specified values.
        /// </returns>
        public static SimpleWebToken Create(string issuer, string audience)
        {
            return Create(issuer, audience, DateTime.MaxValue);
        }

        /// <summary>
        /// Factory method for creating a new SimpleWebToken.
        /// </summary>
        /// <param name="issuer">The token issuer.</param>
        /// <param name="claims">The claims to be encoded into the token.</param>
        /// <returns>
        /// A new SimpleWebToken instance with specified values.
        /// </returns>
        public static SimpleWebToken Create(string issuer, IEnumerable<Claim> claims)
        {
            return Create(issuer, null, DateTime.MaxValue, claims);
        }

        /// <summary>
        /// Factory method for creating a new SimpleWebToken.
        /// </summary>
        /// <param name="issuer">The token issuer.</param>
        /// <param name="audience">The token audience.</param>
        /// <param name="claims">The claims to be encoded into the token.</param>
        /// <returns>
        /// A new SimpleWebToken instance with specified values.
        /// </returns>
        public static SimpleWebToken Create(string issuer, string audience, IEnumerable<Claim> claims)
        {
            return Create(issuer, audience, DateTime.MaxValue, claims);
        }

        /// <summary>
        /// Factory method for creating a new SimpleWebToken.
        /// </summary>
        /// <param name="issuer">The token issuer.</param>
        /// <param name="audience">The token audience.</param>
        /// <param name="expiresOn">The UTC time when the token should expire.</param>
        /// <returns>
        /// A new SimpleWebToken instance with specified values.
        /// </returns>
        public static SimpleWebToken Create(string issuer, string audience, DateTime expiresOn)
        {
            return Create(issuer, audience, expiresOn, null);
        }

        /// <summary>
        /// Factory method for creating a new SimpleWebToken.
        /// </summary>
        /// <param name="issuer">The token issuer.</param>
        /// <param name="audience">The token audience.</param>
        /// <param name="expiresOn">The UTC time when the token should expire.</param>
        /// <param name="claims">The claims to be encoded into the token.</param>
        /// <returns>
        /// A new SimpleWebToken instance with specified values.
        /// </returns>
        public static SimpleWebToken Create(string issuer, string audience, DateTime expiresOn, IEnumerable<Claim> claims)
        {
            if (string.IsNullOrEmpty(issuer))
                throw new ArgumentNullException("issuer");

            var token = new SimpleWebToken();
            token.tokenAudience = audience;
            token.tokenExpiryTime = expiresOn;
            token.tokenIssuer = issuer;
            token.tokenClaims = new List<Claim>();

            if (claims != null)
            {
                foreach (Claim claim in claims)
                {
                    if (IsReservedClaimType(claim.Type))
                        throw new ArgumentException("ClaimType " + claim.Type + " is reserved for system use");

                    token.tokenClaims.Add(claim);
                }
            }

            return token;
        }

        /// <summary>
        /// The default constructor is private since creation is exposed through factory methods.
        /// </summary>
        private SimpleWebToken()
        {
        }

        /// <summary>
        /// The Audience for the token.
        /// </summary>
        public string Audience
        {
            get { return this.tokenAudience; }
        }

        /// <summary>
        /// The Claims in the token.
        /// </summary>
        public IReadOnlyCollection<Claim> Claims
        {
            get { return this.tokenClaims.AsReadOnly(); }
        }

        /// <summary>
        /// The expiry datetime for the token.
        /// </summary>
        public DateTime ExpiresOn
        {
            get { return this.tokenExpiryTime; }
        }

        /// <summary>
        /// The issuer of the token.
        /// </summary>
        public string Issuer
        {
            get { return this.tokenIssuer; }
        }

        /// <summary>
        /// The signature value of the token.
        /// </summary>
        public string Signature
        {
            get { return this.tokenSignature; }
        }

        /// <summary>
        /// Gets a value indicating whether the token is expired.
        /// </summary>
        public bool IsExpiredNow
        {
            get
            {
                return DateTime.UtcNow > this.ExpiresOn;
            }
        }

        /// <summary>
        /// Verifies the signature of the token.
        /// </summary>
        /// <param name="tokenSigningKey">
        /// The key that was used to sign the token.
        /// </param>
        /// <returns>
        /// True if the signature is valid, false otherwise.
        /// </returns>
        public bool VerifySignature(byte[] tokenSigningKey)
        {
            ValidateKey(tokenSigningKey);

            if (this.tokenSignature == null || this.tokenUnsignedString == null)
                throw new SecurityException("Token has never been signed");

            string verifySignature;
            using (var sha256 = new HMACSHA256(tokenSigningKey))
            {
                verifySignature = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(this.tokenUnsignedString)));
            }

            // a secure, constant-time comparison algorithm to ensure time channel information leaking does not occur.
            return this.tokenSignature.SecureCompare(verifySignature);
        }

        /// <summary>
        /// Signs the token.
        /// </summary>
        /// <param name="tokenSigningKey">The key to sign the token with.</param>
        /// <returns>
        /// A string representation of the signed token.
        /// </returns>
        public string SignToken(byte[] tokenSigningKey)
        {
            ValidateKey(tokenSigningKey);

            this.tokenUnsignedString = this.Encode();
            using (var sha256 = new HMACSHA256(tokenSigningKey))
            {
                this.tokenSignature =
                    Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(this.tokenUnsignedString)));
            }

            return this.Encode();
        }

        /// <summary>
        /// Validates the secret key.
        /// </summary>
        /// <param name="tokenSigningKey">The secret key for signing.</param>
        private static void ValidateKey(byte[] tokenSigningKey)
        {
            if (tokenSigningKey == null || tokenSigningKey.Length == 0)
            {
                throw new SecurityException("The token signing key is invalid.");
            }
        }

        /// <summary>
        /// Gets a string representation of the token.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Encode();
        }

        /// <summary>
        /// Used to determine whether the parameter claim type is one of the reserved
        /// SimpleWebToken claim types: Audience, HMACSHA256, ExpiresOn or Issuer.
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns></returns>
        private static bool IsReservedClaimType(string claimType)
        {
            if (string.Compare(claimType, SimpleWebTokenConstants.TokenAudience, StringComparison.OrdinalIgnoreCase) == 0)
                return true;

            if (string.Compare(claimType, SimpleWebTokenConstants.TokenDigest256, StringComparison.OrdinalIgnoreCase) == 0)
                return true;

            if (string.Compare(claimType, SimpleWebTokenConstants.TokenExpiresOn, StringComparison.OrdinalIgnoreCase) == 0)
                return true;

            if (string.Compare(claimType, SimpleWebTokenConstants.TokenIssuer, StringComparison.OrdinalIgnoreCase) == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Parses a SWT token string.
        /// </summary>
        /// <param name="rawToken">The raw token string.</param>
        private void Decode(string rawToken)
        {
            string audience = null, issuer = null, signature = null, unsignedString = null, expires = null;

            // Find the last parameter. The signature must be last per SWT specification.
            int lastSeparator = rawToken.LastIndexOf(SimpleWebTokenConstants.ParameterSeparator);

            // Check whether the last parameter is an hmac.
            if (lastSeparator > 0)
            {
                string lastParamStart = SimpleWebTokenConstants.ParameterSeparator + SimpleWebTokenConstants.TokenDigest256 + "=";
                string lastParam = rawToken.Substring(lastSeparator);

                // Strip the trailing hmac to obtain the original unsigned string for later hmac verification.
                // e.g. name1=value1&name2=value2&HMACSHA256=XXX123 -> name1=value1&name2=value2
                if (lastParam.StartsWith(lastParamStart, StringComparison.Ordinal))
                {
                    unsignedString = rawToken.Substring(0, lastSeparator);
                }
            }
            else if (lastSeparator < 0)
            {
                // If there's no separator, then the last parameter is also the first parameter.
                const string LastParamStart = SimpleWebTokenConstants.TokenDigest256 + "=";

                if (rawToken.StartsWith(LastParamStart, StringComparison.Ordinal))
                {
                    // Strip everything, since there is nothing other than the hmac.
                    unsignedString = string.Empty;
                }
            }
            else
            {
                // lastSeparator == 0 would mean the token begins with '&', which is invalid syntax
                throw new SecurityException("Invalid syntax for the token.");
            }

            // Signature is a mandatory parameter, and it must be the last one.
            // If there's no trailing hmac, Return error.
            if (unsignedString == null)
            {
                throw new SecurityException("Invalid SWT token. HMAC signature is misplaced.");
            }

            // Create a dictionary of SWT claims, checking for duplicates.
            var attributes = GetAttributesFromTokenString(rawToken);

            // Audience is optional.
            if (attributes.TryGetValue(SimpleWebTokenConstants.TokenAudience, out audience))
            {
                attributes.Remove(SimpleWebTokenConstants.TokenAudience);
            }

            // ExpiresOn is optional.
            if (attributes.TryGetValue(SimpleWebTokenConstants.TokenExpiresOn, out expires))
            {
                attributes.Remove(SimpleWebTokenConstants.TokenExpiresOn);
            }

            // Issuer is optional
            if (attributes.TryGetValue(SimpleWebTokenConstants.TokenIssuer, out issuer))
            {
                attributes.Remove(SimpleWebTokenConstants.TokenIssuer);
            }

            // The signature is not optional.
            if (attributes.TryGetValue(SimpleWebTokenConstants.TokenDigest256, out signature))
            {
                attributes.Remove(SimpleWebTokenConstants.TokenDigest256);
            }
            else
            {
                throw new SecurityException("The token does not have a signature.");
            }

            // Audience, ExpiresOn, and Issuer should have been removed from the dictionary by now.
            // Ensure they are not present in any duplicate or alternate casing.
            CheckForReservedClaimType(attributes, SimpleWebTokenConstants.TokenAudience);
            CheckForReservedClaimType(attributes, SimpleWebTokenConstants.TokenExpiresOn);
            CheckForReservedClaimType(attributes, SimpleWebTokenConstants.TokenIssuer);

            List<Claim> claims = CreateClaims(issuer, attributes, SimpleWebTokenConstants.DefaultCompoundClaimDelimiter);

            this.tokenAudience = audience;
            this.tokenClaims = claims;
            this.tokenExpiryTime = DecodeExpiry(expires);
            this.tokenIssuer = issuer;
            this.tokenSignature = signature;
            this.tokenUnsignedString = unsignedString;
        }

        /// <summary>
        /// Enforces casing requirements on reserved claim types. If the claim type is present in any casing, an exception is thrown.
        /// </summary>
        /// <remarks>
        /// This function MUST be called after the reserved claim type has been checked and removed (if present).
        /// </remarks>
        private static void CheckForReservedClaimType(Dictionary<string, string> inputDictionary, string claimType)
        {
            if (inputDictionary.Keys.Contains(claimType, StringComparer.OrdinalIgnoreCase))
            {
                string exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid SWT token. The parameter name '{0}' is reserved and cannot be used in any other casing.",
                    claimType);

                throw new SecurityException(exceptionMessage);
            }
        }

        /// <summary>
        /// A=B,C,D should result in the claims A=B, A=C, and A=D.
        /// Duplicate values are allowed. Empty strings are not allowed.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="claims"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static List<Claim> CreateClaims(string issuer, IEnumerable<KeyValuePair<string, string>> claims, char delimiter)
        {
            var decodedClaims = new List<Claim>();

            foreach (KeyValuePair<string, string> claim in claims)
            {
                if (string.IsNullOrEmpty(claim.Value))
                {
                    throw new SecurityException("Invalid SWT token. All claims must have a value.");
                }

                if (claim.Value.IndexOf(delimiter) >= 0)
                {
                    string[] values = claim.Value.Split(delimiter);

                    foreach (string value in values)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            throw new SecurityException("Invalid SWT token. All claims must have a value.");
                        }
                        else
                        {
                            decodedClaims.Add(new Claim(claim.Key, value, ClaimValueTypes.String, issuer));
                        }
                    }
                }
                else
                {
                    decodedClaims.Add(new Claim(claim.Key, claim.Value, ClaimValueTypes.String, issuer));
                }
            }

            return decodedClaims;
        }

        /// <summary>
        /// Decodes the expiry time.
        /// </summary>
        /// <param name="expiry">The expiry time in unix format.</param>
        /// <returns>
        /// A DateTime value which contains the equivalent date time in UTC.
        /// </returns>
        private static DateTime DecodeExpiry(string expiry)
        {
            if (expiry == null)
                return DateTime.MaxValue;

            long totalSeconds = 0;
            if (!long.TryParse(expiry, out totalSeconds))
            {
                throw new SecurityException("The Date format for expiry is unrecognized.");
            }

            long maxSeconds = (long)((DateTime.MaxValue - SimpleWebTokenConstants.BaseTime).TotalSeconds) - 1;

            if (totalSeconds > maxSeconds)
            {
                totalSeconds = maxSeconds;
            }

            return SimpleWebTokenConstants.BaseTime + TimeSpan.FromSeconds(totalSeconds);
        }

        /// <summary>
        /// Encodes the claims to create the raw token string.
        /// </summary>
        /// <returns>
        /// A string which contains the encoded token.
        /// </returns>
        private string Encode()
        {
            IDictionary<string, string> claims = EncodeClaims(this.tokenClaims, SimpleWebTokenConstants.DefaultCompoundClaimDelimiter);

            if (!string.IsNullOrEmpty(this.tokenAudience))
                claims[SimpleWebTokenConstants.TokenAudience] = this.tokenAudience;

            if (this.tokenExpiryTime != DateTime.MaxValue)
                claims[SimpleWebTokenConstants.TokenExpiresOn] = EncodeExpiry(this.tokenExpiryTime);

            if (!string.IsNullOrEmpty(this.tokenIssuer))
                claims[SimpleWebTokenConstants.TokenIssuer] = this.tokenIssuer;

            var encodedClaims = new StringBuilder(Encode(claims));

            // According to the SWT spec, the signature is always last.
            if (!string.IsNullOrEmpty(this.tokenSignature))
            {
                encodedClaims.AppendFormat("&{0}={1}", SimpleWebTokenConstants.TokenDigest256, HttpUtility.UrlEncode(this.tokenSignature));
            }

            return encodedClaims.ToString();
        }

        /// <summary>
        /// Claims can contain multiple claims with the same claim type.  This method takes
        /// the values and combine them into a single output claim, so foo=bar and foo=blah will
        /// be returned as a single claim in the output foo=bar,blah (if the delimiter is comma).
        /// </summary>
        /// <param name="claims">The claims to be encoded.</param>
        /// <param name="delimiter">The delimiter for compound claim values.</param>
        /// <returns>
        /// A dictionary of claims.
        /// </returns>
        private static IDictionary<string, string> EncodeClaims(IEnumerable<Claim> claims, char delimiter)
        {
            if (claims == null)
                throw new ArgumentNullException("claims");

            // SWT and Shared Secret claim names are case-sensitive
            var outputClaims = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (Claim claim in claims)
            {
                // aggregate duplicate claims types into a list
                if (outputClaims.ContainsKey(claim.Type))
                {
                    outputClaims[claim.Type] = outputClaims[claim.Type] + delimiter + claim.Value;
                }
                else
                {
                    outputClaims[claim.Type] = claim.Value;
                }
            }

            return outputClaims;
        }

        /// <summary>
        /// Encodes the expiry time for the token to unix format.
        /// </summary>
        /// <param name="expiry">The expiry time for the token.</param>
        /// <returns>
        /// A string which contains the expiry time in unix format.
        /// </returns>
        private static string EncodeExpiry(DateTime expiry)
        {
            if (expiry < SimpleWebTokenConstants.BaseTime)
                throw new ArgumentException("expiry is before base time of 1970-01-01T00:00:00Z");

            TimeSpan expiryTime = expiry - SimpleWebTokenConstants.BaseTime;

            // the WRAP protocol is expecting an integer expiry time
            return ((long)expiryTime.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the different claim attributes from the token string.
        /// </summary>
        /// <param name="encodedTokenString">The token string.</param>
        /// <returns>
        /// A dictionay which contains the attributes.
        /// </returns>
        private static Dictionary<string, string> GetAttributesFromTokenString(string encodedTokenString)
        {
            if (encodedTokenString == null) throw new ArgumentNullException("encodedTokenString");

            var tokenComponents = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (string nameValue in encodedTokenString.Split('&'))
            {
                string[] keyValueArray = nameValue.Split('=');

                if ((keyValueArray.Length == 1 || keyValueArray.Length > 2)
                    && !string.IsNullOrEmpty(keyValueArray[0]))
                {
                    throw new ArgumentException("The request is not properly formatted.", "encodedTokenString");
                }

                if (keyValueArray.Length != 2)
                {
                    throw new ArgumentException("The request is not properly formatted.", "encodedTokenString");
                }

                string key = HttpUtility.UrlDecode(keyValueArray[0].Trim());
                string value = HttpUtility.UrlDecode(keyValueArray[1].Trim().Trim('"'));

                tokenComponents.Add(key, value);
            }

            return tokenComponents;
        }

        /// <summary>
        /// Encodes the claim attributes to a string.
        /// </summary>
        /// <param name="attributes">The claim attributes.</param>
        /// <returns>
        /// The claims encoded in string format.
        /// </returns>
        private static string Encode(IEnumerable<KeyValuePair<string, string>> attributes)
        {
            var result = new StringBuilder();

            foreach (KeyValuePair<string, string> attribute in attributes)
            {
                if (result.Length != 0)
                    result.Append('&');

                result.AppendFormat("{0}={1}", HttpUtility.UrlEncode(attribute.Key), HttpUtility.UrlEncode(attribute.Value));
            }

            return result.ToString();
        }
    }
}