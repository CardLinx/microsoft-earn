//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the data required for sending merchant transaction report email
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using Analytics.API.Contract;

    /// <summary>
    /// Defines the data required for sending merchant transaction report email
    /// </summary>
    public class MerchantTemplateData : EmailTemplateData
    {
        /// <summary>
        /// Gets or sets the report start date.
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        ///  Gets or sets the report end date.
        /// </summary>
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Gets or sets the schedule for receiving merchant email
        /// </summary>
        public string ScheduleType { get; set; }

        /// <summary>
        /// Gets or sets the list of redemptions grouped by Merchant Id
        /// </summary>
        public Dictionary<string, List<RedemptionContract>> RedemptionsByMerchant { get; set; }

        /// <summary>
        /// Gets or sets the URL for Merchant Portal.
        /// </summary>
        public string MerchantPortalUrl { get; set; }
    }
}