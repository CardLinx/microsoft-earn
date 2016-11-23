//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The user debug security provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Lomo.Authentication.Tokens;

    /// <summary>
    /// The user token security provider.
    /// </summary>
    public class UserTokenSecurityProvider : ISecurityProvider
    {
        #region Public Methods and Operators

        /// <summary>
        /// The authenticate.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="flags">
        /// A general purpose flags set
        /// </param>
        /// <returns>
        /// The <see cref="AuthPayload"/>.
        /// </returns>
        public AuthPayload Authenticate(SecurityCredentials credentials, HashSet<string> flags = null)
        {
            AuthPayload result = null;

            if (credentials.Token != null)
            {
                LomoUserIdSecurityToken token = new LomoUserIdSecurityToken(
                                           credentials.Token, 
                                           ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenSigningKey],
                                           ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenPassword],
                                           ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenSalt],
                                           Convert.ToUInt64(ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenClockSkew]));

                var identity = new CustomIdentity(new Guid(token.UserId), null, credentials.SecurityProviderName);
                result = new AuthPayload(identity);
                result.CredentialAuthorizationParameters[Resource] = token.Resource;
                result.CredentialAuthorizationParameters[Action] = token.Action;
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

            if (roles.Length > 0)
            {
                // Create dictionary from the specified roles.
                Dictionary<string,string> authParameters = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
                foreach (string role in roles)
                {
                    string[] segments = role.Split(':');
                    if (segments.Length == 2)
                    {
                        authParameters[segments[0]] = segments[1];
                    }
                }

                // Ensure the auth parameters match.
                if (authParameters.ContainsKey(Resource) == true &&
                    authParameters.ContainsKey(Action) == true)
                {
                    string tokenResource = payload.CredentialAuthorizationParameters[Resource];
                    string tokenAction = payload.CredentialAuthorizationParameters[Action];
                    if (authParameters[Resource].Equals(tokenResource, StringComparison.OrdinalIgnoreCase) == true &&
                        authParameters[Action].Equals(tokenAction, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        #endregion

        #region public constants

        /// <summary>
        /// The key under which the resource specified within a token can be found.
        /// </summary>
        public const string Resource = "Resource";

        /// <summary>
        /// The key under which the action specified within a token can be found.
        /// </summary>
        public const string Action = "Action";

        #endregion
    }
}