//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Web.Mvc;
    using Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Get email template controller
    /// </summary>
    public class ShareController : Controller
    {
        /// <summary>
        /// Binds the daily email templates
        /// </summary>
        /// <returns>
        /// the view
        /// </returns>
        [HttpPost]
        public ActionResult Deal()
        {
            ViewBag.referrer = "BO_EMAIL_SHARE";
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            try
            {
                Request.InputStream.Position = 0;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var content = reader.ReadToEnd();
                    return View(JsonConvert.DeserializeObject<IEnumerable<ShareDealModel>>(content));
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}