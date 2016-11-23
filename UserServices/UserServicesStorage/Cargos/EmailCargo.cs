//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the Email cargo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the Email Cargo.
    /// </summary>
    [DataContract]
    public class EmailCargo : BaseCargo
    {
        /// <summary>
        /// Gets or sets the Subject for the Email
        /// </summary>
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [JsonProperty(PropertyName = "email_address")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public Guid UserId { get; set; }
   
        /// <summary>
        /// Gets or sets the unsubscribe url.
        /// </summary>
        [JsonProperty(PropertyName = "unsubscribe_url")]
        public string UnsubscribeUrl { get; set; }

        /// <summary>
        /// Gets or sets the hints.
        /// </summary>
        [JsonProperty(PropertyName = "hints")]
        public EmailJobHints Hints { get; set; }

        /// <summary>
        /// Gets or sets the campaign.
        /// </summary>
        [JsonProperty(PropertyName = "campaign")]
        public string Campaign { get; set; }

        /// <summary>
        /// Gets or sets the URL for the Email template.
        /// </summary>
        [JsonProperty(PropertyName = "emailrenderingserviceaddress")]
        public string EmailRenderingServiceAddress { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Job Id: {0}; User Id: {1};  EmailAddress: {2}; EmailRenderingServiceURL: {3}",
                this.Id, this.UserId, this.EmailAddress, this.EmailRenderingServiceAddress);
        }
    }
}