//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    using System;
    using Commerce.Configuration;
    using Lomo.Commerce.Utilities;
    using Users.Dal.DataModel;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Contains logic to add commerce analytics information to the analytics service
    /// </summary>
    public static class Analytics
    {
        /// <summary>
        /// Initializes the Analytics client
        /// </summary>
        /// <param name="commerceConfig">
        /// The CommerceConfig object to use during initialization.
        /// </param>
        public static void Initialize(CommerceConfig commerceConfig)
        {
            PartnerFactory.AnalyticsClient(commerceConfig).Initialize("commerce");
        }

        /// <summary>
        /// Adds an item to the analytics
        /// </summary>
        /// <param name="analyticsAction">
        /// Type of action
        /// </param>
        /// <param name="trackingId">
        /// Tracking Id for the request, AKA Event ID.
        /// </param>
        /// <param name="userId">
        /// Id of the user performing the action
        /// </param>
        /// <param name="dealId">
        /// Id of the deal
        /// </param>
        /// <param name="partnerDealId">
        /// Partner Deal Id
        /// </param>
        public static void Add(AnalyticsAction analyticsAction, 
                               Guid trackingId,
                               Guid dealId = default(Guid))
        {
            if (CommerceServiceConfig.Instance.EnableServiceHealth == true)
            {
                AnalyticsClient.AnalyticsItem item = new AnalyticsClient.AnalyticsItem
                {
                    Action = analyticsAction.ToString(),
                    ClientId = AnalyticsClientId,
                    EventId = trackingId,
                    DealId = dealId,
                };
                PartnerFactory.AnalyticsClient(CommerceServiceConfig.Instance).Add(item);
            }
        }

        /// <summary>
        /// Adds a user registration event to the analytics.
        /// </summary>
        /// <param name="userId">
        /// The canonical user ID to associate with the event.
        /// </param>
        /// <param name="registerUserEventId">
        /// The ID of this register user event.
        /// </param>
        /// <param name="correlationId">
        /// The ID of the event to which this event will be correlated.
        /// </param>
        /// <param name="referrer">
        /// The referrer to which to associate the registration of this user, if any.
        /// </param>
        public static void AddRegisterUserEvent(Guid userId,
                                                Guid registerUserEventId,
                                                Guid correlationId,
                                                string referrer)
        {
            if (CommerceServiceConfig.Instance.EnableServiceHealth == true)
            {
                AnalyticsClient.AnalyticsItem item = new AnalyticsClient.AnalyticsItem
                {
                    Action = AnalyticsClient.Actions.Actions.RegisterUser,
                    UserId = GetAnidFromUserId(userId, CommerceServiceConfig.Instance),
                    ClientId = AnalyticsClientId,
                    EventId = registerUserEventId,
                    ParentEventId = correlationId,
                    ReferrerClientId = referrer
                };

                PartnerFactory.AnalyticsClient(CommerceServiceConfig.Instance).Add(item);
            }
        }

        /// <summary>
        /// Adds an add card event to the analytics.
        /// </summary>
        /// <param name="userId">
        /// The canonical user ID to associate with the event.
        /// </param>
        /// <param name="addCardEventId">
        /// The ID of this add card event.
        /// </param>
        /// <param name="correlationId">
        /// The ID of the event to which this event will be correlated.
        /// </param>
        /// <param name="referrer">
        /// The referrer to which to associate the adding of this card, if any.
        /// </param>
        public static void AddAddCardEvent(Guid userId,
                                           Guid addCardEventId,
                                           Guid correlationId,
                                           string referrer)
        {
            if (CommerceServiceConfig.Instance.EnableServiceHealth == true)
            {
                AnalyticsClient.AnalyticsItem item = new AnalyticsClient.AnalyticsItem
                {
                    Action = AnalyticsAction.AddCard.ToString(),
                    UserId = userId.ToString(),
                    ClientId = AnalyticsClientId,
                    EventId = addCardEventId,
                    ParentEventId = correlationId,
                    ReferrerClientId = referrer
                };

                PartnerFactory.AnalyticsClient(CommerceServiceConfig.Instance).Add(item);
            }
        }

        /// <summary>
        /// Adds a redemption event to the analytics.
        /// </summary>
        /// <param name="userId">
        /// The canonical user ID to associate with the event.
        /// </param>
        /// <param name="redemptionEventId">
        /// The ID of this redemption event.
        /// </param>
        /// <param name="correlationId">
        /// The ID of the event to which this event will be correlated.
        /// </param>
        /// <param name="dealId">
        /// The ID of the deal being redeemed.
        /// </param>
        /// <param name="currency">
        /// The currency involved in the transaction in which the deal was redeeemed.
        /// </param>
        /// <param name="authorizationAmount">
        /// The authorization amount in the transaction in which the deal was redeemed.
        /// </param>
        /// <param name="discountAmount">
        /// The discount amount from the transaction in which the deal was redeemed.
        /// </param>
        /// <param name="discountId">
        /// The ID of the discount within the deal that was specifically redeemed.
        /// </param>
        /// <param name="partnerMerchantId">
        /// The ID of the merchant as assigned by the processing partner.
        /// </param>
        /// <param name="config">
        /// Optional configuration
        /// </param>
        public static void AddRedemptionEvent(Guid userId,
                                              Guid redemptionEventId,
                                              Guid correlationId,
                                              Guid dealId,
                                              string currency,
                                              int authorizationAmount,
                                              int discountAmount,
                                              Guid discountId,
                                              string partnerMerchantId,
                                              CommerceConfig config = null)
        {
            if (config == null)
            {
                config = CommerceServiceConfig.Instance;
            }

            if (config.EnableServiceHealth == true)
            {
                AnalyticsClient.Payloads.DealRedemptionPayload payload = new AnalyticsClient.Payloads.DealRedemptionPayload
                {
                    AuthorizationAmount = authorizationAmount,
                    Currency = currency,
                    DiscountAmount = discountAmount,
                    DiscountId = discountId,
                    PartnerMerchantId = partnerMerchantId
                };

                AnalyticsClient.AnalyticsItem item = new AnalyticsClient.AnalyticsItem
                {
                    Action = AnalyticsClient.Actions.Actions.RedeemedDeal,
                    UserId = GetAnidFromUserId(userId, config),
                    ClientId = AnalyticsClientId,
                    EventId = redemptionEventId,
                    ParentEventId = correlationId,
                    DealId = dealId,
                    JPayload = JObject.FromObject(payload)
                };

                PartnerFactory.AnalyticsClient(config).Add(item);
            }
        }

        /// <summary>
        /// Adds a settlement event to the analytics.
        /// </summary>
        /// <param name="userId">
        /// The canonical user ID to associate with the event.
        /// </param>
        /// <param name="settlementEventId">
        /// The ID of this settlement event.
        /// </param>
        /// <param name="correlationId">
        /// The ID of the event to which this event will be correlated.
        /// </param>
        /// <param name="dealId">
        /// The ID of the deal being redeemed.
        /// </param>
        /// <param name="currency">
        /// The currency involved in the transaction in which the deal was redeeemed.
        /// </param>
        /// <param name="settlementAmount">
        /// The settlement amount in the transaction in which the deal was redeemed.
        /// </param>
        /// <param name="discountAmount">
        /// The discount amount from the transaction in which the deal was redeemed.
        /// </param>
        /// <param name="discountId">
        /// The ID of the discount within the deal that was specifically redeemed.
        /// </param>
        /// <param name="partnerMerchantId">
        /// The ID of the merchant as assigned by the processing partner.
        /// </param>
        public static void AddSettlementEvent(Guid userId,
                                              Guid settlementEventId,
                                              Guid correlationId,
                                              Guid dealId,
                                              string currency,
                                              int settlementAmount,
                                              int discountAmount,
                                              Guid discountId,
                                              string partnerMerchantId)
        {
            if (CommerceWorkerConfig.Instance.EnableServiceHealth == true)
            {
                AnalyticsClient.Payloads.DealSettlementPayload payload = new AnalyticsClient.Payloads.DealSettlementPayload
                {
                    SettlementAmount = settlementAmount,
                    Currency = currency,
                    DiscountAmount = discountAmount,
                    DiscountId = discountId,
                    PartnerMerchantId = partnerMerchantId
                };

                AnalyticsClient.AnalyticsItem item = new AnalyticsClient.AnalyticsItem
                {
                    Action = AnalyticsClient.Actions.Actions.Settlement,
                    UserId = GetAnidFromUserId(userId, CommerceWorkerConfig.Instance),
                    ClientId = AnalyticsClientId,
                    EventId = settlementEventId,
                    ParentEventId = correlationId,
                    DealId = dealId,
                    JPayload = JObject.FromObject(payload)
                };

                PartnerFactory.AnalyticsClient(CommerceWorkerConfig.Instance).Add(item);
            }
        }

        /// <summary>
        /// Obtains the ANID for the specified canonical user ID.
        /// </summary>
        /// <param name="userId">
        /// The canonical user ID for whose ANID to obtain.
        /// </param>
        /// <param name="commerceConfig">
        /// The configuration instance to use.
        /// </param>
        /// <returns>
        /// The ANID for the specified user.
        /// </returns>
        private static string GetAnidFromUserId(Guid userId,
                                                CommerceConfig commerceConfig)
        {
            string puid = String.Empty;
            User user = PartnerFactory.UsersDal(commerceConfig).GetUserByUserId(userId);
            if (user != null)
            {
                puid = user.MsId;
            }
            return CommerceAnalyticsFactory.AnalyticsUserInfo(commerceConfig).GetAnidFromPuid(puid);
        }

        /// <summary>
        /// Identifier for the commerce service within analytics
        /// </summary>
        private const string AnalyticsClientId = "Commerce";
    }
}