//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Earn.Dashboard.Web.Service;
using Earn.DataContract;

namespace Earn.Dashboard.Web.Controllers
{
    public class OffersController : Controller
    {
        private readonly SelectListItem[] statesList = new[]
        {
            new SelectListItem {Text = "Washington", Value = "wa"},
            new SelectListItem {Text = "Massachusetts", Value = "ma"},
            new SelectListItem {Text = "Arizona", Value = "az"}
        };

        public async Task<ActionResult> Index(string state = "wa")
        {
            ViewBag.States = new SelectList(statesList, "Value", "Text", state);
            List<Deal> deals = await OffersService.Client.ExecuteStoredProcedureAsync<List<Deal>>(OffersService.GetRewardNetworkDealsByState.SelfLink, state).ConfigureAwait(false);
            return View(deals);
        }
    }
}