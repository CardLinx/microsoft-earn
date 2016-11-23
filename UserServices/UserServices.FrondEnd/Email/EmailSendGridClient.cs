//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email send grid client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Email
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Configuration;
    using Microsoft.WindowsAzure;

    using SendGrid;
    using SendGrid.Transport;
    using Microsoft.Azure;

    /// <summary>
    /// The email send grid client.
    /// </summary>
    internal class EmailSendGridClient : IEmailClient
    {
        #region Constants

        /// <summary>
        /// Send Grid Password Setting
        /// </summary>
        private const string SendGridPasswordSetting = "LoMo.SendGrid.Password";

        /// <summary>
        /// Send Grid User Setting
        /// </summary>
        private const string SendGridUserSetting = "LoMo.SendGrid.User";

        /// <summary>
        /// Send Grid Password Setting
        /// </summary>
        private const string SendGridTestPasswordSetting = "LoMo.SendGrid.TestPassword";

        /// <summary>
        /// Send Grid User Setting
        /// </summary>
        private const string SendGridTestUserSetting = "LoMo.SendGrid.TestUser";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="emailInformation">
        /// The email information.
        /// </param>
        /// <param name="correlationId">the correlation id</param>
        public void Send(EmailInformation emailInformation, Guid? correlationId)
        {
            if (emailInformation == null)
            {
                throw new ArgumentNullException("emailInformation");
            }

            if (emailInformation.From == null || emailInformation.To == null || emailInformation.To.Count < 1)
            {
                throw new ArgumentException("From or To values are missing", "emailInformation");
            }
            SMTP smtpInstance;
            string userName;
            string password;

            if (emailInformation.IsTest)
            {
                userName = GetSetting(SendGridTestUserSetting);
                password = GetSetting(SendGridTestPasswordSetting);
                smtpInstance = SMTP.GetInstance(new NetworkCredential(userName,password), port: 587);
            }
            else
            {
                userName = GetSetting(SendGridUserSetting);
                password = GetSetting(SendGridPasswordSetting);
                smtpInstance = SMTP.GetInstance(new NetworkCredential(userName, password), port: 587);
            }

            IMail mail = Mail.GetInstance();
            mail.From = new MailAddress(emailInformation.From, emailInformation.FromDisplayName);
            mail.Subject = emailInformation.Subject;
            foreach (string to in emailInformation.To)
            {
                mail.AddTo(to);
            }

            mail.DisableGoogleAnalytics();
            if (!string.IsNullOrEmpty(emailInformation.HtmlBody))
            {
                mail.Html = emailInformation.HtmlBody;
            }

            if (!string.IsNullOrEmpty(emailInformation.TextBody))
            {
                mail.Text = emailInformation.TextBody;
            }

            if (!string.IsNullOrEmpty(emailInformation.ReplayTo))
            {
                mail.ReplyTo = new[] { new MailAddress(emailInformation.ReplayTo) };
            }

            if (!string.IsNullOrEmpty(emailInformation.Category))
            {
                mail.SetCategory(emailInformation.Category);
            }

            Dictionary<string, string> uniqueIdentifiers;

            if (emailInformation.UniqueIdentifiers != null)
            {
                uniqueIdentifiers = new Dictionary<string, string>(emailInformation.UniqueIdentifiers);
            }
            else
            {
                uniqueIdentifiers = new Dictionary<string, string>();
            }

            if (correlationId.HasValue)
            {
                uniqueIdentifiers["correlation_id"] = correlationId.ToString();
            }

            if (uniqueIdentifiers.Count > 0)
            {
                mail.AddUniqueIdentifiers(uniqueIdentifiers);
            }

            smtpInstance.DeliverAsync(mail);
        }

        #region Private Methods

        /// <summary>
        /// Get setting from configuration file. Throw an error if the configuration not found
        /// </summary>
        /// <param name="keyName"> the key name</param>
        /// <returns> the configuration value</returns>
        private string GetSetting(string keyName)
        {
            string value = CloudConfigurationManager.GetSetting(keyName);
            if (value == null)
            {
                throw new ConfigurationErrorsException(string.Format("Key: {0} is missing is configuration file", keyName));
            }

            return value;
        }

        #endregion

        #endregion
    }
}