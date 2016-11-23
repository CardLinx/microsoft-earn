//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Web.Mvc;
using Earn.Dashboard.Web.Attributes;

namespace Earn.Dashboard.Web.Controllers
{
    [AuthorizeSG(Roles = "Admin,Support")]
    public class SupportController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("customer");
        }

        public ActionResult Customer()
        {
            return View();
        }
    }
}