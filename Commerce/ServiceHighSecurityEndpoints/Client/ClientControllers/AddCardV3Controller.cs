//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System.Web.Mvc;

    /// <summary>
    /// Add Card MVC Controller for v3 endpoint.
    /// </summary>
    public class AddCardV3Controller : Controller
    {
        /// <summary>
        /// Default controller for Add card view. Currently in use.
        /// </summary>
        /// <returns>The add card view.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Client/Views/V3/Index.cshtml");
        }

        /// <summary>
        /// Default controller for Add card settings view. Currently in use.
        /// </summary>
        /// <returns>The add card settings view.</returns>
        [HttpGet]
        public ActionResult AddCardSettings()
        {
            return View("~/Client/Views/V2/AddCardSettings.cshtml");
        }

        /// <summary>
        /// Default controller for Add card settings view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddCardSingleStep()
        {
            return View("~/Client/Views/UnAuthenticated/SingleStep.cshtml");
        }
    }
}