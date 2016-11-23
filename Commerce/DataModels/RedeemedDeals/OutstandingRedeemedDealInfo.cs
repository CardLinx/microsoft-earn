//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Xml.XPath;

    /// <summary>
    /// Represents information about an outstanding redeemed deal.
    /// </summary>
    public class OutstandingRedeemedDealInfo
    {
        /// <summary>
        /// Gets or sets the ID for this deal redemption.
        /// </summary>
        public Guid RedeemedDealId { get; set; }

        /// <summary>
        /// Gets or sets PartnerMerchantId
        /// </summary>
        public string PartnerMerchantId { get; set; }

        /// <summary> 
        /// Gets or sets MerchantName
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets OfferId
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets AcquirerReferenceNumber
        /// </summary>
        public string AcquirerReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets DiscountAmount
        /// </summary>
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets TransactionDate
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets ReferenceNumber
        /// </summary>
        public int ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets Settlement Amount
        /// </summary>
        public int SettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets Discount Id
        /// </summary>
        public string DiscountId { get; set; }

        /// <summary>
        /// Gets or sets Deal Id
        /// </summary>
        public string DealId { get; set; }

        /// <summary>
        /// Gets or sets the Reimbursement Tender for the discount.
        /// </summary>
        public ReimbursementTender ReimbursementTender {  get; set; }

        /// <summary>
        /// Gets or sets Partner-specific data.
        /// </summary>
        public IXPathNavigable PartnerData { get; set; }

        /// <summary>
        /// Gets or sets global user Id associated with this transaction
        /// </summary>
        public Guid GlobalUserId { get; set; }

        /// <summary>
        /// Gets or sets Partner Redeemed Deal Scope Id. This is returned by partner in their clearing/settlement message
        /// </summary>
        public string PartnerRedeemedDealScopeId { get; set;}
    }
}