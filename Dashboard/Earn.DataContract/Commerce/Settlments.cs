//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Serialization;

namespace Earn.DataContract.Commerce
{
    [DataContract]
    public class Settlement
    {
        [DataMember(EmitDefaultValue = false, Name = "partner_name")]
        public string PartnerName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "card_brand_id")]
        public int CardBrandId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "card_brand_name")]
        public string CardBrandName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "last_4_digits")]
        public string LastFourDigits { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "partner_merchant_id")]
        public int PartnerMerchantId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "merchant_name")]
        public string MerchantName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "authorization_date_time_local")]
        public DateTime AuthorizationDateTimeLocal { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "authorization_date_time_utc")]
        public DateTime? AuthorizationDateTimeUtc { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "utc_reached_terminal_state")]
        public DateTime? UtcReachedTerminalState { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "transaction_type_id")]
        public int TransactionTypeId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "transaction_type")]
        public string TransactionType { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "authorization_amount")]
        public double AuthorizationAmount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "settlement_amount")]
        public double SettlementAmount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "discount_amount")]
        public double DiscountAmount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "credit_status")]
        public string CreditStatus { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "current_state")]
        public string CurrentState { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "global_user_id")]
        public Guid GlobalUserId { get; set; }

    }
}