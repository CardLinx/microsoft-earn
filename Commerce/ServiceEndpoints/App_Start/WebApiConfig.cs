//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;

    /// <summary>
    /// Configures MVC Web API.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers MVC Web APIs.
        /// </summary>
        /// <param name="config">
        /// Configuration to register.
        /// </param>
        /// <remarks>
        /// Post-conditions:
        /// * MVC Web APIs have been registered.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Parameter config cannot be null.
        /// </exception>
        public static void Register(HttpConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            config.EnableCors();

            // Service health APIs.
            config.Routes.MapHttpRoute("Ping", "api/commerce/ping", new { controller = "Ping" });

            // UI APIs.
            config.Routes.MapHttpRoute("V2Cards", "api/commerce/v2/cards", new { controller = "V2Cards" });
            config.Routes.MapHttpRoute("V2ReferralTypes", "api/commerce/v2/referraltypes", new { controller = "ReferralTypes" });
            config.Routes.MapHttpRoute("V2Referrals", "api/commerce/v2/referrals", new { controller = "Referrals" });
            config.Routes.MapHttpRoute("V2SecureTokens", "api/commerce/v2/securetokens/{action}", new { controller = "V2SecureTokens" });
            config.Routes.MapHttpRoute("V1Cards", "api/commerce/cards", new { controller = "Cards" });
            config.Routes.MapHttpRoute("RedemptionHistory", "api/commerce/redemptionhistory/{action}", new { controller = "RedemptionHistory", action = "get" });
            config.Routes.MapHttpRoute("DistributeMssv", "api/commerce/msftstorevouchers/{action}", new { controller = "Mssv" });
            config.Routes.MapHttpRoute("GetMssVoucherDistributionHistory", "api/commerce/msftstorevouchers/{action}", new { controller = "Mssv" });

            // Service APIs.
            config.Routes.MapHttpRoute("ServiceRedemptionHistory", "api/commerce/service/redemptionhistory", new { controller = "ServiceRedemptionHistory" });
            config.Routes.MapHttpRoute("V3ServiceDeals", "api/commerce/v3/service/deals", new { controller = "V3ServiceDeals" });
            config.Routes.MapHttpRoute("ServiceMerchantReports", "api/commerce/service/merchantreports", new { controller = "ServiceMerchantReports" });
            config.Routes.MapHttpRoute("ServiceDealReports", "api/commerce/service/dealreports", new { controller = "ServiceDealReports" });
            config.Routes.MapHttpRoute("ServiceRewardsProgramCardEnrollment", "api/commerce/ServiceRewardsProgramCardEnrollment", new { controller = "ServiceRewardsProgramCardEnrollment", action = "put" });

            // Worker APIs.
            config.Routes.MapHttpRoute("ServiceClaimedDeals", "api/commerce/service/claimeddeals", new { controller = "ServiceClaimedDeals" });
            config.Routes.MapHttpRoute("ServiceCards", "api/commerce/service/cards", new { controller = "ServiceCards" });
            config.Routes.MapHttpRoute("ServiceDiscounts", "api/commerce/service/discounts", new { controller = "ServiceDiscounts" });

            // Partner inbound APIs.
            config.Routes.MapHttpRoute("VisaApi", "api/commerce/visa/{action}", new { controller = "Visa" });
            config.Routes.MapHttpRoute("MasterCardApi", "api/commerce/mc/{action}", new { controller = "MasterCard" });
            config.Routes.MapHttpRoute("AmexApi", "api/commerce/amex/{action}", new { controller = "Amex" });

            // Enable XML serializer in addition to JSON serializer.
            config.Formatters.XmlFormatter.UseXmlSerializer = true;

            // Application Insights
            config.Services.Add(typeof(IExceptionLogger), new AiExceptionLogger());
        }
    }
}