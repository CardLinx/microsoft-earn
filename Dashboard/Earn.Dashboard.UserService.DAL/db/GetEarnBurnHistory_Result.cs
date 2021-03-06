//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//

// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>

namespace Earn.Dashboard.CustomerService.DAL.db
{
    using System;
    
    public partial class GetEarnBurnHistory_Result
    {
        public Nullable<int> ReimbursementTenderId { get; set; }
        public string MerchantName { get; set; }
        public string DiscountSummary { get; set; }
        public Nullable<decimal> Percent { get; set; }
        public Nullable<System.DateTime> PurchaseDateTime { get; set; }
        public Nullable<int> AuthorizationAmount { get; set; }
        public Nullable<bool> Reversed { get; set; }
        public Nullable<int> CreditStatusId { get; set; }
        public Nullable<int> DiscountAmount { get; set; }
        public string LastFourDigits { get; set; }
        public Nullable<int> CardBrandId { get; set; }
    }
}