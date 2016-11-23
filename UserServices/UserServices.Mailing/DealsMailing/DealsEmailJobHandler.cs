//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Handler for Deals Email
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using LoMo.UserServices.DataContract;
    using LoMo.UserServices.Storage.UserHistory;
    using Microsoft.WindowsAzure;
    using Lomo.Logging;
    using Microsoft.Azure;

    /// <summary>
    /// Handler for Deals Email
    /// </summary>
    public class DealsEmailJobHandler : IEmailJobHandler
    {
        #region Consts

        /// <summary>
        /// User Services Address setting
        /// </summary>
        private const string UserServicesAddress = "LoMo.UserServices.Address";

        /// <summary>
        /// The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.UserServices.ConnectionString";

        /// <summary>
        /// from address key
        /// </summary>
        private const string EmailFromAddress = "LoMo.DealsMailing.EmailFromAddres";

        /// <summary>
        /// from address for test account
        /// </summary>
        private const string EmailFromAddressTestAccount = "LoMo.EmailFromAddresTestAccount";

        /// <summary>
        /// from display key
        /// </summary>
        private const string FromDisplay = "LoMo.EmailJobs.EmailFromDisplay";

        #endregion

        #region Member Variables

        /// <summary>
        /// User History storage
        /// </summary>
        private IUserHistoryStorage _userHistoryStorage;

        /// <summary>
        /// URI for user services
        /// </summary>
        private Uri _userServicesAddress;

        /// <summary>
        /// User services client
        /// </summary>
        private IUserServicesClient _userServicesClient;

        /// <summary>
        /// Deals email content creator 
        /// </summary>
        private IEmailContentCreator _emailContentCreator;

        /// <summary>
        /// From address for deals email
        /// </summary>
        private string _emailFromAddress;

        /// <summary>
        /// From address for deals email, if using sendgrid test account
        /// </summary>
        private string _emailFromAddressTestAccount;

        /// <summary>
        /// Display name for from address
        /// </summary>
        private string _emailFromDisplay;

        #endregion

        #region Public Methods

        /// <summary>
        /// Intializes the Deals Email handler
        /// </summary>
        public void Initialize()
        {
            Log.Verbose("Initializing {0}", this.GetType().Name);

            string storageSetting = CloudConfigurationManager.GetSetting(StorageSetting);
            _userHistoryStorage = new UserHistoryStorage(storageSetting);

            _userServicesAddress = new Uri(CloudConfigurationManager.GetSetting(UserServicesAddress));
            _userServicesClient = new UserServiceClient(_userServicesAddress);

            _emailFromAddress = CloudConfigurationManager.GetSetting(EmailFromAddress);
            _emailFromAddressTestAccount = CloudConfigurationManager.GetSetting(EmailFromAddressTestAccount);
            _emailFromDisplay = CloudConfigurationManager.GetSetting(FromDisplay);

            _emailContentCreator = new DealsEmailContentCreator();
            _emailContentCreator.Initialize();

            Log.Verbose("Initialized {0}", this.GetType().Name);
        }

        /// <summary>
        /// Handles the Deals email job
        /// </summary>
        /// <param name="emailCargo">Deals email cargo</param>
        public void Handle(EmailCargo emailCargo)
        {
            DealsEmailCargo dealsEmailCargo = emailCargo as DealsEmailCargo;
            if (dealsEmailCargo != null)
            {
                EmailContent emailContent = PrepareEmail(dealsEmailCargo);
                bool isTest = (dealsEmailCargo.Hints != null) && dealsEmailCargo.Hints.IsTest;
                SendEmailRequest request = new SendEmailRequest
                    {
                        Content = emailContent,
                        ToList = new List<string> { dealsEmailCargo.EmailAddress },
                        IsTest = isTest
                    };

                // Send the email
                this._userServicesClient.SendEmail(dealsEmailCargo.Id, request, null);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the email content for deals email
        /// </summary>
        /// <param name="dealsEmailCargo">Deals Email cargo</param>
        /// <returns>EmailContent for deals email</returns>
        private EmailContent PrepareEmail(DealsEmailCargo dealsEmailCargo)
        {
            EmailData emailData = _emailContentCreator.GetContent(dealsEmailCargo);
            bool isTest = (dealsEmailCargo.Hints != null) && dealsEmailCargo.Hints.IsTest;
            EmailContent emailContent = new EmailContent
                {
                    From = isTest ? this._emailFromAddressTestAccount : this._emailFromAddress,
                    FromDisplay = this._emailFromDisplay,
                    Subject = emailData.Subject,
                    HtmlBody = emailData.HtmlBody,
                    TextBody = emailData.TextBody,
                    Category = dealsEmailCargo.Campaign
                };

            if (!dealsEmailCargo.Hints.IsTestEmail)
            {
                // Saving the email data in the history table
                UserEmailEntity emailToSend = new UserEmailEntity(dealsEmailCargo.UserId, dealsEmailCargo.LocationId,DateTime.UtcNow, EmailType.WeeklyDeal);
                emailToSend.SetSerializedPayload(new UserEmailPayload {DealIds = emailData.DealIds});
                this._userHistoryStorage.SaveUserEmailEntity(emailToSend);
            }

            return emailContent;
        }

        #endregion
    }
}