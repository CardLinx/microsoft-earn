//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The deal redemption payload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Payloads
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The payload for a deal redemption from the Commerce system.
    /// </summary>
    public class DealRedemptionPayload : PayloadBase
    {
        /// <summary>
        /// Gets or sets the currency involved in the transaction that redeemed the deal.
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the authorization amount of the transaction that redeemed the deal.
        /// </summary>
        /// <remarks>
        /// This value is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        [JsonProperty(PropertyName = "authorization_amount")]
        public int AuthorizationAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount amount from the transaction that redeemed the deal.
        /// </summary>
        /// <remarks>
        /// This value is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        [JsonProperty(PropertyName = "discount_amount")]
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the canonical ID of the discount that was redeemed.
        /// </summary>
        /// <remarks>
        /// This discount belongs to the deal specified in the AnalyticsItem.
        /// </remarks>
        [JsonProperty(PropertyName = "discount_id")]
        public Guid DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the ID assigned by the processing partner to the merchant at which the transaction that redeemed the
        /// deal occurred.
        /// </summary>
        /// <remarks>
        /// This ID is assigned by the processing partner, e.g. First Data, and may be used to determine the actual location of
        /// the transaction for merchants with more than one location.
        /// </remarks>
        [JsonProperty(PropertyName = "partner_merchant_id")]
        public string PartnerMerchantId { get; set; }
    }
}