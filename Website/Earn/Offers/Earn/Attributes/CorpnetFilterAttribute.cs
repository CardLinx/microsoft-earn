//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Attributes
{
    public class CorpnetFilterAttribute : ActionFilterAttribute
    {
        static List<string> AllowedIps = new List<string>()
        {
            "131.107."
        };

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (QueryContainsCorpnet(context) || IncomingCookieExists(context))
            {
                WriteCookie(context);
            }
            else
            {
                string ip = Utility.GetIPAddress(context.HttpContext);
                if (!IsMicrosoftIP(ip))
                {
                    context.RequestContext.HttpContext.Response.Write("This resource is fobidden outside Microsoft CORPNET. If you think this is an error, email earnsupport@microsoft.com ");
                    context.RequestContext.HttpContext.Response.End();
                }
            }
        }

        private void WriteCookie(ActionExecutingContext context)
        {
            HttpCookie cookie = new HttpCookie("corpnet", "true");
            cookie.Domain = "earn.microsoft.com";
            cookie.Secure = false;
            cookie.Expires = DateTime.UtcNow.AddYears(1);
            cookie.HttpOnly = false;
            context.RequestContext.HttpContext.Response.Cookies.Add(cookie);
        }

        private bool IncomingCookieExists(ActionExecutingContext context)
        {
            HttpCookie cookie = context.RequestContext.HttpContext.Request.Cookies["corpnet"];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return false;
            }

            return true;
        }

        private bool QueryContainsCorpnet(ActionExecutingContext context)
        {
            return context.RequestContext.HttpContext.Request.Url.Query != null && context.RequestContext.HttpContext.Request.Url.Query.ToLower().Contains("corpnet");
        }

        private bool IsMicrosoftIP(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            foreach (string allowedIp in AllowedIps)
            {
                if (ip.StartsWith(allowedIp))
                {
                    return true;
                }
            }

            return false;
        }
    }
}