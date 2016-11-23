//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary></summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using OffersEmail.DataContracts;
    using OffersEmail.Models;

    /// <summary>
    /// Earn program templates
    /// </summary>
    public class EarnController : Controller
    {
        private DateTime startDoubleEarnPctDate;
        private DateTime endDoubleEarnPctDate;
        private TimeZoneInfo pctZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        public EarnController()
        {
            var utcStartDate = (new DateTime(2015, 11, 6, 8, 0, 0, DateTimeKind.Utc));
            var utcEndDate = (new DateTime(2015, 11, 9, 8, 0, 0, DateTimeKind.Utc));

            startDoubleEarnPctDate = TimeZoneInfo.ConvertTimeFromUtc(utcStartDate, pctZone);
            endDoubleEarnPctDate = TimeZoneInfo.ConvertTimeFromUtc(utcEndDate, pctZone);
        }

        [HttpPost]
        public ActionResult WelcomeToEarn()
        {
            Random rnd = new Random();
            ViewBag.brand = rnd.Next(1, 56);

            return View("~/views/earn/welcome/WelcomeToEarn.cshtml");
        }

        [HttpPost]
        public ActionResult IfUserHasEarned()
        {
            return GetHTML("~/views/earn/welcome/IfUserHasEarned.cshtml");
        }

        [HttpPost]
        public ActionResult IfUserHasNotEarned()
        {
            return GetHTML("~/views/earn/welcome/IfUserHasNotEarned.cshtml");
        }

        [HttpPost]
        public ActionResult Newsletter()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<CampaignDataContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.UnsubscribeUrl))
                    {
                        return View("~/views/earn/newsletters/2015/07.July.cshtml", model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "UnsubscribeUrl is missing");
        }

        [HttpPost]
        public ActionResult NewsletterAugust2015()
        {
            return GetHTML("~/views/earn/newsletters/2015/08.August.cshtml");
        }

        [HttpPost]
        public ActionResult NewsletterSeptember2015()
        {
            return GetHTML("~/views/earn/newsletters/2015/09.September.cshtml");
        }

        [HttpPost]
        public ActionResult NewsletterNovember2015()
        {
            return GetHTML("~/views/earn/newsletters/2015/11.November.cshtml");
        }

        [HttpPost]
        public ActionResult NewsletterNovember2015Summary()
        {
            return GetHTML("~/views/earn/newsletters/2015/11.NovemberSummary.cshtml");
        }

        [HttpPost]
        public ActionResult PromoDecember2015_2x3()
        {
            return GetHTML("~/views/earn/promo/December2015_2x3.cshtml");
        }

        [HttpPost]
        public ActionResult PromoDecember2015_2x4()
        {
            return GetHTML("~/views/earn/promo/December2015_2x4.cshtml");
        }

        [HttpPost]
        public ActionResult NewsletterDecember2015()
        {
            return GetHTML("~/views/earn/newsletters/2015/12.December.cshtml");
        }

        [HttpPost]
        public ActionResult NewsletterBlackFriday2015()
        {
            return GetHTML("~/views/earn/newsletters/2015/11.BlackFriday.cshtml");
        }

        [HttpPost]
        public ActionResult NewsletterJanuary2016()
        {
            return GetHTML("~/views/earn/newsletters/2016/01.January.cshtml");
        }

        [HttpPost]
        public ActionResult NewsletterFebruary2016()
        {
            return GetHTML("~/views/earn/newsletters/2016/02.February.cshtml");
        }

        [HttpPost]
        public ActionResult NewsletterMarch2016()
        {
            return GetHTML("~/views/earn/newsletters/2016/03.March.cshtml");
        }

        private ActionResult GetHTML(string viewPath)
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    return View(viewPath);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }

        /// <summary>
        /// Auth Action.
        /// </summary>
        /// <returns>The Auth view</returns>
        [HttpPost]
        public ActionResult Earn()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    CardLinkModel model = JsonConvert.DeserializeObject<CardLinkModel>(content);

                    if (string.IsNullOrWhiteSpace(model.MerchantName) || model.Percent == 0.0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "merchant_name or percent is missing");
                    }

                    var acceptTypes = this.Request.AcceptTypes != null ? this.Request.AcceptTypes.FirstOrDefault() : null;
                    if (acceptTypes != null && acceptTypes.Equals("text/plain", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return Content(string.Format(Resources.Earn.EarnSmsTemplate, model.Percent, model.MerchantName));
                    }

                    ViewBag.products = GenerateRandomProducts();
                    ViewBag.giftCards = GenerateRandomBrands();

                    var pctNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pctZone);

                    string view = (startDoubleEarnPctDate <= pctNow) && (pctNow <= endDoubleEarnPctDate)
                        ? "~/views/earn/DoubleEarn.cshtml"
                        : "~/views/earn/Earn.cshtml";

                    return View(view, model);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult EarnSms()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    CardLinkModel model = JsonConvert.DeserializeObject<CardLinkModel>(content);

                    if (string.IsNullOrWhiteSpace(model.MerchantName) || model.Percent == 0.0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "merchant_name or percent is missing");
                    }

                    var pctNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pctZone);

                    string template = (startDoubleEarnPctDate <= pctNow) && (pctNow <= endDoubleEarnPctDate)
                        ? Resources.Earn.DoubleEarnSmsTemplate
                        : Resources.Earn.EarnSmsTemplate;

                    return Content(string.Format(template, model.Percent, model.MerchantName));
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }

        /// <summary>
        /// Auth Action.
        /// </summary>
        /// <returns>The Auth view</returns>
        [HttpPost]
        public ActionResult Burn()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    CardLinkModel model = JsonConvert.DeserializeObject<CardLinkModel>(content);

                    if (string.IsNullOrWhiteSpace(model.CreditAmount))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "credit_amount is missing");
                    }

                    var acceptTypes = this.Request.AcceptTypes != null ? this.Request.AcceptTypes.FirstOrDefault() : null;
                    if (acceptTypes != null && acceptTypes.Equals("text/plain", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return Content(string.Format(Resources.Earn.BurnSmsTemplate, model.CreditAmount));
                    }

                    ViewBag.products = GenerateRandomProducts();
                    ViewBag.giftCards = GenerateRandomBrands();

                    return View(model);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }

        private int[] GenerateRandomBrands()
        {
            int[] result = new int[3];
            HashSet<int> generatedIds = new HashSet<int>();
            Random r = new Random();

            for (int i = 0; i < 3; i++)
            {
                int n = 0;
                do
                {
                    n = r.Next(1, 10);
                }
                while (generatedIds.Contains(n));

                generatedIds.Add(n);
                result[i] = n;
            }

            return result;
        }

        private int[] GenerateRandomProducts()
        {
            int[] result = new int[3];
            Random r = new Random();

            result[0] = r.Next(1, 7);
            result[1] = r.Next(7, 12);

            do
            {
                result[2] = r.Next(1, 6);
            }
            while (result[2] == result[0]);

            return result;
        }

        /// <summary>
        /// Auth Action.
        /// </summary>
        /// <returns>The Auth view</returns>
        [HttpPost]
        public ActionResult BurnSms()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    CardLinkModel model = JsonConvert.DeserializeObject<CardLinkModel>(content);

                    if (string.IsNullOrWhiteSpace(model.CreditAmount))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "credit_amount is missing");
                    }

                    return Content(string.Format(Resources.Earn.BurnSmsTemplate, model.CreditAmount));
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }

        /// <summary>
        /// DeprecateFacebook email template.
        /// </summary>
        /// <returns>
        /// Return the view
        /// </returns>
        [HttpPost]
        public ActionResult DeprecateFacebook()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            ViewBag.campaign = "BO_DeprecateFB";
            ViewBag.referrer = "BO_EMAIL";
            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<CampaignDataContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.UnsubscribeUrl))
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "UnsubscribeUrl is missing");
        }

        /// <summary>
        /// MigrateUser email template.
        /// </summary>
        /// <returns>
        /// Return the view
        /// </returns>
        [HttpPost]
        public ActionResult MigrateUser()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            ViewBag.campaign = "BO_MigrateMC";
            ViewBag.referrer = "BO_EMAIL";
            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<CampaignDataContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.UnsubscribeUrl) && !string.IsNullOrWhiteSpace(model.Content))
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "UnsubscribeUrl is missing");
        }
    }
}