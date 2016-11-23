//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to define contracts for creation of notification content
    /// </summary>
    public interface INotificationContentCreator
    {
        /// <summary>
        /// Create Auth Email Content
        /// </summary>
        /// <param name="data">
        /// Auth Email Notification Data
        /// </param>
        /// <param name="subjectLine">
        /// Email subect
        /// </param>
        /// <param name="templatePath">Template path</param>
        /// <returns>
        /// Notification content
        /// </returns>
        Task<NotificationContent> CreateAuthEmailContentAsync(AuthEmailNotificationData data, string subjectLine, string templatePath = null);

        /// <summary>
        /// Create Auth Sms Content
        /// </summary>
        /// <param name="data">
        /// Auth Sms Notification Data
        /// </param>
        /// <param name="templatePath">Template path</param>
        /// <returns>
        /// Notification cotent
        /// </returns>
        Task<NotificationContent> CreateAuthSmsContentAsync(AuthSmsNotificationData data, string templatePath = null);

        /// <summary>
        /// Create Add Card Email Content
        /// </summary>
        /// <param name="data">
        /// Add Card Email Notification Data
        /// </param>
        /// <param name="subjectLine">
        /// Email subect
        /// </param>
        /// <returns>
        /// Notification content
        /// </returns>
        Task<NotificationContent> CreateAddCardEmailContentAsync(AddCardEmailNotificationData data, string subjectLine);

        /// <summary>
        /// Create Add Card Sms Content
        /// </summary>
        /// <param name="data">
        /// Add Card Sms Notification Data
        /// </param>
        /// <returns>
        /// Notification content
        /// </returns>
        Task<NotificationContent> CreateAddCardSmsContentAsync(AddCardSmsNotificationData data);
    }
}