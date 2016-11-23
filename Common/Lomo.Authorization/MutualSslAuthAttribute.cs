//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Lomo.AssemblyUtils;
    using Lomo.Logging;

    /// <summary>
    ///     An IApiAuthAttribute implementation for mutual SSL.
    /// </summary>
    public class MutualSslAuthAttribute : AuthorizationFilterAttribute, IApiAuthAttribute
    {
        #region Fields

        /// <summary>
        /// The mutual SSL auth attribute instance.
        /// </summary>
        private readonly IApiAuthAttribute mutualSslAuthAttributeInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the MutualSslAuthAttribute class.
        /// </summary>
        /// <param name="role">
        /// The single role for which the client must be authorized.
        /// </param>
        public MutualSslAuthAttribute(string role)
            : this(new string[] { role })
        {
        }

        /// <summary>
        /// Initializes a new instance of the MutualSslAuthAttribute class.
        /// </summary>
        /// <param name="roles">
        /// The list roles. Client must be authorized for one of these roles.
        /// </param>
        public MutualSslAuthAttribute(string[] roles)
            : this()
        {
            this.Roles = roles;
        }

        /// <summary>
        /// Initializes a new instance of the MutualSslAuthAttribute class.
        /// </summary>
        public MutualSslAuthAttribute()
        {
            // If configuration app settings do not include an assembly from which to create a mock MutualSslAuthAttribute instance,
            // use this object. Otherwise, create an instance of the mock class.
            string mockApiAuthAssembly = ConfigurationManager.AppSettings["mockApiAuth"];
            if (string.IsNullOrWhiteSpace(mockApiAuthAssembly))
            {
                this.mutualSslAuthAttributeInstance = this;
            }
            else
            {
                this.mutualSslAuthAttributeInstance =
                    LateBinding.BuildObjectFromLateBoundAssembly<IApiAuthAttribute>(
                        "MutualSslAuthAttribute", LateBinding.GetLateBoundAssemblyTypes(mockApiAuthAssembly));
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
            Log.Verbose("MutualSslAuthAttribute.OnAuthorization");

            // Attempt to get credentials from the the client certificate.
            SecurityCredentials credentials = this.GetCredentialsFromClientCertificate(actionContext);

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
                else
                {
                    Log.Warn("Unable to build auth payload while authenticating credentials.");
                }
            }
            else
            {
                Log.Warn("Unable to build credentials from client certificate.");
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
                this.mutualSslAuthAttributeInstance.AuthorizeUser(actionContext);
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
        /// The get user credentials from the client certificate.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="SecurityCredentials"/>.
        /// </returns>
        private SecurityCredentials GetCredentialsFromClientCertificate(HttpActionContext actionContext)
        {
            SecurityCredentials credentials = null;

            X509Certificate2 certificate = actionContext.Request.GetClientCertificate();
            if (certificate != null)
            {
                credentials = new SecurityCredentials();
                credentials.Token = certificate.Thumbprint;
                credentials.SecurityProviderName = MutualSslSecurityProvider.Name;
                credentials.Name = certificate.Subject;
            }

            return credentials;
        }

        #endregion
    }
}