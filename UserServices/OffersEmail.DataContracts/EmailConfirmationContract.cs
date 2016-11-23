//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// //   Contract for Confirming user email address
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Contract for Confirming user email address
    /// </summary>
    public class EmailConfirmationContract
    {
        /// <summary>
        /// Gets or sets the location where deals are selected
        /// </summary>
        [JsonProperty(PropertyName = "confirmation_url")]
        public string ConfirmationUrl { get; set; }
    }
}