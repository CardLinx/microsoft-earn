//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.FrontEnd.Controllers
{
    using System.IO;
    using System.Net;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ViewModels;

    /// <summary>
    /// The seasonal email templates
    /// </summary>
    public class SeasonalController : Controller
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// The valentine's email template view
        /// </returns>
        [HttpPost]
        public ActionResult Valentine(string campaign, string referrer)
        {
            return RenderView(campaign, referrer);
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// The Memorial day email template view
        /// </returns>
        [HttpPost]
        public ActionResult Memorial(string campaign, string referrer)
        {
            return RenderView(campaign, referrer);
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>The Father's Day email template</returns>
        [HttpPost]
        public ActionResult Father(string campaign, string referrer)
        {
            return RenderView(campaign, referrer);
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// The Independence's Day email template
        /// </returns>
        [HttpPost]
        public ActionResult Independence(string campaign, string referrer)
        {
            return RenderView(campaign, referrer);
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// The Independence's Day email template
        /// </returns>
        [HttpPost]
        public ActionResult IndependenceFollowUp(string campaign, string referrer)
        {
            return RenderView(campaign, referrer);
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// The Summer email template
        /// </returns>
        [HttpPost]
        public ActionResult Summer(string campaign, string referrer)
        {
            return RenderView(campaign, referrer);
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>The seasonal email template view</returns>
        private ActionResult RenderView(string campaign, string referrer)
        {
            ViewBag.campaign = string.IsNullOrEmpty(campaign) ? string.Empty : campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                return View(new DailyDealsVM(JsonConvert.DeserializeObject(content) as JObject));
            }
        }
    }
}