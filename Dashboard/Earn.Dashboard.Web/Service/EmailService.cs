//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;

namespace Earn.Dashboard.Web.Service
{
    public class EmailService
    {
        public static void Send(string emailHtml, string toList = null, string ccList = null, string subject = null)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["EmailFromAddress"], ConfigurationManager.AppSettings["EmailFromName"]),
                Subject = subject ?? ConfigurationManager.AppSettings["EmailSubject"]
            };

            if (string.IsNullOrWhiteSpace(toList))
                toList = ConfigurationManager.AppSettings["EmailToAddress"];
            if (!string.IsNullOrWhiteSpace(toList))
            {
                var to = toList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in to.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    mail.To.Add(new MailAddress(s));
                }
            }

            if (string.IsNullOrWhiteSpace(ccList))
                ccList = ConfigurationManager.AppSettings["EmailCcAddress"];
            if (!string.IsNullOrWhiteSpace(ccList))
            {
                var cc = ccList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in cc.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    mail.CC.Add(new MailAddress(s));
                }
            }

            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailHtml, null, MediaTypeNames.Text.Html));
            mail.Body = emailHtml;

            //Attach Agreement
            //if (agreementFiles != null)
            //{
            //    foreach (var file in agreementFiles)
            //    {
            //        mail.Attachments.Add(new Attachment(file));
            //    }
            //}

            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]))
            {
                Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SmtpUser"], ConfigurationManager.AppSettings["SmtpPassword"]),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            smtpClient.Send(mail);
        }
    }
}