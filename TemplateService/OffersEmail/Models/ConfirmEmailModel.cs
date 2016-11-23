//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Confirm Email
    /// </summary>
    public class ConfirmEmailModel
    {
        /// <summary>
        /// Gets or sets confirmation url
        /// </summary>
        [JsonProperty(PropertyName = "confirmation_url")]
        public string ConfirmationUrl { get; set; }
    }
}