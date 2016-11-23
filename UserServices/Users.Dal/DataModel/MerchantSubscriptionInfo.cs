//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The MerchantSubscriptionInfo
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal.DataModel
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The MerchantSubscription Info
    /// </summary>
    public class MerchantSubscriptionInfo
    {
        #region Public Properties

        /// <summary>
        ///  Gets or sets UserId.
        /// </summary>
        [DataMember(IsRequired = true, Name = "user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscription is active.
        /// </summary>
        [DataMember(IsRequired = true, Name = "is_active")]
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Gets or sets the type of subscription
        /// </summary>
        [DataMember(Name = "subscription_type")]
        public SubscriptionType SubscriptionType { get; set; }


        /// <summary>
        /// Gets or sets the updated date time
        /// </summary>
        [DataMember(Name = "updated_date_time")]
        public DateTime UpdatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the interval for receiving email
        /// </summary>
        [DataMember(Name = "email_report_interval")]
        public ScheduleType EmailReportInterval { get; set; }

        /// <summary>
        /// Gets or sets the settings for merchant user
        /// </summary>
        [DataMember(Name = "preferences")]
        public MerchantPreferences Preferences { get; set; }

        #endregion
    }
}