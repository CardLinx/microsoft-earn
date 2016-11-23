//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using Lomo.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    ///     The security.
    /// </summary>
    public class Security
    {
        /// <summary>
        ///     The anonymous id.
        /// </summary>
        public static Guid AnonymousId = new Guid("9e0e5731-11bd-4f6f-8ac9-6e58151a5f03");

        /// <summary>
        ///     The anonymous name.
        /// </summary>
        private const string AnonymousName = "Anonymous";

        /// <summary>
        ///     The authorizers.
        /// </summary>
        private static readonly Dictionary<string, ISecurityProvider> authorizers = new Dictionary<string, ISecurityProvider>();

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="authorizerName">
        /// The SecurityProvider name.
        /// </param>
        /// <param name="securityProvider">
        /// The SecurityProvider.
        /// </param>
        public static void Add(string authorizerName, ISecurityProvider securityProvider)
        {
            authorizers.Add(authorizerName, securityProvider);
        }

        /// <summary>
        /// The authenticate.
        /// </summary>
        /// <param name="credentials">
        /// The user credentials.
        /// </param>
        /// <param name="flags">the flags</param>
        /// <returns>
        /// The auth payload
        /// </returns>
        internal static AuthPayload Authenticate(SecurityCredentials credentials, HashSet<string> flags = null)
        {
            AuthPayload result = null;

            if (authorizers.ContainsKey(credentials.SecurityProviderName) == true)
            {
                ISecurityProvider securityProvider = authorizers[credentials.SecurityProviderName];
                result = securityProvider.Authenticate(credentials, flags);
            }
            else
            {
                Log.Warn("Unregistered security provider was specified: {0}", credentials.SecurityProviderName);
            }

            return result;
        }

        /// <summary>
        /// The authorize.
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
        internal static bool Authorize(AuthPayload payload, string[] roles)
        {
            if (payload != null)
            {
                CustomIdentity identity = payload.CustomIdentity;
                ISecurityProvider securityProvider = authorizers[identity.SecurityProviderName];
                bool isAuthorized = securityProvider.IsAuthorized(payload, roles);

                if (isAuthorized)
                {
                    string userName = identity.Name;
                    string providerName = identity.SecurityProviderName;

                    Thread.CurrentPrincipal =
                        new CustomPrincipal(identity, roles);

                    Log.Verbose("User Id {0} is authorized", identity.UserId);

                    return true;
                }
                else
                {
                    Log.Verbose("Anauthorized user");
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// The set anonymous.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        internal static void SetAnonymous(string[] roles)
        {
            Thread.CurrentPrincipal =
                new CustomPrincipal(new CustomIdentity(AnonymousId, AnonymousName, "local",true), roles);
        }

        #region Public Methods and Operators

        /// <summary>
        /// The get user id.
        /// </summary>
        /// <param name="allowAnonymous"> The allow Anonymous. </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">Custom identity doesn't exists</exception>
        /// <exception cref="InvalidOperationException"> The anonymous identity is set but <para>allowAnonymous</para> is false  </exception>
        public static Guid GetUserId(bool allowAnonymous = false)
        {
            try
            {
                var identity = Thread.CurrentPrincipal.Identity as CustomIdentity;
                if (identity == null)
                {
                    throw new UnauthorizedAccessException("Unauthorized. Missing Custom Identity.");
                }
                Guid userId = identity.UserId;
                if (!allowAnonymous && userId == AnonymousId)
                {
                    throw new InvalidOperationException("User Must be authnticated");
                }
                return userId;
            }
            catch (Exception e)
            {
                Log.Error(e,"Couldn't get user id from identity. ");
                throw e;
            }
        }

        #endregion
    }
}