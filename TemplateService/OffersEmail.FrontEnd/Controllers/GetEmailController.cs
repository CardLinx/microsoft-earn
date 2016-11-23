//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Resources;
    using ViewModels;

    /// <summary>
    /// Get email template controller
    /// </summary>
    public class GetEmailController : Controller
    {
        /// <summary>
        /// Binds the daily email templates
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult DailyDeals(string campaign, string referrer)
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var content = reader.ReadToEnd();
                    ////return this.View(new DailyDealsVM(JsonConvert.DeserializeObject(content) as JObject));
                    //// new layout for the daily deals template
                    return View("~/Views/Newsletter/Weekly.cshtml", new DailyDealsVM(JsonConvert.DeserializeObject(content) as JObject));
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Binds the trending email templates
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult Trending(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                return this.View(new DailyDealsVM(JsonConvert.DeserializeObject(content) as JObject));
            }
        }

        /// <summary>
        /// Binds the MSN email template.
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view.</returns>
        [HttpPost]
        public ActionResult MsnDeals(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                return this.View(new MsnDealsVM(JsonConvert.DeserializeObject(content) as JObject));
            }
        }

        /// <summary>
        /// Binds the MSN Intro email template.
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view.</returns>
        [HttpPost]
        public ActionResult MsnDealsIntro1(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                return this.View(new MsnDealsVM(JsonConvert.DeserializeObject(content) as JObject));
            }
        }

        /// <summary>
        /// Binds the MSN Intro email template.
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view.</returns>
        [HttpPost]
        public ActionResult Gifts(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                return this.View(new MsnDealsVM(JsonConvert.DeserializeObject(content) as JObject));
            }
        }

        /// <summary>
        /// Binds the CardLink email template.
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>
        /// the view.
        /// </returns>
        [HttpPost]
        public ActionResult CardLinkDeals(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                return this.View(new MsnDealsVM(JsonConvert.DeserializeObject(content) as JObject));
            }
        }

        /// <summary>
        /// Binds the merchant email template
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult Autoprospecting(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                return this.View(JsonConvert.DeserializeObject<AutoprospectingVM>(content));
            }
        }

        /// <summary>
        /// Binds the merchant email template
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult MerchantReport(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                return this.View(JsonConvert.DeserializeObject<MerchantReportVM>(content));
            }
        }

        /// <summary>
        /// Binds the merchant email template
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult AutoprospectingCreateNewOffer(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                var viewModel = JsonConvert.DeserializeObject<AutoprospectingCreateOfferVM>(content);

                return AutoprospectingCreateViewForOffer(viewModel);
            }
        }

        /// <summary>
        /// Binds the merchant email template to Ad info
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult AutoprospectingCreateNewAd(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                var viewModel = JsonConvert.DeserializeObject<AutoprospectingCreateAdVM>(content);

                return AutoprospectingCreateViewForAd(viewModel);
            }
        }

        /// <summary>
        /// Binds the merchant email template to Ad info
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult AutoprospectingCreateNewBusinessClaim(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                var viewModel = JsonConvert.DeserializeObject<AutoprospectingBusinessClaimVM>(content);

                switch (viewModel.ApTemplate)
                {
                    case "BPClaimFB1":
                        return this.View("BPClaimFB1", viewModel);
                    default:
                        return this.View(viewModel);
                }
            }
        }

        /// <summary>
        /// Binds autoprospecting email template to Ad/Offer/BusinessClaim info
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult AutoprospectingCreateRuleBased(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                var genericViewModel = JsonConvert.DeserializeObject<AutoprospectingCreateRuleBasedVM>(content);

                switch (genericViewModel.ApplicationName)
                {
                    case "AdExpressUI":
                        {
                            var viewModel = JsonConvert.DeserializeObject<AutoprospectingCreateAdVM>(genericViewModel.SerializedAppEntity);
                            viewModel.CustomerName = genericViewModel.CustomerName;
                            viewModel.GettingStartedUrl = genericViewModel.GettingStartedUrl;
                            viewModel.CouponCode = genericViewModel.CouponCode;
                            viewModel.ApTemplate = genericViewModel.ApTemplate;

                            return AutoprospectingCreateViewForAd(viewModel);
                        }

                    default:
                        return new HttpStatusCodeResult(HttpStatusCode.NotImplemented, string.Format("ApplicationName {0} not supported", genericViewModel.ApplicationName));
                }
            }
        }

        /// <summary>
        /// Confirm email update action.
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>The Confirm email update template</returns>
        [HttpPost]
        public ActionResult ConfirmEmailUpdate(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var confirmationUrl = string.Empty;
                var json = JsonConvert.DeserializeObject(content) as JObject;

                if (json != null)
                {
                    confirmationUrl = json["confirmation_url"].Value<string>();
                }

                if (string.IsNullOrWhiteSpace(confirmationUrl))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "confirmation_url is missing");
                }

                var acceptTypes = this.Request.AcceptTypes;
                if (acceptTypes != null && (acceptTypes.Length > 0
                                                    && acceptTypes[0].Equals("text/plain", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return Content(string.Format(Confirm.UpdateEmailTextContent, confirmationUrl));
                }

                return this.View(new ConfirmSubscriptionVM(confirmationUrl));
            }
        }

        /// <summary>
        /// Confirm email for the unauthenticated sign up flow.
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analytics</param>
        /// <returns>The Confirm email template for unauthenticated CLO sign up</returns>
        [HttpPost]
        public ActionResult ConfirmUnAuthenticatedSignupEmail(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var confirmationUrl = string.Empty;
                var json = JsonConvert.DeserializeObject(content) as JObject;

                if (json != null)
                {
                    confirmationUrl = json["confirmation_url"].Value<string>();
                }

                if (string.IsNullOrWhiteSpace(confirmationUrl))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "confirmation_url is missing");
                }

                var acceptTypes = this.Request.AcceptTypes;
                if (acceptTypes != null && (acceptTypes.Length > 0
                                                    && acceptTypes[0].Equals("text/plain", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return Content(string.Format(UnAuthCLOEmailConfirm.ConfirmEmailTextContent, confirmationUrl));
                }

                ViewBag.showWarning = true;
                ViewBag.doNotShowMore = true;
                return this.View("ConfirmUnAuthenticatedSignupEmail", new UnauthenticatedSignupEmailConfirmationVM(confirmationUrl));
            }
        }

        /// <summary>
        /// Link your account to MSID/FBID.
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analytics</param>
        /// <returns>The link your account to MSID/FBID template.</returns>
        [HttpPost]
        public ActionResult UnAuthenticatedAccountLinkingInviteEmail(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var confirmationUrl = string.Empty;
                var json = JsonConvert.DeserializeObject(content) as JObject;

                if (json != null)
                {
                    confirmationUrl = json["confirmation_url"].Value<string>();
                }

                if (string.IsNullOrWhiteSpace(confirmationUrl))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "confirmation_url is missing");
                }

                var acceptTypes = this.Request.AcceptTypes;
                if (acceptTypes != null && (acceptTypes.Length > 0
                                                    && acceptTypes[0].Equals("text/plain", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return Content(string.Format(UnauthenticatedAccountLinkingInvite.ConfirmEmailTextContent, confirmationUrl));
                }

                ViewBag.showWarning = true;
                ViewBag.doNotShowMore = true;
                return this.View("UnAuthenticatedAccountLinkingInviteEmail", new UnauthenticatedAccountLinkingInviteEmailVM(confirmationUrl));
            }
        }

        /// <summary>
        /// Promotional Redeem email
        /// </summary>
        /// <returns>The Promotional Redeem HTML</returns>
        [HttpPost]
        public ActionResult PromotionalRedeem()
        {
            return View();
        }

        /// <summary>
        /// ChangeTerms email.
        /// </summary>
        /// <returns>The ChangeTerms view</returns>
        [HttpPost]
        public ActionResult ChangeTerms()
        {
            ViewBag.campaign = "changeterms";
            ViewBag.referrer = "BO_NOTIFY";

            return View();
        }

        /// <summary>
        /// Generates email to provider use feedback to merchant for store visit and merchant response
        /// </summary>
        /// <param name="campaign">The Specific Campaign of this email</param>
        /// <param name="referrer">The referrer for analyitics</param>
        /// <returns>the view</returns>
        [HttpPost]
        public ActionResult Feedback(string campaign = "na", string referrer = "BO_EMAIL")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            // Need to do this for an unknown reason within MVC. When passing url parameters to the method, position is set to the end of the stream.
            Request.InputStream.Position = 0;

            using (var reader = new StreamReader(Request.InputStream))
            {
                string content = reader.ReadToEnd();
                var viewModel = JsonConvert.DeserializeObject<FeedbackEmailVM>(content);

                if (string.Equals(viewModel.IsForMerchant, "true", StringComparison.OrdinalIgnoreCase))
                {
                    return this.View("FeedbackToMerchant", viewModel);
                }

                return this.View("FeedbackToUser", viewModel);
            }
        }

        /// <summary>
        /// Create view for autoprospecting offer email
        /// </summary>
        /// <param name="viewModel">View model</param>
        /// <returns>The View</returns>
        private ActionResult AutoprospectingCreateViewForOffer(AutoprospectingCreateOfferVM viewModel)
        {
            switch (viewModel.ApTemplate)
            {
                case "RestaurantAP1":
                    return this.View("AutoprospectingRestaurantTemplate", viewModel);
                case "UserInviteAP1":
                    return this.View("InviteUserTemplate", viewModel);
                case "UserInviteAPEmailSubscription":
                    return this.View("InviteUserAPEmailSubscription", viewModel);
                case "DeemGenericAP1":
                case "DeemRestaurantAP1":
                    return this.View("AutoprospectingDeemTemplate", viewModel);
                default:
                    return this.View(viewModel);
            }
        }

        /// <summary>
        /// Create view for autoprospecting ad email
        /// </summary>
        /// <param name="viewModel">View model</param>
        /// <returns>The View</returns>
        private ActionResult AutoprospectingCreateViewForAd(AutoprospectingCreateAdVM viewModel)
        {
            switch (viewModel.ApTemplate)
            {
                case "BAXAP1":
                    var viewName = !string.IsNullOrEmpty(viewModel.CouponCode)
                        ? "AutoprospectingBAXWithCoupon"
                        : "AutoprospectingBAXWithoutCoupon";
                    return this.View(viewName, viewModel);
                case "BAXAP2":
                    return this.View("AutoprospectingBAXRichTemplate1", viewModel);
                case "BAXAPRich2":
                    return this.View("BAXAPRich2", viewModel);
                case "BAXAPRich2FU":
                    return this.View("BAXAPRich2", viewModel);
                case "BAXDropOff1":
                    return this.View("BAXDropOff1", viewModel);
                case "BAXDropOff2":
                    return this.View("BAXDropOff2", viewModel);
                case "BAXDropOffWC":
                    return this.View("BAXDropOffWithoutCoupon", viewModel);
                case "AP1_Followup1":
                    return this.View("BAXFollowUpEmail1", viewModel);
                default:
                    return this.View(viewModel);
            }
        }
    }
}