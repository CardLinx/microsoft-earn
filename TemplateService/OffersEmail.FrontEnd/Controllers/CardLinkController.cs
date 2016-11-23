//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
// 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    /// The CardLink email templates controller
    /// </summary>
    public class CardLinkController : Controller
    {
        /// <summary>
        /// Sign Up Action.
        /// </summary>
        /// <returns>The Sign Up view</returns>
        [HttpPost]
        public ActionResult SignUp()
        {
            ViewBag.campaign = "signup";
            ViewBag.referrer = "BO_NOTIFY";

            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var userName = string.Empty;
                var merchantName = string.Empty;

                var json = JsonConvert.DeserializeObject(content) as JObject;
                if (json != null)
                {
                    userName = json["user_name"].Value<string>();
                    merchantName = json["merchant_name"].Value<string>();
                }

                if (string.IsNullOrWhiteSpace(merchantName))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "user_name or merchant_name is missing");
                }

                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = string.Empty;
                }

                var acceptTypes = this.Request.AcceptTypes;
                if (acceptTypes != null && (acceptTypes.Length > 0
                                                    && acceptTypes[0].Equals("text/plain", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return Content(string.Format(CardLink.SignUpEmailTextContent, merchantName));
                }

                return this.View(new CardLinkVM { UserName = userName, MerchantName = merchantName });
            }
        }

        /// <summary>
        /// Linked Action.
        /// </summary>
        /// <returns>The Linked view</returns>
        [HttpPost]
        public ActionResult Linked()
        {
            ViewBag.campaign = "linked";
            ViewBag.referrer = "BO_NOTIFY";
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var userName = string.Empty;
                var merchantName = string.Empty;
                var discountSummary = string.Empty;

                var json = JsonConvert.DeserializeObject(content) as JObject;
                if (json != null)
                {
                    userName = json["user_name"].Value<string>();
                    merchantName = json["merchant_name"].Value<string>();
                    discountSummary = json["discount_summary"].Value<string>();
                }

                if (string.IsNullOrWhiteSpace(merchantName) || string.IsNullOrWhiteSpace(discountSummary))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "user_name or merchant_name or discount_summary is missing");
                }

                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = string.Empty;
                }

                var acceptTypes = this.Request.AcceptTypes;
                if (acceptTypes != null && (acceptTypes.Length > 0
                                                    && acceptTypes[0].Equals("text/plain", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return Content(string.Format(CardLink.LinkedEmailTextContent, userName, merchantName, discountSummary));
                }

                return this.View(new CardLinkVM { UserName = userName, MerchantName = merchantName, DiscountSummary = discountSummary });
            }
        }

        /// <summary>
        /// Auth Action.
        /// </summary>
        /// <returns>The Auth view</returns>
        [HttpPost]
        public ActionResult Auth()
        {
            ViewBag.campaign = "auth";
            return GetAuthOrSettleResult();
        }

        /// <summary>
        /// Auth SMS template.
        /// </summary>
        /// <returns>The Auth SMS template.</returns>
        [HttpPost]
        public ActionResult AuthSms()
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();

                var merchantName = string.Empty;
                var discountSummary = string.Empty;

                var json = JsonConvert.DeserializeObject(content) as JObject;
                if (json != null)
                {
                    merchantName = json["merchant_name"].Value<string>();
                    discountSummary = json["discount_summary"].Value<string>();
                }

                if (string.IsNullOrWhiteSpace(merchantName)
                    || string.IsNullOrWhiteSpace(discountSummary))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "merchant_name or discount_summary is missing");
                }

                return Content(string.Format(CardLink.AuthSmsTextContent, merchantName, discountSummary));
            }
        }

        /// <summary>
        /// Settle Action.
        /// </summary>
        /// <returns>The Settle view</returns>
        [HttpPost]
        public ActionResult Settle()
        {
            ViewBag.campaign = "settle";
            return GetAuthOrSettleResult();
        }

        /// <summary>
        /// Complete Profile Action.
        /// </summary>
        /// <returns>The Complete Profile view</returns>
        [HttpPost]
        public ActionResult CompleteProfile()
        {
            ViewBag.campaign = "complete";
            ViewBag.referrer = "BO_NOTIFY";

            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var userName = string.Empty;

                var json = JsonConvert.DeserializeObject(content) as JObject;
                if (json != null)
                {
                    userName = json["user_name"].Value<string>();
                }

                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = string.Empty;
                }

                return this.View(new CardLinkVM { UserName = userName });
            }
        }

        /// <summary>
        /// Add Card Action.
        /// </summary>
        /// <returns>The Add Card view</returns>
        [HttpPost]
        public ActionResult AddCard()
        {
            ViewBag.campaign = "add";
            ViewBag.referrer = "BO_NOTIFY";
            ViewBag.doNotShowMore = true;

            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var last_four_digits = string.Empty;
                var card_type = string.Empty;
                var unauthenticateduserStatus = string.Empty;

                var json = JsonConvert.DeserializeObject(content) as JObject;
                if (json != null)
                {
                    last_four_digits = json["last_four_digits"].Value<string>();
                    card_type = json["card_type"].Value<string>();
                    if (json["unauthenticated_user_status"] != null)
                    {
                        unauthenticateduserStatus = json["unauthenticated_user_status"].Value<string>();
                    }
                }

                if (string.IsNullOrWhiteSpace(last_four_digits))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "last_four_digits is missing");
                }

                if (string.IsNullOrWhiteSpace(card_type))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "card_type is missing");
                }

                if (string.Compare(unauthenticateduserStatus, "UnConfirmedEmail") == 0)
                {
                    // unverified user
                    ViewBag.CardAddedMessage = CardLink.AddCard_ThanksUnverified;
                    ViewBag.isUnVerifiedUser = true;
                }
                else
                {
                   ViewBag.CardAddedMessage = string.Format(CardLink.AddCard_Thanks, card_type, last_four_digits);
                   ViewBag.isUnVerifiedUser = false;
                }

                return this.View(new CardLinkVM { LastFourDigits = last_four_digits });
            }
        }

        /// <summary>
        /// Add Card Action.
        /// </summary>
        /// <returns>The Add Card sms view</returns>
        [HttpPost]
        public ActionResult AddCardSms()
        {
            ViewBag.campaign = "add";
            ViewBag.referrer = "BO_NOTIFY";

            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var last_four_digits = string.Empty;
                var unauthenticateduserStatus = string.Empty;
                var card_type = string.Empty;

                var json = JsonConvert.DeserializeObject(content) as JObject;
                if (json != null)
                {
                    last_four_digits = json["last_four_digits"].Value<string>();
                    card_type = json["card_type"].Value<string>();

                    if (json["unauthenticated_user_status"] != null)
                    {
                        unauthenticateduserStatus = json["unauthenticated_user_status"].Value<string>();
                    }
                }

                if (string.IsNullOrWhiteSpace(last_four_digits))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "last_four_digits is missing");
                }

                if (string.IsNullOrWhiteSpace(card_type))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "card_type is missing");
                }

                if (string.Compare(unauthenticateduserStatus, "UnConfirmedEmail") == 0)
                {
                    // unverified user
                    return Content(CardLink.AddCardSmsContentUnVerifiedUser);
                }

                return Content(string.Format(CardLink.AddCardSmsContent, card_type, last_four_digits));
            }
        }

        /// <summary>
        /// Delete card action.
        /// </summary>
        /// <returns>The DeleteCard view</returns>
        [HttpPost]
        public ActionResult DeleteCard()
        {
            ViewBag.campaign = "delete";
            ViewBag.referrer = "BO_NOTIFY";

            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var userName = string.Empty;
                var activeCardsCount = 0;

                var json = JsonConvert.DeserializeObject(content) as JObject;
                if (json != null)
                {
                    userName = json["user_name"].Value<string>();

                    var activeCardsCountObject = json["active_cards_count"];
                    if (activeCardsCountObject != null)
                    {
                        activeCardsCount = activeCardsCountObject.Value<int>();
                    }
                }

                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = string.Empty;
                }

                return this.View(new CardLinkVM { UserName = userName, ActiveCardsCount = activeCardsCount });
            }
        }

        #region Private members
        /// <summary>
        /// Gets the author settle result.
        /// </summary>
        /// <returns>
        /// The Auth or Settle view.
        /// </returns>
        private ActionResult GetAuthOrSettleResult()
        {
            ViewBag.referrer = "BO_NOTIFY";

            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            using (var reader = new StreamReader(Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var userName = string.Empty;
                var merchantName = string.Empty;
                var discountSummary = string.Empty;
                var creditAmount = string.Empty;
                var lastFourDigits = string.Empty;
                var unauthenticateduserStatus = string.Empty;
                var emailVerificationUrl = string.Empty;
                var accountLinkingUrl = string.Empty;
                var transactionId = string.Empty;
                var transactionDate = string.Empty;
                var userId = string.Empty;
                var partnerMerchantId = string.Empty;
                var partnerId = string.Empty;
                var dealId = string.Empty;
                var discountId = string.Empty;
                
                var json = JsonConvert.DeserializeObject(content) as JObject;

                if (json != null)
                {
                    userName = json["user_name"].Value<string>();
                    merchantName = json["merchant_name"].Value<string>();
                    discountSummary = json["discount_summary"].Value<string>();
                    creditAmount = json["credit_amount"].Value<string>();
                    lastFourDigits = json["last_four_digits"].Value<string>();

                    unauthenticateduserStatus = GetPropertyValue(json, "unauthenticated_user_status", string.Empty);
                    emailVerificationUrl = GetPropertyValue(json, "email_verification_url", string.Empty);
                    accountLinkingUrl = GetPropertyValue(json, "account_linking_url", string.Empty);
                    transactionId = GetPropertyValue(json, "transaction_id", string.Empty);
                    transactionDate = GetPropertyValue(json, "transaction_date", string.Empty);
                    userId = GetPropertyValue(json, "user_id", string.Empty);
                    partnerMerchantId = GetPropertyValue(json, "partner_merchant_id", string.Empty);
                    partnerId = GetPropertyValue(json, "partner_id", string.Empty);
                    dealId = GetPropertyValue(json, "deal_id", string.Empty);
                    discountId = GetPropertyValue(json, "discount_id", string.Empty);
                }

                if (string.IsNullOrWhiteSpace(merchantName)
                    || string.IsNullOrWhiteSpace(discountSummary)
                    || string.IsNullOrWhiteSpace(creditAmount)
                    || string.IsNullOrWhiteSpace(lastFourDigits))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "user_name or merchant_name or discount_summary or credit_amount or last_four_digits is missing");
                }

                if (string.Compare(unauthenticateduserStatus, "UnConfirmedEmail", StringComparison.OrdinalIgnoreCase) == 0 && string.IsNullOrEmpty(emailVerificationUrl))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "email_verification_url is missing.");
                }

                if (string.Compare(unauthenticateduserStatus, "UnLinkedEmail", StringComparison.OrdinalIgnoreCase) == 0 && string.IsNullOrEmpty(accountLinkingUrl))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "account_linking_url is missing.");
                }

                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = string.Empty;
                }

                var acceptTypes = this.Request.AcceptTypes;
                if (acceptTypes != null && (acceptTypes.Length > 0
                                                    && acceptTypes[0].Equals("text/plain", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var textFormat = string.Empty;

                    if (string.Compare(unauthenticateduserStatus, "UnConfirmedEmail", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        textFormat = Request.Path.IndexOf("Auth", StringComparison.InvariantCultureIgnoreCase) >= 0
                                                   ? CardLink.AuthEmailUnAuthenticatedSignUpEmailUnconfirmedTextContent
                                                   : CardLink.SettleEmailUnAuthenticatedSignUpEmailUnconfirmedTextContent;

                        return Content(string.Format(textFormat, userName, merchantName, discountSummary, emailVerificationUrl));
                    }

                    if (string.Compare(unauthenticateduserStatus, "UnLinkedEmail", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        textFormat = Request.Path.IndexOf("Auth", StringComparison.InvariantCultureIgnoreCase) >= 0
                                                  ? CardLink.AuthEmailUnAuthenticatedSignUpEmailUnlinkedTextContent
                                                  : CardLink.SettleEmailUnAuthenticatedSignUpEmailUnlinkedTextContent;

                        return Content(string.Format(textFormat, userName, merchantName, discountSummary, accountLinkingUrl));
                    }

                    textFormat = Request.Path.IndexOf("Auth", StringComparison.InvariantCultureIgnoreCase) >= 0
                                                  ? CardLink.AuthEmailTextContent
                                                  : CardLink.SettleEmailTextContent;

                    return Content(string.Format(textFormat, userName, merchantName, discountSummary, creditAmount, lastFourDigits));
                }

                var cardLinkVm = new CardLinkVM
                                 {
                                     UserName = userName,
                                     MerchantName = merchantName,
                                     DiscountSummary = discountSummary,
                                     CreditAmount = creditAmount,
                                     LastFourDigits = lastFourDigits,
                                     EmailConfirmationUrl = emailVerificationUrl,
                                     AccountLinkingUrl = accountLinkingUrl,
                                     TransactionId = transactionId,
                                     TransactionDate = transactionDate,
                                     UserId = userId,
                                     PartnerMerchantId = partnerMerchantId,
                                     PartnerId = partnerId,
                                     DealId = dealId,
                                     DiscountId = discountId
                                 };
                
                if (string.Compare(unauthenticateduserStatus, "UnConfirmedEmail", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ViewBag.showWarning = true;
                    
                    if (ViewBag.campaign == "auth")
                    {
                        return this.View("AuthForUnauthenticatedUserUnconfirmedEmail", cardLinkVm);
                    }
                    else
                    {
                        return this.View("SettleForUnauthenticatedUserUnconfirmedEmail", cardLinkVm);
                    }
                }

                if (string.Compare(unauthenticateduserStatus, "UnLinkedEmail", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ViewBag.showWarning = true;
                    
                    if (ViewBag.campaign == "auth")
                    {
                        return this.View("AuthForUnauthenticatedUserUnlinkedAccount", cardLinkVm);
                    }
                    else
                    {
                        return this.View("SettleForUnauthenticatedUserUnlinkedAccountEmail", cardLinkVm);
                    }
                }

                ////GenericPreprocessor.Process(cardLinkVm, true);

                return this.View(cardLinkVm);
            }
        }
        
        /// <summary>
        /// Gets the value of a property from JSON object
        /// </summary>
        /// <param name="jsonObject">JSON object</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="defaultValue">Default value of property</param>
        /// <returns>Property value</returns>
        private string GetPropertyValue(JObject jsonObject, string propertyName, string defaultValue = null)
        {
            var jsonToken = jsonObject[propertyName];
            var propertyValue = jsonToken != null ? jsonToken.Value<string>() : defaultValue;
            return propertyValue;
        }

        #endregion
    }
}