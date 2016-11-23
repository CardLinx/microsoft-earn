//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// email deals model
    /// </summary>
    public class EmailDealsModel
    {
        /// <summary>
        /// Gets or sets deals
        /// </summary>
        [JsonProperty(PropertyName = "deals")]
        public IEnumerable<DealModel> Deals { get; set; }

        /// <summary>
        /// Gets or sets location
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets unsubscribeUrl
        /// </summary>
        [JsonProperty(PropertyName = "unsubscribeUrl")]
        public string UnsubscribeUrl { get; set; }
    }
}