//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication
{
    /// <summary>
    /// Lomo user account object
    /// </summary>
    public class LomoUserAccountInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LomoUserAccountInfo"/> class.
        /// </summary>
        internal LomoUserAccountInfo()
        {
            this.ProviderType = IdentityProviderType.None;
        }

        /// <summary>
        /// Gets a value indicating whether the user is authenticated successfully. 
        /// </summary>
        public bool IsAuthenticated
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the user's first name and last name combined. May be null.
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the user id of the authenticated user. 
        /// </summary>
        public string UserId 
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the email address of the user. May be null.
        /// </summary>
        public string UserEmail
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the identity provider that served the user identity.
        /// </summary>
        public IdentityProviderType ProviderType
        {
            get;
            internal set;
        }
    }
}