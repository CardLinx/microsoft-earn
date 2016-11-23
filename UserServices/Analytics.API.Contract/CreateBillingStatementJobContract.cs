//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The create billing statement job contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Analytics.API.Contract
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The create billing statement job contract.
    /// </summary>
    public class CreateBillingStatementJobContract
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the job id.
        /// </summary>
        [DataMember(Name = "job_id")]
        public Guid JobId { get; set; }

        #endregion
    }
}