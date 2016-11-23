//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Helper;
using Earn.Offers.Earn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Attributes
{
    public class LocationAttribute : ActionFilterAttribute
    {
        static List<string> AllowedStates = new List<string>
            {
                "wa",
                "ma",
                "az"
            };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string state = "wa";
            bool status = GetStateFromCookie(filterContext.HttpContext.Request, out state) || GetStateFromHeader(filterContext.HttpContext.Request, out state);
            if (!status)
            {
                state = "wa";
            }

            filterContext.HttpContext.Items.Add("state", state);
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
                filterContext.HttpContext.Trace.Write("(Logging Filter)Exception thrown");

            base.OnActionExecuted(filterContext);
        }


        private bool GetStateFromHeader(HttpRequestBase request, out string state)
        {
            state = null;
            string revIpHeaderValue = request.Headers["X-FD-RevIP"];
            if (string.IsNullOrWhiteSpace(revIpHeaderValue))
            {
                return false;
            }

            UserLocation location = FrontdoorUtility.ParseReverseIp(revIpHeaderValue);
            {
                if (location != null && !string.IsNullOrWhiteSpace(location.State))
                {
                    switch (state.ToLower())
                    {
                        case "washington":
                            state = "wa";
                            return true;
                        case "arizona":
                            state = "az";
                            return true;
                        case "massachusetts":
                            state = "ma";
                            return true;
                    }
                }
            }

            return false;
        }

        private bool GetStateFromCookie(HttpRequestBase request, out string state)
        {
            state = null;
            HttpCookie cookie = request.Cookies["earn_loc"];
            if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
            {
                if (AllowedStates.Contains(cookie.Value, StringComparer.OrdinalIgnoreCase))
                {
                    state = cookie.Value.ToLower();
                    return true;
                }
            }

            return false;
        }
    }
}