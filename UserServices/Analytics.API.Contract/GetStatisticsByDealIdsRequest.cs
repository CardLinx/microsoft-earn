//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The get statistics by deal ids request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Analytics.API.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The get statistics by deal ids request.
    /// </summary>
    public class GetStatisticsByDealIdsRequest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the deal ids.
        /// </summary>
        [DataMember(Name = "deal_ids")]
        public IList<Guid> DealIds { get; set; }

        /// <summary>
        /// Gets or sets the from date time.
        /// </summary>
        [DataMember(Name = "from_date_time")]
        public DateTime FromDateTime { get; set; }


        /// <summary>
        /// Gets or sets the to date time.
        /// </summary>
        [DataMember(Name = "to_date_time")]
        public DateTime ToDateTime { get; set; }

        #endregion
    }
}