//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.ViewModels
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Feedback email view model. It contains information about merchant, user and their feedbacks
    /// </summary>
    public class FeedbackEmailVM
    {
        /// <summary>
        /// Gets or sets the business name
        /// </summary>
        [JsonProperty(PropertyName = "business_name", Required = Required.Always)]
        public string BusinessName { get; set; }

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        [JsonProperty(PropertyName = "user_name", Required = Required.AllowNull)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the business contact name like busines owner name
        /// </summary>
        [JsonProperty(PropertyName = "business_contact_name", Required = Required.AllowNull)]
        public string BusinessContactName { get; set; }

        /// <summary>
        /// Gets or sets the Is For Merchant
        /// </summary>
        [JsonProperty(PropertyName = "is_for_merchant", Required = Required.Always)]
        public string IsForMerchant { get; set; }

        /// <summary>
        /// Gets or sets the list of feedbacks
        /// </summary>
        [JsonProperty(PropertyName = "feedbacks", Required = Required.Always)]
        public IList<FeedbackVM> Feedbacks { get; set; }

        /// <summary>
        /// Gets or sets the detail about user visit to a merchant store
        /// </summary>
        [JsonProperty(PropertyName = "visit", Required = Required.Always)]
        public FeedbackVisitVM Visit { get; set; }

        /// <summary>
        /// Gets or sets the feedbackUrl which has a token in it and allows merchant/user to provide feedback
        /// </summary>
        [JsonProperty(PropertyName = "feedback_url", Required = Required.Always)]
        public string FeedbackUrl { get; set; } 
    }
}