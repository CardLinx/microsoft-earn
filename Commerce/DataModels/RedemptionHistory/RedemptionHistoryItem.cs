//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using Lomo.Commerce.DataContracts;

    /// <summary>
    /// Represents a history item.
    /// </summary>
    public class RedemptionHistoryItem
    {
        /// <summary>
        /// Gets or sets the name of the merchant whose deal was claimed.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the summary description of the deal.
        /// </summary>
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount offered within the deal.
        /// </summary>
        public int DealAmount { get; set; }

        /// <summary>
        /// Gets or sets the percent of the discount offered within the deal.
        /// </summary>
        public decimal DealPercent { get; set; }

        /// <summary>
        /// Gets or sets the currency used within the deal, e.g. USD.
        /// </summary>
        public string DealCurrency { get; set; }

        /// <summary>
        /// Gets or sets the minimum purchase amount to cause the redemption to occur.
        /// </summary>
        public int MinimumPurchase { get; set; }

        /// <summary>
        /// Gets or sets the maximum discount that could be granted for the deal within this history item.
        /// </summary>
        public int MaximumDiscount { get; set; }

        /// <summary>
        /// Gets or sets the event involving this deal claim described within this history item.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be RedemptionEvent.None.
        /// </remarks>
        public RedemptionEvent Event { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time at which an Event involving this deal claim occurred.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be DateTime.MinValue.
        /// </remarks>
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// Gets or sets the amount of the specified currency involved in the event.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be 0.
        /// </remarks>
        public int EventAmount { get; set; }

        /// <summary>
        /// Gets or sets the currency involved in the event.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be null.
        /// </remarks>
        public string EventCurrency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the redeemed deal has been reversed.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occured, this value will be false.
        /// </remarks>
        public bool Reversed { get; set; }

        /// <summary>
        /// Gets or sets the status of the credit for this deal redemption.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be CreditStatus.AuthorizationReceived.
        /// </remarks>
        public CreditStatus CreditStatus { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount for this deal redemption.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be 0.
        /// </remarks>
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the name on the card used to redeem the deal.
        /// </summary>
        public string NameOnCard { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the card number used to redeem the deal.
        /// </summary>
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the expiration date for the card used to redeem the deal.
        /// </summary>
        public DateTime CardExpiration { get; set; }

        /// <summary>
        /// Gets or sets the brand of the card used to redeem the deal.
        /// </summary>
        public CardBrand CardBrand { get; set; }

        /// <summary>
        /// Gets or sets the means through which deal reimbursement will be tendered, e.g. Currency or CSV.
        /// </summary>
        public ReimbursementTender ReimbursementTender { get; set; }
    }
}