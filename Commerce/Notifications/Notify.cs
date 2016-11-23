//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;
    using LoMo.UserServices.DataContract;
    using Newtonsoft.Json;
    using Users.Dal;

    /// <summary>
    /// Contains methods to notify users of actions taken with the Commerce system.
    /// </summary>
    public abstract class Notify
    {
        /// <summary>
        /// Initializes a new instance of the Notify class.
        /// </summary>
        /// <param name="context">
        /// The context for this API call.
        /// </param>
        protected Notify(CommerceContext context)
        {
            Context = context;
            UsersDal = PartnerFactory.UsersDal(Context.Config);
            Uri userServicesClientUri = new Uri(Context.Config.UserServicesClientEndpoint);
            UserServicesClient = PartnerFactory.UserServicesClient(userServicesClientUri, Context.Config);
            SmsServiceUrl = Context.Config.SmsServiceClientEndpoint;
        }

        /// <summary>
        /// Sends notification to a user.
        /// </summary>
        public abstract void SendNotification();

        /// <summary>
        /// Sends an email notification in response to an event.
        /// </summary>
        /// <param name="toAddress">
        /// E-mail address to which to send the notification.
        /// </param>
        /// <param name="content">
        /// Content of the email payload
        /// </param>
        /// <param name="isEarn">Is Earn</param>
        internal void SendEmailNotification(string toAddress, NotificationContent content, bool isEarn = false)
        {
            string fromAddress = isEarn ? FromAddressEarn : FromAddressClo;
            string fromDisplay = isEarn ? FromFieldTextEarn : FromFieldTextClo;

            // Build the send e-mail request.
            EmailContent emailContent = new EmailContent
            {
                From = fromAddress,
                FromDisplay = fromDisplay,
                Subject = content.Subject,
                HtmlBody = content.HtmlBody,
                TextBody = content.TextBody
            };

            SendEmailRequest sendEmailRequest = new SendEmailRequest
            {
                Content = emailContent,
                IsTest = false,
                ToList = new List<string> { toAddress }
            };

            // Send the e-mail.
            try
            {
                // Use a generous timeout to ensure best chance of success. Because this can be a time consuming call, consumers
                //  should run notifications on a separate thread.
                UserServicesClient.SendEmail(Context.Log.ActivityId, sendEmailRequest, new TimeSpan(0, 0, 0, 5, 0));
            }
            catch (HttpRequestException ex)
            {
                Context.Log.Warning("Unable to send notification. {0}", ex.Message);
            }

        }

        /// <summary>
        /// Send SMS Notification to the user.
        /// </summary>
        /// <param name="userId">
        /// The User Id of the user.
        /// </param>
        /// <param name="text">
        /// Actual sms text to be sent.
        /// </param>
        internal void SendSmsNotification(Guid userId, string text)
        {
            if (userId != Guid.Empty)
            {
                try
                {
                    SmsMessage message = new SmsMessage()
                    {
                        UserId = userId.ToString(),
                        From = FromPhoneNumber,
                        Body = text
                    };

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpContent content = new StringContent(JsonConvert.SerializeObject(message)))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };

                            Task<HttpResponseMessage> responseTask = client.PostAsync(SmsServiceUrl, content);
                            TimeSpan? requestTimeout = TimeSpan.FromSeconds(5);

                            if (!responseTask.Wait(requestTimeout.Value))
                            {
                                throw new TimeoutException(String.Format("Operation didn't complete within the defined " +
                                                                         "timeout. Timeout={0}", requestTimeout));
                            }
                            responseTask.Result.EnsureSuccessStatusCode();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Context.Log.Warning("Unable to send sms notification. {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Get the canonical user object from the Users DAL if it exists. Otherwise, creates an empty user with safe default
        /// values.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user to load.
        /// </param>
        /// <return>
        /// The User Object
        /// </return>
        protected Users.Dal.DataModel.User RetrieveUser(Guid userId)
        {
            // Attempt to load the user from the Users DAL.
            Users.Dal.DataModel.User user = UsersDal.GetUserByUserId(userId);

            // Set safe default values if canonical user object does not exist or is not fully populated.
            if (user == null)
            {
                user = new Users.Dal.DataModel.User();
            }

            if (user.Name == null)
            {
                user.Name = String.Empty;
            }

            if (user.Email == null)
            {
                user.Email = String.Empty;
            }

            // Place only first name in salutation. If there is no name, make sure no space will be added.
            string[] nameParts = user.Name.Split(' ');
            SalutationName = String.Format(" {0}", nameParts[0]);
            if (SalutationName == " ")
            {
                SalutationName = String.Empty;
            }

            return user;
        }

        /// <summary>
        /// Class to define contract to send sms message.
        /// </summary>
        internal class SmsMessage
        {
            /// <summary>
            /// Gets or sets User Id.
            /// </summary>
            [JsonProperty(PropertyName = "userId")]
            public string UserId { get; set; }

            /// <summary>
            /// Gets or sets From Phone Number.
            /// </summary>
            [JsonProperty(PropertyName = "from")]
            public string From { get; set; }

            /// <summary>
            /// Gets or sets the text of the message.
            /// </summary>
            [JsonProperty(PropertyName = "body")]
            public string Body { get; set; }
        }

        /// <summary>
        /// Service endpoint for SMS Service.
        /// </summary>
        internal string SmsServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the context for this API call.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the name to use in the salutation sent within the notification.
        /// </summary>
        protected string SalutationName { get; set; }

        /// <summary>
        /// The key under which the first name placeholder value can be found.
        /// </summary>
        protected const string FirstName = "first";

        /// <summary>
        /// The key under which the offer title placeholder value can be found.
        /// </summary>
        protected const string OfferTitle = "offerTitle";

        /// <summary>
        /// The key under which the merchant placeholder value can be found.
        /// </summary>
        protected const string Merchant = "merchant";

        /// <summary>
        /// Gets or sets the Users Data Access Layer object.
        /// </summary>
        protected IUsersDal UsersDal { get; set; }

        /// <summary>
        /// Gets or sets the UserServicesClient through which e-mail notifications can be sent.
        /// </summary>
        private IUserServicesClient UserServicesClient { get; set; }

        /// <summary>
        /// The e-mail address from which notifications originate.
        /// </summary>
        private const string FromAddressClo = "cardlinked@bingoffers.com";

        /// <summary>
        /// The text to place in the From field in notification e-mails.
        /// </summary>
        private const string FromFieldTextClo = "Bing Offers";

        /// <summary>
        /// Phone number from which to send SMS messages.
        /// </summary>
        private const string FromPhoneNumber = "+12064557290";

        /// <summary>
        /// The e-mail address from which notifications originate.
        /// </summary>
        private const string FromAddressEarn = "earn@microsoft.com";

        /// <summary>
        /// The text to place in the From field in notification e-mails.
        /// </summary>
        private const string FromFieldTextEarn = "Earn By Microsoft";
    }
}