//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The redemption model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Analytics.API.Contract
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The redemption contract.
    /// </summary>
    [DataContract(Name = "redemption")]
    public class RedemptionContract
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the provider id
        /// </summary>
        [DataMember(Name = "reseller_id")]
        public string ResellerId { get; set; }

        /// <summary>
        ///     Gets or sets the provider deal id.
        /// </summary>
        [DataMember(Name = "provider_deal_id")]
        public string ProviderDealId { get; set; }

        /// <summary>
        /// Gets or sets the deal id.
        /// </summary>
        [DataMember(Name = "deal_id", EmitDefaultValue = false)]
        public Guid DealId { get; set; }

        /// <summary>
        ///     Gets or sets the merchant id.
        /// </summary>
        [DataMember(Name = "merchant_id", EmitDefaultValue = false)]
        public Guid? MerchantId { get; set; }


        /// <summary>
        ///     Gets or sets the merchant name.
        /// </summary>
        [DataMember(Name = "merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        ///     Gets or sets the merchant location.
        /// </summary>
        [DataMember(Name = "merchant_location")]
        public LocationContract MerchantLocation { get; set; }

        /// <summary>
        ///     Gets or sets the transaction id.
        /// </summary>
        [DataMember(Name = "transaction_id")]
        public Guid TransactionId { get; set; }

        /// <summary>
        ///     Gets or sets the authorization date time local.
        /// </summary>
        [DataMember(Name = "authorization_time_local")]
        public DateTime? AuthorizationDateTimeLocal { get; set; }

        /// <summary>
        ///     Gets or sets the authorization date time utc.
        /// </summary>
        [DataMember(Name = "authorization_time_utc")]
        public DateTime? AuthorizationDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the credit approval date time utc.
        /// </summary>
        [DataMember(Name = "credit_approval_time_utc")]
        public DateTime? CreditApprovalDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the event date time utc.
        /// </summary>
        [DataMember(Name = "event_time_utc")]
        public DateTime EventDateTimeUtc { get; set; }


        /// <summary>
        ///     Gets or sets the card brand.
        /// </summary>
        [DataMember(Name = "card_brand")]
        public string CardBrand { get; set; }

        /// <summary>
        ///     Gets or sets the card last four digits.
        /// </summary>
        [DataMember(Name = "card_last_four_digits")]
        public string CardLastFourDigits { get; set; }

        /// <summary>
        ///     Gets or sets the currency.
        /// </summary>
        [DataMember(Name = "transaction_currency")]
        public string Currency { get; set; }

        /// <summary>
        ///     Gets or sets the discount amount.
        /// </summary>
        [DataMember(Name = "transaction_discount_amount")]
        public int DiscountAmount { get; set; }

        /// <summary>
        ///     Gets or sets the amount.
        /// </summary>
        [DataMember(Name = "transaction_amount")]
        public int Amount { get; set; }

        /// <summary>
        ///     Gets or sets the credit status.
        /// </summary>
        [DataMember(Name = "transaction_credit_status")]
        public string CreditStatus { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [DataMember(Name = "transaction_status")]
        public string Status { get; set; }

        #endregion
    }
}