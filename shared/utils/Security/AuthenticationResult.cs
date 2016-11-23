//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System.Net;
    using System.Security.Principal;

    using global::Common.Utils;

    /// <summary>
    /// Represents the result of an authentication operation.
    /// It tells the caller whether authentication succeeded and
    ///     1. if suceeded, what is the authenticated IPrincipal.
    ///     2. if failed, what is the reason for failure.
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Creates an AuthenticationResult that represents a successful authentication.
        /// </summary>
        /// <param name="principal">The authenticated Principal.</param>
        /// <returns>
        /// An AuthenticationResult instance.
        /// </returns>
        public static AuthenticationResult CreateSuccessfulAuthenticationResult(IPrincipal principal)
        {
            return new AuthenticationResult() { ErrorMessage = null, IsAuthenticated = true, Principal = principal };
        }

        /// <summary>
        /// Creates an AuthenticationResult that represents a failed authentication.
        /// </summary>
        /// <param name="failedReasonFormat">The reason for the authentication failure.</param>
        /// <param name="replacements">The format replacements for the failure reason.</param>
        /// <returns>
        /// An AuthenticationResult instance.
        /// </returns>
        public static AuthenticationResult CreateFailedAuthenticationResult(string failedReasonFormat, params object[] replacements)
        {
            return new AuthenticationResult()
            {
                ErrorMessage = failedReasonFormat.FormatInvariant(replacements),
                IsAuthenticated = false,
                Principal = null
            };
        }

        /// <summary>
        /// Creates an AuthenticationResult that represents a failed authentication containing exclusive HTTP status code.
        /// </summary>
        /// <param name="status">The HTTP status that should be returned to the caller</param>
        /// <param name="failedReasonFormat">The reason for the authentication failure.</param>
        /// <param name="replacements">The format replacements for the failure reason.</param>
        /// <returns>
        /// An AuthenticationResult instance.
        /// </returns>
        public static AuthenticationResult CreateFailedAuthenticationResult(HttpStatusCode status, string failedReasonFormat, params object[] replacements)
        {
            var result = CreateFailedAuthenticationResult(failedReasonFormat, replacements);
            result.ReturnStatus = status;

            return result;
        }

        /// <summary>
        /// Construction should happen only through the factory methods, so default constructor is private.
        /// </summary>
        private AuthenticationResult()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the authentication 
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Gets the authenticated principal. This will be null if the authentication failed.
        /// </summary>
        public IPrincipal Principal { get; private set; }

        /// <summary>
        /// The error message if authentication failed.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Gets the exclusive HTTP status code that needs to be returned to the caller
        /// </summary>
        public HttpStatusCode? ReturnStatus { get; private set; }
    }
}