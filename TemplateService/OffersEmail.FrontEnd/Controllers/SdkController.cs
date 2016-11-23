//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary></summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.FrontEnd.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// The SDK controller for email template services.
    /// </summary>
    public class SdkController : Controller
    {
        /// <summary>
        /// Index view.
        /// </summary>
        /// <returns>The index view.</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}