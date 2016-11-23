//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The query payload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Payloads
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// The query payload.
    /// </summary>
    [XmlRoot("data")]
    public class QueryPayload : PayloadBase
    {

        #region Public Properties

        /// <summary>
        /// Gets or sets the coords.
        /// </summary>
        [XmlAttribute("coord")]
        [JsonProperty(PropertyName = "coord")]
        public string Coordinate { get; set; }

        /// <summary>
        /// Gets or sets the deal ids.
        /// </summary>
        [XmlAttribute("deals")]
        [JsonProperty(PropertyName = "deals")]
        public string DealIds { get; set; }

        /// <summary>
        /// Gets or sets the deals count.
        /// </summary>
        [XmlAttribute("cnt")]
        [JsonProperty(PropertyName = "cnt")]
        public int DealsCount { get; set; }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        [XmlArray("cats")]
        [JsonProperty(PropertyName = "cats")]
        public List<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the business ids.
        /// </summary>
        [XmlArray("biz")]
        [JsonProperty(PropertyName = "biz")]
        public List<string> BusinessIds { get; set; }


        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        [XmlArray("kywds")]
        [JsonProperty(PropertyName = "kywds")]
        public List<string> Keywords { get; set; }

        #endregion
    }
}