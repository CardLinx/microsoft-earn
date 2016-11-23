//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System.Collections.Generic;

    /// <summary>
    /// The authentication and authorization payload.
    /// </summary>
    public class AuthPayload
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthPayload"/> class.
        /// </summary>
        public AuthPayload(CustomIdentity customIdentity)
        {
            CustomIdentity = customIdentity;
            CredentialAuthorizationParameters = new Dictionary<string,string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the custom identity.
        /// </summary>
        public CustomIdentity CustomIdentity { get; private set; }

        /// <summary>
        /// Gets or sets parameters passed with credentials used within authorization.
        /// </summary>
        public Dictionary<string,string> CredentialAuthorizationParameters { get; private set; }

        #endregion
    }
}