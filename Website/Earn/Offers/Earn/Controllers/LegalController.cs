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
    public class LegalController : Controller
    {
        public ActionResult TermsOfUse()
        {
            return View("~/offers/earn/views/legal/termsofuse.cshtml");
        }


        [RequireHttps]
        public ActionResult Faq()
        {
            return View("~/offers/earn/views/legal/faq.cshtml");
        }
    }
}