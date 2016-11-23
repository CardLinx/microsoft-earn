//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.DataContract;
using Lomo.Commerce.DataContracts;
using LoMo.UserServices.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Earn.Offers.Earn.Models
{
    [DataContract]
    public class AccountsPageModel
    {
        [DataMember(EmitDefaultValue = false, Name = "local_deals")]
        public List<Deal> LocalDeals
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "top_deals")]
        public List<Deal> TopDeals
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "selected_state")]
        public string SelectedState
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "user_info")]
        public User UserInfo
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "user_id")]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Cards object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "cards")]
        public IEnumerable<V2CardDataContract> Cards
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "redemption_history")]
        public List<EarnBurnTransactionItemDataContract> RedemptionHistory
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "pending_redemption_history")]
        public List<EarnBurnTransactionItemDataContract> PendingRedemptionHistory
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "referral_code_reports")]
        public List<ReferralCodeReportDataContract> ReferralReport
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "referral_code")]
        public string ReferralCode
        {
            get;
            set;
        }


        [DataMember(EmitDefaultValue = false, Name = "earn_total")]
        public string EarnTotal
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "page")]
        public string Page
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "sort_by")]
        public string SortBy
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false, Name = "sort_order")]
        public string SortOrder
        {
            get;
            set;
        }
    }
}