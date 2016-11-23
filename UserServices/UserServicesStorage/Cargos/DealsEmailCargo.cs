//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the Deals email cargo
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// Defines the deals email cargo
    /// </summary>
    [DataContract]
    public class DealsEmailCargo : EmailCargo
    {
        /// <summary>
        /// Gets or sets the Anid of the user
        /// </summary>
        [JsonProperty(PropertyName = "anid")]
        public string Anid { get; set; }

        /// <summary>
        /// Gets or sets the location name.
        /// </summary>
        [JsonProperty(PropertyName = "location_id")]
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        [JsonProperty(PropertyName = "categories")]
        public IEnumerable<Guid> Categories { get; set; }

        /// <summary>
        /// Gets or sets if the email campaign is for CLO or Pre paid deals
        /// </summary>
        [JsonProperty(PropertyName = "dealtype")]
        public bool IsCloDeal { get; set; }

        /// <summary>
        /// Gets or sets the dealids to be sent in the email
        /// </summary>
        [JsonProperty(PropertyName = "dealids")]
        public IEnumerable<Guid> DealIds { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Job Id: {0}; User Id: {1}; Location Id: {2}; EmailAddress: {3}; EmailRenderingServiceURL: {4}", 
                this.Id, this.UserId, this.LocationId, this.EmailAddress, this.EmailRenderingServiceAddress);
        }
    }
}