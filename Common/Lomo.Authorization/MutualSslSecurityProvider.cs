//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The mutual SSL security provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// The mutual SSL security provider.
    /// </summary>
    public class MutualSslSecurityProvider : ISecurityProvider
    {
        #region Public Constants

        /// <summary>
        /// The name of the mutual SSL security provider.
        /// </summary>
        public const string Name = "MutualSsl";

        #endregion

        #region Private members

        /// <summary>
        /// The registered thumbprints.
        /// </summary>
        private static readonly Dictionary<string,string> registeredThumbprints;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="MutualSslSecurityProvider"/> class.
        /// </summary>
        static MutualSslSecurityProvider()
        {
            registeredThumbprints = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            foreach(string key in AuthorizationConfig.Instance.CertificateMap.AllKeys)
            {
                registeredThumbprints[AuthorizationConfig.Instance.CertificateMap[key].Value] = key;
            }
        }

        #endregion

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
        public AuthPayload Authenticate(SecurityCredentials credentials, HashSet<string> flags = null)
        {
            AuthPayload result = null;

            // If the client certificate thumbprint is one of the registered thumbprints, create a CustomIdentity object for it.
            if (registeredThumbprints.ContainsKey(credentials.Token))
            {
                result = new AuthPayload(new CustomIdentity(credentials.Token, credentials.Name, credentials.SecurityProviderName));
            }

            return result;
        }

        /// <summary>
        /// The is authorized.
        /// </summary>
        /// <param name="payload">
        /// The payload.
        /// </param>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsAuthorized(AuthPayload payload, string[] roles)
        {
            bool result = false;

            foreach(string role in roles)
            {
                KeyValueConfigurationElement roleElement = AuthorizationConfig.Instance.CertificateMap[role];
                if (roleElement != null)
                {
                    string[] thumbprints = roleElement.Value.Split(',');
                    if (thumbprints.Contains(payload.CustomIdentity.PresentedClientToken, StringComparer.OrdinalIgnoreCase) == true)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}