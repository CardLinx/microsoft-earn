//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ViewModels;

    /// <summary>
    /// The Bing Offers news letter controller
    /// </summary>
    public class NewsLetterController : Controller
    {
        /// <summary>
        /// Weekly news letter email template.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// The Weekly template
        /// </returns>
        public ActionResult Weekly(string campaign, string referrer)
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
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
                    return View(new DailyDealsVM(JsonConvert.DeserializeObject(content) as JObject));
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}