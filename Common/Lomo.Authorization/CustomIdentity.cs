//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System;
    using System.Security.Principal;

    /// <summary>
    /// The custom identity.
    /// </summary>
    public class CustomIdentity : IIdentity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomIdentity"/> class
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="securityProviderName">The security provider name.</param>
        /// <param name="externalUserId"> the external id</param>
        public CustomIdentity(Guid userId, string userName, string securityProviderName, string externalUserId = null)
            : this(userId, userName, securityProviderName, false, externalUserId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomIdentity"/> class
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="securityProviderName">The security provider name.</param>
        /// <param name="isAnonymous">whether user is anonymous</param>
        /// <param name="externalUserId">the external id</param>
        public CustomIdentity(Guid userId, string userName, string securityProviderName, bool isAnonymous, string externalUserId = null)
        {
            UserId = userId;
            Name = userName;
            SecurityProviderName = securityProviderName;
            IsAnonymous = isAnonymous;
            ExternalUserId = externalUserId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomIdentity"/> class.
        /// </summary>
        /// <param name="presentedClientToken">
        /// The token presented by the calling client.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="securityProviderName">
        /// The security provider name.
        /// </param>
        public CustomIdentity(string presentedClientToken, string userName, string securityProviderName)
        {
            PresentedClientToken = presentedClientToken;
            Name = userName;
            SecurityProviderName = securityProviderName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomIdentity"/> class.
        /// </summary>
        /// <param name="customIdentity">custom identity object</param>
        public CustomIdentity(CustomIdentity customIdentity)
        {
            UserId = customIdentity.UserId;
            Name = customIdentity.Name;
            EmailAddress = customIdentity.EmailAddress;
            ExternalUserId = customIdentity.ExternalUserId;
            IsAnonymous = customIdentity.IsAnonymous;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the security provider name.
        /// </summary>
        public string SecurityProviderName { get; protected set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public Guid UserId { get; protected set; }

        /// <summary>
        /// Gets or sets the presented client token.
        /// </summary>
        public string PresentedClientToken { get; protected set; }

        /// <summary>
        /// Gets the authentication type.
        /// </summary>
        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        /// <summary>
        /// Gets a value indicating whether is authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        public string EmailAddress { get; set; }


        /// <summary>
        /// Gets if Anonymous
        /// </summary>
        public bool IsAnonymous { get; private set; }

        /// <summary>
        /// Gets or sets the external user id
        /// </summary>
        public string ExternalUserId { get; private set; }

        #endregion
    }
}