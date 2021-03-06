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

namespace Earn.Dashboard.DAL.db.Commerce
{
    using System;
    using System.Collections.Generic;
    
    public partial class RedemptionReport
    {
        public System.Guid TransactionId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerMerchantId { get; set; }
        public string CardBrand { get; set; }
        public string LastFourDigits { get; set; }
        public Nullable<System.Guid> DealId { get; set; }
        public int DiscountId { get; set; }
        public System.Guid GlobalDiscountId { get; set; }
        public System.DateTime AuthorizationDateTimeLocal { get; set; }
        public Nullable<System.DateTime> AuthorizationDateTimeUtc { get; set; }
        public Nullable<System.DateTime> SettlementDate { get; set; }
        public Nullable<System.DateTime> CreditApprovalDateTimeUtc { get; set; }
        public string Currency { get; set; }
        public int AuthorizationAmount { get; set; }
        public int SettlementAmount { get; set; }
        public int DiscountAmount { get; set; }
        public string CreditStatus { get; set; }
        public string CurrentState { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateUtc { get; set; }
        public Nullable<System.DateTime> UtcReachedTerminalState { get; set; }
        public System.Guid GlobalUserID { get; set; }
    }
}