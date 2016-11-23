//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Earn.Dashboard.Web.Service;
using Earn.Dashboard.Web.Utils;

namespace Earn.Dashboard.Web.Controllers
{
    public class EmailController : Controller
    {
        [AllowAnonymous]
        public ActionResult DailyStatistics()
        {
            DataTable dataTable = AnalyticsService.DailyStatistics(DateTime.UtcNow.AddDays(-1));
            string to = ConfigurationManager.AppSettings["AnalyticsEmailAddress"];
            if (Request.Headers["x-earn-statistics"] != null &&
                Request.Headers["x-earn-statistics"] == "23b14b1addec494e9d5a9e94c976e37d")
            {
                EmailService.Send(PickTemplate(dataTable), to);
                Log.Verbose("Statistics email sent successfully");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private string PickTemplate(DataTable dataTable)
        {
            ViewData.Model = dataTable;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext, this.ControllerContext.RouteData.Values["action"].ToString(), null);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}