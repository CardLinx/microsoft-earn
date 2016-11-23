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
    /// Represents a query for deal reports.
    /// </summary>
    [DataContract]
    public class DealReportsQuery
    {
        /// <summary>
        /// Gets or sets the queries for deals for which to retrieve reports.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_report_queries")]
        public IEnumerable<DealReportQuery> DealReportQueries { get; set; }

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
        /// Gets or sets a value that indicates whether to return information on discounts within the deals.
        /// </summary>
        /// <remarks>
        /// * When this parameter is true and the discount list for a deal is populated, information on only those discounts
        ///   will be returned.
        /// * When this parameter is true and the discount list for a deal is not populated, information for all discounts
        ///   within the deal will be returned.
        /// * When this parameter is false, no discount information for any deal will be returned, regardless of whether
        ///   discount lists are populated.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "report_discounts")]
        public bool ReportDiscounts { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to return redemption details for discounts within the deals.
        /// </summary>
        /// <remarks>
        /// If ReportDiscounts is False, this parameter is ignored.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "redemption_details")]
        public bool RedemptionDetails { get; set; }
    }
}