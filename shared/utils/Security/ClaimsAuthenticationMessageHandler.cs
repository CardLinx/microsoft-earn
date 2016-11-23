//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Common.Utils;

    /// <summary>
    /// Provides authentication for a service.
    /// </summary>
    public class ClaimsAuthenticationMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// Maintains a lookup table of authentication schemes and the associated authentication handler.
        /// </summary>
        private readonly IReadOnlyDictionary<string, IAuthenticationHandler> authenticationHandlerDictionary;

        /// <summary>
        /// Initializes a new instance of
        /// </summary>
        /// <param name="authenticationHandlers">
        /// The collection of authentication handlers to use. Only one handler per scheme is allowed.
        /// </param>
        public ClaimsAuthenticationMessageHandler(IEnumerable<IAuthenticationHandler> authenticationHandlers)
        {
            if (authenticationHandlers == null)
            {
                throw new ArgumentNullException("authenticationHandlers");
            }

            // Scheme names are case insensitive.
            var authHandlers = new Dictionary<string, IAuthenticationHandler>(StringComparer.OrdinalIgnoreCase);
            foreach (var authenticationHandler in authenticationHandlers)
            {
                // Make sure caller is not trying to pass in multiple schemes separated by commas.
                if (authenticationHandler.AuthenticationScheme.Contains(","))
                {
                    throw new ArgumentException(
                        "The scheme name '{0}' contains comma which is not allowed.".FormatInvariant(
                            authenticationHandler.AuthenticationScheme));
                }

                // Only one handler per scheme is allowed.
                if (authHandlers.ContainsKey(authenticationHandler.AuthenticationScheme))
                {
                    throw new ArgumentException(
                        "More than one authentication handler was specified for the scheme '{0}'".FormatInvariant(
                            authenticationHandler.AuthenticationScheme));
                }

                authHandlers[authenticationHandler.AuthenticationScheme] = authenticationHandler;
            }

            this.authenticationHandlerDictionary = authHandlers;
        }

        /// <summary>
        /// Authenticates the message.
        /// </summary>
        /// <param name="request">The current request instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The response for the request.
        /// </returns>
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            IAuthenticationHandler authenticationHandler = null;
            AuthenticationResult authenticationResult = null;

            if (request.Headers.Authorization != null && this.IsAuthenticationHandlerAvailableForScheme(request.Headers.Authorization.Scheme))
            {
                authenticationHandler = this.authenticationHandlerDictionary[request.Headers.Authorization.Scheme];

                authenticationResult = authenticationHandler.Authenticate(request);

                this.LogAuthenticationResult(authenticationResult);

                if (authenticationResult.IsAuthenticated)
                {
                    this.SetPrincipal(request, authenticationResult.Principal);
                }
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            // Try to suppress response content when the cancellation token has fired; ASP.NET will log to the Application event log if there's content in this case.
            if (cancellationToken.IsCancellationRequested)
            {
                response = new HttpResponseMessage(response.StatusCode);
            }

            // if the authentication result contains an explicit return status then create the response based on that
            if (authenticationResult != null && authenticationResult.ReturnStatus.HasValue)
            {
                response = new HttpResponseMessage(authenticationResult.ReturnStatus.Value);
                response.Content = new StringContent(authenticationResult.ErrorMessage);
            }

            if (authenticationHandler != null)
            {
                authenticationHandler.HandleResponse(request, response, authenticationResult.Principal);
            }

            return response;
        }

        /// <summary>
        /// Logs the authentication result.
        /// </summary>
        /// <param name="authenticationResult">
        /// The authentication result to be logged.
        /// </param>
        private void LogAuthenticationResult(AuthenticationResult authenticationResult)
        {
        }

        /// <summary>
        /// Checks if an authentication handler has been registered for the specified scheme.
        /// </summary>
        /// <param name="scheme">The authentication scheme.</param>
        /// <returns>
        /// True if an authenticaiton handler is available, false otherwise.
        /// </returns>
        private bool IsAuthenticationHandlerAvailableForScheme(string scheme)
        {
            return this.authenticationHandlerDictionary.ContainsKey(scheme);
        }

        /// <summary>
        /// Set the principal for the current call context.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="principal">The principal instance.</param>
        private void SetPrincipal(HttpRequestMessage request, IPrincipal principal)
        {
            if (principal == null) return;

            Thread.CurrentPrincipal = principal;

            var context = request.GetRequestContext();

            if (context != null)
            {
                context.Principal = principal;
            }
        }
    }
}