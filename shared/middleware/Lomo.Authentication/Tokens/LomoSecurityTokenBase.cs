//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using Lomo.Authentication.Exceptions;
    using Lomo.Authentication.Helper;

 /// <summary>
    /// Base class for Lomo Security Tokens.
    /// </summary>
    public abstract class LomoSecurityTokenBase
    {
        #region private data members

        /// <summary>
        /// Collection of claims wrapped by this token.
        /// </summary>
        private NameValueCollection claims;
        
        /// <summary>
        /// Base64 encoded raw token.
        /// </summary>
        private string rawTokenBase64EncodedString;
        
        /// <summary>
        /// Encryption Password used to derive the symmetric encryption key.
        /// </summary>
        private string encryptionPassword;

        /// <summary>
        /// Salt used to derive the symmetric encryption IV.
        /// </summary>
        private string encryptionSalt;

        /// <summary>
        /// The signing key used for summetric signature.
        /// </summary>
        private string signingKey;

        /// <summary>
        /// The maxClockSkew interval in seconds to use for token's validity determination.
        /// </summary>
        private ulong maxClockSkew;

        /// <summary>
        /// Determines if the object is mutable.
        /// </summary>
        private bool isReadOnly = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LomoSecurityTokenBase"/> class.
        /// </summary>
        /// <param name="base64EncodedString">The base64 encoded token string.</param>
        /// <param name="signingKey">The signing key. Minimum length should be 8 characters.</param>
        /// <param name="decryptionPassword">The decryption password to dervice the decryption key from. Minimum length should be 8 characters.</param>
        /// <param name="decryptionSalt">The decryption salt to derive the decryption IV from. Minimum length should be 8 characters.</param>
        /// <param name="maxClockSkewInSeconds"> The maxClockSkew interval in seconds to use for token's validity determination.</param>
        public LomoSecurityTokenBase(string base64EncodedString, string signingKey, string decryptionPassword, string decryptionSalt, ulong maxClockSkewInSeconds)
        {
            this.ValidateArgumentString("base64EncodedString", base64EncodedString);
            this.ValidateArgumentString("signingKey", signingKey);
            this.ValidateArgumentString("decryptionPassword", decryptionPassword);
            this.ValidateArgumentString("decryptionSalt", decryptionSalt);
            this.ValidateSecretComplexity("signingKey", signingKey);
            this.ValidateSecretComplexity("decryptionPassword", decryptionPassword);
            this.ValidateSecretComplexity("decryptionSalt", decryptionSalt);

            if (maxClockSkewInSeconds < 0)
            {
                throw new ArgumentException("maxClockSkew needs to be non-negative");
            }

            this.maxClockSkew = maxClockSkewInSeconds;
            this.rawTokenBase64EncodedString = base64EncodedString;
            this.encryptionPassword = decryptionPassword;
            this.encryptionSalt = decryptionSalt;
            this.signingKey = signingKey;

            string[] tokenParts = this.rawTokenBase64EncodedString.Split('.');
            if (tokenParts == null || tokenParts.Length != 2 || string.IsNullOrWhiteSpace(tokenParts[0]) || string.IsNullOrWhiteSpace(tokenParts[1]))
            {
                throw new LomoSecurityTokenException("Invalid token format. Missing signature.");
            }

            byte[] encryptedTokenBytes = null;
            byte[] tokenSignatureBytes = null;

            try
            {
                encryptedTokenBytes = Convert.FromBase64String(tokenParts[0]);
                tokenSignatureBytes = Convert.FromBase64String(tokenParts[1]);
            }
            catch (FormatException)
            {
                throw new LomoSecurityTokenException("Invalid characters in token.");
            }

            if (!CryptoUtility.HMACSHA256SignatureProvider.VerifySignature(encryptedTokenBytes, tokenSignatureBytes, this.signingKey))
            {
                throw new LomoSecurityTokenException("Invalid signature.");
            }

            string plaintext = null;
            try
            {
                plaintext = CryptoUtility.RijndaelEncryptionProvider.DecryptData(encryptedTokenBytes, this.encryptionPassword, this.encryptionSalt);
            }
            catch (ArgumentException e)
            {
                throw new LomoSecurityTokenException("Invalid credentials", e);
            }
            catch (InvalidOperationException e)
            {
                throw new LomoSecurityTokenException("Invalid token format", e);
            }
            catch (CryptographicException e)
            {
                throw new LomoSecurityTokenException("Invalid token format", e);
            }

            this.InitializeFromString(plaintext);
            isReadOnly = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LomoSecurityTokenBase"/> class.
        /// </summary>
        /// <param name="issuer">The issuer of the token.</param>
        /// <param name="resource">The resource the token is meant for.</param>
        /// <param name="action">The action of the token.</param>
        /// <param name="tokenLifeTimeInSeconds">The lifetime of the token in seconds.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <param name="encryptionPassword">The encryption password to dervice the encryption key from. Minimum length should be 8 characters.</param>
        /// <param name="encryptionSalt">The encryption salt to derive the encryption IV from. Minimum length should be 8 characters.</param>
        public LomoSecurityTokenBase(string issuer, string resource, string action, long tokenLifeTimeInSeconds, string signingKey, string encryptionPassword, string encryptionSalt)
        {
            this.ValidateArgumentString("issuer", issuer);
            this.ValidateArgumentString("resource", resource);
            this.ValidateArgumentString("action", action);
            this.ValidateArgumentString("signingKey", signingKey);
            this.ValidateArgumentString("encryptionPassword", encryptionPassword);
            this.ValidateArgumentString("encryptionSalt", encryptionSalt);

            this.ValidateSecretComplexity("signingKey", signingKey);
            this.ValidateSecretComplexity("encryptionPassword", encryptionPassword);
            this.ValidateSecretComplexity("encryptionSalt", encryptionSalt);

            if (tokenLifeTimeInSeconds < 0)
            {
                throw new ArgumentException("tokenLifeTimeInSeconds has to be positive");
            }

            this.encryptionPassword = encryptionPassword;
            this.encryptionSalt = encryptionSalt;
            this.signingKey = signingKey;
            this.claims = new NameValueCollection();
            this.AddClaim(LomoClaimTypes.IssuerClaimType, issuer);
            this.AddClaim(LomoClaimTypes.ResourceClaimType, resource);
            this.AddClaim(LomoClaimTypes.ActionClaimType, action);
            this.AddClaim(LomoClaimTypes.ExpirationClaimType, DateTime.UtcNow.AddSeconds(tokenLifeTimeInSeconds).ConvertToEpochTime().ToString());
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the issuer name of the token.
        /// </summary>
        public string Issuer
        {
            get
            {
                if (this.claims != null)
                {
                    return this.claims[LomoClaimTypes.IssuerClaimType];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the resource for which the token is issued.
        /// </summary>
        public string Resource
        {
            get
            {
                if (this.claims != null)
                {
                    return this.claims[LomoClaimTypes.ResourceClaimType];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the action for which the token is issued.
        /// </summary>
        public string Action
        {
            get
            {
                if (this.claims != null)
                {
                    return this.claims[LomoClaimTypes.ActionClaimType];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the token is expired.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                if (this.claims != null)
                {
                    string expirationTimeString = this.claims[LomoClaimTypes.ExpirationClaimType];
                    ulong expirationTime = ulong.MinValue;

                    if (!string.IsNullOrEmpty(expirationTimeString) && ulong.TryParse(expirationTimeString, out expirationTime))
                    {
                        ulong currentTime = DateTime.UtcNow.ConvertToEpochTime();
                        return currentTime > expirationTime + this.maxClockSkew;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets all the claims contained within the token.
        /// </summary>
        public IEnumerable<string> AllClaimTypes
        {
            get
            {
                if (claims == null)
                {
                    return null;
                }

                return claims.AllKeys;
            }
        }

        /// <summary>
        /// Gets a claimValue if present in the token represented by the claimType, else returns null.
        /// </summary>
        /// <param name="claimType">The type of the claim.</param>
        /// <returns>Value of the claim.</returns>
        public string this[string claimType]
        {
            get
            {
                if (claims == null)
                {
                    return null;
                }

                return claims[claimType];
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds a claim to the token.
        /// </summary>
        /// <param name="claimType">Type of the claim to add.</param>
        /// <param name="claimValue">Value of the claim to add.</param>
        public void AddClaim(string claimType, string claimValue)
        {
            if (string.IsNullOrEmpty(claimType))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(claimValue))
            {
                throw new ArgumentNullException("value");
            }

            if (this.isReadOnly)
            {
                throw new InvalidOperationException("Object is immutable.");
            }

            this.claims.Add(claimType, claimValue);
        }

        /// <summary>
        /// Validates the issuername, action and resource claims of the token.
        /// </summary>
        /// <param name="issuerName">Issuer name to validate.</param>
        /// <param name="action">Action name to validate.</param>
        /// <param name="resource">Resource name to validate.</param>
        /// <returns>True if the claims within the token are valid else returns false.</returns>
        public virtual bool ValidateToken(string issuerName, string action, string resource)
        {
            if (string.IsNullOrEmpty(issuerName))
            {
                throw new ArgumentNullException("issuerName");
            }

            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentNullException("action");
            }

            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException("resource");
            }

            if (this.claims == null)
            {
                return false;
            }

            if (string.Compare(this.claims[LomoClaimTypes.IssuerClaimType], issuerName) != 0)
            {
                return false;
            }

            if (string.Compare(this.claims[LomoClaimTypes.ActionClaimType], action) != 0)
            {
                return false;
            }

            if (string.Compare(this.claims[LomoClaimTypes.ResourceClaimType], resource) != 0)
            {
                return false;
            }

            if (this.IsExpired)
            {
                return false;
            }

            return ValidateClaims();
        }

        /// <summary>
        /// Converts the token object to base64encoded string representation after signing and encrypting it.
        /// </summary>
        /// <returns>The base64encoded string representation of the token object.</returns>
        public override string ToString()
        {
            if (this.isReadOnly)
            {
                return this.rawTokenBase64EncodedString;
            }

            StringBuilder content = new StringBuilder();
            bool firstClaim = true;
            foreach (string key in this.claims.AllKeys)
            {
                if (!firstClaim)
                {
                    content.Append('&');
                }

                content.Append(key).Append('=').Append(this.claims[key]);
                firstClaim = false;
            }

            string tokenString = content.ToString();
            byte[] encryptedTokenString = CryptoUtility.RijndaelEncryptionProvider.EncryptData(tokenString, this.encryptionPassword, this.encryptionSalt);

            byte[] tokenStringSignature = CryptoUtility.HMACSHA256SignatureProvider.GetSignature(encryptedTokenString, this.signingKey);
            this.rawTokenBase64EncodedString = Convert.ToBase64String(encryptedTokenString) + "." + Convert.ToBase64String(tokenStringSignature);
            return this.rawTokenBase64EncodedString;
        }

        #endregion

        #region protected methods

        /// <summary>
        /// Validates the claims added in the subclass.
        /// </summary>
        /// <returns>Returns True if the claims in the token are valid else returns false.</returns>
        protected abstract bool ValidateClaims();

        #endregion

        #region private methods

        /// <summary>
        /// Populates the token object by deserializing the input string.
        /// </summary>
        /// <param name="plainText">Token string</param>
        private void InitializeFromString(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new LomoSecurityTokenException("Invalid token format.");
            }

            try
            {
                this.claims = HttpUtility.ParseQueryString(plainText);
            }
            catch (ArgumentNullException)
            {
                throw new LomoSecurityTokenException("Invalid token format. ");
            }
        }

        /// <summary>
        /// Validates the string arguments.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="argumentValue">Value of the argument.</param>
        private void ValidateArgumentString(string argumentName, string argumentValue)
        {
            if (string.IsNullOrEmpty(argumentValue))
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Validate the secrets' complexity.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="argumentValue">Value of the argument.</param>
        private void ValidateSecretComplexity(string argumentName, string argumentValue)
        {
            if (argumentValue.Length < 8)
            {
                throw new ArgumentException(string.Format("{0} does not meet the security requirements. Minimum length is 8 characters", argumentName));
            }
        }

        #endregion
    }
}