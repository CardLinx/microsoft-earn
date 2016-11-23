//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Daily deals View Model
    /// </summary>
    public class DailyDealsVM
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyDealsVM" /> class.
        /// </summary>
        /// <param name="dailyDealsContract">the deals contract</param>
        public DailyDealsVM(JObject dailyDealsContract)
        {
            this.UnsubscribeUrl = dailyDealsContract["unsubscribeUrl"].Value<string>();
            this.Location = dailyDealsContract["location"].Value<string>();

            var deals = dailyDealsContract["deals"] as JArray;

            if (deals != null)
            {
                this.HeroDeal = new DealVM(deals.FirstOrDefault() as JObject);
                this.FeaturedDeals = deals.Skip(1).Select(d => new DealVM(d as JObject));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Top Deal
        /// </summary>
        public DealVM HeroDeal { get; set; }

        /// <summary>
        /// Gets or sets The other deals
        /// </summary>
        public IEnumerable<DealVM> FeaturedDeals { get; set; }

        /// <summary>
        /// Gets or sets the unsubscribe url
        /// </summary>
        public string UnsubscribeUrl { get; set; }

        /// <summary>
        /// Gets or sets the location of the deals
        /// </summary>
        public string Location { get; set; }

        #endregion
    }
}