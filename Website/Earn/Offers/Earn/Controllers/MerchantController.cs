//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Helper;
using Earn.Offers.Earn.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Controllers
{
    public class MerchantController : Controller
    {
        public ActionResult ExhibitC()
        {
            return File(Server.MapPath("/Offers/Earn/Views/Merchant/Agreements/ExhibitC.htm"), "text/html");
        }

        public ActionResult ExhibitD()
        {
            return File(Server.MapPath("/Offers/Earn/Views/Merchant/Agreements/ExhibitD.htm"), "text/html");
        }

        public ActionResult InsertionOrder()
        {
            return View("~/offers/earn/views/merchant/insertionorder.cshtml", new MerchantModel());
        }

        [HttpPost]
        public ActionResult InsertionOrder([Bind(Include = "LegalCompanyName,DbaCompanyName,Name,Title,Address,EmailAddress,PhoneNumber,Terms,ReferredBy")] MerchantModel merchantModel)
        {
            if (ModelState.IsValid && merchantModel.Terms)
            {
                merchantModel.SignedOn = DateTime.UtcNow;
                SendEmail(MerchantEmail(merchantModel));
                return View("~/offers/earn/views/merchant/ThankYou.cshtml");
            }

            if (!merchantModel.Terms)
            {
                ModelState.AddModelError("Terms", "You must agree to the above terms and conditions before continuing.");
            }
            return View("~/offers/earn/views/merchant/insertionorder.cshtml", merchantModel);
        }

        private string MerchantEmail(MerchantModel merchantModel)
        {
            if (merchantModel == null)
            {
                return null;
            }

            ViewData.Model = merchantModel;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext, "~/offers/earn/views/merchant/Email.cshtml", null);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        private static void SendEmail(string emailHtml)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["EmailFromAddress"], ConfigurationManager.AppSettings["EmailFromName"]),
                Subject = ConfigurationManager.AppSettings["EmailSubject"]
            };

            mail.To.Add(ConfigurationManager.AppSettings["EmailToAddress"]);

            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailHtml, null, MediaTypeNames.Text.Html));
            mail.Body = emailHtml;

            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]))
            {
                Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SmtpUser"], ConfigurationManager.AppSettings["SmtpPassword"]),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtpClient.Send(mail);
        }
    }
}