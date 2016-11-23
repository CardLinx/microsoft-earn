//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.IO;
using System.Web.Mvc;
using Earn.Dashboard.Web.Service;
using Earn.Dashboard.Web.Utils;

namespace Earn.Dashboard.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            if (Response.StatusCode == 200)
            {
                Log.Warn("error route requested explicitly");
                Response.StatusCode = 404;
            }

            bool ignoreError = Response.StatusCode == 404 || Response.StatusCode == 400;
            if (Config.IsProduction && ViewData.Model != null && !ignoreError)
            {
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindView(ControllerContext, "~/Views/Email/Error.cshtml", null);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    EmailService.Send(sw.GetStringBuilder().ToString());
                    Log.Verbose("error email sent successfully");
                }
            }

            return View();
        }
    }
}