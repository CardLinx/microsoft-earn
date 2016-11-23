//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a history item.
    /// </summary>
    [DataContract]
    public class RedemptionHistoryItemDataContract
    {
        /// <summary>
        /// Gets or sets the name of the merchant whose deal was claimed.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "merchant_Name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the summary description of the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_summary")]
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount offered within the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_amount")]
        public int DealAmount { get; set; }

        /// <summary>
        /// Gets or sets the percent of the discount offered within the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_percent")]
        public decimal DealPercent { get; set; }

        /// <summary>
        /// Gets or sets the currency used within the deal, e.g. USD.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_currency")]
        public string DealCurrency { get; set; }

        /// <summary>
        /// Gets or sets the minimum purchase amount to cause the redemption to occur.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "minimum_purchase")]
        public int MinimumPurchase { get; set; }

        /// <summary>
        /// Gets or sets the maximum discount that could be granted for the deal within this history item.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "maximum_discount")]
        public int MaximumDiscount { get; set; }

        /// <summary>
        /// Gets or sets the event involving this deal claim described within this history item.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be "None".
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "event")]
        public string Event { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time at which an Event involving this deal claim occurred.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be DateTime.MinValue.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "event_date_time")]
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// Gets or sets the amount of the specified currency involved in the event.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be 0.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "event_amount")]
        public int EventAmount { get; set; }

        /// <summary>
        /// Gets or sets the currency involved in the event.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be null.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "event_currency")]
        public string EventCurrency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the redeemed deal has been reversed.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occured, this value will be false.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "reversed")]
        public bool Reversed { get; set; }

        /// <summary>
        /// Gets or sets the status of the credit for this deal redemption.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be "Unprocessed".
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "credit_status")]
        public string CreditStatus { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount for this deal redemption.
        /// </summary>
        /// <remarks>
        /// If no Event involving this deal claim has occurred, this value will be 0.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "discount_amount")]
        public int DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the name on the card used to redeem the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "name_on_card")]
        public string NameOnCard { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the card number used to redeem the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "last_four_digits")]
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the expiration date for the card used to redeem the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "card_expiration")]
        public DateTime CardExpiration { get; set; }

        /// <summary>
        /// Gets or sets the brand of the card used to redeem the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "card_brand")]
        public string CardBrand { get; set; }
    }
}