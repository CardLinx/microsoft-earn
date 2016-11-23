//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Serialization;

namespace Earn.DataContract.Commerce
{
    [DataContract]
    public class EarnBurnLineItem
    {
        [DataMember(Name = "transaction_id")]
        public Guid? TransactionId { get; set; }

        [DataMember(Name = "user_id")]
        public Guid? UserId { get; set; }

        [DataMember(Name = "transaction_date")]
        public DateTime? TransactionDate { get; set; }

        [DataMember(Name = "earn_credit")]
        public double? EarnCredit { get; set; }

        [DataMember(Name = "burn_debit")]
        public double? BurnDebit { get; set; }

        [DataMember(Name = "has_redeemed_deal_record")]
        public bool? HasRedeemedDealRecord { get; set; }

        [DataMember(Name = "transaction_type_id")]
        public int? TransactionTypeId { get; set; }

        [DataMember(Name = "transaction_type")]
        public string TransactionType { get; set; }

        [DataMember(Name = "deal_summary")]
        public string DealSummary { get; set; }

        [DataMember(Name = "deal_percent")]
        public decimal? DealPercent { get; set; }

        [DataMember(Name = "merchant_name")]
        public string MerchantName { get; set; }

        [DataMember(Name = "transaction_amount")]
        public double TransactionAmount { get; set; }

        [DataMember(Name = "reversed")]
        public bool? Reversed { get; set; }

        [DataMember(Name = "transaction_status_id")]
        public int? TransactionStatusId { get; set; }

        [DataMember(Name = "transaction_status")]
        public string TransactionStatus { get; set; }

        [DataMember(Name = "last_4_digits")]
        public string Last4Digits { get; set; }

        [DataMember(Name = "card_brand_id")]
        public int? CardBrandId { get; set; }

        [DataMember(Name = "card_brand")]
        public string CardBrand { get; set; }

        [DataMember(Name = "perma_pending")]
        public bool? PermaPending { get; set; }

        [DataMember(Name = "review_status_id")]
        public int? ReviewStatusId { get; set; }

        [DataMember(Name = "review_status")]
        public string ReviewStatus { get; set; }

        [DataMember(Name = "redeem_deal_id")]
        public Guid? RedeemDealId { get; set; }
    }
}