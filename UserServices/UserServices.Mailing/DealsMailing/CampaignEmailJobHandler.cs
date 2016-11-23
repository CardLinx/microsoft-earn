//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Handler for Campaign Email
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using LoMo.UserServices.DataContract;
    using Lomo.Logging;
    using Microsoft.WindowsAzure;
    using Microsoft.Azure;

    /// <summary>
    /// Handler for Campaign Email
    /// </summary>
    public class CampaignEmailJobHandler : IEmailJobHandler
    {
        #region Consts

        /// <summary>
        /// User Services Address setting
        /// </summary>
        private const string UserServicesAddress = "LoMo.UserServices.Address";

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

            _userServicesAddress = new Uri(CloudConfigurationManager.GetSetting(UserServicesAddress));
            _userServicesClient = new UserServiceClient(_userServicesAddress);

            _emailFromAddress = CloudConfigurationManager.GetSetting(EmailFromAddress);
            _emailFromAddressTestAccount = CloudConfigurationManager.GetSetting(EmailFromAddressTestAccount);
            _emailFromDisplay = CloudConfigurationManager.GetSetting(FromDisplay);

            _emailContentCreator = new CampaignEmailContentCreator();
            _emailContentCreator.Initialize();

            Log.Verbose("Initialized {0}", this.GetType().Name);
        }

        /// <summary>
        /// Handles the Deals email job
        /// </summary>
        /// <param name="emailCargo">Deals email cargo</param>
        public void Handle(EmailCargo emailCargo)
        {
            CampaignEmailCargo campaignEmailCargo = emailCargo as CampaignEmailCargo;
            if (campaignEmailCargo != null)
            {
                EmailContent emailContent = PrepareEmail(campaignEmailCargo);
                if (emailContent != null)
                {
                    bool isTest = (campaignEmailCargo.Hints != null) && campaignEmailCargo.Hints.IsTest;
                    SendEmailRequest request = new SendEmailRequest
                        {
                            Content = emailContent,
                            ToList = new List<string> {campaignEmailCargo.EmailAddress},
                            IsTest = isTest
                        };

                    // Send the email
                    this._userServicesClient.SendEmail(campaignEmailCargo.Id, request, null);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the email content for deals email
        /// </summary>
        /// <param name="campaignEmailCargo">Deals Email cargo</param>
        /// <returns>EmailContent for deals email</returns>
        private EmailContent PrepareEmail(CampaignEmailCargo campaignEmailCargo)
        {
            EmailContent emailContent = null;
            EmailData emailData = _emailContentCreator.GetContent(campaignEmailCargo);
            if (emailData != null)
            {
                bool isTest = (campaignEmailCargo.Hints != null) && campaignEmailCargo.Hints.IsTest;
                emailContent = new EmailContent
                    {
                        From = isTest ? this._emailFromAddressTestAccount : this._emailFromAddress,
                        FromDisplay = this._emailFromDisplay,
                        Subject = emailData.Subject,
                        HtmlBody = emailData.HtmlBody,
                        TextBody = emailData.TextBody,
                        Category = campaignEmailCargo.Campaign
                    };
            }

            return emailContent;
        }

        #endregion
    }
}