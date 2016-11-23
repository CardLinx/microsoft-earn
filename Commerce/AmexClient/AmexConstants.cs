//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    /// <summary>
    /// Constants for Amex.
    /// </summary>
    public static class AmexConstants
    {
        /// <summary>
        /// Code for the Amex language field
        /// </summary>
        public const string LanguageCode = "en";

        /// <summary>
        /// Country code
        /// </summary>
        public const string CountryCode = "US";

        /// <summary>
        /// Language code
        /// </summary>
        public const string Currency = "USD";

        /// <summary>
        /// Partner Id
        /// </summary>
        public const string PartnerId = "TODO_PARTNER_ID_HERE";

        /// <summary>
        /// Distribution channel
        /// </summary>
        public const string DistributionChannel = "9994";

        /// <summary>
        /// Message Id
        /// </summary>
        public const string MessageId = "HS256";

        /// <summary>
        /// Header Record Identifier
        /// </summary>
        public const string HeaderIdentifier = "H";

        /// <summary>
        /// Detail Record Identifier
        /// </summary>
        public const string DetailIdentifier = "D";

        /// <summary>
        /// Trailer Record Identifier
        /// </summary>
        public const string TrailerIdentifier = "T";

        /// <summary>
        /// Partner To Amex code
        /// </summary>
        public const string FromTo = "P2A";

        /// <summary>
        /// SFTP file type code
        /// </summary>
        public const string FileType = "10";

        /// <summary>
        /// Delimiter in SFTP files
        /// </summary>
        public const string Delimiter = "|";

        /// <summary>
        /// Amex Discount Indicator
        /// </summary>
        public const string DiscountIndicator = "C";

        /// <summary>
        /// Key under which we find thumbprint for amex cert
        /// </summary>
        public const string AmexEncryptionCertThumbprintKey = "Amex.EncryptionCertificate.Thumbprint";

        /// <summary>
        /// Key under which we find thumbprint for amex cert
        /// </summary>
        public const string CommerceClientCertThumbprintKey = "Commerce.Client.Certificate.Thumbprint";

        /// <summary>
        /// Key under which we find thumbprint for decryption cert
        /// </summary>
        public const string CommerceDecryptionCertThumbprintKey = "Commerce.Decryption.Certificate.Thumbprint";
        
        /// <summary>
        /// Key under which we find Amex's add card endpoint URI
        /// </summary>
        public const string AmexAddCardUri = "Amex.AddCardUri";

        /// <summary>
        /// Key under which we find Amex's remove card endpoint URI
        /// </summary>
        public const string AmexRemoveCardUri = "Amex.RemoveCardUri";

        /// <summary>
        /// Key under which we find ACS OAuth 2 URI
        /// </summary>
        public const string AmexAcsOAuthUri = "Amex.AcsOAuthUri";

        #region OAuth

        /// <summary>
        /// Key under which we find Amex's OAuth Uri
        /// </summary>
        public const string AmexOAuthUri = "Amex.OAuthUri";

        /// <summary>
        /// Key under which we find Amex's OAuth Client Id
        /// </summary>
        public const string AmexOAuthClientId = "Amex.OAuthClientId";

        /// <summary>
        /// Key under which we find Amex's OAuth Client Secret
        /// </summary>
        public const string AmexOAuthClientSecret = "Amex.OAuthClientSecret";

        #endregion
    }
}