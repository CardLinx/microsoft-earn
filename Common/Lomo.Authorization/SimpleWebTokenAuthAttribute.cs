//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Lomo.AssemblyUtils;
    using Lomo.Authentication;
    using Lomo.Logging;

    /// <summary>
    ///     An IApiAuthAttribute implementation for simple web tokens.
    /// </summary>
    public class SimpleWebTokenAuthAttribute : AuthorizationFilterAttribute, IApiAuthAttribute
    {
        #region Fields

        /// <summary>
        /// The simple web token auth attribute instance.
        /// </summary>
        private readonly IApiAuthAttribute simpleWebTokenAuthAttributeInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the SimpleWebTokenAuthAttribute class.
        /// </summary>
        /// <param name="role">
        /// The single role for which the client must be authorized.
        /// </param>
        public SimpleWebTokenAuthAttribute(string role)
            : this(new string[] { role })
        {
        }

        /// <summary>
        /// Initializes a new instance of the SimpleWebTokenAuthAttribute class.
        /// </summary>
        /// <param name="roles">
        /// The list roles. Client must be authorized for one of these roles.
        /// </param>
        public SimpleWebTokenAuthAttribute(string[] roles)
            : this()
        {
            this.Roles = roles;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleWebTokenAuthAttribute class.
        /// </summary>
        public SimpleWebTokenAuthAttribute()
        {
            // If configuration app settings do not include an assembly from which to create a mock SimpleWebTokenAuthAttribute
            // instance, use this object. Otherwise, create an instance of the mock class.
            string mockApiAuthAssembly = ConfigurationManager.AppSettings["mockApiAuth"];
            if (string.IsNullOrWhiteSpace(mockApiAuthAssembly))
            {
                this.simpleWebTokenAuthAttributeInstance = this;
            }
            else
            {
                this.simpleWebTokenAuthAttributeInstance =
                    LateBinding.BuildObjectFromLateBoundAssembly<IApiAuthAttribute>(
                        "SimpleWebTokenAuthAttribute", LateBinding.GetLateBoundAssemblyTypes(mockApiAuthAssembly));
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets Roles.
        /// </summary>
        public string[] Roles { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Attempts to authorize the client issuing the request.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public void AuthorizeUser(HttpActionContext actionContext)
        {
            Log.Verbose("SimpleWebTokenAuthAttribute.OnAuthorization");

            // Attempt to get credentials from the the client certificate.
            SecurityCredentials credentials = this.GetCredentialsFromAuthorizationHeader(actionContext);

            bool isAuthorized = false;
            if (credentials != null)
            {
                AuthPayload payload = Security.Authenticate(credentials);
                if (payload != null)
                {
                    if (Security.Authorize(payload, this.Roles))
                    {
                        isAuthorized = true;
                        Log.Verbose("User is authorized");
                    }
                    else
                    {
                        Log.Warn("Unauthorized user");
                    }
                }
            }

            if (!isAuthorized)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        /// <summary>
        /// Called to authenticate a request
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                this.simpleWebTokenAuthAttributeInstance.AuthorizeUser(actionContext);
            }
            catch (Exception e)
            {
                Log.Error(e, string.Format("Unhandled authentication error. Request: {0}", actionContext.Request != null && actionContext.Request.RequestUri != null ? actionContext.Request.RequestUri.ToString() : string.Empty));
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get user credentials from the authorization header.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="SecurityCredentials"/>.
        /// </returns>
        private SecurityCredentials GetCredentialsFromAuthorizationHeader(HttpActionContext actionContext)
        {
            SecurityCredentials credentials = null;

            if (actionContext.Request.Headers != null && actionContext.Request.Headers.Authorization != null)
            {
                string token = actionContext.Request.Headers.Authorization.Parameter;
                credentials = new SecurityCredentials
                {
                    Token = token,
                    SecurityProviderName = actionContext.Request.Headers.Authorization.Scheme,
                    Name = Roles[0]
                };
            }

            return credentials;
        }

        #endregion
    }
}