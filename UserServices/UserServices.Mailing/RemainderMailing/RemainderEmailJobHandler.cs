//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Handler for remainder email
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LoMo.UserServices.Storage.UserHistory;
    using LoMo.UserServices.DataContract;
    using Lomo.Logging;
    using Microsoft.WindowsAzure;
    using Microsoft.Azure;

    /// <summary>
    /// Handler for remainder email
    /// </summary>
    public class RemainderEmailJobHandler : IEmailJobHandler
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

        /// <summary>
        /// The windows between remainder emails.
        /// </summary>
        private const string WindowBetweenRemainderEmails = "LoMo.DealsMailing.WindowBetweenRemainderEmails";

        /// <summary>
        /// The mail history lookback setting.
        /// </summary>
        private const string MailHistoryLookBackSetting = "LoMo.DealsMailing.MailHistoryLookback";

        #endregion

        #region Member Variables

        /// <summary>
        /// URI for user services
        /// </summary>
        private Uri _userServicesAddress;

        /// <summary>
        /// User History storage
        /// </summary>
        private IUserHistoryStorage _userHistoryStorage;

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

        /// <summary>
        /// Cool off period between consecutive remainder emails
        /// </summary>
        private TimeSpan _windowsBetweenRemainderEmails;

        /// <summary>
        /// Number of entries to check in the history table
        /// </summary>
        private int _mailHistoryLookback;

        #endregion


        /// <summary>
        /// Intializes the Remainder Email handler
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

            _windowsBetweenRemainderEmails = TimeSpan.Parse(CloudConfigurationManager.GetSetting(WindowBetweenRemainderEmails));
            _mailHistoryLookback = int.Parse(CloudConfigurationManager.GetSetting(MailHistoryLookBackSetting));
            _emailContentCreator = new RemainderEmailContentCreator();
            _emailContentCreator.Initialize();

            Log.Verbose("Initialized {0}", this.GetType().Name);
        }

        /// <summary>
        /// Handles the Remainder email job
        /// </summary>
        /// <param name="emailCargo">RemainderEmail cargo</param>
        public void Handle(EmailCargo emailCargo)
        {
            PromotionalEmailCargo remainderEmailCargo = emailCargo as PromotionalEmailCargo;
            if (remainderEmailCargo != null)
            {
                var emailHistoryEntities = this._userHistoryStorage.GetUserEmailEntities(remainderEmailCargo.UserId, _mailHistoryLookback).ToList();
                if (this.IsSendTimeWindowValid(emailHistoryEntities.FirstOrDefault(elem => string.Compare(elem.EmailType, EmailType.CompleteSignup.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0), remainderEmailCargo))
                {
                    EmailData emailData = _emailContentCreator.GetContent(remainderEmailCargo);
                    bool isTest = (remainderEmailCargo.Hints != null) && remainderEmailCargo.Hints.IsTest;
                    EmailContent emailContent = new EmailContent
                    {
                        From = isTest ? this._emailFromAddressTestAccount : this._emailFromAddress,
                        FromDisplay = this._emailFromDisplay,
                        Subject = emailData.Subject,
                        HtmlBody = emailData.HtmlBody,
                        TextBody = emailData.TextBody,
                        Category = remainderEmailCargo.Campaign
                    };
                    SendEmailRequest request = new SendEmailRequest
                    {
                        Content = emailContent,
                        ToList = new List<string> { remainderEmailCargo.EmailAddress },
                        IsTest = isTest
                    };

                    // Send the email
                    this._userServicesClient.SendEmail(remainderEmailCargo.Id, request, null);

                    PromotionalEmailType remainderType;
                    if (Enum.TryParse(remainderEmailCargo.PromotionalEmailType, true, out remainderType))
                    {
                        // Saving the email data in the history table
                        UserEmailEntity emailToSend = new UserEmailEntity(remainderEmailCargo.UserId, string.Empty, DateTime.UtcNow, EmailType.CompleteSignup);
                        this._userHistoryStorage.SaveUserEmailEntity(emailToSend);
                    }

                }
            }
        }

        /// <summary>
        /// The is send time window valid.
        /// </summary>
        /// <param name="lastEmailEntity">
        /// The last email entity.
        /// </param>
        /// <param name="remainderEmailCargo">
        /// The job.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsSendTimeWindowValid(UserEmailEntity lastEmailEntity, PromotionalEmailCargo remainderEmailCargo)
        {
            bool isSendTimeWindowValid = true;

            if (lastEmailEntity != null && lastEmailEntity.EmailDate > (DateTime.UtcNow - this._windowsBetweenRemainderEmails))
            {
                Log.Error(
                         "Can't send remainder email due to time windows constraint. Job Details=[{0}]; Last Email Date={1}; Window Between Emails={2}",
                         remainderEmailCargo.ToString(),
                         lastEmailEntity.EmailDate,
                         this._windowsBetweenRemainderEmails);

                isSendTimeWindowValid = false;
            }

            return isSendTimeWindowValid;
        }

    }
}