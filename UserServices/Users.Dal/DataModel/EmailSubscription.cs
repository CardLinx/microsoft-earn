//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email subscription.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal.DataModel
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The email subscription.
    /// </summary>
    public class EmailSubscription
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the subscription is active.
        /// </summary>
        [DataMember(IsRequired = true, Name = "is_active")]
        public bool IsActive { get; set; }

        /// <summary>
        ///     Gets or sets LocationId.
        /// </summary>
        [DataMember(IsRequired = true, Name = "location_id")]
        public string LocationId { get; set; }

        /// <summary>
        ///     Gets or sets UserId.
        /// </summary>
        [DataMember(IsRequired = true, Name = "user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of subscription
        /// </summary>
        [DataMember(Name = "subscription_type")]
        public SubscriptionType SubscriptionType { get; set; }

        /// <summary>
        /// Gets or sets updated date
        /// </summary>
        [DataMember(Name = "updated_date")]
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the email address  for this subscription
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        #endregion
    }
}