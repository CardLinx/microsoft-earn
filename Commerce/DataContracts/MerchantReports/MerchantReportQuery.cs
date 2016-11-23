//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a query for a merchant report.
    /// </summary>
    [DataContract]
    public class MerchantReportQuery
    {
        /// <summary>
        /// Gets or sets the list of partners and partner merchant IDs whose report to generate.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "partner_merchant_ids")]
        public IEnumerable<PartnerMerchantIds> PartnerMerchantIds { get; set; }

        /// <summary>
        /// Gets or sets the day from which to start the query.
        /// </summary>
        /// <remarks>
        /// * If not specified, DateTime.MinValue will be used.
        /// * The time is ignored; only the Date portion is respected.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "start_day")]
        public DateTime StartDay { get; set; }

        /// <summary>
        /// Gets or sets the day at which to end the query.
        /// </summary>
        /// <remarks>
        /// * If not specified, DateTime.MaxValue will be used.
        /// * The time is ignored; only the Date portion is respected.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "end_day")]
        public DateTime EndDay { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to return redemption details for deals belonging to the merchant.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "redemption_details")]
        public bool RedemptionDetails { get; set; }
    }
}