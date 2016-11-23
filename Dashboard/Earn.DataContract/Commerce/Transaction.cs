//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Serialization;

namespace Earn.DataContract.Commerce
{
    [DataContract]
    public class Transaction
    {
        [DataMember(EmitDefaultValue = false, Name = "transaction_id")]
        public long TransactionLinkId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "merchant_id")]
        public int MerchantId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "merchant_name")]
        public string MerchantName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "transaction_type_id")]
        public int TransactionTypeId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "transaction_type")]
        public string TransactionType { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "percent")]
        public decimal Percent { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "date_added")]
        public DateTime? DateAdded { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "transaction_amount")]
        public double TransactionAmount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "discount_amount")]
        public double DiscountAmount { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "last_4_digits")]
        public string LastFourDigits { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "card_brand_id")]
        public int CardBrandId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "card_brand_name")]
        public string CardBrandName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "user_global_id")]
        public Guid UserGlobalId { get; set; }
    }
}