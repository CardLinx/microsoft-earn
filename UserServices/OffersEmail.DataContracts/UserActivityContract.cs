//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Defines the overall activity of the user in Bing Offers for a given period
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the overall activity of the user in Bing Offers for a given period
    /// </summary>
    public class UserActivityContract
    {
        /// <summary>
        /// Gets or sets the total amount spent by the user in Bing offers in a given period
        /// </summary>
        [JsonProperty(PropertyName = "user_spend")]
        public string UserSpend { get; set; }

        /// <summary>
        /// Gets or sets the total amount saved by the user in Bing offers in a given period
        /// </summary>
        [JsonProperty(PropertyName = "user_saved")]
        public string UserSaved { get; set; }

        /// <summary>
        /// Gets or sets the number of redemptions by the user in Bing offers in a given period
        /// </summary>
        [JsonProperty(PropertyName = "user_redemptions")]
        public int UserRedemptions { get; set; }

        /// <summary>
        /// Gets or sets the date of last redemption by the user
        /// </summary>
        [JsonProperty(PropertyName = "last_redemption_date")]
        public DateTime LastRedemptionDate { get; set; }
    }
}