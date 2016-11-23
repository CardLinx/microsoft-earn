//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Xml.Serialization;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents a redeemed deal in the data store.
    /// </summary>
    public class RedeemedDeal
    {
        /// <summary>
        /// Initializes a new instance of the RedeemedDeal class.
        /// </summary>
        public RedeemedDeal()
        {
            Id = Guid.NewGuid();
            PartnerRedeemedDealId = General.GenerateShortGuid(Id);
        }

        /// <summary>
        /// Gets or sets the canonical ID for this redeemed deal.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the claimed deal redeemed by this object.
        /// </summary>
        public Guid ClaimedDealId { get; set; }

        /// <summary>
        /// Gets or sets the type of redemption event.
        /// </summary>
        public RedemptionEvent CallbackEvent { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the purchase.
        /// </summary>
        public DateTime PurchaseDateTime { get; set; }

        /// <summary>
        /// Gets or sets the authorization amount for the purchase.
        /// </summary>
        public int AuthorizationAmount { get; set; }

        /// <summary>
        /// Gets or sets the currency used for the purchase.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the partner scope ID for the redeemed deal.
        /// </summary>
        public string PartnerRedeemedDealScopeId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the redeemed deal.
        /// </summary>
        public string PartnerRedeemedDealId { get; set; }

        /// <summary>
        /// Gets or sets the analytics event ID for this deal redemption.
        /// </summary>
        public Guid AnalyticsEventId { get; set; }
    }
}