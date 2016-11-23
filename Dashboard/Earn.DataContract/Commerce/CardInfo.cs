//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Serialization;

namespace Earn.DataContract.Commerce
{
    [DataContract]
    public class CardInfo
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "user_id")]
        public int UserId { get; set; }

        [DataMember(Name = "global_user_id")]
        public Guid GlobalUserId { get; set; }

        [DataMember(Name = "last_4_digits")]
        public string Last4Digits { get; set; }

        [DataMember(Name = "card_brand_id")]
        public int CardBrandId { get; set; }

        [DataMember(Name = "card_brand")]
        public string CardBrand { get; set; }

        [DataMember( Name = "date_added_utc")]
        public DateTime? DateAddedUTC { get; set; }

        [DataMember(Name = "active")]
        public bool Active { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
}