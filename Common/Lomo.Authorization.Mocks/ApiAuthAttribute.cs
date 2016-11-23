//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading;
    using System.Web.Http.Controllers;
    using Lomo.Authentication.Tokens;

    /// <summary>
    /// A mock implementation of the IApiAuthAttribute interface.
    /// </summary>
    public class ApiAuthAttribute : IApiAuthAttribute
    {
        #region Public Methods and Operators

        /// <summary>
        /// Attempts to authorize the user for the request.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public void AuthorizeUser(HttpActionContext actionContext)
        {
//TODO: This implements enough mock functionality to populate the security context. Add support for more scenarios as needed.
            if (actionContext.Request.Headers != null && actionContext.Request.Headers.Authorization != null)
            {
                string scheme = actionContext.Request.Headers.Authorization.Scheme;
                string parameter = actionContext.Request.Headers.Authorization.Parameter;
                Tuple<string, string> key = new Tuple<string, string>(scheme, parameter);
                if (users.ContainsKey(key) == false)
                {
                    Guid userId;
                    if (scheme.Equals("usertoken", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        LomoUserIdSecurityToken token = new LomoUserIdSecurityToken(
                                       parameter,
                                       ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenSigningKey],
                                       ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenPassword],
                                       ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenSalt],
                                       Convert.ToUInt64(ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenClockSkew]));
                        userId = new Guid(token.UserId);
                    }
                    else
                    {
                        userId = Guid.NewGuid();
                    }

                    users[key] = new MockUser { Id = userId, Name = String.Concat("test", userId) };
                }

                MockUser user = users[key];
                Thread.CurrentPrincipal = new CustomPrincipal(new CustomIdentity(user.Id, user.Name, scheme), null);
            }
        }

        #endregion
        
        #region Public fields

        public static Dictionary<Tuple<string, string>, MockUser> users = new Dictionary<Tuple<string, string>, MockUser>();

        #endregion
    }

    /// <summary>
    /// A private mock user class.
    /// </summary>
    public class MockUser
    {
        #region Public properties

        /// <summary>
        /// The Id of the mock user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Name of the mock user.
        /// </summary>
        public string Name { get; set; }

        #endregion Public properties
    }
}