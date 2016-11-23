//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;
    using Users.Dal.DataModel;

    /// <summary>
    /// Contains methods to notify users of authorization events.
    /// </summary>
    public class NotifyAuthorization : Notify
    {
        /// <summary>
        /// Initializes a new instance of the NotifyAuthorization class.
        /// </summary>
        /// <param name="context">
        /// The context for this API call.
        /// </param>
        public NotifyAuthorization(CommerceContext context)
            : base(context)
        {
            CardOperations = CommerceOperationsFactory.CardOperations(Context);
        }

        /// <summary>
        /// Sends notification to a user upon redemption of a deal.
        /// </summary>
        public override void SendNotification()
        {
            if (TransactionMetNotificationThreshold())
            {
                IEnumerable<InternalCard> cards = CardOperations.RetrieveCardsByPartnerCardId();

                foreach (InternalCard internalCard in cards)
                {
                    Send(internalCard.GlobalUserId, CommerceServiceConfig.Instance.Environment);
                }
            }
        }

        /// <summary>
        /// Send email and sms
        /// </summary>
        /// <param name="userId">
        /// User Id to send notification to
        /// </param>
        /// <param name="environment">
        /// Environment for which notification needs to be sent
        /// </param>
        internal void Send(Guid userId, string environment)
        {
            try
            {
                Users.Dal.DataModel.User user = RetrieveUser(userId);

                //By default, enable both email and phone notification 
                TransactionNotificationPreference transactionNotificationPreference =
                    TransactionNotificationPreference.Email | TransactionNotificationPreference.Phone;

                if (user.Info != null && user.Info.Preferences != null)
                {
                    transactionNotificationPreference = user.Info.Preferences.TransactionNotificationMedium;
                }


                if (transactionNotificationPreference == TransactionNotificationPreference.None)
                {
                    Context.Log.Verbose("User has turned off both SMS & Email auth notification");
                    return;
                }

                INotificationContentCreator creator = NotificationContentCreatorFactory.NotificationContentCreator(Context);
                RedeemedDealInfo redeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];
                Context.Log.Verbose("Credit Amount : {0}", redeemedDealInfo.DiscountText);
                Context.Log.Verbose("DiscountSummary : {0}", redeemedDealInfo.DiscountSummary);
                Context.Log.Verbose("LastFourDigits : {0}", redeemedDealInfo.LastFourDigits);
                Context.Log.Verbose("MerchantName : {0}", redeemedDealInfo.MerchantName);
                Context.Log.Verbose("UserName : {0}", SalutationName);
                Context.Log.Verbose("ReimbursementTender : {0}", redeemedDealInfo.ReimbursementTenderId);

                AuthEmailNotificationData authEmailNotificationData = new AuthEmailNotificationData()
                {
                    CreditAmount = redeemedDealInfo.DiscountText,
                    DiscountSummary = redeemedDealInfo.DiscountSummary,
                    LastFourDigits = redeemedDealInfo.LastFourDigits,
                    MerchantName = redeemedDealInfo.MerchantName,
                    UserName = SalutationName,
                    UserId = userId.ToString(),
                    DealId = redeemedDealInfo.ParentDealId.ToString(),
                    DiscountId = redeemedDealInfo.GlobalId.ToString(),
                    TransactionDate = redeemedDealInfo.TransactionDate.ToLongDateString(),
                    TransactionId = redeemedDealInfo.TransactionId,
                    PartnerId = redeemedDealInfo.PartnerId.ToString(CultureInfo.InvariantCulture),
                    PartnerMerchantId = redeemedDealInfo.PartnerMerchantId,
                    Percent = (float)Math.Round(redeemedDealInfo.Percent, 2)
                };

                NotificationContent content;
                if ( (transactionNotificationPreference & TransactionNotificationPreference.Email) == TransactionNotificationPreference.Email)
                {
                    EnvironmentType environmentType;
                    if (Enum.TryParse(environment, true, out environmentType))
                    {
                        authEmailNotificationData.PopulateAuthStatusAndEmailLink(user, UsersDal, environmentType);
                    }

                    
                    bool isEarn = true;
                    if (redeemedDealInfo.ReimbursementTenderId == (int) ReimbursementTender.MicrosoftEarn)
                    {
                        content =
                            creator.CreateAuthEmailContentAsync(authEmailNotificationData, authEmailSubjectEarn,
                                authEmailTemplatePathEarn).Result;
                    }
                    else if (redeemedDealInfo.ReimbursementTenderId == (int) ReimbursementTender.MicrosoftBurn)
                    {
                        content =
                            creator.CreateAuthEmailContentAsync(authEmailNotificationData, authEmailSubjectBurn,
                                authEmailTemplatePathBurn).Result;
                    }
                    else
                    {
                        isEarn = false;
                        content =
                            creator.CreateAuthEmailContentAsync(authEmailNotificationData, authEmailSubjectClo).Result;
                    }

                    Context.Log.Verbose("About to send Email Auth notification");
                    SendEmailNotification(user.Email, content, isEarn);
                    Context.Log.Verbose("Email notification sent");
                }
                else
                {
                    Context.Log.Verbose("User has turned off Email Auth notification");
                }

                if ((transactionNotificationPreference & TransactionNotificationPreference.Phone) ==
                    TransactionNotificationPreference.Phone)
                {
                    AuthSmsNotificationData authSmsNotificationData = new AuthSmsNotificationData()
                    {
                        DiscountSummary = redeemedDealInfo.DiscountSummary,
                        MerchantName = redeemedDealInfo.MerchantName,
                        Percent = (float) Math.Round(redeemedDealInfo.Percent, 2),
                        CreditAmount = redeemedDealInfo.DiscountText
                    };

                    if (redeemedDealInfo.ReimbursementTenderId == (int) ReimbursementTender.MicrosoftEarn)
                    {
                        content =
                            creator.CreateAuthSmsContentAsync(authSmsNotificationData, authSmsTemplatePathEarn).Result;
                    }
                    else if (redeemedDealInfo.ReimbursementTenderId == (int) ReimbursementTender.MicrosoftBurn)
                    {
                        content =
                            creator.CreateAuthSmsContentAsync(authSmsNotificationData, authSmsTemplatePathBurn).Result;
                    }
                    else
                    {
                        content = creator.CreateAuthSmsContentAsync(authSmsNotificationData).Result;
                    }

                    Context.Log.Verbose("About to send SMS Auth notification");
                    SendSmsNotification(userId, content.TextBody);
                    Context.Log.Verbose("SMS Notification sent");
                }
                else
                {
                    Context.Log.Verbose("User has turned off SMS Auth notification");
                }
            }
            catch (Exception exception)
            {
                // catch all exception, log them as warning.
                // but continue sending other notifications if needed
                Context.Log.Warning("Sending notification resulted in error. User Id : {0}", exception, userId);
            }
        }

        /// <summary>
        /// Checks if this transaction has earned any discount amount in order to send a notification
        /// </summary>
        /// <returns>If we have to send a notification for the transaction or not</returns>
        private bool TransactionMetNotificationThreshold()
        {
            Context.Log.Verbose("Checking if the discount amount for the transaction is good enough for a notification");
            //By default make it true. It's better to send a notification wrongly than not to send one.
            bool transactionMetNotificationThreshold = true;

            const int thresholdAmount = 0; //No point in sending a notification where the customer did not save anything

            RedeemedDealInfo redeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];
            if (redeemedDealInfo.DiscountAmount <= thresholdAmount)
            {
                Context.Log.Verbose("Discount amount {0) is too small/has not met the threshold limit of {1} to send notification", redeemedDealInfo.DiscountAmount, thresholdAmount);
                //Do not send a notification if the discount has not met the threshold
                transactionMetNotificationThreshold = false;
            }

            return transactionMetNotificationThreshold;
        }

        /// <summary>
        /// Gets or sets the data access object to use for Card operations.
        /// </summary>
        private ICardOperations CardOperations { get; set; }

        /// <summary>
        /// Relative Path for Auth Email Template for Earn
        /// </summary>
        private const string authEmailTemplatePathEarn = "/Earn/Earn";

        /// <summary>
        /// Relative Path for Auth Email Template for Earn
        /// </summary>
        private const string authEmailTemplatePathBurn = "/Earn/Burn";

        /// <summary>
        /// Relative Path for Auth Email Template for Earn
        /// </summary>
        private const string authSmsTemplatePathEarn = "/Earn/EarnSms";

        /// <summary>
        /// Relative Path for Auth Email Template for Earn
        /// </summary>
        private const string authSmsTemplatePathBurn = "/Earn/BurnSms";

        /// <summary>
        /// Earn email subject
        /// </summary>
        private const string authEmailSubjectEarn = "You Earned it!";

        /// <summary>
        /// Earn email subject
        /// </summary>
        private const string authEmailSubjectBurn = "Your Earning has paid off!!";

        /// <summary>
        /// Earn email subject
        /// </summary>
        internal string authEmailSubjectClo = "Your discount is on its way!";
    }
}