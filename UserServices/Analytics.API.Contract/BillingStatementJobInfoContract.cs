//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The billing statement job info contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Analytics.API.Contract
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The billing statement job info contract.
    /// </summary>
    public class BillingStatementJobInfoContract
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the job id.
        /// </summary>
        [DataMember(Name = "job_id")]
        public Guid JobId { get; set; }


        /// <summary>
        /// Gets or sets the from date.
        /// </summary>
        [DataMember(Name = "from_date")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Gets or sets the to date.
        /// </summary>
        [DataMember(Name = "to_date")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Gets or sets the statement date.
        /// </summary>
        [DataMember(Name = "statement_date")]
        public DateTime StatementDate { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        [DataMember(Name = "status_description")]
        public string StatusDescription { get; set; }

        #endregion
    }
}