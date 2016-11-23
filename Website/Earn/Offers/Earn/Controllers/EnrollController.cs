//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Attributes;
using Earn.Offers.Earn.Models;
using Earn.Offers.Earn.Services;
using LoMo.UserServices.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Controllers
{
    [RequireHttps]
    public class EnrollController : Controller
    {
        [MicrosoftAccountAuthentication]
        public async Task<ActionResult> Index(bool debug = false, string step = "step1")
        {
            try
            {
                LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
                ViewBag.CurrentStep = step;
                if (!User.Identity.IsAuthenticated)
                {
                    return HandleUnauthenticatedUser(liveIdAuthResult);
                }

                UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
                ViewBag.UserId = userModel.UserId;
                string secureToken = HttpContext.Items["backendtoken"] as string;
                bool status = await CommerceService.IsUserRegisteredWithCardLink(userModel, secureToken);


                if (liveIdAuthResult != null)
                {
                    ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                    ViewBag.SignOutHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                }

                if (debug)
                {
                    return View("~/offers/earn/views/enroll/enroll.cshtml");
                }

                if (status)
                {
                    return HandleRegisteredUser();
                }

                return View("~/offers/earn/views/enroll/enroll.cshtml");

            }
            catch (Exception e)
            {
                return HandleServerError();
            }
        }

        [MicrosoftAccountAuthentication]
        [HttpPost]
        public async Task<ActionResult> GetReferralCode()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 400;
                return Content("You need to login to perform this operation.");
            }

            UserModel userModel = new UserModel(User.Identity as ClaimsIdentity);
            string secureToken = HttpContext.Items["backendtoken"] as string;
            bool status = await CommerceService.IsUserRegisteredWithCardLink(userModel, secureToken);
            if (!status)
            {
                Response.StatusCode = 404;
                return Content("User is not registered with Microsoft Earn.");
            }

            string referralCode = await CommerceService.LoadReferralCode(userModel, secureToken);
            return Content(referralCode);
        }

        private ActionResult HandleUnauthenticatedUser(LiveIdAuthResult result)
        {
            ViewBag.SignInHtmlLink = result.SignInHtmlLink; ;
            return View("~/offers/earn/views/enroll/enrollunauthenticated.cshtml");
        }

        private ActionResult HandleRegisteredUser()
        {
            return new RedirectResult("/");
        }

        private ActionResult HandleServerError()
        {
            return new RedirectResult("/");
        }
    }
}