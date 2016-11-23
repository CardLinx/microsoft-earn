//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The subscribe by email request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.UserServices.DataContract
{
    using Newtonsoft.Json;

    /// <summary>
    /// The subscribe by email request.
    /// </summary>
    public class SubscribeByEmailRequest
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the subscription info.
        /// </summary>
        [JsonProperty(PropertyName = "subscription_info")]
        public Location SubscriptionInfo { get; set; }
    }
}