//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents information about a settled deal.
    /// </summary>
    public class SettledDealInfo
    {
        /// <summary>
        /// Gets or sets the ID of the user credited with the settlement.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the parent deal involved in this settlement.
        /// </summary>
        public Guid ParentDealId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the deal involved in this settlement.
        /// </summary>
        public Guid DealId { get; set; }

        /// <summary>
        /// Gets or sets the merchant whose deal redemption was settled.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the ID assigned by the processing partner to the merchant at which the transaction that settled
        /// occurred.
        /// </summary>
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets the currency used within the deal, e.g. USD.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the settlement amount for the transaction.
        /// </summary>
        /// <remarks>
        /// This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        public int SettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount amount from the transaction.
        /// </summary>
        /// <remarks>
        /// This is in the smallest unit of the specified currency, e.g. cents in USD.
        /// </remarks>
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the ID of the settlement event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the event to which this settlement event is correlated.
        /// </summary>
        public Guid CorrelationId { get; set; }
    }
}