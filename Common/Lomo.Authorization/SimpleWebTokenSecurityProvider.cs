//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The simple web token security provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Lomo.Authentication;

    /// <summary>
    /// The simple web token security provider.
    /// </summary>
    public class SimpleWebTokenSecurityProvider : ISecurityProvider
    {
        #region Public Constants

        /// <summary>
        /// The name of the simple web token security provider.
        /// </summary>
        public const string Name = "SimpleWebToken";

        #endregion

        #region Private properties

        /// <summary>
        /// Gets or sets the namespace in which the resources being accessed can be found.
        /// </summary>
        private string ResourceNamespace { get; set; }

        /// <summary>
        /// Gets or sets the template for the resource used during validation.
        /// </summary>
        private string ResourceTemplate { get; set; }

        /// <summary>
        /// Gets or sets the trusted key that must be used to sign valid tickets.
        /// </summary>
        private string TrustedSigningKey { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SimpleWebTokenSecurityProvider class.
        /// </summary>
        /// <param name="resourceNamespace">
        /// The namespace in which the resources being accessed can be found.
        /// </param>
        /// <param name="resourceTemplate">
        /// The template for the resource used during validation.
        /// </param>
        /// <param name="trustedSigningKey">
        /// The trusted key that must be used to sign valid tickets.
        /// </param>
        public SimpleWebTokenSecurityProvider(string resourceNamespace,
                                              string resourceTemplate,
                                              string trustedSigningKey)
        {
            ResourceNamespace = resourceNamespace;
            ResourceTemplate = resourceTemplate;
            TrustedSigningKey = trustedSigningKey;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The authenticate.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="flags">A general purpose flags set</param>
        /// <returns>
        /// The <see cref="AuthPayload"/>.
        /// </returns>
        public AuthPayload Authenticate(SecurityCredentials credentials, HashSet<string> flags = null)
        {
            AuthPayload result = null;

            if (SimpleWebTokenValidator.Validate(credentials.Token, ResourceNamespace, String.Format(ResourceTemplate, credentials.Name),
                                                 TrustedSigningKey) == true)
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

            Dictionary<string, string> parameters =
                                     SimpleWebTokenValidator.ExtractTokenProperties(payload.CustomIdentity.PresentedClientToken);
            if (String.Equals(parameters["Audience"],
                              String.Format(ResourceTemplate, roles[0]), StringComparison.OrdinalIgnoreCase) == true)
            {
                result = true;
            }

            return result;
        }

        #endregion
    }
}