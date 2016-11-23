//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using Lomo.AssemblyUtils;
    using Lomo.Logging;

    /// <summary>
    ///     Implements the ApiAuth attribute.
    ///     SecurityCredentials: These are the values extracted from the request headers. They contain an external id
    ///     that must be authenticated.
    ///     CustomeIdentity: Authenticated Security Crendentials are converetd into a Custome Identity. These represent an authenticated user\app.
    /// </summary>
    public class ApiAuthAttribute : AuthorizationFilterAttribute, IApiAuthAttribute
    {
        #region Constants

        /// <summary>
        ///     The bing auhtentication header.
        /// </summary>
        private const string BingAuhtenticationHeader = "X-FD-BingIDToken"; // we need to investigate

        #endregion

        #region Fields

        /// <summary>
        /// The api auth attribute instance.
        /// </summary>
        private readonly IApiAuthAttribute apiAuthAttributeInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the ApiAuthAttribute class.
        /// </summary>
        /// <param name="role">
        /// The single role for which the user must be authorized.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        public ApiAuthAttribute(string role, string[] flags = null)
            : this(new[] { role }, flags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ApiAuthAttribute class.
        /// </summary>
        /// <param name="roles">
        /// The roles for which the user must be authorized.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        public ApiAuthAttribute(string[] roles = null, string[] flags = null)
        {
            this.Roles = roles;
            
            this.Flags = flags == null ? new HashSet<string>() : new HashSet<string>(flags);
            
            // If configuration app settings do not include an assembly from which to create a mock ApiAuthAttribute instance,
            // use this object. Otherwise, create an instance of the mock class.
            string mockApiAuthAssembly = ConfigurationManager.AppSettings["mockApiAuth"];
            if (string.IsNullOrWhiteSpace(mockApiAuthAssembly))
            {
                this.apiAuthAttributeInstance = this;
            }
            else
            {
                this.apiAuthAttributeInstance =
                    LateBinding.BuildObjectFromLateBoundAssembly<IApiAuthAttribute>(
                        "ApiAuthAttribute", LateBinding.GetLateBoundAssemblyTypes(mockApiAuthAssembly));
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether allow anonymous.
        /// </summary>
        public bool AllowAnonymous { get; set; }

        /// <summary>
        ///     Gets or sets Roles.
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// Gets or sets the property bag.
        /// </summary>
        public HashSet<string> Flags { get; set; } 

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Attempts to authorize the user for the request.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public virtual void AuthorizeUser(HttpActionContext actionContext)
        {
            Log.Verbose("ApiAuthAttribute.OnAuthorization");

            string reason = string.Empty;

            // Attempt to get credentials from the HTTP authentication header. This could support application auth. or debug scenarios
            SecurityCredentials credentials = this.GetCredentialsFromAuthorizationHeader(actionContext);

            // If we did not have credentials specifed in the HTTP Authentication header 
            if (credentials == null)
            {
                // Attempt to get credentials from the Bing authentication header.
                credentials = this.GetCredentialsFromBingHeader(actionContext);
            }

            bool isAuthorized = false;
            if (credentials != null)
            {
                AuthPayload payload = Security.Authenticate(credentials, this.Flags);
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
                if (this.AllowAnonymous)
                {
                    var roles = new[] { "reader" };
                    Security.SetAnonymous(roles);
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                                                 {
                                                     ReasonPhrase =
                                                         reason
                                                 };
                }
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
                this.apiAuthAttributeInstance.AuthorizeUser(actionContext);
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
        /// Not implemented
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
        }

        /// <summary>
        /// The get user credentials from authorization header.
        ///     The header name is: Authentication
        ///     HttpHeader: Authentication bearer XXXXXXXXXXXXXXXXXXX
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
                credentials = new SecurityCredentials();

                // this should be an encrypted token.
                credentials.Token = token;

                // The HTTP authentication header has the form: Authentication: <scheme> token
                // We are using the scheme to select the security provider. This might not be a good idea for production
                // Once we figure out the scenarios we should support only a Provider for applications 
                credentials.SecurityProviderName = actionContext.Request.Headers.Authorization.Scheme;
                credentials.IdentificationCode = token;
                credentials.Name = token; // what do we use for a name? 
            }

            return credentials;
        }

        /// <summary>
        /// The get user credentials from bing header.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="SecurityCredentials"/>.
        /// </returns>
        private SecurityCredentials GetCredentialsFromBingHeader(HttpActionContext actionContext)
        {
            SecurityCredentials credentials = null;
            IEnumerable<string> headers;

            if (actionContext.Request.Headers != null
                && actionContext.Request.Headers.TryGetValues(BingAuhtenticationHeader, out headers))
            {
                try
                {
                    string token = (from h in headers select h).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        credentials = new SecurityCredentials();
                        credentials.Token = token;
                        credentials.SecurityProviderName = "lomo";
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error While reading bing header");

                    throw;
                }
            }

            return credentials;
        }

        #endregion

        // The instance of a class that implements the IApiAuthAttribute interface to use at runtime.
    }
}