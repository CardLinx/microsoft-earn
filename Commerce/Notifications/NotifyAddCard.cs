//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using System;
    using System.Collections.Generic;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;
    using Users.Dal.DataModel;

    /// <summary>
    /// Contains methods to notify users of add card event.
    /// </summary>
    public class NotifyAddCard : Notify
    {
        /// <summary>
        /// Initializes a new instance of the NotifyAddCard class.
        /// </summary>
        /// <param name="context">
        /// The context for this API call.
        /// </param>
        public NotifyAddCard(CommerceContext context)
            : base(context)
        {
            CardOperations = CommerceOperationsFactory.CardOperations(Context);
        }

        /// <summary>
        /// Sends notification to a user upon successful addition of a new card.
        /// </summary>
        public override void SendNotification()
        {
            // TODO: Need to target Earn and Clo users with diff email template.
            // disabling add card notification to accomodate the Earn changes. 

            ////Guid userId = (Guid)Context[Key.UserId];

            ////try
            ////{
            ////    Users.Dal.DataModel.User user = RetrieveUser(userId);

            ////    Card card = CardOperations.RetrieveCard();

            ////    AddCardEmailNotificationData emailNotificationData = new AddCardEmailNotificationData()
            ////    {
            ////        CardType = card.CardBrand.ToString(),
            ////        LastFourDigits = card.LastFourDigits
            ////    };

            ////    emailNotificationData.PopulateAuthStatus(user);

            ////    INotificationContentCreator creator = NotificationContentCreatorFactory.NotificationContentCreator(Context);
            ////    NotificationContent content = creator.CreateAddCardEmailContentAsync(emailNotificationData, AddCardEventSubject).Result;

            ////    Context.Log.Verbose("About to send Email Add Card notification");
            ////    SendEmailNotification(user.Email, content);
            ////    Context.Log.Verbose("Email notification sent");

            ////    AddCardSmsNotificationData smsNotificationData = new AddCardSmsNotificationData
            ////    {
            ////        CardType = card.CardBrand.ToString(),
            ////        LastFourDigits = card.LastFourDigits
            ////    };
            ////    smsNotificationData.PopulateAuthStatus(user);

            ////    content = creator.CreateAddCardSmsContentAsync(smsNotificationData).Result;
            ////    Context.Log.Verbose("About to send SMS Add Card notification");
            ////    SendSmsNotification(userId, content.TextBody);
            ////    Context.Log.Verbose("SMS Notification sent");
            ////}
            ////catch (Exception exception)
            ////{
            ////    // catch all exception, log them as warning.
            ////    // but continue sending other notifications if needed
            ////    Context.Log.Warning("Sending notification resulted in error. User Id : {0}", exception, userId);
            ////}
        }

        /// <summary>
        /// Gets or sets the data access object to use for Card operations.
        /// </summary>
        private ICardOperations CardOperations { get; set; }

        /// <summary>
        /// The subject to use for authorization event notifications.
        /// </summary>
        internal string AddCardEventSubject = "Your card has been added!";
    }
}