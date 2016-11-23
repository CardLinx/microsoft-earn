//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Handler for Confirmation emails
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Microsoft.WindowsAzure;
    using LoMo.UserServices.DataContract;
    using OffersEmail.DataContracts;
    using Users.Dal;
    using Users.Dal.DataModel;
    using Lomo.Logging;
    using Microsoft.Azure;

    /// <summary>
    /// Handler for Confirmation emails
    /// </summary>
    public class ConfirmationEmailJobHandler : IPriorityEmailJobHandler
    {
        #region Constants

        /// <summary>
        /// The confirmation from display name setting.
        /// </summary>
        private const string ConfirmationFromDisplayNameSetting = "LoMo.Confirmation.FromDisplayName";

        /// <summary>
        /// The confirmation from address setting.
        /// </summary>
        private const string ConfirmationFromAddressSetting = "LoMo.Confirmation.FromAddress";

        /// <summary>
        /// The confirmation email subject setting
        /// </summary>
        private const string ConfirmationEmailSubject = "LoMo.Confirmation.Subject";

        /// <summary>
        /// The account link confirmation subject setting
        /// </summary>
        private const string ALinkConfirmationSubject = "LoMo.AlinkConfirmation.Subject";

        /// <summary>
        /// The confirmation email category setting.
        /// </summary>
        private const string ConfirmationEmailCategorySetting = "LoMo.Confirmation.EmailCategory";

        /// <summary>
        /// The azure environment setting.
        /// </summary>
        private const string AzureEnvironmentSetting = "LoMo.AzureEnvironment";

        /// <summary>
        /// The message queue name.
        /// </summary>
        private const string UserServicesAddress = "LoMo.UserServices.Address";

        /// <summary>
        /// The unauthenticated email confirmation template url setting.
        /// </summary>
        private const string UnauthenticatedEmailConfirmationTemplateUrlSetting = "LoMo.UserEmail.UnauthenticatedConfirmationTemplateUrl";

        /// <summary>
        /// The authenticated email confirmation template url setting.
        /// </summary>
        private const string AuthenticatedEmailConfirmationTemplateUrlSetting = "LoMo.UserEmail.AuthenticatedConfirmationTemplateUrl";

        /// <summary>
        /// The authenticated email confirmation template url setting.
        /// </summary>
        private const string LinkEmailAccountTemplateUrlSetting = "LoMo.User.LinkEmailAccountTemplateUrl";

        #endregion

        #region Members

        /// <summary>
        /// Send grid campaign name for confirmation email
        /// </summary>
        private string _confirmationEmailCategory;

        /// <summary>
        /// From address for confirmation email
        /// </summary>
        private string _confirmationFromAddress;

        /// <summary>
        /// Display for confirmation email
        /// </summary>
        private string _confirmationFromDisplayName;

        /// <summary>
        /// Subject for confirmation email
        /// </summary>
        private string _confirmationEmailSubject;

        /// <summary>
        /// Subject for confirmation of account link code email
        /// </summary>
        private string _alinkConfirmationSubject;

        /// <summary>
        /// Azure environment
        /// </summary>
        private string _environment;

        /// <summary>
        /// Url for unauthenticated user email confirmation template
        /// </summary>
        private string _unauthenticatedEmailConfirmationTemplateUrl;

        /// <summary>
        /// Url for authenticated user email confirmation template
        /// </summary>
        private string _authenticatedEmailConfirmationTemplateUrl;

        /// <summary>
        /// template Url for sending email to unauthenticated user asking them to sign in with their MS/FB account.
        /// </summary>
        private string _linkEmailAccountTemplateUrl;

        /// <summary>
        /// URI for user services
        /// </summary>
        private Uri _userServicesAddress;

        /// <summary>
        ///  User services client
        /// </summary>
        private IUserServicesClient _userServicesClient;

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Initializes the handler
        /// </summary>
        public void Initialize()
        {
            Log.Verbose("Initializing {0}", this.GetType().Name);

            _confirmationEmailCategory = this.GetMandatorySetting(ConfirmationEmailCategorySetting);
            _confirmationFromAddress = this.GetMandatorySetting(ConfirmationFromAddressSetting);
            _confirmationFromDisplayName = this.GetMandatorySetting(ConfirmationFromDisplayNameSetting);
            _confirmationEmailSubject = this.GetMandatorySetting(ConfirmationEmailSubject);
            _alinkConfirmationSubject = this.GetMandatorySetting(ALinkConfirmationSubject);
            _environment = this.GetMandatorySetting(AzureEnvironmentSetting);
            _unauthenticatedEmailConfirmationTemplateUrl = this.GetMandatorySetting(UnauthenticatedEmailConfirmationTemplateUrlSetting);
            _authenticatedEmailConfirmationTemplateUrl = this.GetMandatorySetting(AuthenticatedEmailConfirmationTemplateUrlSetting);
            _linkEmailAccountTemplateUrl = this.GetMandatorySetting(LinkEmailAccountTemplateUrlSetting);
            _userServicesAddress = new Uri(CloudConfigurationManager.GetSetting(UserServicesAddress));
            _userServicesClient = new UserServiceClient(_userServicesAddress);

            Log.Verbose("Initialized {0}", this.GetType().Name);
        }

        /// <summary>
        /// Executes the confirmation email job
        /// </summary>
        /// <param name="priorityEmailCargo">Cargo for confirmation email</param>
        public void Handle(PriorityEmailCargo priorityEmailCargo)
        {
            ConfirmationEmailCargo confirmationEmailCargo = priorityEmailCargo as ConfirmationEmailCargo;
            if (confirmationEmailCargo != null)
            {
                Log.Verbose("Starting Confirmation email job : {0}", confirmationEmailCargo.ToString());
                EnvironmentType azureEnvironment;
                if (Enum.TryParse(_environment, true, out azureEnvironment))
                {
                    EmailConfirmationContract emailConfirmationContract = new EmailConfirmationContract();
                    EmailRenderingClient<EmailConfirmationContract> emailRenderingClient = new EmailRenderingClient<EmailConfirmationContract>();
                    switch (confirmationEmailCargo.EntityType)
                    {
                        case EntityType.AuthenticatedEmailAddress:
                            emailConfirmationContract.ConfirmationUrl = ConfirmationLinkGenerator.GetLink(confirmationEmailCargo.UserIdHash, confirmationEmailCargo.ConfirmationCode, EntityType.AuthenticatedEmailAddress, azureEnvironment).AbsoluteUri;
                            emailRenderingClient.EmailRenderingServiceUrl = this._authenticatedEmailConfirmationTemplateUrl;
                            break;
                        case EntityType.UnAuthenticatedEmailAddress:
                            emailConfirmationContract.ConfirmationUrl = ConfirmationLinkGenerator.GetLink(confirmationEmailCargo.UserIdHash, confirmationEmailCargo.ConfirmationCode, EntityType.UnAuthenticatedEmailAddress, azureEnvironment).AbsoluteUri;
                            emailRenderingClient.EmailRenderingServiceUrl = this._unauthenticatedEmailConfirmationTemplateUrl;
                            break;
                        case EntityType.AccountLink:
                            emailConfirmationContract.ConfirmationUrl = ConfirmationLinkGenerator.GetLink(confirmationEmailCargo.UserIdHash, confirmationEmailCargo.ConfirmationCode, EntityType.AccountLink, azureEnvironment).AbsoluteUri;
                            emailRenderingClient.EmailRenderingServiceUrl = this._linkEmailAccountTemplateUrl;
                            break;
                        default:
                            throw new InvalidEntityTypeException("Unknown entity type in Confirmation Emailcargo");
                    }
                    EmailContent emailContent = new EmailContent
                    {
                        From = this._confirmationFromAddress,
                        FromDisplay = this._confirmationFromDisplayName,
                        Subject = confirmationEmailCargo.EntityType == EntityType.AccountLink ? this._alinkConfirmationSubject : this._confirmationEmailSubject,
                        HtmlBody = emailRenderingClient.RenderHtml(emailConfirmationContract),
                        Category = this._confirmationEmailCategory
                    };
                    SendEmailRequest request = new SendEmailRequest
                    {
                        Content = emailContent,
                        ToList = new List<string> { confirmationEmailCargo.EmailAddress }
                    };

                    Log.Verbose("Sending email for the email job : {0}", confirmationEmailCargo.ToString());

                    // Send the email
                    this._userServicesClient.SendEmail(confirmationEmailCargo.Id, request, null);

                    Log.Verbose("Successfully sent email for the email job : {0}", confirmationEmailCargo.ToString());
                }
            }
            else
            {
                Log.Error("Error in executing confirmation email job : Invalid priority email cargo");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get setting from configuration file. Throw an error if the configuration not found
        /// </summary>
        /// <param name="keyName"> the key name</param>
        /// <returns> the configuration value</returns>
        private string GetMandatorySetting(string keyName)
        {
            string value = CloudConfigurationManager.GetSetting(keyName);
            if (value == null)
            {
                throw new ConfigurationErrorsException(string.Format("Key: {0} is missing is configuration file", keyName));
            }

            return value;
        }
    }

        #endregion

}