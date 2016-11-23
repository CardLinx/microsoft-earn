//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Serialization;

namespace Earn.DataContract.Commerce
{
    [DataContract]
    public class EarnBurnHistoryRecord
    {
        [DataMember(Name = "reimbursement_tender_id")]
        public int? ReimbursementTenderId { get; set; }

        [DataMember(Name = "reimbursement_tender")]
        public string ReimbursementTender { get; set; }

        [DataMember(Name = "card_brand_id")]
        public int? CardBrandId { get; set; }

        [DataMember(Name = "card_brand")]
        public string CardBrand { get; set; }

        [DataMember(Name = "credit_status_id")]
        public int? CreditStatusId { get; set; }

        [DataMember(Name = "credit_status")]
        public string CreditStatus { get; set; }

        [DataMember(Name = "discount_amount")]
        public double? DiscountAmount { get; set; }

        [DataMember(Name = "discount_summary")]
        public string DiscountSummary { get; set; }

        [DataMember(Name = "authorization_amount")]
        public double? AuthorizationAmount { get; set; }

        [DataMember(Name = "last_four_digits")]
        public string LastFourDigits { get; set; }

        [DataMember(Name = "merchant_name")]
        public string MerchantName { get; set; }

        [DataMember(Name = "percent")]
        public decimal? Percent { get; set; }

        [DataMember(Name = "purchase_datetime")]
        public DateTime? PurchaseDateTime { get; set; }

        [DataMember(Name = "reversed")]
        public bool? Reversed { get; set; }
    }
}