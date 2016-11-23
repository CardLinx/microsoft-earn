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
    /// Feedback view model. It contains comments that user gives to the merchant regarding store visit and merchant response.
    /// </summary>
    public class FeedbackVM
    {
        /// <summary>
        /// Gets or sets the comment that user provides to the merchant and merchant response
        /// </summary>
        [JsonProperty(PropertyName = "comment", Required = Required.Always)]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date when comment was provided
        /// </summary>
        [JsonProperty(PropertyName = "inserted_date", Required = Required.Always)]
        public DateTime InsertedDate { get; set; }

        /// <summary>
        /// Gets or sets the user type whether merchant or user. User = 1, Merchant = 2
        /// </summary>
        [JsonProperty(PropertyName = "user_type", Required = Required.Always)]
        public byte UserType { get; set; }
    }
}