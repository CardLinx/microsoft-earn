//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// //   Contract for Merchant Store Location
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Contract for Merchant Store Location
    /// </summary>
    public class MerchantLocationContract
    {
        /// <summary>
        /// Gets or sets address for the merchant store.
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the city of the merchant store.
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets state of the merchant store.
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets postal code for the merchant store.
        /// </summary>
        [JsonProperty(PropertyName = "postal")]
        public string Postal { get; set; }
    }
}