//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Attributes;
using Earn.Offers.Earn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Controllers
{
    [RequireHttps]
    [MicrosoftAccountAuthentication]
    public class LearnController : Controller
    {
        public ActionResult Index()
        {
            LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;
            LearnPageModel learnPageModel = new LearnPageModel();
            if (liveIdAuthResult != null)
            {
                learnPageModel.LiveIdResult = liveIdAuthResult;
            }

            return View("~/offers/earn/views/learn/learn.cshtml", learnPageModel);
        }
    }
}