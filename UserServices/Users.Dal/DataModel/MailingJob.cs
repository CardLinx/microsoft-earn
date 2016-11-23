//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email job.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal.DataModel
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The email job.
    /// </summary>
    public class MailingJob
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the JobId.
        /// </summary>
        [DataMember(IsRequired = true, Name = "job_id")]
        public Guid JobId { get; set; }

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        [DataMember(IsRequired = true, Name = "user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of subscription
        /// </summary>
        [DataMember(Name = "subscription_type")]
        public SubscriptionType SubscriptionType { get; set; }

        #endregion
    }
}