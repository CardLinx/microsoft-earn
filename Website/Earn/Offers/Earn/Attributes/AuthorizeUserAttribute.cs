//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Attributes
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// The login URL
        /// </summary>
        private const string LoginUrl = "https://www.bing.com/fd/auth/signin?action=interactive&provider=windows_live_id&return_url=";

        /// <summary>
        /// The fake authentication URL
        /// </summary>
        private const string FakeAuthUrl = "https://www.bing.com/offers/earn/fakeauth?url=";

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Processes HTTP requests that fail authorization.
        /// </summary>
        /// <param name="filterContext">Encapsulates the information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute" />. The <paramref name="filterContext" /> object contains the controller, HTTP context, request context, action result, and route data.</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string currentUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri);

            if (filterContext.HttpContext.Request.Url.Authority.Contains("earn.bing.com"))
            {
                currentUrl = string.Format("{0}{1}", FakeAuthUrl, currentUrl);
            }

            string redirectUrl = string.Format("{0}{1}", LoginUrl, HttpUtility.UrlEncode(currentUrl));
            filterContext.Result = new RedirectResult(redirectUrl);
        }

    }
}