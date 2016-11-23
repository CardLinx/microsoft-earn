//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Attributes
{
    public class RootDomainRedirectFilterAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext context)
        {
            if (!context.RequestContext.HttpContext.Request.Url.Host.ToLower().Contains("earn.microsoft.com"))
            {
                UriBuilder uriBuilder = new UriBuilder(context.RequestContext.HttpContext.Request.Url);
                uriBuilder.Host = "earn.microsoft.com";
                context.RequestContext.HttpContext.Response.Redirect(uriBuilder.ToString(), true);
            }
        }
    }
}