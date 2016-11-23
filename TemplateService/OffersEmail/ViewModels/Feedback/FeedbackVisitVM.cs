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
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Feedback visit view model. It gives detail about a user visiting store to get a deal.
    /// </summary>
    public class FeedbackVisitVM
    {
        /// <summary>
        /// Gets or sets the date when user visited merchant store
        /// </summary>
        [JsonProperty(PropertyName = "visit_date", Required = Required.Always)]
        public DateTime VisitDate { get; set; }

        /// <summary>
        /// Gets or sets the discount summary of deal that user redeemed
        /// </summary>
        [JsonProperty(PropertyName = "discount_summary", Required = Required.Always)]
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the disocunt amount that user got
        /// </summary>
        [JsonProperty(PropertyName = "discount_amount", Required = Required.Always)]
        public string DiscountAmount { get; set; }
    }
}