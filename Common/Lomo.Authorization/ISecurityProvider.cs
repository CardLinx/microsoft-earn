//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System.Collections.Generic;

    /// <summary>
    /// The SecurityProvider interface.
    /// </summary>
    public interface ISecurityProvider
    {
        #region Public Methods and Operators

        /// <summary>
        /// The authenticate.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="flags">A general purpose flags set </param>
        /// <returns>
        /// The <see cref="AuthPayload"/>.
        /// </returns>
        AuthPayload Authenticate(SecurityCredentials credentials, HashSet<string> flags = null);

        /// <summary>
        /// The is authorized.
        /// </summary>
        /// <param name="payload">
        /// The profile.
        /// </param>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsAuthorized(AuthPayload payload, string[] roles);

        #endregion
    }
}