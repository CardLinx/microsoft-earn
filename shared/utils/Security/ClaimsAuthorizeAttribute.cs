//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    /// <summary>
    /// Authorizes the claims made by the principal making the request.
    /// This expects the Principal to be a ClaimsPrincipal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// The comma separated list of schemes.
        /// </summary>
        private string schemes = string.Empty;

        /// <summary>
        /// The list of supported schemes.
        /// </summary>
        private string[] splitSchemes = new string[0];

        /// <summary>
        /// The comma separated list of claim types that should be presented by the principal.
        /// </summary>
        private string claimTypes = string.Empty;

        /// <summary>
        /// The list of claim types that should be presented by the principal.
        /// </summary>
        private string[] splitRequiredClaimTypes = new string[0];

        /// <summary>
        /// The list of claims that needs to verified. The claims are separated using '##'
        /// </summary>
        private string claims = string.Empty;

        /// <summary>
        /// The list of claims that should be present.
        /// </summary>
        private Dictionary<string, List<string>> splitClaims = new Dictionary<string, List<string>>();

        /// <summary>
        /// The comma separated list of issuer claims allowed for this API.
        /// </summary>
        private string issuers = string.Empty;

        /// <summary>
        /// The collection of issuer claims allowed for this API.
        /// </summary>
        private string[] splitIssuers = new string[0];

        /// <summary>
        /// The comma separated list of audience claims allowed for this API.
        /// </summary>
        private string audiences = string.Empty;

        /// <summary>
        /// The colleciton of audience claims allowed for this API.
        /// </summary>
        private string[] splitAudiences = new string[0];

        /// <summary>
        /// The authentication schemes allowed for the API.
        /// This is used to specify the required scheme if the request is made without an Authorization header or
        /// with a different scheme.
        /// </summary>
        public string Schemes
        {
            get
            {
                return this.schemes;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.schemes = string.Empty;
                    this.splitSchemes = new string[0];
                }
                else
                {
                    this.schemes = value;
                    this.splitSchemes = this.schemes.Split(',');
                }
            }
        }

        /// <summary>
        /// The comma separated list of claim types that needs to be presented by the principal.
        /// </summary>
        public string ClaimTypes
        {
            get
            {
                return this.claimTypes;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.claimTypes = string.Empty;
                    this.splitRequiredClaimTypes = new string[0];
                }
                else
                {
                    this.claimTypes = value;
                    this.splitRequiredClaimTypes = this.claimTypes.Split(',');
                }
            }
        }

        /// <summary>
        /// The comma separated list of allowed audience claims for this API.
        /// If this is specified, the principal should specify an Audience claim with value
        /// equal to one of the values in this comma separated list.
        /// </summary>
        public string Issuers
        {
            get
            {
                return this.issuers;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.issuers = string.Empty;
                    this.splitIssuers = new string[0];
                }
                else
                {
                    this.issuers = value;
                    this.splitIssuers = this.issuers.Split(',');
                }
            }
        }

        /// <summary>
        /// The comma separated list of allowed audience claims for this API.
        /// </summary>
        public string Audiences
        {
            get
            {
                return this.audiences;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.audiences = string.Empty;
                    this.splitAudiences = new string[0];
                }
                else
                {
                    this.audiences = value;
                    this.splitAudiences = this.audiences.Split(',');
                }
            }
        }

        /// <summary>
        /// Not used now.
        /// </summary>
        public string Claims
        {
            get
            {
                return this.claims;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.claims = string.Empty;
                    this.splitClaims = new Dictionary<string, List<string>>();
                }
                else
                {
                    this.claims = value;
                    this.SplitClaims();
                }
            }
        }

        /// <summary>
        /// Splits the claims into individual claims.
        /// </summary>
        private void SplitClaims()
        {
            this.splitClaims = new Dictionary<string, List<string>>();

            string[] compoundClaims = this.claims.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var compoundClaim in compoundClaims)
            {
                string[] claimTypeAndValue = compoundClaim.Split(new string[] { "::" }, StringSplitOptions.None);

                if (claimTypeAndValue.Length != 2)
                {
                    throw new ArgumentException("The claims string is not well formed.");
                }

                string claimType = claimTypeAndValue[0];
                string[] claimValues = claimTypeAndValue[1].Split(',');

                foreach (var claimValue in claimValues)
                {
                    if (this.splitClaims.ContainsKey(claimType))
                    {
                        this.splitClaims[claimType].Add(claimValue);
                    }
                    else
                    {
                        this.splitClaims.Add(claimType, new List<string> { claimValue });
                    }
                }
            }
        }

        /// <summary>
        /// Called when an action is being authorized. This method uses the user <see cref="IPrincipal"/>
        /// returned via <see cref="HttpRequestContext.Principal"/>. Authorization is denied if
        /// - the request is not associated with any user.
        /// - the user is not authenticated,
        /// - the user is authenticated but is not in the authorized group of <see cref="Users"/> (if defined), or if the user
        /// is not in any of the authorized <see cref="HolMonRoles"/> (if defined).
        /// If authorization is denied then this method will invoke <see cref="HandleUnauthorizedRequest(HttpActionContext)"/> to process the unauthorized request.
        /// </summary>
        /// <remarks>You can use <see cref="AllowAnonymousAttribute"/> to cause authorization checks to be skipped for a particular
        /// action or controller.</remarks>
        /// <param name="actionContext">The context.</param>
        /// <exception cref="ArgumentNullException">The context parameter is null.</exception>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            // Should we even do authorization?
            if (SkipAuthorization(actionContext))
            {
                return;
            }

            // Step 1. caller should be authenticated
            if (!IsAuthenticated(actionContext))
            {
                this.HandleUnauthenticatedRequest(actionContext);
                return;
            }

            // Step 2. Did authentication happen using the expected scheme.
            if (!this.IsAuthenticatedWithExpectedScheme(actionContext))
            {
                this.HandleUnauthenticatedRequest(actionContext);
                return;
            }

            // Step 3. Is the authenticated user allowed to invoke the action?
            if (!this.IsAuthorized(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }

            // Step 4. Are all the required claims present?
            if (!this.RequiredClaimTypesArePresent(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }

            // Step 5. Validate issuer.
            if (!this.ValidateIssuer(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }

            // Step 6. Validate Audience.
            if (!this.ValidateAudience(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }

            // Step 7. Validate claims.
            if (!this.ValidateClaims(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
                return;
            }
        }

        /// <summary>
        /// Validates the specified claims against the principal.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>
        /// True if all the specified claims have been presented by the principal.
        /// </returns>
        private bool ValidateClaims(HttpActionContext actionContext)
        {
            if (this.splitClaims.Count == 0)
            {
                return true;
            }

            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipal(actionContext);

            foreach (var claim in this.splitClaims)
            {
                bool hasClaim = claim.Value.Any(claimValue => claimsPrincipal.HasClaim(claim.Key, claimValue));

                if (!hasClaim)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates the issuer claim.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>
        /// True if the isser claim could be validated, false otherwise.
        /// </returns>
        private bool ValidateIssuer(HttpActionContext actionContext)
        {
            if (this.splitIssuers.Length == 0)
            {
                return true;
            }

            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipal(actionContext);

            return
                this.splitIssuers.Any(
                    allowedIssuer => claimsPrincipal.HasClaim(HolMonClaimTypes.TokenIssuer, allowedIssuer));
        }

        /// <summary>
        /// Validates the audience claim.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>
        /// True if the audience claim could be validated, false otherwise.
        /// </returns>
        private bool ValidateAudience(HttpActionContext actionContext)
        {
            if (this.splitAudiences.Length == 0)
            {
                return true;
            }

            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipal(actionContext);

            return
                this.splitAudiences.Any(
                    allowedAudience => claimsPrincipal.HasClaim(HolMonClaimTypes.TokenAudience, allowedAudience));
        }

        /// <summary>
        /// Checks whether all the required claims are presented by the principal.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>
        /// True if all the claims are presented by the principal, false otherwise.
        /// </returns>
        private bool RequiredClaimTypesArePresent(HttpActionContext actionContext)
        {
            if (this.splitRequiredClaimTypes.Length == 0)
            {
                return true;
            }

            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipal(actionContext);

            // Make sure all required claims are presented by the principal.
            return this.splitRequiredClaimTypes.All(requiredClaim => claimsPrincipal.HasClaim(claim => claim.Type == requiredClaim));
        }

        /// <summary>
        /// Checks if the principal was authenticated with the expected scheme.
        /// The request should be authenticated as a ClaimsPrincipal before this method is called.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        /// <returns>
        /// True if the authentication was done with the expected scheme, false otherwise.
        /// </returns>
        private bool IsAuthenticatedWithExpectedScheme(HttpActionContext actionContext)
        {
            // Don't care about the scheme.
            if (this.splitSchemes.Length == 0)
            {
                return true;
            }

            // It is up to the caller to make sure the user is authenticated before calling this method.
            if (!IsAuthenticated(actionContext))
            {
                throw new InvalidOperationException("The user is not authenticated.");
            }

            var claimsPrincipal = GetClaimsPrincipal(actionContext);

            return this.splitSchemes.Any(supportedScheme => claimsPrincipal.HasClaim(HolMonClaimTypes.AuthenticationScheme, supportedScheme));
        }

        /// <summary>
        /// Checks if the request is authenticated as a ClaimsPrincipal.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        /// <returns>
        /// True if the caller is an authenticated ClaimsPrincipal.
        /// </returns>
        private static bool IsAuthenticated(HttpActionContext actionContext)
        {
            var claimsPrincipal = actionContext.ControllerContext.RequestContext.Principal as ClaimsPrincipal;

            if (claimsPrincipal == null || claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Processes requests that fail authentication. 
        /// </summary>
        /// <param name="actionContext">The context.</param>
        private void HandleUnauthenticatedRequest(HttpActionContext actionContext)
        {
            HttpResponseMessage response = null;

            try
            {
                response = actionContext.ControllerContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "Authentication failed for the request.");

                if (!string.IsNullOrWhiteSpace(this.Schemes))
                {
                    var host = actionContext.Request.RequestUri.DnsSafeHost;
                    response.Headers.Add(
                        "WWW-Authenticate",
                        string.Format(CultureInfo.InvariantCulture, "{0} realm=\"{1}\"", this.Schemes, host));
                }

                actionContext.Response = response;
            }
            catch (Exception)
            {
                if (response != null)
                {
                    response.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// Processes requests that fail authorization. 
        /// </summary>
        /// <param name="actionContext">The context.</param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            actionContext.Response =
                actionContext.ControllerContext.Request.CreateErrorResponse(
                    HttpStatusCode.Forbidden,
                    "Authorization failed for the request.");
        }

        /// <summary>
        /// Checks whether authorization should be skipped.
        /// </summary>
        /// <param name="actionContext">
        /// The context.
        /// </param>
        /// <returns>
        /// True if authorization should be skipped, false otherwise.
        /// </returns>
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        /// <summary>
        /// Retrieves the principal from the action context. This should be a ClaimsPrincipal, otherwise 
        /// an exception is thrown.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The ClaimsPrincipal.</returns>
        private static ClaimsPrincipal GetClaimsPrincipal(HttpActionContext actionContext)
        {
            IPrincipal principal = actionContext.ControllerContext.RequestContext.Principal;

            var claimsPrincipal = principal as ClaimsPrincipal;

            if (claimsPrincipal == null)
            {
                throw new InvalidOperationException("The principal associated with the request is not a claims principal.");
            }

            return claimsPrincipal;
        }
    }
}