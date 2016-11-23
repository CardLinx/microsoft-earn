//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System.Net.Http;
    using System.Security.Principal;

    /// <summary>
    /// Interface for authentication handlers for specific authentication schem
    /// </summary>
    public interface IAuthenticationHandler
    {
        /// <summary>
        /// The scheme supported by this authentication handler.
        /// </summary>
        string AuthenticationScheme { get; }

        /// <summary>
        /// Authenticates the http request.
        /// </summary>
        /// <param name="request">The http request to be authenticated.</param>
        /// <returns>
        /// An instance of AuthenticationResult which specifies if the authentication was successful or not.
        /// If successful, it will contain the IPrincipal.
        /// If not successful, it will contain the error message.
        /// </returns>
        AuthenticationResult Authenticate(HttpRequestMessage request);

        /// <summary>
        /// This method is called before the response is sent to the caller.
        /// Provides a chance for the authentication handler to modify the response 
        /// or add headers to it.
        /// </summary>
        /// <param name="request">The http request.</param>
        /// <param name="response">The http response.</param>
        /// <param name="principal">
        /// The principal associated with the request. Will be null if the request was not authenticated successfully.
        /// </param>
        void HandleResponse(HttpRequestMessage request, HttpResponseMessage response, IPrincipal principal);
    }
}