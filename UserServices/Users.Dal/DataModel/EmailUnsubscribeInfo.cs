//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Email UnsubscribeInfo
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal.DataModel
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Email Unsubscribe Info Object
    /// </summary>
    [DataContract]
    public class EmailUnsubscribeInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        [DataMember(IsRequired = true, Name = "user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        [DataMember(IsRequired = true, Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the unsubscribe url
        /// </summary>
        [DataMember(IsRequired = true, Name = "unsubscribe_url")]
        public string UnsubscribeUrl { get; set; }

        /// <summary>
        /// Gets or sets the unsubscribe url last updated time
        /// </summary>
        [DataMember(IsRequired = true, Name = "unsubscribe_url_last_updated")]
        public DateTime LastUpdatedTime { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            // email is PII information and therefore omitted
            return string.Format("user id={0}; unsubscribe url={1}; last updated time={2}", this.UserId, this.UnsubscribeUrl, this.LastUpdatedTime);
        }
        #endregion
    }
}