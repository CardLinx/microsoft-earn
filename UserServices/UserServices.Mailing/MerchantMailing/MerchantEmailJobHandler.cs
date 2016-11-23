//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailTemplatesFetcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using LoMo.UserServices.DataContract;
    using Microsoft.WindowsAzure;
    using System.Collections.Generic;
    using Lomo.Logging;
    using Microsoft.Azure;

    /// <summary>
    /// 
    /// </summary>
    public class MerchantEmailJobHandler : IEmailJobHandler
    {
        #region Consts

        /// <summary>
        /// The message queue name.
        /// </summary>
        private const string UserServicesAddress = "LoMo.UserServices.Address";

        /// <summary>
        /// from address for test account
        /// </summary>
        private const string EmailFromAddressTestAccount = "LoMo.EmailFromAddresTestAccount";

        /// <summary>
        /// from display key
        /// </summary>
        private const string FromDisplay = "LoMo.EmailJobs.EmailFromDisplay";

        /// <summary>
        /// Email from address for Merchant Emails
        /// </summary>
        private const string MerchantEmailFromAddress = "LoMo.MerchantMailing.EmailFromAddres";

        #endregion

        #region Member Variables

        /// <summary>
        /// URI for user services
        /// </summary>
        private Uri _userServicesAddress;

        /// <summary>
        ///  User services client
        /// </summary>
        private IUserServicesClient _userServicesClient;

        /// <summary>
        /// From address for deals email, if using sendgrid test account
        /// </summary>
        private string _emailFromAddressTestAccount;

        /// <summary>
        /// Display name for from address
        /// </summary>
        private string _emailFromDisplay;

        /// <summary>
        /// From address for merchant email
        /// </summary>
        private string _merchantEmailFromAddress;

        /// <summary>
        /// Merchant email content creator 
        /// </summary>
        private IEmailContentCreator _emailContentCreator;

        #endregion

        #region Public Methods

        /// <summary>
        /// Intializes the Merchant Email handler
        /// </summary>
        public void Initialize()
        {
            Log.Verbose("Initializing {0}", this.GetType().Name);

            _userServicesAddress = new Uri(CloudConfigurationManager.GetSetting(UserServicesAddress));
            _userServicesClient = new UserServiceClient(_userServicesAddress);
         
            _emailFromAddressTestAccount = CloudConfigurationManager.GetSetting(EmailFromAddressTestAccount);
            _emailFromDisplay = CloudConfigurationManager.GetSetting(FromDisplay);
           
            _merchantEmailFromAddress = CloudConfigurationManager.GetSetting(MerchantEmailFromAddress);
            _emailContentCreator = new MerchantReportContentCreator();
            _emailContentCreator.Initialize();

            Log.Verbose("Initialized {0}", this.GetType().Name);
        }

        /// <summary>
        /// Handles the Merchant Email job
        /// </summary>
        /// <param name="emailCargo">Merchant Email cargo</param>
        public void Handle(EmailCargo emailCargo)
        {
            MerchantReportEmailCargo merchantReportEmailCargo = emailCargo as MerchantReportEmailCargo;
            if (merchantReportEmailCargo != null)
            {
                EmailData emailData = _emailContentCreator.GetContent(merchantReportEmailCargo);
                if (emailData != null)
                {
                    bool isTest = (merchantReportEmailCargo.Hints != null) && merchantReportEmailCargo.Hints.IsTest;
                    EmailContent emailContent = new EmailContent
                        {
                            From = isTest ? this._emailFromAddressTestAccount : this._merchantEmailFromAddress,
                            FromDisplay = this._emailFromDisplay,
                            Subject = emailData.Subject,
                            HtmlBody = emailData.HtmlBody,
                            TextBody = emailData.TextBody,
                            Category = merchantReportEmailCargo.Campaign
                        };
                    SendEmailRequest request = new SendEmailRequest
                        {
                            Content = emailContent,
                            ToList = new List<string> {merchantReportEmailCargo.EmailAddress},
                            IsTest = isTest
                        };

                    // Send the email
                    Log.Verbose("Sending email for the email job : {0}",emailCargo.ToString());
                    this._userServicesClient.SendEmail(merchantReportEmailCargo.Id, request, null);
                }
                JobFetcher.CompleteJob(merchantReportEmailCargo.Id);
            }
            else
            {
                Log.Error("Invalid handler for the email job : {0}", emailCargo.ToString());
            }
        }

        #endregion
    }
}