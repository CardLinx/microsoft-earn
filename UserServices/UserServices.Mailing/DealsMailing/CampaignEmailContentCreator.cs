//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the Campaign Email content creator
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using LoMo.Templating;
    using LoMo.UserServices.DataContract;
    using OffersEmail.DataContracts;

    /// <summary>
    /// Defines the Campaign Email content creator
    /// </summary>
    public class CampaignEmailContentCreator : IEmailContentCreator
    {
        #region Data Members

        /// <summary>
        /// The template model creator.
        /// </summary>
        private CampaignTemplateCreator _templateModelCreator;

        #endregion

        public void Initialize()
        {
            _templateModelCreator = new CampaignTemplateCreator();
        }

        /// <summary>
        /// The get content.
        /// </summary>
        /// <param name="emailCargo">
        /// The email Job.
        /// </param>
        /// <returns>
        /// The <see cref="EmailContent"/>.
        /// </returns>
        /// <exception cref="TemplateRenderException">
        /// error while rendering the template
        /// </exception>
        public EmailData GetContent(object emailCargo)
        {
            EmailData emailData = null;
            CampaignEmailCargo campaignEmailCargo = emailCargo as CampaignEmailCargo;
            if (campaignEmailCargo != null)
            {
                string locationId = campaignEmailCargo.LocationId;
                CampaignTemplateData campaignTemplateData = new CampaignTemplateData
                    {
                        EmailAddress = campaignEmailCargo.EmailAddress,
                        UnsubscribeUrl = campaignEmailCargo.UnsubscribeUrl,
                        LocationId = locationId,
                        Content = campaignEmailCargo.Content,
                        IncludeBusinessNames = campaignEmailCargo.IncludeBusinessNames
                    };

                var model = this._templateModelCreator.GenerateModel(campaignTemplateData);
                if (model != null)
                {
                    EmailRenderingClient<CampaignDataContract> emailRenderingClient = new EmailRenderingClient
                        <CampaignDataContract>
                        {
                            EmailRenderingServiceUrl = campaignEmailCargo.EmailRenderingServiceAddress
                        };
                    emailData = new EmailData
                        {
                            Subject = campaignEmailCargo.Subject,
                            HtmlBody = emailRenderingClient.RenderHtml(model)
                        };
                }
            }

            return emailData;
        }
    }
}