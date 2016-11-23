//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Earn.Dashboard.Web.Models.PageModels;
using Earn.Dashboard.Web.Service;
using Earn.DataContract.Commerce;

namespace Earn.Dashboard.Web.Controllers
{
    public class ReportsController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("webanalytics");
        }

        public ActionResult WebAnalytics()
        {
            return View();
        }

        public async Task<ActionResult> MerchantReport()
        {
            MerchantReportPageModel model = new MerchantReportPageModel();
            model.Merchants = await FetchMerchantsSelectList();
            model.CardBrands = await CustomerService.FetchCardBrandsAsync();
            model.TransactionTypes = CustomerService.FetchTransactionTypes();

            return View(model);
        }

        private async Task<IEnumerable<string>> FetchMerchantsSelectList()
        {
            var merchants = await CustomerService.FetchMerchantsAsync();
            merchants.Insert(0, DAL.CommerceDal.AllMerchants);
            return merchants;
        }
    }
}