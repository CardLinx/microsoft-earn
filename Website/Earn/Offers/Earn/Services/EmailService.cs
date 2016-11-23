//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Models;
using Microsoft.Azure;
using SendGrid;
using SendGrid.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Earn.Offers.Earn.Services
{
    public class EmailService
    {
        public static async Task SendEmail(EmailInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            if (info.From == null || info.To == null || info.To.Count < 1)
            {
                throw new ArgumentException("From or To values are missing", "emailInformation");
            }

            string userName = CloudConfigurationManager.GetSetting("SendGridUsername");
            string password = CloudConfigurationManager.GetSetting("SendGridPassword");
            SMTP smtpInstance = SMTP.GetInstance(new NetworkCredential(userName, password), port: 587);

            IMail mail = Mail.GetInstance();
            mail.From = new MailAddress(info.From, info.FromDisplayName);
            mail.Subject = info.Subject;
            foreach (string to in info.To)
            {
                mail.AddTo(to);
            }

            mail.DisableGoogleAnalytics();
            if (!string.IsNullOrEmpty(info.HtmlBody))
            {
                mail.Html = info.HtmlBody;
            }

            if (!string.IsNullOrEmpty(info.TextBody))
            {
                mail.Text = info.TextBody;
            }

            if (!string.IsNullOrEmpty(info.ReplyTo))
            {
                mail.ReplyTo = new[] { new MailAddress(info.ReplyTo) };
            }

            if (!string.IsNullOrEmpty(info.Category))
            {
                mail.SetCategory(info.Category);
            }

            await smtpInstance.DeliverAsync(mail);
        }
    }
}