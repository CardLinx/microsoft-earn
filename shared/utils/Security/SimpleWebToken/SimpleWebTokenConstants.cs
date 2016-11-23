//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;

    /// <summary>
    /// The string constant related to SWT tokens.
    /// </summary>
    public static class SimpleWebTokenConstants
    {
        /// <summary>
        /// The delimiter used for separating each value of a compound claim.
        /// </summary>
        public const char DefaultCompoundClaimDelimiter = ',';

        /// <summary>
        /// The delimiter for separating claims.
        /// </summary>
        public const char ParameterSeparator = '&';

        /// <summary>
        /// The name of the audience claim.
        /// </summary>
        public const string TokenAudience = "Audience";

        /// <summary>
        /// The name of the expiry time claim.
        /// </summary>
        public const string TokenExpiresOn = "ExpiresOn";

        /// <summary>
        /// The name of the issuer claim.
        /// </summary>
        public const string TokenIssuer = "Issuer";

        /// <summary>
        /// The name of the token signature claim.
        /// </summary>
        public const string TokenDigest256 = "HMACSHA256";

        /// <summary>
        /// The unix epoch time. The expiry time in SWT tokens are encoded in unix time format.
        /// </summary>
        public static readonly DateTime BaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    }
}