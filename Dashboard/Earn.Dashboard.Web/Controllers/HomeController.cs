//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Data;
using System.Web.Caching;
using System.Web.Mvc;
using Earn.Dashboard.Web.Models.PageModels;
using Earn.Dashboard.Web.Service;
using Earn.Dashboard.Web.Utils;

namespace Earn.Dashboard.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // While cleaning the subscription, the report server was deleted.

            //DataTable dataTable;
            //if (HttpContext.Cache[Config.DailyStatistics] == null)
            //{
            //    dataTable = AnalyticsService.DailyStatistics(DateTime.UtcNow);
            //    HttpContext.Cache.Insert(Config.DailyStatistics, dataTable, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
            //}
            //else
            //{
            //    dataTable = (DataTable)HttpContext.Cache[Config.DailyStatistics];
            //}

            //HomePageModel model = new HomePageModel(dataTable);

            HomePageModel model = new HomePageModel(null);
            return View(model);
        }
    }
}