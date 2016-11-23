//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text;

    using global::Common.Utils;

    /// <summary>
    /// Base class for authentication handlers.
    /// </summary>
    public abstract class AuthenticationHandlerBase : IAuthenticationHandler
    {
        /// <summary>
        /// The scheme supported by this authentication handler.
        /// </summary>
        public string AuthenticationScheme { get; protected set; }

        /// <summary>
        /// Initializes a new instance of AuthenticationHandlerBase class.
        /// </summary>
        /// <param name="authenticationScheme">The authentication scheme supported by this handler.</param>
        protected AuthenticationHandlerBase(string authenticationScheme)
        {
            if (string.IsNullOrWhiteSpace(authenticationScheme)) throw new ArgumentNullException("authenticationScheme");

            this.AuthenticationScheme = authenticationScheme;
        }

        /// <summary>
        /// Authenticates the request.
        /// </summary>
        /// <param name="request">The request to be authenticated.</param>
        /// <returns>
        /// An instance of AuthenticationResult which specifies if the authentication was successful or not.
        /// If successful, it will contain the IPrincipal.
        /// If not successful, it will contain the error message.
        /// </returns>
        public abstract AuthenticationResult Authenticate(HttpRequestMessage request);

        /// <summary>
        /// This method is called before the response is sent to the caller.
        /// This provides a chance for the authentication handler to modify the response.
        /// </summary>
        /// <param name="request">The http request.</param>
        /// <param name="response">The http response.</param>
        /// <param name="principal">The principal making the request. 
        /// Pass in null if the request was not authenticated.
        /// </param>
        public virtual void HandleResponse(HttpRequestMessage request, HttpResponseMessage response, IPrincipal principal)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (principal == null && response.StatusCode == HttpStatusCode.Unauthorized)
            {

            }
        }

        /// <summary>
        /// Validate the request.
        /// </summary>
        /// <param name="request">The request to be validated.</param>
        /// <param name="authenticationResult">
        /// The authentication result to be used if the validation fails.
        /// </param>
        /// <returns>
        /// True if validation was successful, false otherwise.
        /// </returns>
        protected bool ValidateRequest(
            HttpRequestMessage request,
            out AuthenticationResult authenticationResult)
        {
            if (request == null)
            {
                authenticationResult = AuthenticationResult.CreateFailedAuthenticationResult("The request is null.");
                return false;
            }

            var authenticationHeaderValue = request.Headers.Authorization;
            if (authenticationHeaderValue == null)
            {
                {
                    authenticationResult =
                        AuthenticationResult.CreateFailedAuthenticationResult("The authentication header is not present.");
                    return false;
                }
            }

            if (string.Compare(this.AuthenticationScheme, authenticationHeaderValue.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new InvalidOperationException(
                    "This authentication handler supports only authentication scheme: " + this.AuthenticationScheme);
            }

            if (string.IsNullOrWhiteSpace(authenticationHeaderValue.Parameter))
            {
                authenticationResult = AuthenticationResult.CreateFailedAuthenticationResult("The authentication header does not have a value.");
                return false;
            }

            authenticationResult = null;

            return true;
        }

        /// <summary>
        /// Logs the specified claims.
        /// </summary>
        /// <param name="claims">The claims to be logged.</param>
        protected void LogClaims(IEnumerable<Claim> claims)
        {
            if (claims == null) return;

            var builder = new StringBuilder();
            builder.AppendLine("The request has the following claims: ");

            foreach (var claim in claims)
            {
                builder.AppendLine("Claim of type '{0}' has value '{1}'.".FormatInvariant(claim.Type, claim.Value));
            }

        }
    }
}