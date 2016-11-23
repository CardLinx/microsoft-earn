//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Daily Deal Business Model
    /// </summary>
    public class DealPreviewBusinessModel
    {
        /// <summary>
        /// Gets or sets business name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        ///  Gets or sets Name.
        /// </summary>
        [JsonProperty(PropertyName = "locations")]
        public List<DealPreviewBusinessLocationModel> Locations { get; set; }
    }
}