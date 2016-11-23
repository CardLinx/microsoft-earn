//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The deal settlement payload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Payloads
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The payload for a deal settlement from the Commerce system.
    /// </summary>
    public class DealSettlementPayload : PayloadBase
    {
        /// <summary>
        /// Gets or sets the currency involved in the transaction that redeemed the deal.
        /// </summary>
        /// <remarks>
        /// This should always be the same as for the corresponding redemption event.
        /// </remarks>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the authorization amount of the transaction that redeemed the deal.
        /// </summary>
        /// <remarks>
        /// * This value is in the smallest unit of the specified currency, e.g. cents in USD.
        /// * This value will often be different from the authorization amount in the corresponding deal redemption payload.
        /// </remarks>
        [JsonProperty(PropertyName = "settlement_amount")]
        public int SettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount amount from the transaction that redeemed the deal.
        /// </summary>
        /// <remarks>
        /// * This value is in the smallest unit of the specified currency, e.g. cents in USD.
        /// * This value is not guaranteed to match the value in the corresponding redemption event as the discount is recomputed
        ///   when the settlement amount does not match the authorization amount.
        /// </remarks>
        [JsonProperty(PropertyName = "discount_amount")]
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the canonical ID of the discount that was redeemed.
        /// </summary>
        /// <remarks>
        /// * This discount belongs to the deal specified in the AnalyticsItem.
        /// * This value is not guaranteed to match the value in the corresponding redemption event as deal selection logic is
        ///   executed again when the settlement amount does not match the authorization amount.
        /// </remarks>
        [JsonProperty(PropertyName = "discount_id")]
        public Guid DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the ID assigned by the processing partner to the merchant at which the transaction that redeemed the
        /// deal occurred.
        /// </summary>
        /// <remarks>
        /// * This ID is assigned by the processing partner, e.g. First Data, and may be used to determine the actual location of
        ///   the transaction for merchants with more than one location.
        /// * This should always be the same as for the corresponding redemption event.
        /// </remarks>
        [JsonProperty(PropertyName = "partner_merchant_id")]
        public string PartnerMerchantId { get; set; }
    }
}