//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Campaign contract
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Campaign contract
    /// </summary>
    public class CampaignDataContract
    {
        /// <summary>
        /// Gets or sets the content for the campaign email
        /// </summary>
        [JsonProperty(PropertyName = "Content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the unsubscribe url
        /// </summary>
        [JsonProperty(PropertyName = "unsubscribeUrl")]
        public string UnsubscribeUrl { get; set; }

        /// <summary>
        /// Gets or sets the location for the campaign
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the postal code for the campaign
        /// </summary>
        [JsonProperty(PropertyName = "postal_code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the businesses.
        /// </summary>
        [JsonProperty(PropertyName = "businesses")]
        public List<string> Businesses { get; set; }
    }
}