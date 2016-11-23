//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailContentCreator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LoMo.Templating;
    using LoMo.UserServices.DataContract;
    using DotM.DataContracts;
    using DealsServerClient;
    using LoMo.UserServices.Storage.UserHistory;
    using OffersEmail.DataContracts;
    using Lomo.Logging;
    using Microsoft.WindowsAzure;
    using Microsoft.Azure;

    /// <summary>
    /// The email content creator.
    /// </summary>
    public class DealsEmailContentCreator : IEmailContentCreator
    {
        /// <summary>
        /// The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.UserServices.ConnectionString";

        /// <summary>
        /// The deals server address setting.
        /// </summary>
        private const string DealsServerAddressSetting = "LoMo.DealsServer.Address";

        /// <summary>
        /// The client name.
        /// </summary>
        private const string ClientName = "BO_EMAIL";

        /// <summary>
        /// The windows between user emails.
        /// </summary>
        private const string WindowBetweenUserEmails = "LoMo.DealsMailing.WindowBetweenUserEmails";

        /// <summary>
        /// The mail history lookback setting.
        /// </summary>
        private const string MailHistoryLookBackSetting = "LoMo.DealsMailing.MailHistoryLookback";

        #region Data Members

        private IUserHistoryStorage userHistoryStorage;

        /// <summary>
        /// The deals selector.
        /// </summary>
        private IDealsSelector dealsSelector;

        /// <summary>
        /// The template model creator.
        /// </summary>
        private DealsTemplateCreator templateModelCreator;

        private TimeSpan windowsBetweenUserEmails;

        private int mailHistoryLookback;

        #endregion

        public void Initialize()
        {
            string storageSetting = CloudConfigurationManager.GetSetting(StorageSetting);
            userHistoryStorage = new UserHistoryStorage(storageSetting);
            windowsBetweenUserEmails = TimeSpan.Parse(CloudConfigurationManager.GetSetting(WindowBetweenUserEmails));
            mailHistoryLookback = int.Parse(CloudConfigurationManager.GetSetting(MailHistoryLookBackSetting));
            templateModelCreator = new DealsTemplateCreator();
            Uri dealsServerBaseAddress = new Uri(CloudConfigurationManager.GetSetting(DealsServerAddressSetting));
            DealsClient dealsClient = new DealsClient(dealsServerBaseAddress, ClientName);
            dealsSelector = new NaiveDealsSelector(dealsClient);
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
            DealsEmailCargo dealsEmailCargo = emailCargo as DealsEmailCargo;
            if (dealsEmailCargo != null)
            {
                string locationId = dealsEmailCargo.LocationId;
                bool isSendTimeWindowValid = true;
                IEnumerable<UserEmailEntity> emailHistoryEntities = null;

                //If this is not a test email, check the history to make sure we are not sending the email to the same user within the sendtime window.
                if (!dealsEmailCargo.Hints.IsTestEmail)
                {
                    emailHistoryEntities = this.userHistoryStorage.GetUserEmailEntities(dealsEmailCargo.UserId, mailHistoryLookback).ToList();
                    isSendTimeWindowValid = this.IsSendTimeWindowValid(emailHistoryEntities.FirstOrDefault(elem => elem.LocationId == locationId), dealsEmailCargo);
                }
                if (isSendTimeWindowValid)
                {
                    IEnumerable<Guid> dealsToExclude = null;
                    //if dealids are not included the cargo, we have to select random deals. Need to check in the history to make sure we are excluding deals that have
                    //already been sent in the past few weeks (based on the mail history lookback settings)
                    if (emailHistoryEntities != null && dealsEmailCargo.DealIds == null)
                    {
                        dealsToExclude = this.GetDealsToExclude(emailHistoryEntities);
                    }

                    EmailRenderingClient<DailyDealsContract> emailRenderingClient = new EmailRenderingClient<DailyDealsContract>
                    {
                        EmailRenderingServiceUrl = dealsEmailCargo.EmailRenderingServiceAddress
                    };
                    IEnumerable<Deal> deals = null;
                    if (dealsEmailCargo.Hints != null && dealsEmailCargo.Hints.IncludeDeals)
                    {
                        deals = this.dealsSelector.GetDeals(emailCargo as DealsEmailCargo, dealsToExclude).ToList();
                    }
                    if (deals != null && deals.Any())
                    {
                        DealsTemplateData dailyDealsTemplateData = new DealsTemplateData
                            {
                                EmailAddress = dealsEmailCargo.EmailAddress,
                                UnsubscribeUrl = dealsEmailCargo.UnsubscribeUrl,
                                LocationId = locationId,
                                Deals = deals,
                                DealEmailType =
                                    dealsEmailCargo.DealIds != null && dealsEmailCargo.DealIds.Any()
                                        ? DealEmailType.TrendingDeal
                                        : DealEmailType.WeeklyDeal
                            };

                        var model = this.templateModelCreator.GenerateModel(dailyDealsTemplateData);
                        emailData = new EmailData
                            {
                                Subject = !string.IsNullOrEmpty(dealsEmailCargo.Subject)
                                              ? dealsEmailCargo.Subject
                                              : this.RenderSubject(model),
                                HtmlBody = emailRenderingClient.RenderHtml(model),
                                TextBody = string.Empty,
                                DealIds = deals.Select(elem => new Guid(elem.Id)).ToList()
                            };
                    }
                    else
                    {
                        int dealsCount = deals != null ? deals.Count() : 0;
                        throw new ModelContentException(string.Format("Number of deals is: {0}. This is insufficient for email model creation", dealsCount));
                    }
                }
            }

            return emailData;
        }
        /// <summary>
        /// The render subject.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string RenderSubject(DailyDealsContract model)
        {
            string subject = null;
            if (model != null && model.Deals != null && model.Deals.Any())
            {
                DealContract contract = model.Deals.First();
                if (contract.DealType == DealType.CardLinked)
                {
                    if (contract.CardLinkInfos.Any() && (!string.IsNullOrEmpty(contract.CardLinkInfos[0].DiscountAmount) || !string.IsNullOrEmpty(contract.CardLinkInfos[0].Discount))
                        && !string.IsNullOrEmpty(contract.BusinessName))
                    {
                        subject = string.Format("Save {0} at {1}",
                            !string.IsNullOrEmpty(contract.CardLinkInfos[0].DiscountAmount) ? contract.CardLinkInfos[0].DiscountAmount : contract.CardLinkInfos[0].Discount,
                            contract.BusinessName);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(contract.Discount) && !string.IsNullOrEmpty(contract.BusinessName))
                    {
                        subject = string.Format("Save {0} at {1}", contract.Discount, contract.BusinessName);
                    }
                }

                if (string.IsNullOrEmpty(subject))
                {
                    subject = model.Deals.First().Title;
                }
            }

            return subject;
        }

        /// <summary>
        /// The is send time window valid.
        /// </summary>
        /// <param name="lastEmailEntity">
        /// The last email entity.
        /// </param>
        /// <param name="emailCargo">
        /// The job.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsSendTimeWindowValid(UserEmailEntity lastEmailEntity, EmailCargo emailCargo)
        {
            bool isSendTimeWindowValid = true;

            if (lastEmailEntity != null && lastEmailEntity.EmailDate > (DateTime.UtcNow - this.windowsBetweenUserEmails))
            {
                Log.Error(
                         "Can't send deals email due to time windows constraint. Job Details=[{0}]; Last Email Date={1}; Window Between Emails={2}",
                         emailCargo.ToString(),
                         lastEmailEntity.EmailDate,
                         this.windowsBetweenUserEmails);

                isSendTimeWindowValid = false;
            }

            return isSendTimeWindowValid;
        }


        /// <summary>
        /// The get deals to exclude.
        /// </summary>
        /// <param name="userEmailEntities">
        /// The user email entities.
        /// </param>
        /// <returns>
        /// List of deals
        /// </returns>
        private IEnumerable<Guid> GetDealsToExclude(IEnumerable<UserEmailEntity> userEmailEntities)
        {
            HashSet<Guid> dealsToExclude = new HashSet<Guid>();
            foreach (UserEmailEntity entity in userEmailEntities)
            {
                var emailPayload = entity.GetDeserializedPayload();
                if (emailPayload != null && emailPayload.DealIds != null)
                {
                    foreach (Guid dealId in emailPayload.DealIds)
                    {
                        dealsToExclude.Add(dealId);
                    }
                }
            }

            return dealsToExclude.ToList();
        }
    }
}