//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Creates the email content for remainder emails
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;

    /// <summary>
    /// Creates the email content for remainder emails
    /// </summary>
    public class RemainderEmailContentCreator : IEmailContentCreator
    {
        /// <summary>
        /// Intializes the RemainderEmail content creator
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Returns the email content for remainder email
        /// </summary>
        /// <param name="emailCargo">Remainder Email cargo</param>
        /// <returns>EmailData for the remainder email</returns>
        public EmailData GetContent(object emailCargo)
        {
            PromotionalEmailCargo remainderEmailCargo = emailCargo as PromotionalEmailCargo;
            EmailData emailData = null;
            if (remainderEmailCargo != null)
            {
                PromotionalEmailType remainderType;
                if (Enum.TryParse(remainderEmailCargo.PromotionalEmailType, true, out remainderType))
                {
                    switch (remainderType)
                    {
                        case PromotionalEmailType.CompleteSignup:
                            emailData = GetContentForCompleteSignup(remainderEmailCargo);
                            break;
                    }
                }
            }
            return emailData;
        }

        /// <summary>
        /// Returns the email content for add card remainder email
        /// </summary>
        /// <param name="remainderEmailCargo">Remainder email cargo</param>
        /// <returns>EmailData for add card remainder email</returns>
        private EmailData GetContentForCompleteSignup(PromotionalEmailCargo remainderEmailCargo)
        {
            EmailData emailData;
            EmailRenderingClient<object> emailRenderingClient = new EmailRenderingClient<object>
            {
                EmailRenderingServiceUrl = remainderEmailCargo.EmailRenderingServiceAddress
            };
            emailData = new EmailData
            {
                Subject = remainderEmailCargo.Subject,
                HtmlBody = emailRenderingClient.RenderHtml(),
                TextBody = string.Empty
            };

            return emailData;
        }
    }
}