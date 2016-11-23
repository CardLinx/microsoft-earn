//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Defines the template contract for Bing offers activity + top deals email
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the template contract for Bing offers activity + top deals email
    /// </summary>
    public class DealDashboardContract
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the activities of all the users in Bing Offers
        /// </summary>
        [JsonProperty(PropertyName = "total_activity")]
        public AggregatedActivityContract TotalActivity { get; set; }

        /// <summary>
        /// Gets or sets the activities of the specified user in Bing Offers
        /// </summary>
        [JsonProperty(PropertyName = "user_activity")]
        public UserActivityContract UserActivity { get; set; }

        /// <summary>
        /// Gets or sets the deals
        /// </summary>
        [JsonProperty(PropertyName = "deals")]
        public DealContract[] Deals { get; set; }

        /// <summary>
        /// Gets or sets the unsubscribe url
        /// </summary>
        [JsonProperty(PropertyName = "unsubscribeUrl")]
        public string UnsubscribeUrl { get; set; }

        /// <summary>
        /// Gets or sets the location where deals are selected
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        #endregion
    }
}