//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The deals email model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System.Collections.Generic;

    using DotM.DataContracts;

    using Newtonsoft.Json;

    /// <summary>
    /// The deals email model.
    /// </summary>
    public class DealsEmailModel
    {
        /// <summary>
        /// Gets or sets the deals.
        /// </summary>
        [JsonProperty(PropertyName = "deals")]
        public IEnumerable<Deal> Deals { get; set; }

        /// <summary>
        /// Gets or sets the unsubscribe url.
        /// </summary>
        [JsonProperty(PropertyName = "unsubscribeUrl")]
        public string UnsubscribeUrl { get; set; }
    }
}