//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Users.Dal
{
    using System;

    using DataModel;
    using Newtonsoft.Json;

    /// <summary>
    ///     The data contract converters.
    /// </summary>
    internal static class DataContractConverters
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="emailSubscriptionsResult">
        /// The email subscriptions result.
        /// </param>
        /// <returns>
        /// The <see cref="EmailSubscription"/>.
        /// </returns>
        internal static EmailSubscription Convert(GetEmailSubscriptions_Result emailSubscriptionsResult)
        {
            EmailSubscription emailSubscription = null;
            if (emailSubscriptionsResult != null)
            {
                emailSubscription = new EmailSubscription
                    {
                        UserId = emailSubscriptionsResult.UserId,
                        IsActive = emailSubscriptionsResult.IsActive,
                        LocationId = emailSubscriptionsResult.LocationId,
                        UpdatedDate = emailSubscriptionsResult.CreatedDate,
                        Email = emailSubscriptionsResult.Email
                    };

                SubscriptionType subscriptionType;
                Enum.TryParse(emailSubscriptionsResult.SubscriptionType, out subscriptionType);
                emailSubscription.SubscriptionType = subscriptionType;
            }

            return emailSubscription;
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="databaseUser">
        /// The data base user.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        internal static User Convert(UserDb databaseUser)
        {
            if (databaseUser == null)
            {
                return null;
            }

            var userInfo = new UserInfo();
            if (databaseUser.Json != null)
            {
                userInfo = JsonConvert.DeserializeObject<UserInfo>(databaseUser.Json);
            }

            return new User
            {
                Email = databaseUser.Email,
                IsEmailConfirmed = databaseUser.IsEmailConfirmed,
                PhoneNumber = databaseUser.PhoneNumber,
                MsId = databaseUser.MsId,
                Id = databaseUser.Id,
                Info = userInfo,
                Name = databaseUser.Name,
                IsSuppressed = databaseUser.IsSuppressed,
                Source = databaseUser.Source
            };
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="databaseEmailUnsubscribe">
        /// The database email unsubscribe.
        /// </param>
        /// <returns>
        /// The <see cref="EmailUnsubscribeInfo"/>.
        /// </returns>
        internal static EmailUnsubscribeInfo Convert(EmailUnsubscribeUrl_Result databaseEmailUnsubscribe)
        {
            if (databaseEmailUnsubscribe == null)
            {
                return null;
            }

            return new EmailUnsubscribeInfo
                       {
                           UserId = databaseEmailUnsubscribe.UserId,
                           Email = databaseEmailUnsubscribe.Email, 
                           LastUpdatedTime = databaseEmailUnsubscribe.UpdatedDate, 
                           UnsubscribeUrl = databaseEmailUnsubscribe.UnsubscribeUrl, 
                       };
        }

        /// <summary>
        /// Converts the results from GetMerchantSubscriptionByUserId stored proc into MerchantSubscriptionInfo object
        /// </summary>
        /// <param name="merchantSubscriptionsResult">Stored procedure return type</param>
        /// <returns>MerchantSubscriptionInfo object</returns>
        internal static MerchantSubscriptionInfo Convert(GetMerchantSubscriptionsByUserId_Result merchantSubscriptionsResult)
        {
            MerchantSubscriptionInfo merchantSubscription = null;
            if (merchantSubscriptionsResult != null)
            {
                merchantSubscription = new MerchantSubscriptionInfo
                {
                    UserId = merchantSubscriptionsResult.UserId,
                    IsActive = merchantSubscriptionsResult.IsActive
                };

                SubscriptionType subscriptionType;
                if (Enum.TryParse(merchantSubscriptionsResult.SubscriptionType, out subscriptionType))
                {
                    merchantSubscription.SubscriptionType = subscriptionType;
                }

                ScheduleType scheduleType;
                if (Enum.TryParse(merchantSubscriptionsResult.ScheduleType, out scheduleType))
                {
                    merchantSubscription.EmailReportInterval = scheduleType;
                }

                if (merchantSubscriptionsResult.Preferences != null)
                {
                    merchantSubscription.Preferences = JsonConvert.DeserializeObject<MerchantPreferences>(merchantSubscriptionsResult.Preferences);
                }
                merchantSubscription.UpdatedDateTime = merchantSubscriptionsResult.UpdatedDate;
            }

            return merchantSubscription;
        }

        /// <summary>
        /// Converts the results from GetEmailJobs stored proc into MailingJob object
        /// </summary>
        /// <param name="emailJobsResult">stored proc return type</param>
        /// <returns>instance of mailing job</returns>
        internal static MailingJob Convert(GetEmailJobs_Result emailJobsResult)
        {
            MailingJob emailJob = null;
            if (emailJobsResult != null)
            {
                emailJob = new MailingJob
                {
                    JobId = emailJobsResult.JobId,
                    UserId = emailJobsResult.UserId
                };

                SubscriptionType subscriptionType;
                Enum.TryParse(emailJobsResult.SubscriptionType, out subscriptionType);
                emailJob.SubscriptionType = subscriptionType;
            }

            return emailJob;
        }
    }
}