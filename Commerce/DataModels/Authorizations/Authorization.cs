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
    /// Represents a Authorization in the data store.
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// Initializes a new instance of the RedeemedDeal class.
        /// </summary>
        public Authorization()
        {
            Id = Guid.NewGuid();
            PartnerRedeemedDealId = General.GenerateShortGuid(Id);
        }

        /// <summary>
        /// Gets or sets the currency used for the purchase.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the canonical ID for this authorization attempt.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the purchase.
        /// </summary>
        public DateTime PurchaseDateTime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the claimed deal authorized by this object.
        /// </summary>
        public Guid ClaimedDealId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the redeemed deal.
        /// </summary>
        public string PartnerRedeemedDealId { get; set; }

        /// <summary>
        /// Gets or sets the authorization amount for the purchase.
        /// </summary>
        public int AuthorizationAmount { get; set; }

        /// <summary>
        /// Gets or sets the transaction scope Id for the authorization.
        /// </summary>
        public string TransactionScopeId { get; set; }

        /// <summary>
        /// Gets or sets the transaction Id for the authorization.
        /// </summary>
        public string TransactionId { get; set; }
    }
}