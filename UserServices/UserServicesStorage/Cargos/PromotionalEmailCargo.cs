//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the remainder email cargo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using Newtonsoft.Json;

    /// <summary>
    /// Different types of promotional emails
    /// </summary>
    public enum PromotionalEmailType
    {
        /// <summary>
        /// Remainder for the user to complete the signup by adding a card
        /// </summary>
        CompleteSignup
    }

    /// <summary>
    /// Defines the promotional email cargo.
    /// </summary>
    public class PromotionalEmailCargo : EmailCargo
    {
        /// <summary>
        /// Gets or sets the type of email
        /// </summary>
        [JsonProperty(PropertyName = "promotional_email_type")]
        public string PromotionalEmailType { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Job Id: {0}; EmailAddress: {1}; EmailRenderingServiceURL: {2}; RemainderType: {3}",this.Id, this.EmailAddress, this.EmailRenderingServiceAddress, this.PromotionalEmailType);
        }
    }
}