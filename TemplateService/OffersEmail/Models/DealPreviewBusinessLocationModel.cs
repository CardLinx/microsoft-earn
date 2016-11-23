//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Daily deal details location model
    /// </summary>
    public class DealPreviewBusinessLocationModel
    {
        /// <summary>
        /// Gets or sets business loaction city
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets business loaction state
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
    }
}