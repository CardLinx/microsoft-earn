//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    /// <summary>
    /// Interface for classes that provide functionality to request a simple web token for a specified resource.
    /// </summary>
    public interface ISimpleWebTokenRequestor
    {
        /// <summary>
        /// Requests a token for the specified resource using the specified credentials.
        /// </summary>
        /// <param name="clientName">
        /// The name of the client requesting a token to access the resource.
        /// </param>
        /// <param name="password">
        /// The password for the client requesting a token to access the resource.
        /// </param>
        /// <param name="resourceNamespace">
        /// The namespace to which the resource belongs.
        /// </param>
        /// <param name="resource">
        /// The resource for which an access token is being requested, i.e. the resource URI.
        /// </param>
        /// <returns>
        /// * The requested token, if successful.
        /// * Else returns null.
        /// </returns>
        string RequestToken(string clientName,
                            string password,
                            string resourceNamespace,
                            string resource);
    }
}