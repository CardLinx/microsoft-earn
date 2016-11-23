//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Defines the campaign email cargo
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the campaign email cargo
    /// </summary>
    [DataContract]
    public class CampaignEmailCargo : EmailCargo
    {
        /// <summary>
        /// Gets or sets the location name.
        /// </summary>
        [JsonProperty(PropertyName = "location_id")]
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets the content of the campaign email
        /// </summary>
        [JsonProperty(PropertyName = "Content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets if CLO business names need to be included in the mail
        /// </summary>
        [JsonProperty(PropertyName = "IncludeBusinessNames")]
        public bool IncludeBusinessNames { get; set; }
    }
}