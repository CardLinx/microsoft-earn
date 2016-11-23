//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Microsoft.Passport.RPS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Controllers
{
    public class ExpireCookieController : Controller
    {
        // GET: LiveidLogout
        public FileResult Index()
        {
            return LogoutUser();
        }

        private FileResult LogoutUser()
        {
            // Sign out of Windows Live ID:
            // - Set authentication cookies to null and expire.fnotepad
            // - Include appropriate P3P compact policy header for Internet
            //     Explorer to delete cookies in a third-party context.
            // - Return the green check mark.
            Microsoft.Passport.RPS.RPS oRPS;
            RPSHttpAuth oHttpAuth;
            string logoutHeaders;
            string siteName = ConfigurationManager.AppSettings["SiteName"];
            System.Drawing.Image img;

            oRPS = (Microsoft.Passport.RPS.RPS)HttpContext.Application["globalRPS"];
            if (oRPS != null)
            {
                // Write HTTP Headers to the response stream to expire
                //   the authentication cookies.
                oHttpAuth = (RPSHttpAuth)oRPS.GetObject("rps.http.auth");
                logoutHeaders = oHttpAuth.GetLogoutHeaders(siteName);
                oHttpAuth.WriteHeaders(System.Web.HttpContext.Current.Response, logoutHeaders);
            }
            // A P3P compact policy header is required for Internet Explorer
            //   to delete cookies in a third-party context. Include it here only
            //   if you need to override the policy specified in the RPSServer.xml file.
            HttpContext.Response.AddHeader("P3P", "CP=\"CAO CONi OTR OUR DEM ONL\"");
            HttpContext.Response.AddHeader("Cache-Control", "max-age=0, no-cache, no-store, must-revalidate");

            string base64EncodedImage = "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";
            byte[] imageBytes = Convert.FromBase64String(base64EncodedImage);
            // Return the sign-out check-mark GIF image.
            // var path = Server.MapPath("/offers/earn/content/images/check.gif");
            //img = System.Drawing.Image.FromFile(path);
            //HttpContext.Response.ContentType = "image/gif";
            //img.Save(HttpContext.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
            return base.File(imageBytes, "image/gif");
        }
    }
}