//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the data required for sending deals email
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System.Collections.Generic;
    using DotM.DataContracts;

    /// <summary>
    /// Defines the data required for sending deals email
    /// </summary>
    public class DealsTemplateData : EmailTemplateData
    {
        /// <summary>
        /// Gets or sets the location name.
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets the list of deals.
        /// </summary>
        public IEnumerable<Deal> Deals { get; set; }

        /// <summary>
        /// Gets or sets the type of deal email
        /// </summary>
        public DealEmailType DealEmailType { get; set; }
    }

    /// <summary>
    /// Type of deal email being sent
    /// </summary>
    public enum DealEmailType
    {
        /// <summary>
        /// Regular weekly deal - CLO or Pre paid
        /// </summary>
        WeeklyDeal,

        /// <summary>
        /// Trending CLO deal for the region
        /// </summary>
        TrendingDeal
    }
}