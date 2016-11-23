//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Models;
using Earn.Offers.Earn.Helper;
using Earn.Offers.Earn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Earn.Offers.Earn.Controllers.Api
{
    public class AnalyticsController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]AnalyticsModel analyticsModel)
        {
            try
            {
                if (analyticsModel != null && analyticsModel.BrowserId != null && analyticsModel.SessionId != null)
                {
                    analyticsModel.ServerTimeStamp = DateTimeOffset.UtcNow;
                    analyticsModel.UserAgent = GetUserAgent();
                    analyticsModel.IPAddress = GetClientIp();
                    DeviceType deviceType = GetDeviceType(analyticsModel.UserAgent, null);
                    analyticsModel.DeviceType = deviceType.ToString();

                    bool isAuthenticated = User.Identity.IsAuthenticated;
                    if (isAuthenticated)
                    {
                        analyticsModel.IsAuthenticated = true;

                        ClaimsPrincipal cp = User as ClaimsPrincipal;
                        Claim userIdClaim = cp.Claims.Where(x => x.Type == ClaimTypes.Sid).FirstOrDefault();
                        if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
                        {
                            analyticsModel.AuthenticatedUserId = userIdClaim.Value;
                        }
                    }

                    bool status = await AnalyticsService.TrySaveAnalyticsData(analyticsModel);
                }
            }
            catch (Exception ex)
            {
            }

            return Ok();
        }

        private string GetClientIp()
        {
            string ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        private string GetUserAgent()
        {
            return Request.Headers.UserAgent.ToString();
        }

        public static DeviceType GetDeviceType(string userAgent, HttpBrowserCapabilitiesBase browser)
        {
            userAgent = string.IsNullOrEmpty(userAgent) ? string.Empty : userAgent.ToLower();

            if (userAgent.Contains("ipad") || userAgent.Contains("tablet") || userAgent.Contains("kindle"))
            {
                return DeviceType.Tablet;
            }

            if (userAgent.Contains("android") && !userAgent.Contains("mobi"))
            {
                return DeviceType.Tablet;
            }

            if ((browser != null && browser.IsMobileDevice) || userAgent.Contains("mobi"))
            {
                return DeviceType.Mobile;
            }

            return DeviceType.Desktop;
        }

    }
}