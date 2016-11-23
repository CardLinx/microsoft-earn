//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Class to create notification content, via interaction with underlying templating service
    /// </summary>
    public class NotificationContentCreator : INotificationContentCreator
    {
        /// <summary>
        /// Create Auth Email Content
        /// </summary>
        /// <param name="data">
        /// Auth Email Notification Data
        /// </param>
        /// <param name="subjectLine">
        /// Email subect
        /// </param>
        /// <param name="templatePath">Template Path</param>
        /// <returns>
        /// Notification content
        /// </returns>
        public async Task<NotificationContent> CreateAuthEmailContentAsync(AuthEmailNotificationData data, string subjectLine, string templatePath = authEmailTemplatePathClo)
        {
            try
            {
                return await CreateEmailContentAsync(
                         JsonConvert.SerializeObject(data),
                         TemplateServiceBaseAddress + templatePath,
                         subjectLine).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Logger.Warning("Unable to create content for Auth Email \r\n" +
                               "Data : \r\n" +
                               "{0}", data, exception);
                throw;
            }
        }

        /// <summary>
        /// Create Auth Sms Content
        /// </summary>
        /// <param name="data">
        /// Auth Sms Notification Data
        /// </param>
        /// <param name="templatePath">Template path</param>
        /// <returns>
        /// Notification content
        /// </returns>
        public async Task<NotificationContent> CreateAuthSmsContentAsync(AuthSmsNotificationData data, string templatePath = authSmsTemplatePathClo)
        {
            try
            {
                string textBody = await RetrieveTemplateAsync(
                                    TemplateServiceBaseAddress + templatePath,
                                    JsonConvert.SerializeObject(data)).ConfigureAwait(false);

                NotificationContent content = new NotificationContent()
                {
                    TextBody = textBody,
                };

                return content;
            }
            catch (Exception exception)
            {
                Logger.Warning("Unable to create content for Auth Sms \r\n" +
                               "Data : \r\n" +
                               "{0}", data, exception);
                throw;
            }
        }

        /// <summary>
        /// Create Add Card Email Content
        /// </summary>
        /// <param name="data">
        /// Add Card Email Notification Data
        /// </param>
        /// <param name="subjectLine">
        /// Email subect
        /// </param>
        /// <returns>
        /// Notification content
        /// </returns>
        public async Task<NotificationContent> CreateAddCardEmailContentAsync(AddCardEmailNotificationData data, string subjectLine)
        {
            try
            {
                return await CreateEmailContentAsync(
                         JsonConvert.SerializeObject(data),
                         TemplateServiceBaseAddress + addCardEmailTemplatePath,
                         subjectLine).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Logger.Warning("Unable to create content for Auth Email \r\n" +
                               "Data : \r\n" +
                               "{0}", data, exception);
                throw;
            }
        }


        /// <summary>
        /// Create Add Card Sms Content
        /// </summary>
        /// <param name="data">
        /// Add Card Sms Notification Data
        /// </param>
        /// <returns>
        /// Notification content
        /// </returns>
        public async Task<NotificationContent> CreateAddCardSmsContentAsync(AddCardSmsNotificationData data)
        {
            try
            {
                string textBody = await RetrieveTemplateAsync(
                                    TemplateServiceBaseAddress + addCardSmsTemplatePath,
                                    JsonConvert.SerializeObject(data)).ConfigureAwait(false);

                NotificationContent content = new NotificationContent()
                {
                    TextBody = textBody,
                };

                return content;
            }
            catch (Exception exception)
            {
                Logger.Warning("Unable to create content for Auth Sms \r\n" +
                               "Data : \r\n" +
                               "{0}", data, exception);
                throw;
            }

        }

        /// <summary>
        /// Generic Method to Create Email Content
        /// </summary>
        /// <param name="payload">
        /// Payload of the request for the template service
        /// </param>
        /// <param name="path">
        /// Path of the template service
        /// </param>
        /// <param name="subjectLine">
        /// Subject Line of the content
        /// </param>
        /// <returns>
        /// Notification Content
        /// </returns>
        private async Task<NotificationContent> CreateEmailContentAsync(string payload, string path, string subjectLine)
        {
            string htmlBody = await RetrieveTemplateAsync(
                                   path,
                                   payload).ConfigureAwait(false);

            string textBody = await RetrieveTemplateAsync(
                                    path,
                                    payload,
                                    "text/plain").ConfigureAwait(false);

            NotificationContent content = new NotificationContent()
            {
                HtmlBody = htmlBody,
                TextBody = textBody,
                Subject = subjectLine
            };

            return content;
        }

        /// <summary>
        /// Retrieves a template from the template service
        /// </summary>
        /// <param name="uri">
        /// Uri to send request to
        /// </param>
        /// <param name="payload">
        /// Payload of the data to be put in template
        /// </param>
        /// <param name="acceptHeaderValue">
        /// Accept header if needed
        /// </param>
        /// <returns>
        /// Populated template as string
        /// </returns>
        private async Task<string> RetrieveTemplateAsync(string uri, string payload, string acceptHeaderValue = null)
        {
            HttpClient client = new HttpClient();
            if (acceptHeaderValue != null)
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeaderValue));
            }
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(uri, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Base uri address of the template service
        /// </summary>
        public string TemplateServiceBaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the commerce logger
        /// </summary>
        public CommerceLog Logger { get; set; }

        /// <summary>
        /// Relative path for First Claimed Deal Template
        /// </summary>
        private const string firstClaimedDealTemplatePath = "/CardLink/SignUp";

        /// <summary>
        /// Relative Path for Subsequent Claimed Deal Template
        /// </summary>
        private const string subsequentClaimedDealTemplatePath = "/CardLink/Linked";

        /// <summary>
        /// Relative Path for Auth Sms Template
        /// </summary>
        private const string authSmsTemplatePathClo = "/CardLink/AuthSms";

        /// <summary>
        /// Relative Path for Auth Email Template
        /// </summary>
        private const string authEmailTemplatePathClo = "/CardLink/Auth";

        /// <summary>
        /// Relative Path for Add Card Email Template
        /// </summary>
        private const string addCardEmailTemplatePath = "/CardLink/AddCard";

        /// <summary>
        /// Relative Path for Add Card Sms Template
        /// </summary>
        private const string addCardSmsTemplatePath = "/CardLink/AddCardSms";
    }
}