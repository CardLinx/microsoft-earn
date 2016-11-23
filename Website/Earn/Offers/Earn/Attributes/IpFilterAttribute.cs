//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Attributes
{
    public class IpFilterAttribute : ActionFilterAttribute
    {
        static List<string> AllowedIps = new List<string>()
        {
            "131.107.",
            "127.0.0."
        };

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            string ip = Utility.GetIPAddress(context.HttpContext);
            if (!IsMicrosoftIP(ip))
            {
                context.RequestContext.HttpContext.Response.Write("Good Try. But this resource can not be accessed outside Microsoft CORPNET. Although I'd say keep trying.");
                context.RequestContext.HttpContext.Response.End();
            }
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


